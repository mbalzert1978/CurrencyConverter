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

    private Currency(string code)
    {
        Code = code;
    }


    public static Currency FromStr(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new Error(
                "400", EmptyErrorMessage);

        if (code.Length != 3)
        {
            throw new Error(
                "400", InvalidLengthErrorMessage);
        }

        if (code.Any(c => !char.IsLetter(c)))
        {
            throw new Error(
                "400", InvalidCharactersErrorMessage);
        }

        return new Currency(code.ToUpperInvariant());

    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Code;
    }
}