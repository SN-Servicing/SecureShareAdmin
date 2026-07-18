namespace Snsc.SecureShareAdmin.Domain;

public sealed class ZoneType
{
    public int ZoneTypeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PrimaryObjectIdFieldName { get; init; } = string.Empty;
    public string SecondaryObjectIdFieldName { get; init; } = string.Empty;
}
