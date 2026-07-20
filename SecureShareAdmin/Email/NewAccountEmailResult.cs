namespace Snsc.SecureShareAdmin.Email;

public sealed class NewAccountEmailResult
{
    public bool Sent { get; init; }
    public bool SkippedForEnvironment { get; init; }
    public string? ErrorMessage { get; init; }

    public static NewAccountEmailResult Success()
    {
        return new NewAccountEmailResult { Sent = true };
    }

    public static NewAccountEmailResult Skipped()
    {
        return new NewAccountEmailResult { SkippedForEnvironment = true };
    }

    public static NewAccountEmailResult Failed(string message)
    {
        return new NewAccountEmailResult { ErrorMessage = message };
    }
}
