namespace Domain;

public sealed class Error(string code, string? description = null) : Exception
{
    public string Code { get; } = code;
    public string? Description { get; } = description;
    public static readonly Error None = new(string.Empty, string.Empty);
}
