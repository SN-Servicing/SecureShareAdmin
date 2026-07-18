namespace Snsc.SecureShareAdmin.Domain;

public sealed class Zone
{
    public int ZoneId { get; init; }
    public int ZonePermissionId { get; init; }
    public int ZoneTypeId { get; init; }
    public string ZoneTypeName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string PrimaryFieldName { get; init; } = string.Empty;
    public string PrimaryIdValue { get; init; } = string.Empty;
    public string SecondaryFieldName { get; init; } = string.Empty;
    public string SecondaryIdValue { get; init; } = string.Empty;
    public int FileCount { get; init; }
    public bool NotificationOptIn { get; init; }
}
