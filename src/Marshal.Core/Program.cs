using Marshal.Core;

// marshal <export-dir>
// walks a weekend export, normalizes every log family, runs whatever checks are dropped in.

if (args.Length < 1)
{
    Console.WriteLine("usage: marshal <export-dir>");
    return 1;
}

var exportDir = args[0];
if (!Directory.Exists(exportDir))
{
    Console.Error.WriteLine($"export dir not found: {exportDir}");
    return 2;
}

var host = new PluginHost();
var checks = host.Load();
Console.WriteLine($"loaded {checks.Count} check(s)");

var reader = new LogStreamReader(windowSize: 4096);
long records = 0;
var started = DateTimeOffset.UtcNow;

foreach (var file in Directory.EnumerateFiles(exportDir, "*.log", SearchOption.AllDirectories))
{
    foreach (var record in reader.Read(file))
    {
        records++;
        foreach (var check in checks)
        {
            if (!string.Equals(check.LogFamily, record.LogFamily, StringComparison.OrdinalIgnoreCase))
                continue;
            var finding = check.Evaluate(record);
            if (finding is { Severity: >= Severity.Warn })
                Console.WriteLine($"[{finding.Severity}] car {finding.CarNumber} {finding.CheckId}: {finding.Message}");
        }
    }
}

var elapsed = DateTimeOffset.UtcNow - started;
Console.WriteLine($"parsed {records:N0} records in {elapsed:mm\\:ss}");
return 0;
