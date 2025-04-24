namespace KlabTestFramework.System.Abstractions;

public record ComponentConfigValdationResult
{
    public string[] Errors { get; } = [];

    public bool IsValid => Errors.Length == 0;

    public static ComponentConfigValdationResult Success()
    {
        return new([]);
    }

    public static ComponentConfigValdationResult Failure(string[] errors)
    {
        return new(errors);
    }

    private ComponentConfigValdationResult(string[] errors)
    {
        Errors = errors;
    }
}
