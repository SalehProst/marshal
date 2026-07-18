using System.Globalization;

namespace Marshal.Core;

/// <summary>
/// Streams an export instead of loading it. We only ever hold a window of records at once,
/// which is the whole reason a 40GB weekend fits in memory you actually have.
/// </summary>
public sealed class LogStreamReader
{
    private readonly int _windowSize;

    public LogStreamReader(int windowSize = 4096) => _windowSize = windowSize;

    public IEnumerable<NormalizedRecord> Read(string path)
    {
        var family = Path.GetFileName(Path.GetDirectoryName(path)) ?? "unknown";
        long offset = 0;

        using var reader = new StreamReader(path);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            offset += line.Length + 1;
            if (line.Length == 0 || line[0] == '#') continue;

            // export format: car|timestamp|metric|value|unit
            var f = line.Split('|');
            if (f.Length < 4) continue;

            if (!DateTimeOffset.TryParse(f[1], CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var ts)) continue;
            if (!double.TryParse(f[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
                continue;

            yield return new NormalizedRecord
            {
                CarNumber = f[0].Trim(),
                LogFamily = family,
                Timestamp = ts,
                Metric = f[2].Trim(),
                Value = val,
                Unit = f.Length > 4 ? f[4].Trim() : null,
                SourceOffset = offset,
                SourceFile = path,
            };
        }
    }
}
