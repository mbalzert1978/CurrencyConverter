namespace Domain;

public sealed record Error(string Code, string? Description = null)
{
    internal const string UnreachableMessage = "Unreachable should not happen.";
    internal const string NotFoundMessage = "Rate not found.";
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Error Unreachable = new(StatusCode.InternalServerError, UnreachableMessage);
    public static readonly Error NotFound = new(StatusCode.NotFound, NotFoundMessage);
    public static Error BadRequest(string message) => new(StatusCode.BadRequest, message);
}

public static class StatusCode
{
    public const string BadRequest = "400";
    public const string NotFound = "404";
    public const string InternalServerError = "500";
    public const string UnprocessableEntity = "422";
    public const string Conflict = "409";
}
