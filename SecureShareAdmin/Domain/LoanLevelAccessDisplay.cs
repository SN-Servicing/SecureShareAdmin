namespace Snsc.SecureShareAdmin.Domain;

public static class LoanLevelAccessDisplay
{
    public const string MissingMarker = "\u00ABmissing\u00BB";
    public const string NoInvestorAccessMarker = "\u00ABNo investor access\u00BB";

    public static string FormatText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? MissingMarker : value;
    }

    public static string FormatUserId(int? userId)
    {
        return userId.HasValue ? userId.Value.ToString() : MissingMarker;
    }

    public static string FormatDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("d") : MissingMarker;
    }

    public static string FormatAdverseDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("d") : string.Empty;
    }

    public static string FormatEnteredTermUsersDate(DateTime? value)
    {
        return value.HasValue
            ? "Entered Term Users: " + value.Value.ToString("d")
            : string.Empty;
    }

    public static bool IsMissing(string displayValue)
    {
        return displayValue == MissingMarker;
    }
}
