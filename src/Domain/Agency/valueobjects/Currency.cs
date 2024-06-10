namespace Domain;

public class Currency : ValueObject
{
    public static readonly Currency Default = new("DEFAULT");

    internal string Code { get; private init; }
    internal const string EmptyMessage = "Currency code cannot be empty.";
    internal const string InvalidCharactersMessage = "Currency code can only contain letters.";
    internal const string InvalidLengthMessage =
        "Currency code must be exactly 3 characters long.";

    internal Currency(string code) => Code = code;

    public static void TryFromStr(string code, out Currency currency, out Error error)
    {
        (currency, error) = code switch
        {
            null => (Default, Error.BadRequest(EmptyMessage)),
            { } when string.IsNullOrWhiteSpace(code) => (Default, Error.BadRequest(EmptyMessage)),
            { } when code.Length != 3 => (Default, Error.BadRequest(InvalidLengthMessage)),
            { } when code.Any(c => !char.IsLetter(c)) => (Default, Error.BadRequest(InvalidCharactersMessage)),
            _ => (new Currency(code.ToUpperInvariant()), Error.None)
        };
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
    }
}
