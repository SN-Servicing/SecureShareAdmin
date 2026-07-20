namespace Snsc.SecureShareAdmin.Domain;

public sealed class ExternalUserDisabledReason
{
    private const string CannotReenableByUserSuffix = " Cannot be reenabled by user.";

    public int ExternalUserDisabledReasonId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool AllowAutomaticReenable { get; init; }

    public string DisplayDescription
    {
        get
        {
            if (ExternalUserDisabledReasonId == 0 || AllowAutomaticReenable)
            {
                return Description;
            }

            return Description + CannotReenableByUserSuffix;
        }
    }
}
