namespace Snsc.SecureShareAdmin.Domain;

public sealed class LoanLevelAccessValidation
{
    public LoanLevelDirectoryAccount? DirectoryAccount { get; init; }
    public IReadOnlyList<string> InvestorAccessGroups { get; init; } = Array.Empty<string>();
    public string? ErrorMessage { get; init; }
}
