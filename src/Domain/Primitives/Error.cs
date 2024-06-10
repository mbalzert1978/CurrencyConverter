namespace Domain;

public sealed record Error(string Code, string? Description = null)
{

    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Exception UnreachableException =
        new("Unreachable should not happen.", new InvalidOperationException("500"));
}

public static class StatusCode
{
    public const string BadRequest = "400";
    public const string NotFound = "404";
    public const string InternalServerError = "500";
    public const string UnprocessableEntity = "422";
    public const string Conflict = "409";
}
