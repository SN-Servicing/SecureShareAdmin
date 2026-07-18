namespace Snsc.SecureShareAdmin.Domain;

public sealed class SharedFile
{
    public int SharedFileId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string UploadedBy { get; init; } = string.Empty;
    public DateTime CreatedOnUtc { get; init; }
    public decimal? FileSizeBytes { get; init; }
}
