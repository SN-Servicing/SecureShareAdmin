using System.Data;
using System.Security;
using System.Xml;
using Microsoft.Extensions.Options;

namespace Snsc.SecureShareAdmin.Configuration;

public sealed class SnConfigClient
{
    private readonly SnConfigOptions _options;
    private readonly object _lock = new();
    private readonly Dictionary<string, string> _connectionStrings = new(StringComparer.OrdinalIgnoreCase);
    private bool _loaded;

    public SnConfigClient(IOptions<SnConfigOptions> options)
    {
        _options = options.Value;
    }

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
                DataSet? dataSet = LoadFromService();
                PopulateConnectionStrings(dataSet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("[SnConfigClient] Failed to load SNConfig: " + ex.Message);
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
        if (dataSet == null || dataSet.Tables.Count < 2)
        {
            return;
        }

        DataTable table = dataSet.Tables[1];
        if (!table.Columns.Contains("ApplicationType") ||
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
}
