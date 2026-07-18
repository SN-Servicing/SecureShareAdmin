namespace Snsc.SecureShareAdmin.Configuration;

public sealed class SecureShareOptions
{
    public const string SectionName = "SecureShare";

    public int MaxFileSizeMB { get; set; } = 20;
}
