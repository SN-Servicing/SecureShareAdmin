using System.Data;
using System.Security;
using System.Xml;
using Microsoft.Extensions.Options;

namespace Snsc.SecureShareAdmin.Configuration;

public sealed class SnConfigClient
{
    // SNConfig names are inverted vs AWS docs: AmazonSecretAccessKeyID holds the AKIA… access key id.
    private const string AutomationSupportScope = "SnscAutomationSupport";
    private const string AmazonAccessKeySetting = "AmazonSecretAccessKeyID";
    private const string AmazonSecretKeySetting = "AmazonSecretKeyID";
    private const string AmazonRegionSetting = "AmazonRegion";

    private readonly SnConfigOptions _options;
    private readonly object _lock = new();
    private readonly Dictionary<string, string> _connectionStrings = new(StringComparer.OrdinalIgnoreCase);
    private DataSet? _configData;
    private bool _loaded;

    public SnConfigClient(IOptions<SnConfigOptions> options)
    {
        _options = options.Value;
    }

    public string EnvironmentName => _options.EnvironmentName;

    public string GetConnectionString(string key)
    {
        EnsureLoaded();
        if (_connectionStrings.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Database connection string '{key}' not found in SNConfig. " +
            "Ensure SNConfig has ApplicationType='DBConnectionString' for that key, " +
            "and SnConfig:WebServiceUrl / SnConfig:EnvironmentName are configured.");
    }

    /// <summary>
    /// Reads a value from SNConfigurationData where ScopingType and Name match
    /// (same DataSet used for DB connection strings).
    /// </summary>
    public string? GetConfigValue(string scopingType, string name)
    {
        EnsureLoaded();
        return FindByScopingTypeAndName(scopingType, name);
    }

    public string RequireConfigValue(string scopingType, string name)
    {
        string? value = GetConfigValue(scopingType, name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException(
                $"Missing required SNConfig value: ScopingType='{scopingType}', Name='{name}'.");
        }

        return value;
    }

    public string GetAmazonAccessKeyId()
    {
        return RequireConfigValue(AutomationSupportScope, AmazonAccessKeySetting);
    }

    public string GetAmazonSecretAccessKey()
    {
        return RequireConfigValue(AutomationSupportScope, AmazonSecretKeySetting);
    }

    public string GetAmazonRegion()
    {
        string? region = GetConfigValue(AutomationSupportScope, AmazonRegionSetting);
        return string.IsNullOrWhiteSpace(region) ? "us-east-1" : region.Trim();
    }

    private void EnsureLoaded()
    {
        if (_loaded)
        {
            return;
        }

        lock (_lock)
        {
            if (_loaded)
            {
                return;
            }

            try
            {
                _configData = LoadFromService();
                PopulateConnectionStrings(_configData);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[SnConfigClient] Failed to load SNConfig: " + ex.Message);
                _configData = null;
            }

            _loaded = true;
        }
    }

    private DataSet? LoadFromService()
    {
        string soapBody =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
            "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
            "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
            "<soap:Body>" +
            "<GetConfig xmlns=\"http://tsw-farm/snconfig\">" +
            $"<environmentName>{SecurityElement.Escape(_options.EnvironmentName)}</environmentName>" +
            "</GetConfig>" +
            "</soap:Body>" +
            "</soap:Envelope>";

        using var httpClient = new HttpClient();
        using var content = new StringContent(soapBody, System.Text.Encoding.UTF8, "text/xml");
        content.Headers.Add("SOAPAction", "http://tsw-farm/snconfig/GetConfig");

        using HttpResponseMessage response = httpClient.PostAsync(_options.WebServiceUrl, content).GetAwaiter().GetResult();
        response.EnsureSuccessStatusCode();

        string xml = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var document = new XmlDocument();
        document.LoadXml(xml);

        XmlNode? resultNode = document.SelectSingleNode("//*[local-name()='GetConfigResult']");
        if (resultNode == null)
        {
            return null;
        }

        var dataSet = new DataSet();
        using var reader = new XmlNodeReader(resultNode);
        dataSet.ReadXml(reader, XmlReadMode.Auto);
        return dataSet;
    }

    private void PopulateConnectionStrings(DataSet? dataSet)
    {
        DataTable? table = GetConfigurationDataTable(dataSet);
        if (table == null ||
            !table.Columns.Contains("ApplicationType") ||
            !table.Columns.Contains("Name") ||
            !table.Columns.Contains("Value"))
        {
            return;
        }

        foreach (DataRow row in table.Rows)
        {
            string? appType = row["ApplicationType"] as string;
            if (!string.Equals(appType, "DBConnectionString", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string? name = row["Name"] as string;
            string? value = row["Value"] as string;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                _connectionStrings[name] = value;
            }
        }
    }

    private string? FindByScopingTypeAndName(string scopingType, string name)
    {
        DataTable? table = GetConfigurationDataTable(_configData);
        if (table == null ||
            !table.Columns.Contains("ScopingType") ||
            !table.Columns.Contains("Name") ||
            !table.Columns.Contains("Value"))
        {
            return null;
        }

        foreach (DataRow row in table.Rows)
        {
            string? rowScope = row["ScopingType"] as string;
            string? rowName = row["Name"] as string;
            if (!string.Equals(rowScope, scopingType, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.Equals(rowName, name, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string? value = row["Value"] as string;
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        return null;
    }

    private static DataTable? GetConfigurationDataTable(DataSet? dataSet)
    {
        if (dataSet == null)
        {
            return null;
        }

        if (dataSet.Tables.Contains("SNConfigurationData"))
        {
            return dataSet.Tables["SNConfigurationData"];
        }

        return dataSet.Tables.Count >= 2 ? dataSet.Tables[1] : null;
    }
}
