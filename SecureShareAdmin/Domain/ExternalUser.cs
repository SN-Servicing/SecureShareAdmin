namespace Snsc.SecureShareAdmin.Domain;

public sealed class ExternalUser
{
    public Guid UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public int AccessMask { get; init; }
    public int DisabledReasonId { get; init; }
    public string Comment { get; init; } = string.Empty;
    public bool IsApproved { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime LastLoginDate { get; init; }
}
