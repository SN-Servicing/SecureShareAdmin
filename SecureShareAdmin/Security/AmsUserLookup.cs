using System.Data;
using Dapper;
using Snsc.SecureShareAdmin.Data;

namespace Snsc.SecureShareAdmin.Security;

public sealed class AmsUserLookup
{
    private readonly AppDatabase _database;

    public AmsUserLookup(AppDatabase database)
    {
        _database = database;
    }

    public int? FindAmsUserIdByWindowsLogin(string windowsIdentityName)
    {
        if (string.IsNullOrWhiteSpace(windowsIdentityName))
        {
            return null;
        }

        string ntLoginName = windowsIdentityName;
        int slashIndex = ntLoginName.IndexOf('\\');
        if (slashIndex >= 0 && slashIndex < ntLoginName.Length - 1)
        {
            ntLoginName = ntLoginName[(slashIndex + 1)..];
        }

        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.LmsSystem);
        return connection.QueryFirstOrDefault<int?>(
            "SELECT UserID FROM dbo.[User] WHERE NTLoginName = @NTUserName",
            new { NTUserName = ntLoginName });
    }
}
