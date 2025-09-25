namespace AnticorruptionLayer.Domain;

/// <summary>
/// Domain model representing the customer profile in the clean context.
/// </summary>
public record CustomerProfile(string Id, string FullName, string Category, bool IsPreferred);
