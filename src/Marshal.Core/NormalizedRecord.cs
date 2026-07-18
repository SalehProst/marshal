namespace Marshal.Core;

/// <summary>
/// The common shape every log family is projected onto. Once a raw record speaks this
/// language, comparing a car against the field (or against its own earlier runs) is trivial.
/// </summary>
public sealed record NormalizedRecord
{
    public required string CarNumber { get; init; }
    public required string LogFamily { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Metric { get; init; }
    public required double Value { get; init; }
    public string? Unit { get; init; }

    // kept around so a finding can point back at the line it came from
    public long SourceOffset { get; init; }
    public string? SourceFile { get; init; }
}
