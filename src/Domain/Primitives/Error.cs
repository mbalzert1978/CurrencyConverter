namespace Domain;

public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static readonly Exception UnreachableException =
        new("Unreachable should not happen.", new InvalidOperationException("500"));
}
