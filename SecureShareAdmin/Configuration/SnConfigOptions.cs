namespace Snsc.SecureShareAdmin.Configuration;

public sealed class SnConfigOptions
{
    public const string SectionName = "SnConfig";

    public string WebServiceUrl { get; set; } = "http://snconfig:8080/SNConfig/SNConfig.asmx";
    public string EnvironmentName { get; set; } = string.Empty;
}
