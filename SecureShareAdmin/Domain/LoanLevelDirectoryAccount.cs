namespace Snsc.SecureShareAdmin.Domain;

public sealed class LoanLevelDirectoryAccount
{
    public Guid ExternalUserId { get; init; }
    public int? AmsUserId { get; init; }
    public string? AmsFirstName { get; init; }
    public string? AmsLastName { get; init; }
    public string? AmsEmail { get; init; }
    public bool? AmsAccountIsEnabled { get; init; }
    public DateTime? AmsAccountDisabledDate { get; init; }
    public string? ActiveDirectorySid { get; init; }
    public string? AdFirstName { get; init; }
    public string? AdLastName { get; init; }
    public string? AdEmail { get; init; }
    public bool? AdAccountIsEnabled { get; init; }
    public DateTime? EnteredTerminatedUsersDate { get; init; }

    public string? AmsFullName => CombineName(AmsFirstName, AmsLastName);
    public string? AdFullName => CombineName(AdFirstName, AdLastName);

    private static string? CombineName(string? firstName, string? lastName)
    {
        string combined = $"{firstName} {lastName}".Trim();
        return string.IsNullOrEmpty(combined) ? null : combined;
    }
}
