namespace Snsc.SecureShareAdmin.Domain;

public sealed class ZoneUser
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime LastLoginDate { get; init; }
    public int ZonePermissionId { get; init; }
}
