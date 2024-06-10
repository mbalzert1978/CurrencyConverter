namespace Domain;

public class Currency : ValueObject
{
    public static readonly Currency Default = new("DEFAULT");

    internal string Code { get; private init; }
    internal const string EmptyMessage = "Currency code cannot be empty.";
    internal const string InvalidCharactersMessage = "Currency code can only contain letters.";
    internal const string InvalidLengthMessage = "Currency code must be exactly 3 characters long.";

    internal Currency(string code) => Code = code;

    public static Currency TryFromStr(string code, out Error error)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            error = Error.BadRequest(EmptyMessage);
            return Default;
        }

        if (code.Length != 3)
        {
            error = Error.BadRequest(InvalidLengthMessage);
            return Default;
        }

        if (code.Any(c => !char.IsLetter(c)))
        {
            error = Error.BadRequest(InvalidCharactersMessage);
            return Default;
        }

        error = Error.None;
        return new Currency(code.ToUpperInvariant());
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
    }

}