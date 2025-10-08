using System;
using System.Collections.Generic;

namespace DataAutomate.Pro
{
        public sealed class ExtractSettings
        {
            public string Provider { get; init; } = "mssql";
            public string ConnectionString { get; init; } = "";
            public string OutputRoot { get; init; } = "";
            public bool WriteCsv { get; init; } = true;
            public bool CsvGzip { get; init; } = false;
            public bool WriteParquet { get; init; } = false;
            public bool SanitizeStrings { get; init; } = true;
            public string[] AffixColumns { get; init; } = Array.Empty<string>();
            public int AffixMinLen { get; init; } = 3;
            public int AffixMinFreq { get; init; } = 5;
            public int PageSize { get; init; } = 250_000;
            public int SampleRows { get; init; } = 2_000;
            public string? PartitionByColumn { get; init; } = null;
            public int? PartitionMaxRowsPerFile { get; init; } = 10_000_000;
            public bool ValidateCounts { get; init; } = true;
            public bool ComputeChecksums { get; init; } = true;
            public IReadOnlyList<TableIdent> Tables { get; init; } = Array.Empty<TableIdent>();
        }

        public sealed record TableIdent(string Schema, string Name)
        {
            public override string ToString() => $"{Schema}.{Name}";
        }

        public sealed class RunManifest
        {
            public string ToolVersion { get; set; } = "pro-1.0";
            public DateTime StartedUtc { get; set; }
            public DateTime FinishedUtc { get; set; }
            public string Provider { get; set; } = "";
            public string OutputRoot { get; set; } = "";
            public List<TableManifest> Tables { get; set; } = new();
            public Dictionary<string, string>? Environment { get; set; }
        }

        public sealed class TableManifest
        {
            public string Schema { get; set; } = "";
            public string Name { get; set; } = "";
            public long SourceCount { get; set; }
            public long ExportedCount { get; set; }
            public string OutputDir { get; set; } = "";
            public List<FileEntry> Files { get; set; } = new();
            public List<ColumnProfile> Profiles { get; set; } = new();
            public List<string> Warnings { get; set; } = new();
            public Dictionary<string, string>? Extra { get; set; }
        }

        public sealed class FileEntry
        {
            public string Path { get; set; } = "";
            public long SizeBytes { get; set; }
            public string? Sha256 { get; set; }
            public string Format { get; set; } = "csv";
            public string? Partition { get; set; }
            public long Rows { get; set; }
        }

        public sealed class ColumnProfile
        {
            public string Name { get; set; } = "";
            public string Type { get; set; } = "";
            public long NonNulls { get; set; }
            public long Nulls { get; set; }
            public long Distinct { get; set; }
            public string? Min { get; set; }
            public string? Max { get; set; }
            public double? AvgLen { get; set; }
            public int? MaxLen { get; set; }
            public bool? IsMonotonicInc { get; set; }
            public bool? IsLikelyId { get; set; }
            public bool? IsBinaryLike { get; set; }
        }
}