namespace Domain;

public class Currency : ValueObject
{
    protected string Code { get; private init; }
    private const string EmptyErrorMessage =
        "Currency code cannot be empty.";
    private const string InvalidCharactersErrorMessage =
        "Currency code can only contain letters.";
    private const string InvalidLengthErrorMessage =
        "Currency code must be exactly 3 characters long.";

    internal Currency(string code)
    {
        Code = code;
    }


    public static bool TryFromStr(string code, out Currency? currency, out Error? error)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            currency = null;
            error = new Error(
                "400", EmptyErrorMessage);
            return false;

        }

        if (code.Length != 3)
        {
            currency = null;
            error = new Error(
                "400", InvalidLengthErrorMessage);
            return false;
        }

        if (code.Any(c => !char.IsLetter(c)))
        {
            currency = null;
            error = new Error(
                "400", InvalidCharactersErrorMessage);
            return false;
        }

        currency = new Currency(code.ToUpperInvariant());
        error = null;
        return true;

    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
    }
}