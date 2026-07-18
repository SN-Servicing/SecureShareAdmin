namespace Snsc.Configuration
{
    public class DeploymentSettings
    {
        public string EnvironmentName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["EnvironmentName"] ?? string.Empty;
            }
        }
    }
}