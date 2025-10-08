using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataAutomate.Pro;

namespace DataAutomate.Pro
{
    internal class Cleaning
    {
        public static void SanitizeStrings(DataTable dt)
        {
            foreach (System.Data.DataColumn c in dt.Columns)
            {
                if (c.DataType != typeof(string)) continue;
                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull(c)) continue;
                    var s = ((string)r[c]).Trim();
                    s = Regex.Replace(s, @"\s+", " ");
                    r[c] = s;
                }
            }
        }

        public static void ApplyAffix(DataTable dt, HashSet<string> affCols, int minAffix, int minFreq)
        {
            var target = dt.Columns.Cast<System.Data.DataColumn>()
                .Where(c => c.DataType == typeof(string) && affCols.Contains(c.ColumnName.ToLowerInvariant()))
                .ToList();

            foreach (var c in target)
            {
                var values = dt.AsEnumerable().Where(r => !r.IsNull(c)).Select(r => ((string)r[c]).Trim()).ToList();
                var prefixes = Frequent(values, minAffix, minFreq, true);
                var suffixes = Frequent(values, minAffix, minFreq, false);

                var pCol = new System.Data.DataColumn(c.ColumnName + "_prefix", typeof(string));
                var coreCol = new System.Data.DataColumn(c.ColumnName + "_core", typeof(string));
                var sCol = new System.Data.DataColumn(c.ColumnName + "_suffix", typeof(string));
                dt.Columns.Add(pCol); dt.Columns.Add(coreCol); dt.Columns.Add(sCol);

                foreach (DataRow r in dt.Rows)
                {
                    if (r.IsNull(c)) { r[pCol] = DBNull.Value; r[coreCol] = DBNull.Value; r[sCol] = DBNull.Value; continue; }
                    var s = ((string)r[c]).Trim();
                    var p = prefixes.FirstOrDefault(pre => s.StartsWith(pre, StringComparison.OrdinalIgnoreCase)) ?? "";
                    var u = p.Length > 0 && s.Length > p.Length ? s[p.Length..] : s;
                    var su = suffixes.FirstOrDefault(sf => u.EndsWith(sf, StringComparison.OrdinalIgnoreCase)) ?? "";
                    var core = su.Length > 0 && u.Length > su.Length ? u[..^su.Length] : u;

                    r[pCol] = string.IsNullOrEmpty(p) ? DBNull.Value : p;
                    r[coreCol] = string.IsNullOrWhiteSpace(core) ? DBNull.Value : core;
                    r[sCol] = string.IsNullOrEmpty(su) ? DBNull.Value : su;
                }
            }
        }

        static List<string> Frequent(List<string> vals, int minLen, int minFreq, bool prefix)
        {
            var dict = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var v in vals)
            {
                for (int L = minLen; L <= Math.Min(12, v.Length); L++)
                {
                    var s = prefix ? v[..L] : v[^L..];
                    dict[s] = dict.TryGetValue(s, out var k) ? k + 1 : 1;
                }
            }
            return dict.Where(kv => kv.Value >= minFreq).OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
        }
    }

    public static class Profiler
    {
        public static List<ColumnProfile> Profile(DataTable sample)
        {
            var res = new List<ColumnProfile>();
            foreach (System.Data.DataColumn c in sample.Columns)
            {
                var col = new ColumnProfile { Name = c.ColumnName, Type = c.DataType.Name };
                long nn = 0, nul = 0;
                var set = new HashSet<string?>();
                string? minS = null, maxS = null;
                long totalLen = 0; int maxLen = 0;
                object? prev = null; bool mono = true;

                foreach (DataRow r in sample.Rows)
                {
                    if (r.IsNull(c)) { nul++; continue; }
                    nn++;
                    var v = r[c];
                    var s = v?.ToString();
                    set.Add(s);

                    if (s != null)
                    {
                        totalLen += s.Length;
                        if (s.Length > maxLen) maxLen = s.Length;
                        if (minS == null || string.CompareOrdinal(s, minS) < 0) minS = s;
                        if (maxS == null || string.CompareOrdinal(s, maxS) > 0) maxS = s;
                    }
                    if (prev != null && Compare(prev, v) > 0) mono = false;
                    prev = v;
                }

                col.NonNulls = nn; col.Nulls = nul; col.Distinct = set.Count;
                col.Min = minS; col.Max = maxS;
                if (nn > 0 && (c.DataType == typeof(string))) { col.AvgLen = Math.Round(totalLen / (double)nn, 2); col.MaxLen = maxLen; }
                col.IsMonotonicInc = mono;
                if (c.ColumnName.EndsWith("_id", StringComparison.OrdinalIgnoreCase) || c.ColumnName.Equals("id", StringComparison.OrdinalIgnoreCase))
                    col.IsLikelyId = true;
                if (c.DataType == typeof(byte[]) || (col.MaxLen.HasValue && col.MaxLen > 2048))
                    col.IsBinaryLike = true;

                res.Add(col);
            }
            return res;
        }

        static int Compare(object a, object b)
        {
            if (a is IComparable ca) return ca.CompareTo(b);
            return string.CompareOrdinal(a?.ToString(), b?.ToString());
        }
    }
}

