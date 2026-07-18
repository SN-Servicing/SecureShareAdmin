using System.Data;
using Microsoft.Data.SqlClient;
using Snsc.SecureShareAdmin.Configuration;

namespace Snsc.SecureShareAdmin.Data;

public sealed class AppDatabase
{
    private readonly SnConfigClient _snConfigClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppDatabase(SnConfigClient snConfigClient, IHttpContextAccessor httpContextAccessor)
    {
        _snConfigClient = snConfigClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public IDbConnection CreateOpenConnection(DatabaseTarget database)
    {
        string connectionString = GetConnectionString(database);
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    public string GetConnectionString(DatabaseTarget database)
    {
        string key = "db:" + database.ToString().ToLowerInvariant();
        string originalConnectionString = _snConfigClient.GetConnectionString(key);
        var connectionStringBuilder = new SqlConnectionStringBuilder(originalConnectionString)
        {
            TrustServerCertificate = true,
            ApplicationName = BuildSqlApplicationName()
        };
        return connectionStringBuilder.ConnectionString;
    }

    private string BuildSqlApplicationName()
    {
        HttpRequest request = _httpContextAccessor.HttpContext?.Request
            ?? throw new InvalidOperationException(
                "No active HTTP request. Connection strings that embed app path/port must be resolved during a request.");

        int port = request.Host.Port ?? 80;
        string applicationPath = request.PathBase.HasValue ? request.PathBase.Value! : "/";
        return $"SecureShareAdmin-{Environment.MachineName}-{port}-{applicationPath}";
    }
}
