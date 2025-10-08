
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataAutomate.Pro;



namespace DataAutomate.Pro
{
    public sealed class DataRunner
    {
        public event Action<string>? Log;
        void L(string s) => Log?.Invoke(s);

        public async Task<RunManifest> RunAsync(ExtractSettings cfg, CancellationToken ct)
        {
            Directory.CreateDirectory(cfg.OutputRoot);

            var manifest = new RunManifest
            {
                StartedUtc = DateTime.UtcNow,
                Provider = cfg.Provider,
                OutputRoot = cfg.OutputRoot,
                Environment = new Dictionary<string, string>
                {
                    ["machine"] = Environment.MachineName,
                    ["user"] = Environment.UserName,
                    ["os"] = Environment.OSVersion.ToString()
                }
            };

            using var conn = DbUtils.Create(cfg.Provider, cfg.ConnectionString);
            await conn.OpenAsync(ct);

            var tables = cfg.Tables.Count == 0
                ? await DbUtils.ListTablesAsync(conn)
                : cfg.Tables.ToList();

            foreach (var t in tables)
            {
                ct.ThrowIfCancellationRequested();

                var tm = new TableManifest { Schema = t.Schema, Name = t.Name };
                manifest.Tables.Add(tm);

                L($"> {t} counting …");
                tm.SourceCount = cfg.ValidateCounts ? await DbUtils.CountAsync(conn, t, ct) : 0;

              
                L($"> {t} sampling {cfg.SampleRows} …");
                var sample = await DbUtils.ReadPagedAsync(conn, t, Math.Max(10, cfg.SampleRows), null, cfg.PartitionByColumn, ct)
                                          .FirstOrDefaultAsync(ct)
                             ?? new DataTable();

                if (cfg.SanitizeStrings) Cleaning.SanitizeStrings(sample);
                if (cfg.AffixColumns.Length > 0)
                    Cleaning.ApplyAffix(sample, cfg.AffixColumns.Select(s => s.ToLowerInvariant()).ToHashSet(),
                                        cfg.AffixMinLen, cfg.AffixMinFreq);

                tm.Profiles = Profiler.Profile(sample);

             
                var outDir = Path.Combine(cfg.OutputRoot, Safe($"{t.Schema}.{t.Name}"));
                tm.OutputDir = outDir;
                Directory.CreateDirectory(outDir);

                long exported = 0;
                int page = 0;

                await foreach (var dt in DbUtils.ReadPagedAsync(conn, t, cfg.PageSize, null, cfg.PartitionByColumn, ct))
                {
                    if (cfg.SanitizeStrings) Cleaning.SanitizeStrings(dt);
                    if (cfg.AffixColumns.Length > 0)
                        Cleaning.ApplyAffix(dt, cfg.AffixColumns.Select(s => s.ToLowerInvariant()).ToHashSet(),
                                            cfg.AffixMinLen, cfg.AffixMinFreq);

                    string? partition = null;
                    if (!string.IsNullOrWhiteSpace(cfg.PartitionByColumn) && dt.Columns.Contains(cfg.PartitionByColumn))
                        partition = $"{cfg.PartitionByColumn}=" + DateLabel(dt, cfg.PartitionByColumn!);

                    var dir = partition == null ? outDir : Path.Combine(outDir, partition);
                    var baseName = $"{t.Name}_p{page:00000}";

                    if (cfg.WriteCsv)
                    {
                        (string path, long rows, string? sha) = await Writers.WriteCsvAsync(
                            dt, dir, baseName, cfg.CsvGzip, cfg.ComputeChecksums);
                        tm.Files.Add(new FileEntry
                        {
                            Path = path,
                            SizeBytes = new FileInfo(path).Length,
                            Sha256 = sha,
                            Rows = rows,
                            Format = cfg.CsvGzip ? "csv.gz" : "csv",
                            Partition = partition
                        });
                    }

                    if (cfg.WriteParquet)
                    {
                        (string pathPq, long rowsPq, string? shaPq) = await Writers.WriteParquetAsync(
                            dt, dir, baseName, cfg.ComputeChecksums);
                        tm.Files.Add(new FileEntry
                        {
                            Path = pathPq,
                            SizeBytes = new FileInfo(pathPq).Length,
                            Sha256 = shaPq,
                            Rows = rowsPq,
                            Format = "parquet",
                            Partition = partition
                        });
                    }

                    exported += dt.Rows.Count;
                    L($"> {t} page {page++} rows {dt.Rows.Count} (cum {exported})");
                }

                tm.ExportedCount = exported;
                if (cfg.ValidateCounts && tm.SourceCount != 0 && tm.SourceCount != tm.ExportedCount)
                    tm.Warnings.Add($"CountMismatch: source={tm.SourceCount} exported={tm.ExportedCount}");
            }

            manifest.FinishedUtc = DateTime.UtcNow;
            var manifestPath = Path.Combine(cfg.OutputRoot, $"manifest_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
            await File.WriteAllTextAsync(manifestPath,
                JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }), ct);
            L($"Manifest: {manifestPath}");
            return manifest;
        }

        static string Safe(string s) =>
            System.Text.RegularExpressions.Regex.Replace(s, @"[^A-Za-z0-9_.=-]", "_");

        static string DateLabel(DataTable dt, string col)
        {
            var r = dt.Rows[0];
            if (r.IsNull(col)) return "unknown";
            var d = Convert.ToDateTime(r[col]).ToUniversalTime();
            return d.ToString("yyyyMMdd");
        }
    }

    static class AsyncEnumExt
    {
        public static async Task<T?> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> src, CancellationToken ct)
        {
            await foreach (var x in src.WithCancellation(ct)) return x;
            return default;
        }
    }
}

