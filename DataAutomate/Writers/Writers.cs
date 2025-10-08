using CsvHelper;
using CsvHelper.Configuration;
using DataAutomate.Pro;
using Parquet;
using Parquet.Schema;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DataColumn = System.Data.DataColumn;
using Pq = Parquet;
using PqData = Parquet.Data;
using PqSchema = Parquet.Schema;

namespace DataAutomate.Pro
{
    internal class Writers
    {
        static string? FixString(string? s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var norm = s.Normalize(NormalizationForm.FormC);
            var sb = new StringBuilder(norm.Length);
            foreach (var ch in norm)
            {
                if (ch == '\t' || ch == '\r' || ch == '\n' || !char.IsControl(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        static object? CoerceForCsv(object? v) => v is string s ? FixString(s) : v;

        public static async Task<(string path, long rows, string? sha)> WriteCsvAsync(
            DataTable dt, string dir, string baseName, bool gzip, bool computeSha)
        {
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, gzip ? $"{baseName}.csv.gz" : $"{baseName}.csv");

            if (gzip)
            {
                await using var fs = File.Create(path);
                await using var gz = new GZipStream(fs, CompressionLevel.SmallestSize);
                await using var sw = new StreamWriter(gz, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                using var csv = new CsvWriter(sw, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    ShouldQuote = _ => true
                });

                foreach (System.Data.DataColumn c in dt.Columns)
                    csv.WriteField(FixString(c.ColumnName));
                await csv.NextRecordAsync();

                foreach (DataRow r in dt.Rows)
                {
                    foreach (System.Data.DataColumn c in dt.Columns)
                        csv.WriteField(r.IsNull(c) ? null : CoerceForCsv(r[c]));
                    await csv.NextRecordAsync();
                }
            }
            else
            {
                await using var fs = File.Create(path);
                await using var sw = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
                using var csv = new CsvWriter(sw, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    ShouldQuote = _ => true
                });

                foreach (System.Data.DataColumn c in dt.Columns)
                    csv.WriteField(FixString(c.ColumnName));
                await csv.NextRecordAsync();

                foreach (DataRow r in dt.Rows)
                {
                    foreach (System.Data.DataColumn c in dt.Columns)
                        csv.WriteField(r.IsNull(c) ? null : CoerceForCsv(r[c]));
                    await csv.NextRecordAsync();
                }
            }

            string? hex = null;
            if (computeSha)
            {
                using var fs = File.OpenRead(path);
                var hash = SHA256.HashData(fs);
                hex = Convert.ToHexString(hash).ToLowerInvariant();
            }

            return (path, dt.Rows.Count, hex);
        }

        public static async Task<(string path, long rows, string? sha)> WriteParquetAsync(
        DataTable dt, string dir, string baseName, bool computeSha)
        {
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, $"{baseName}.parquet");

           
            var fields = dt.Columns.Cast<DataColumn>().Select(ToField).ToArray();
            var schema = new PqSchema.ParquetSchema(fields);

            await using (var fs = File.Create(path))
            {
                using var writer = await Pq.ParquetWriter.CreateAsync(schema, fs);
                using var rg = writer.CreateRowGroup();

                
                foreach (var (col, idx) in dt.Columns.Cast<DataColumn>().Select((c, i) => (c, i)))
                {
                    var field = (PqSchema.DataField)fields[idx];
                    var data = MapColumn(dt, col, field);    
                    var pcol = new PqData.DataColumn(field, data);
                    await rg.WriteColumnAsync(pcol);
                }
            }

            string? hex = null;
            if (computeSha)
            {
                using var fs2 = File.OpenRead(path);
                var hash = SHA256.HashData(fs2);
                hex = Convert.ToHexString(hash).ToLowerInvariant();
            }

            return (path, dt.Rows.Count, hex);
        }

        
        static PqSchema.Field ToField(DataColumn c) =>
            c.DataType == typeof(int) || c.DataType == typeof(int?) ? new PqSchema.DataField<int?>(c.ColumnName) :
            c.DataType == typeof(long) || c.DataType == typeof(long?) ? new PqSchema.DataField<long?>(c.ColumnName) :
            c.DataType == typeof(decimal) || c.DataType == typeof(decimal?) ? new PqSchema.DataField<decimal?>(c.ColumnName) :
            c.DataType == typeof(double) || c.DataType == typeof(double?) ? new PqSchema.DataField<double?>(c.ColumnName) :
            c.DataType == typeof(float) || c.DataType == typeof(float?) ? new PqSchema.DataField<float?>(c.ColumnName) :
            c.DataType == typeof(bool) || c.DataType == typeof(bool?) ? new PqSchema.DataField<bool?>(c.ColumnName) :
            c.DataType == typeof(DateTime) || c.DataType == typeof(DateTime?) ? new PqSchema.DataField<DateTime?>(c.ColumnName) :
                                                                                  new PqSchema.DataField<string?>(c.ColumnName);

        static Array MapColumn(DataTable dt, DataColumn c, PqSchema.DataField field)
        {
            
            if (field is PqSchema.DataField<string?>)
                return dt.AsEnumerable()
                         .Select(r => r.IsNull(c) ? null : FixString(r[c]?.ToString()))
                         .ToArray();

            if (field is PqSchema.DataField<int?>)
                return dt.AsEnumerable().Select(r => r.IsNull(c) ? (int?)null : Convert.ToInt32(r[c])).ToArray();
            if (field is PqSchema.DataField<long?>)
                return dt.AsEnumerable().Select(r => r.IsNull(c) ? (long?)null : Convert.ToInt64(r[c])).ToArray();
            if (field is PqSchema.DataField<decimal?>)
                return dt.AsEnumerable().Select(r => r.IsNull(c) ? (decimal?)null : Convert.ToDecimal(r[c])).ToArray();
            if (field is PqSchema.DataField<double?>)
                return dt.AsEnumerable().Select(r => r.IsNull(c) ? (double?)null : Convert.ToDouble(r[c])).ToArray();
            if (field is PqSchema.DataField<float?>)
                return dt.AsEnumerable().Select(r => r.IsNull(c) ? (float?)null : Convert.ToSingle(r[c])).ToArray();
            if (field is PqSchema.DataField<bool?>)
                return dt.AsEnumerable().Select(r => r.IsNull(c) ? (bool?)null : Convert.ToBoolean(r[c])).ToArray();

            
            return dt.AsEnumerable()
                     .Select(r => r.IsNull(c) ? (DateTime?)null : Convert.ToDateTime(r[c]))
                     .ToArray();
        }

    }
}

