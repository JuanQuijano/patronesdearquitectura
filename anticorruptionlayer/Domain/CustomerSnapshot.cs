namespace AnticorruptionLayer.Domain;

/// <summary>
/// Data transfer object that represents the information the domain needs from the external world.
/// </summary>
public record CustomerSnapshot(string ExternalId, string DisplayName, string Segment, int Score);
