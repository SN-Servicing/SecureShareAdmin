namespace Snsc.SecureShareAdmin.Domain;

public sealed class ExternalUserAdminDetails
{
    public required ExternalUser User { get; init; }
    public required IReadOnlyList<ExternalUserDisabledReason> DisabledReasons { get; init; }
}
