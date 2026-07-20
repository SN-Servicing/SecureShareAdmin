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

        IEnumerable<ExternalUserListRow> rows = connection.Query<ExternalUserListRow>(
            "SecureShare.GetExternalUsersByName_Admin",
            parameters,
            commandType: CommandType.StoredProcedure);

        return rows.Select(MapListUser).ToList();
    }

    public ExternalUserAdminDetails? GetDetailsForAdmin(Guid externalUserId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        using SqlMapper.GridReader multi = connection.QueryMultiple(
            "SecureShare.up_SDIs_GetUserDetailsForAdmin",
            new { ExternalUserId = externalUserId },
            commandType: CommandType.StoredProcedure);

        ExternalUserDetailsRow? row = multi.Read<ExternalUserDetailsRow>().FirstOrDefault();
        if (row == null)
        {
            return null;
        }

        IReadOnlyList<ExternalUserDisabledReason> reasons = multi.Read<DisabledReasonRow>()
            .Select(MapDisabledReason)
            .ToList();

        return new ExternalUserAdminDetails
        {
            User = MapDetailsUser(row),
            DisabledReasons = reasons
        };
    }

    public LoanLevelAccessValidation ValidateAccountForLoanLevelAccess(Guid externalUserId)
    {
        try
        {
            using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
            using SqlMapper.GridReader multi = connection.QueryMultiple(
                "SecureShare.up_SDIs_ValidateAccountForLoanLevelAccess",
                new { ExternalUserID = externalUserId },
                commandType: CommandType.StoredProcedure);

            LoanLevelDirectoryRow? directoryRow = multi.Read<LoanLevelDirectoryRow>().FirstOrDefault();
            IReadOnlyList<string> groups = multi.Read<InvestorAccessGroupRow>()
                .Select(row => row.InvestorAccessGroupName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!)
                .ToList();

            return new LoanLevelAccessValidation
            {
                DirectoryAccount = directoryRow == null ? null : MapDirectoryAccount(directoryRow),
                InvestorAccessGroups = groups
            };
        }
        catch (Exception ex)
        {
            return new LoanLevelAccessValidation
            {
                ErrorMessage = $"Unable to load loan access dependencies: {ex.Message}"
            };
        }
    }

    public ExternalUser? GetByUserId(Guid externalUserId)
    {
        return GetDetailsForAdmin(externalUserId)?.User;
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
            0 => (ExternalUserCreateResult.Success, externalUserId ?? Guid.Empty),
            6 => (ExternalUserCreateResult.DuplicateUserName, null),
            7 => (ExternalUserCreateResult.DuplicateEmail, null),
            8 => (ExternalUserCreateResult.Rejected, null),
            _ => (ExternalUserCreateResult.ProviderError, null)
        };
    }

    private static ExternalUser MapListUser(ExternalUserListRow row)
    {
        string firstName = row.FirstName ?? string.Empty;
        string lastName = row.LastName ?? string.Empty;
        string displayName = row.Name ?? string.Empty;
        if (string.IsNullOrWhiteSpace(displayName))
        {
            displayName = $"{firstName} {lastName}".Trim();
        }

        return new ExternalUser
        {
            UserId = ParseGuid(row.ExternalUserId),
            UserName = row.Email ?? row.UserName ?? string.Empty,
            Email = row.Email ?? string.Empty,
            FirstName = firstName,
            LastName = lastName,
            Name = displayName,
            Comment = row.Comment ?? string.Empty,
            IsApproved = row.IsApproved,
            CreationDate = ParseDate(row.CreateDate),
            LastLoginDate = ParseDate(row.LastLoginDate)
        };
    }

    private static ExternalUser MapDetailsUser(ExternalUserDetailsRow row)
    {
        string firstName = row.FirstName ?? string.Empty;
        string lastName = row.LastName ?? string.Empty;
        string displayName = $"{firstName} {lastName}".Trim();
        if (string.IsNullOrEmpty(displayName))
        {
            displayName = row.SNUserAccount ?? row.UserName ?? string.Empty;
        }

        return new ExternalUser
        {
            UserId = row.ExternalUserId,
            UserName = row.UserName ?? string.Empty,
            Email = row.UserName ?? string.Empty,
            FirstName = firstName,
            LastName = lastName,
            Name = displayName,
            AccessMask = row.ExternalUserAccessMaskId,
            DisabledReasonId = row.ExternalUserDisabledReasonId,
            Comment = row.DisabledReasonDescription ?? string.Empty,
            CreationDate = ParseDate(row.CreateDate),
            LastLoginDate = ParseDate(row.LastLoginDate)
        };
    }

    private static ExternalUserDisabledReason MapDisabledReason(DisabledReasonRow row)
    {
        return new ExternalUserDisabledReason
        {
            ExternalUserDisabledReasonId = row.ExternalUserDisabledReasonId,
            Name = row.Name ?? string.Empty,
            Description = row.Description ?? string.Empty,
            AllowAutomaticReenable = row.AllowAutomaticReenable
        };
    }

    private static LoanLevelDirectoryAccount MapDirectoryAccount(LoanLevelDirectoryRow row)
    {
        return new LoanLevelDirectoryAccount
        {
            ExternalUserId = row.ExternalUserId,
            AmsUserId = row.AmsUserID,
            AmsFirstName = NullIfWhiteSpace(row.AmsFirstName),
            AmsLastName = NullIfWhiteSpace(row.AmsLastName),
            AmsEmail = NullIfWhiteSpace(row.AmsEmail),
            AmsAccountIsEnabled = row.AmsAccountIsEnabled,
            AmsAccountDisabledDate = row.AmsAccountDisabledDate,
            ActiveDirectorySid = NullIfWhiteSpace(row.ActiveDirectorySID),
            AdFirstName = NullIfWhiteSpace(row.ADFirstName),
            AdLastName = NullIfWhiteSpace(row.ADLastName),
            AdEmail = NullIfWhiteSpace(row.ADEmail),
            AdAccountIsEnabled = row.ADAccountIsEnabled,
            EnteredTerminatedUsersDate = row.EnteredTerminatedUsersDate
        };
    }

    private static string? NullIfWhiteSpace(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
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

    private sealed class ExternalUserListRow
    {
        public object? ExternalUserId { get; init; }
        public string? Name { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public string? Email { get; init; }
        public string? UserName { get; init; }
        public string? Comment { get; init; }
        public bool IsApproved { get; init; }
        public object? CreateDate { get; init; }
        public object? LastLoginDate { get; init; }
    }

    private sealed class ExternalUserDetailsRow
    {
        public Guid ExternalUserId { get; init; }
        public string? UserName { get; init; }
        public string? FirstName { get; init; }
        public string? LastName { get; init; }
        public int ExternalUserAccessMaskId { get; init; }
        public object? LastLoginDate { get; init; }
        public int ExternalUserDisabledReasonId { get; init; }
        public string? DisabledReasonDescription { get; init; }
        public int ExternalUserNotificationSettingMaskId { get; init; }
        public object? CreateDate { get; init; }
        public string? SNUserAccount { get; init; }
    }

    private sealed class DisabledReasonRow
    {
        public int ExternalUserDisabledReasonId { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public bool AllowAutomaticReenable { get; init; }
    }

    private sealed class LoanLevelDirectoryRow
    {
        public Guid ExternalUserId { get; init; }
        public int? AmsUserID { get; init; }
        public string? AmsFirstName { get; init; }
        public string? AmsLastName { get; init; }
        public string? AmsEmail { get; init; }
        public bool? AmsAccountIsEnabled { get; init; }
        public DateTime? AmsAccountDisabledDate { get; init; }
        public string? ActiveDirectorySID { get; init; }
        public string? ADFirstName { get; init; }
        public string? ADLastName { get; init; }
        public string? ADEmail { get; init; }
        public bool? ADAccountIsEnabled { get; init; }
        public DateTime? EnteredTerminatedUsersDate { get; init; }
    }

    private sealed class InvestorAccessGroupRow
    {
        public string? InvestorAccessGroupName { get; init; }
    }
}
