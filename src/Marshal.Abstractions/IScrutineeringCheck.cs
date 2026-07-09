namespace Marshal.Abstractions;

/// <summary>
/// A single scrutineering check. Implement this in a plugin assembly, compile to a DLL,
/// and drop it next to the host executable — Marshal discovers it at startup.
/// </summary>
public interface IScrutineeringCheck
{
    /// <summary>Short id used in reports, e.g. "plank-wear" or "front-wing-deflection".</summary>
    string Id { get; }

    /// <summary>Log family this check consumes (matches the export folder name).</summary>
    string LogFamily { get; }

    /// <summary>Evaluate one normalized record and return a finding, or null if nothing to flag.</summary>
    Finding? Evaluate(NormalizedRecord record);
}

public sealed record Finding(string CheckId, string CarNumber, string Message, Severity Severity);

public enum Severity { Info, Warn, Fail }
