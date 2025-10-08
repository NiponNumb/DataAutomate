using Dapper;
using DataAutomate.Pro;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace DataAutomate.Pro
{
    internal class DbUtils
    {
        public static DbConnection Create(string provider, string conn) =>
            provider switch
            {
                "mssql" => new SqlConnection(conn),
                "postgres" => new NpgsqlConnection(conn),
                "mysql" => new MySqlConnection(conn),
                "sqlite" => new SqliteConnection(conn),
                _ => throw new ArgumentException("provider")
            };

  
        public static Task<List<TableIdent>> ListTablesAsync(DbConnection conn) =>
            conn switch
            {
                SqlConnection => ListMssql(conn),
                NpgsqlConnection => ListPg(conn),
                MySqlConnection => ListMySql(conn),
                SqliteConnection => ListSqlite(conn),
                _ => throw new NotSupportedException()
            };

        static async Task<List<TableIdent>> ListMssql(DbConnection c)
        {
            var cmd = c.CreateCommand();
            cmd.CommandText = """
                SELECT TABLE_SCHEMA, TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_TYPE='BASE TABLE'
                ORDER BY 1,2
                """;
            var list = new List<TableIdent>();
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                list.Add(new TableIdent(rdr.GetString(0), rdr.GetString(1)));
            return list;
        }

        static async Task<List<TableIdent>> ListPg(DbConnection c)
        {
            var cmd = c.CreateCommand();
            cmd.CommandText = """
                SELECT table_schema, table_name
                FROM information_schema.tables
                WHERE table_type='BASE TABLE'
                  AND table_schema NOT IN ('pg_catalog','information_schema')
                ORDER BY 1,2
                """;
            var list = new List<TableIdent>();
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                list.Add(new TableIdent(rdr.GetString(0), rdr.GetString(1)));
            return list;
        }

        static async Task<List<TableIdent>> ListMySql(DbConnection c)
        {
            var cmd = c.CreateCommand();
            cmd.CommandText = """
                SELECT TABLE_SCHEMA, TABLE_NAME
                FROM information_schema.tables
                WHERE table_type='BASE TABLE' AND TABLE_SCHEMA = DATABASE()
                ORDER BY 1,2
                """;
            var list = new List<TableIdent>();
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                list.Add(new TableIdent(rdr.GetString(0), rdr.GetString(1)));
            return list;
        }

        static async Task<List<TableIdent>> ListSqlite(DbConnection c)
        {
            var cmd = c.CreateCommand();
            cmd.CommandText = """
                SELECT name
                FROM sqlite_master
                WHERE type='table' AND name NOT LIKE 'sqlite_%'
                ORDER BY 1
                """;
            var list = new List<TableIdent>();
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                list.Add(new TableIdent("main", rdr.GetString(0)));
            return list;
        }

       
        public static async Task<long> CountAsync(DbConnection conn, TableIdent t, CancellationToken ct)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = conn switch
            {
                SqlConnection => $"SELECT COUNT_BIG(1) FROM [{t.Schema}].[{t.Name}]",
                NpgsqlConnection => $"SELECT COUNT(1) FROM \"{t.Schema}\".\"{t.Name}\"",
                MySqlConnection => $"SELECT COUNT(1) FROM `{t.Schema}`.`{t.Name}`",
                SqliteConnection => $"SELECT COUNT(1) FROM \"{t.Name}\"",
                _ => throw new NotSupportedException()
            };
            var o = await cmd.ExecuteScalarAsync(ct);
            return Convert.ToInt64(o);
        }

       
        public static async Task<DataTable> ReadSampleAsync(DbConnection conn, TableIdent t, int take, CancellationToken ct)
        {
            using var cmd = conn.CreateCommand();
            switch (conn)
            {
                case SqlConnection:
                    cmd.CommandText = $"SELECT TOP (@n) * FROM [{t.Schema}].[{t.Name}]";
                    var pm = cmd.CreateParameter(); pm.ParameterName = "@n"; pm.Value = take; cmd.Parameters.Add(pm);
                    break;

                case NpgsqlConnection:
                    cmd.CommandText = $"SELECT * FROM \"{t.Schema}\".\"{t.Name}\" LIMIT @n";
                    var pp = cmd.CreateParameter(); pp.ParameterName = "@n"; pp.Value = take; cmd.Parameters.Add(pp);
                    break;

                case MySqlConnection:
                    cmd.CommandText = $"SELECT * FROM `{t.Schema}`.`{t.Name}` LIMIT @n";
                    var py = cmd.CreateParameter(); py.ParameterName = "@n"; py.Value = take; cmd.Parameters.Add(py);
                    break;

                case SqliteConnection:
                    cmd.CommandText = $"SELECT * FROM \"{t.Name}\" LIMIT @n";
                    var ps = cmd.CreateParameter(); ps.ParameterName = "@n"; ps.Value = take; cmd.Parameters.Add(ps);
                    break;

                default: throw new NotSupportedException();
            }

            using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, ct);
            var dt = new DataTable();
            dt.Load(rdr);
            return dt;
        }

        
        public static async IAsyncEnumerable<DataTable> ReadPagedAsync(
            DbConnection conn, TableIdent t, int pageSize, string? orderBy, string? partitionByColumn,
            [EnumeratorCancellation] CancellationToken ct)
        {
            
            string? orderClause = await ResolveOrderByAsync(conn, t, orderBy, ct);

            long offset = 0;
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                using var cmd = conn.CreateCommand();

                switch (conn)
                {
                    case SqlConnection:
                        cmd.CommandText = $"""
                            SELECT * FROM [{t.Schema}].[{t.Name}]
                            {orderClause ?? "ORDER BY (SELECT NULL)"}
                            OFFSET @off ROWS FETCH NEXT @ps ROWS ONLY
                            """;
                        AddParam(cmd, "@off", offset);
                        AddParam(cmd, "@ps", pageSize);
                        break;

                    case NpgsqlConnection:
                        cmd.CommandText = $"""
                            SELECT * FROM "{t.Schema}"."{t.Name}"
                            {(orderClause is null ? "" : orderClause)}
                            LIMIT @ps OFFSET @off
                            """;
                        AddParam(cmd, "@ps", pageSize);
                        AddParam(cmd, "@off", offset);
                        break;

                    case MySqlConnection:
                        cmd.CommandText = $"""
                            SELECT * FROM `{t.Schema}`.`{t.Name}`
                            {(orderClause is null ? "" : orderClause)}
                            LIMIT @ps OFFSET @off
                            """;
                        AddParam(cmd, "@ps", pageSize);
                        AddParam(cmd, "@off", offset);
                        break;

                    case SqliteConnection:
                        cmd.CommandText = $"""
                            SELECT * FROM "{t.Name}"
                            {(orderClause is null ? "" : orderClause)}
                            LIMIT @ps OFFSET @off
                            """;
                        AddParam(cmd, "@ps", pageSize);
                        AddParam(cmd, "@off", offset);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                using var rdr = await cmd.ExecuteReaderAsync(CommandBehavior.SequentialAccess, ct);
                var dt = new DataTable();
                dt.Load(rdr);
                if (dt.Rows.Count == 0) yield break;

                yield return dt;
                offset += dt.Rows.Count;
                if (dt.Rows.Count < pageSize) yield break;
            }
        }

        static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }

       
        static async Task<string?> ResolveOrderByAsync(DbConnection conn, TableIdent t, string? userOrder, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(userOrder))
                return "ORDER BY " + userOrder!;

          
            if (conn is SqlConnection) return await OrderByMssqlAsync(conn, t, ct);
            if (conn is NpgsqlConnection) return await OrderByPgAsync(conn, t, ct);
            if (conn is MySqlConnection) return await OrderByMySqlAsync(conn, t, ct);
            if (conn is SqliteConnection) return "ORDER BY rowid"; // rowid existe salvo tablas WITHOUT ROWID

            return null;
        }

        static async Task<string?> OrderByMssqlAsync(DbConnection c, TableIdent t, CancellationToken ct)
        {
           
            using (var pk = c.CreateCommand())
            {
                pk.CommandText = """
                    SELECT kcu.COLUMN_NAME
                    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                      ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME
                     AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA
                     AND tc.TABLE_NAME   = kcu.TABLE_NAME
                    WHERE tc.CONSTRAINT_TYPE='PRIMARY KEY'
                      AND tc.TABLE_SCHEMA=@s AND tc.TABLE_NAME=@t
                    ORDER BY kcu.ORDINAL_POSITION
                    """;
                AddParam(pk, "@s", t.Schema);
                AddParam(pk, "@t", t.Name);
                var col = (string?)await pk.ExecuteScalarAsync(ct);
                if (!string.IsNullOrWhiteSpace(col))
                    return $"ORDER BY [{col}]";
            }

           
            using (var fc = c.CreateCommand())
            {
                fc.CommandText = """
                    SELECT TOP (1) COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_SCHEMA=@s AND TABLE_NAME=@t
                    ORDER BY ORDINAL_POSITION
                    """;
                AddParam(fc, "@s", t.Schema);
                AddParam(fc, "@t", t.Name);
                var col = (string?)await fc.ExecuteScalarAsync(ct);
                if (!string.IsNullOrWhiteSpace(col))
                    return $"ORDER BY [{col}]";
            }

           
            return "ORDER BY (SELECT NULL)";
        }

        static async Task<string?> OrderByPgAsync(DbConnection c, TableIdent t, CancellationToken ct)
        {
            using var pk = c.CreateCommand();
            pk.CommandText = """
                SELECT a.attname
                FROM pg_index i
                JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
                WHERE i.indrelid = format('%I.%I', @s, @t)::regclass AND i.indisprimary
                LIMIT 1
                """;
            AddParam(pk, "@s", t.Schema);
            AddParam(pk, "@t", t.Name);
            var col = (string?)await pk.ExecuteScalarAsync(ct);
            return string.IsNullOrWhiteSpace(col) ? null : $"ORDER BY \"{col}\"";
        }

        static async Task<string?> OrderByMySqlAsync(DbConnection c, TableIdent t, CancellationToken ct)
        {
            using var pk = c.CreateCommand();
            pk.CommandText = """
                SELECT k.COLUMN_NAME
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage k
                  ON tc.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                 AND tc.TABLE_SCHEMA = k.TABLE_SCHEMA
                 AND tc.TABLE_NAME = k.TABLE_NAME
                WHERE tc.CONSTRAINT_TYPE='PRIMARY KEY'
                  AND tc.TABLE_SCHEMA = DATABASE()
                  AND tc.TABLE_NAME = @tbl
                LIMIT 1
                """;
            AddParam(pk, "@tbl", t.Name);
            var col = (string?)await pk.ExecuteScalarAsync(ct);
            return string.IsNullOrWhiteSpace(col) ? null : $"ORDER BY `{col}`";
        }
    }
}

