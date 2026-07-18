using System;

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security;
using System.Xml;

namespace Snsc.Configuration
{
    public static class ConfigurationManager
    {
        private static readonly Dictionary<string, string> _dbConnectionStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private static readonly DeploymentSettings _deployment = new DeploymentSettings();
        private static readonly object _lock = new object();
        private static DataSet _configData;
        private static bool _loaded;

        public static Dictionary<string, string> DBConnectionStrings
        {
            get
            {
                EnsureLoaded();
                return _dbConnectionStrings;
            }
        }

        public static DeploymentSettings Deployment
        {
            get { return _deployment; }
        }

        /// <summary>
        /// Returns a value from the SNConfig web service dataset.
        /// The dataset contains tables named by section; each row has Key and Value columns.
        /// </summary>
        public static object GetValue(string section, string key, bool required)
        {
            EnsureLoaded();
            string val = GetSectionValue(section, key);
            if (required && val == null)
                throw new InvalidOperationException(
                    string.Format("Missing required SNConfig value: section='{0}', key='{1}'", section, key));
            return val;
        }

        /// <summary>
        /// Calls the SNConfig ASMX SOAP web service to retrieve configuration data.
        /// </summary>
        public static DataSet LoadFromService()
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["SNConfigWebService"];
            if (string.IsNullOrEmpty(url))
            {
                url = "http://snconfig:8080/SNConfig/SNConfig.asmx";
            }

            string environmentName = System.Configuration.ConfigurationManager.AppSettings["EnvironmentName"] ?? string.Empty;

            string soapBody = string.Format(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" " +
                               "xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                               "xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                  "<soap:Body>" +
                    "<GetConfig xmlns=\"http://tsw-farm/snconfig\">" +
                      "<environmentName>{0}</environmentName>" +
                    "</GetConfig>" +
                  "</soap:Body>" +
                "</soap:Envelope>",
                SecurityElement.Escape(environmentName));

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            request.Headers.Add("SOAPAction", "http://tsw-farm/snconfig/GetConfig");

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(soapBody);
            request.ContentLength = bytes.Length;

            using (Stream reqStream = request.GetRequestStream())
                reqStream.Write(bytes, 0, bytes.Length);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream respStream = response.GetResponseStream())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(respStream);

                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

                XmlNode resultNode = doc.SelectSingleNode("//*[local-name()='GetConfigResult']", nsm);
                if (resultNode == null) return null;

                DataSet ds = new DataSet();
                using (XmlNodeReader reader = new XmlNodeReader(resultNode))
                    ds.ReadXml(reader, XmlReadMode.Auto);

                return ds;
            }
        }

        /// <summary>
        /// Looks up a value by section (table name) and key from the cached SNConfig dataset.
        /// Returns null if not found.
        /// </summary>
        internal static string GetSectionValue(string section, string key)
        {
            EnsureLoaded();
            if (_configData == null) return null;

            if (!_configData.Tables.Contains(section)) return null;

            DataTable table = _configData.Tables[section];
            string keyColumn = table.Columns.Contains("Key") ? "Key" :
                               table.Columns.Contains("key") ? "key" : null;
            string valueColumn = table.Columns.Contains("Value") ? "Value" :
                                 table.Columns.Contains("value") ? "value" : null;

            if (keyColumn == null || valueColumn == null) return null;

            foreach (DataRow row in table.Rows)
            {
                if (string.Equals(row[keyColumn] as string, key, StringComparison.OrdinalIgnoreCase))
                    return row[valueColumn] as string;
            }
            return null;
        }

        private static void EnsureLoaded()
        {
            if (_loaded) return;
            lock (_lock)
            {
                if (_loaded) return;
                try
                {
                    _configData = LoadFromService();
                    PopulateConnectionStrings(_configData);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(
                        "[Snsc.Configuration] Failed to load SNConfig: " + ex.Message);
                    _configData = null;
                }
                _loaded = true;
            }
        }

        /// <summary>
        /// Populates the DBConnectionStrings dictionary from the second table in the DataSet.
        /// Filters rows where ApplicationType == "DBConnectionString".
        /// </summary>
        private static void PopulateConnectionStrings(DataSet ds)
        {
            if (ds == null || ds.Tables.Count < 2) return;

            DataTable table = ds.Tables[1];
            bool hasApplicationTypeColumn = table.Columns.Contains("ApplicationType");
            bool hasKeyColumn = table.Columns.Contains("Name");
            bool hasValueColumn = table.Columns.Contains("Value");
            var areAnyColumnsMissing = !hasApplicationTypeColumn || !hasKeyColumn || !hasValueColumn;
            if (areAnyColumnsMissing)
            {
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                string appType = row["ApplicationType"] as string;
                if (string.Equals(appType, "DBConnectionString", StringComparison.OrdinalIgnoreCase))
                {
                    string name = row["Name"] as string;
                    string value = row["Value"] as string;

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                    {
                        _dbConnectionStrings[name] = value;
                    }
                }
            }
        }
    }
}