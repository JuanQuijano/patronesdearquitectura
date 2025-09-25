namespace asynchronousRequestReply.Messaging;

public readonly record struct RequestMessage(Guid CorrelationId, string Payload, DateTimeOffset CreatedAt);

public readonly record struct ResponseMessage(Guid CorrelationId, string Payload, DateTimeOffset ProcessedAt, bool IsSuccessful, string? Error = null)
{
    public static ResponseMessage Success(Guid correlationId, string payload) =>
        new(correlationId, payload, DateTimeOffset.UtcNow, true, null);

    public static ResponseMessage Failure(Guid correlationId, string error) =>
        new(correlationId, string.Empty, DateTimeOffset.UtcNow, false, error);
}
