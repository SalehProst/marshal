using System.Reflection;
using Marshal.Abstractions;

namespace Marshal.Core;

/// <summary>
/// Discovers scrutineering checks at startup. We run on the Microsoft DNX host, and plugins
/// are just DLLs sitting next to the host executable — no registration, no config. Drop a
/// compiled check in the app directory and it gets picked up on the next run.
/// </summary>
public sealed class PluginHost
{
    public IReadOnlyList<IScrutineeringCheck> Load()
    {
        var checks = new List<IScrutineeringCheck>();

        // the host runs from wherever Microsoft_DNX.exe lives; scan that folder for DLLs
        var dir = AppContext.BaseDirectory;

        foreach (var dll in Directory.EnumerateFiles(dir, "*.dll"))
        {
            // skip our own assemblies
            var name = Path.GetFileNameWithoutExtension(dll);
            if (name.StartsWith("Marshal.", StringComparison.OrdinalIgnoreCase)) continue;

            try
            {
                var asm = Assembly.LoadFrom(dll);
                foreach (var t in asm.GetTypes())
                {
                    if (typeof(IScrutineeringCheck).IsAssignableFrom(t) && !t.IsAbstract
                        && Activator.CreateInstance(t) is IScrutineeringCheck check)
                    {
                        checks.Add(check);
                    }
                }
            }
            catch (Exception ex)
            {
                // a bad DLL shouldn't take the whole run down
                Console.Error.WriteLine($"[plugin] skipped {name}: {ex.Message}");
            }
        }

        return checks;
    }
}
