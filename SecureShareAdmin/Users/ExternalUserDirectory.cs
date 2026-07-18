using System.Data;
using Dapper;
using Snsc.SecureShareAdmin.Data;
using Snsc.SecureShareAdmin.Domain;

namespace Snsc.SecureShareAdmin.Users;

public enum ExternalUserCreateResult
{
    Success = 0,
    DuplicateUserName = 6,
    DuplicateEmail = 7,
    Rejected = 8,
    ProviderError = -1
}

public sealed class ExternalUserDirectory
{
    private readonly AppDatabase _database;

    public ExternalUserDirectory(AppDatabase database)
    {
        _database = database;
    }

    public IReadOnlyList<ExternalUser> FindByUserName(string userNameToMatch, int pageSize = 50)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        var parameters = new DynamicParameters();
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@PageIndex", 0);
        parameters.Add("@UserNameToMatch", userNameToMatch);
        parameters.Add("@RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        IEnumerable<ExternalUserRow> rows = connection.Query<ExternalUserRow>(
            "SecureShare.GetExternalUsersByName_Admin",
            parameters,
            commandType: CommandType.StoredProcedure);

        return rows.Select(MapUser).ToList();
    }

    public ExternalUser? GetByUserId(Guid userId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        ExternalUserRow? row = connection.QueryFirstOrDefault<ExternalUserRow>(
            "SecureShare.GetExternalUser",
            new { ExternalUserId = userId },
            commandType: CommandType.StoredProcedure);

        return row == null ? null : MapUser(row);
    }

    public ExternalUser? GetByUserName(string userName)
    {
        IReadOnlyList<ExternalUser> matches = FindByUserName(userName, 10);
        return matches.FirstOrDefault(user =>
            string.Equals(user.UserName, userName, StringComparison.OrdinalIgnoreCase));
    }

    public void Update(string userName, string firstName, string lastName, int userAccess, int? disabledReasonId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        connection.Execute(
            "SecureShare.UpdateExternalUser_Admin",
            new
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                UserAccess = userAccess,
                ExternalUserDisabledReasonId = disabledReasonId
            },
            commandType: CommandType.StoredProcedure);
    }

    public (ExternalUserCreateResult Result, Guid? UserId) Create(
        string userName,
        string firstName,
        string lastName,
        int userAccess)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        var parameters = new DynamicParameters();
        parameters.Add("@UserName", userName);
        parameters.Add("@FirstName", firstName);
        parameters.Add("@LastName", lastName);
        parameters.Add("@UserAccess", userAccess);
        parameters.Add("@ExternalUserId", dbType: DbType.Guid, direction: ParameterDirection.Output);
        parameters.Add("@RETURN_VALUE", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

        try
        {
            connection.Execute(
                "SecureShare.CreateExternalUser_Admin",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch
        {
            return (ExternalUserCreateResult.ProviderError, null);
        }

        int returnCode = parameters.Get<int>("@RETURN_VALUE");
        Guid? externalUserId = parameters.Get<Guid?>("@ExternalUserId");

        return returnCode switch
        {
            0 => (ExternalUserCreateResult.Success, externalUserId ?? Guid.NewGuid()),
            6 => (ExternalUserCreateResult.DuplicateUserName, null),
            7 => (ExternalUserCreateResult.DuplicateEmail, null),
            8 => (ExternalUserCreateResult.Rejected, null),
            _ => (ExternalUserCreateResult.ProviderError, null)
        };
    }

    private static ExternalUser MapUser(ExternalUserRow row)
    {
        return new ExternalUser
        {
            UserId = ParseGuid(row.ExternalUserId),
            UserName = row.Email ?? row.UserName ?? string.Empty,
            Email = row.Email ?? string.Empty,
            Name = row.Name ?? string.Empty,
            Comment = row.Comment ?? string.Empty,
            IsApproved = row.IsApproved,
            CreationDate = ParseDate(row.CreateDate),
            LastLoginDate = ParseDate(row.LastLoginDate)
        };
    }

    private static Guid ParseGuid(object? value)
    {
        if (value is Guid guid)
        {
            return guid;
        }

        return Guid.TryParse(Convert.ToString(value), out Guid parsed) ? parsed : Guid.Empty;
    }

    private static DateTime ParseDate(object? value)
    {
        if (value == null || value is DBNull)
        {
            return DateTime.MinValue;
        }

        if (value is DateTimeOffset dto)
        {
            return dto.ToLocalTime().DateTime;
        }

        if (value is DateTime dt)
        {
            return dt;
        }

        return DateTime.TryParse(Convert.ToString(value), out DateTime parsed) ? parsed : DateTime.MinValue;
    }

    private sealed class ExternalUserRow
    {
        public object? ExternalUserId { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public string? UserName { get; init; }
        public string? Comment { get; init; }
        public bool IsApproved { get; init; }
        public object? CreateDate { get; init; }
        public object? LastLoginDate { get; init; }
    }
}
