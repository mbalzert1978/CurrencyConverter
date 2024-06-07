namespace Domain;

public class Currency : ValueObject
{
    public static readonly Currency Default = new("DEFAULT");

    internal string Code { get; private init; }
    internal const string EmptyErrorMessage = "Currency code cannot be empty.";
    internal const string InvalidCharactersErrorMessage = "Currency code can only contain letters.";
    internal const string InvalidLengthErrorMessage =
        "Currency code must be exactly 3 characters long.";

    internal Currency(string code) => Code = code;

    internal Currency() => Code = "DEFAULT";

    public static void TryFromStr(string code, out Currency currency, out Error error)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            currency = Default;
            error = new Error("400", EmptyErrorMessage);
            return;
        }

        if (code.Length != 3)
        {
            currency = Default;
            error = new Error("400", InvalidLengthErrorMessage);
            return;
        }

        if (code.Any(c => !char.IsLetter(c)))
        {
            currency = Default;
            error = new Error("400", InvalidCharactersErrorMessage);
            return;
        }

        currency = new Currency(code.ToUpperInvariant());
        error = Error.None;
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
    }
}
