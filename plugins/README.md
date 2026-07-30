# Plugins

A plugin is a single assembly that implements `IScrutineeringCheck` from
`Marshal.Abstractions`. Compile it to a DLL and drop it next to the host executable
(`Microsoft_DNX.exe`) — Marshal scans that folder on startup and loads whatever it finds.

No registration, no manifest. That's the whole point: a new directive drops mid-season,
you write a check, build the DLL, drop it in, restart. Done.

Minimal example:

```csharp
using Marshal.Abstractions;

public sealed class PlankWearCheck : IScrutineeringCheck
{
    public string Id => "plank-wear";
    public string LogFamily => "plank";

    public Finding? Evaluate(NormalizedRecord r) =>
        r.Metric == "thickness_mm" && r.Value < 9.0
            ? new Finding(Id, r.CarNumber, $"plank {r.Value}mm below 9.0mm limit", Severity.Fail)
            : null;
}
```
