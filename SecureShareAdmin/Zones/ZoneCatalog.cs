using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Snsc.SecureShareAdmin.Data;
using Snsc.SecureShareAdmin.Domain;

namespace Snsc.SecureShareAdmin.Zones;

public sealed class ZoneCatalog
{
    private readonly AppDatabase _database;

    public ZoneCatalog(AppDatabase database)
    {
        _database = database;
    }

    public IReadOnlyList<Zone> GetZonesForAmsUser(int amsUserId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        IEnumerable<ZoneRow> rows = connection.Query<ZoneRow>(
            "SecureShare.up_SDIs_FileSharingAdminZonesForAmsUser",
            new { AmsUserID = amsUserId },
            commandType: CommandType.StoredProcedure);

        return rows.Select(MapZone).ToList();
    }

    public bool CanUserAdministerZone(int amsUserId, int zoneId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        CanAdministerRow? row = connection.QuerySingleOrDefault<CanAdministerRow>(
            "SecureShare.up_SDIs_CanUserAdministerZone",
            new { AmsUserID = amsUserId, FileSharingZoneID = zoneId },
            commandType: CommandType.StoredProcedure);

        return row?.CanAdminister == true;
    }

    public IReadOnlyList<Zone> GetZonesForExternalUser(Guid aspNetUserId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        IEnumerable<ZoneRow> rows = connection.Query<ZoneRow>(
            "SecureShare.up_SDIs_FileSharingZonesForUser",
            new { AspNetUserID = aspNetUserId },
            commandType: CommandType.StoredProcedure);

        return rows.Select(MapZone).ToList();
    }

    public IReadOnlyList<ZoneType> GetDistinctZoneTypes(IEnumerable<Zone> zones)
    {
        return zones
            .Where(zone => zone.ZoneTypeId > 0)
            .GroupBy(zone => zone.ZoneTypeId)
            .Select(group =>
            {
                Zone sample = group.First();
                return new ZoneType
                {
                    ZoneTypeId = sample.ZoneTypeId,
                    Name = sample.ZoneTypeName,
                    PrimaryObjectIdFieldName = sample.PrimaryFieldName,
                    SecondaryObjectIdFieldName = sample.SecondaryFieldName
                };
            })
            .OrderBy(zoneType => zoneType.Name)
            .ToList();
    }

    public Zone? GetZone(int amsUserId, int zoneId)
    {
        if (!CanUserAdministerZone(amsUserId, zoneId))
        {
            return null;
        }

        return GetZonesForAmsUser(amsUserId).FirstOrDefault(zone => zone.ZoneId == zoneId);
    }

    public IReadOnlyList<ZoneUser> GetUsersForZone(int zoneId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        using var reader = connection.ExecuteReader(
            "SecureShare.up_SDIs_sdiUsersByZone",
            new { ZoneID = zoneId },
            commandType: CommandType.StoredProcedure);

        var users = new List<ZoneUser>();
        while (reader.Read())
        {
            users.Add(new ZoneUser
            {
                UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                UserName = reader.GetString(reader.GetOrdinal("UserName")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                LastLoginDate = ReadDateTime(reader, "LastLoginDate"),
                ZonePermissionId = reader.GetInt32(reader.GetOrdinal("ZonePermissionID"))
            });
        }

        return users.OrderBy(user => user.UserName).ToList();
    }

    public int CreateZone(string primaryObjectId, string secondaryObjectId, int zoneTypeId, string description)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        var parameters = new DynamicParameters();
        parameters.Add("@FileSharingZoneID", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@PrimaryObjectID", primaryObjectId, DbType.String, size: 100);
        parameters.Add("@SecondaryObjectID", secondaryObjectId, DbType.String, size: 100);
        parameters.Add("@FileSharingZoneTypeID", zoneTypeId, DbType.Int32);
        parameters.Add("@Description", description, DbType.String, size: 1024);

        try
        {
            connection.Execute(
                "SecureShare.up_SDIi_sdiFileSharingZone",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            return -1;
        }

        return parameters.Get<int>("@FileSharingZoneID");
    }

    public int UpdateZone(int zoneId, int zoneTypeId, string description, string primaryValue, string secondaryValue)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        try
        {
            connection.Execute(
                "SecureShare.up_SDIu_sdiFileSharingZone",
                new
                {
                    FileSharingZoneID = zoneId,
                    FileSharingZoneTypeID = zoneTypeId,
                    Description = description,
                    PrimaryObjectID = primaryValue,
                    SecondaryObjectID = secondaryValue
                },
                commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Number == 2627)
        {
            return -1;
        }

        return 0;
    }

    public void DeleteZone(int zoneId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        connection.Execute(
            "SecureShare.up_SDId_sdiFileSharingZone",
            new { FileSharingZoneID = zoneId },
            commandType: CommandType.StoredProcedure);
    }

    public void AddUserPermission(int zoneId, Guid userId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        var parameters = new DynamicParameters();
        parameters.Add("@ZonePermissionID", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
        parameters.Add("@FileSharingZoneID", zoneId, DbType.Int32);
        parameters.Add("@AspNetUserID", userId, DbType.Guid);
        connection.Execute(
            "SecureShare.up_SDIi_sdiFileSharingZonePermission",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public void RemoveUserPermission(int zonePermissionId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        connection.Execute(
            "SecureShare.up_SDId_sdiFileSharingZonePermission",
            new { ZonePermissionID = zonePermissionId },
            commandType: CommandType.StoredProcedure);
    }

    public void SetAmsEmailNotification(int zoneId, bool notificationOptIn, int amsUserId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        var parameters = new DynamicParameters();
        parameters.Add("@FileSharingZoneID", zoneId, DbType.Int32);
        parameters.Add("@AmsUserID", amsUserId, DbType.Int64);
        parameters.Add("@NotificationOptIn", notificationOptIn, DbType.Boolean);
        parameters.Add("@nullMap", null, DbType.Binary, size: 1);
        connection.Execute(
            "SecureShare.up_SDIiu_sdiFileSharingZonePermissionAmsUser",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    private static Zone MapZone(ZoneRow row)
    {
        return new Zone
        {
            ZoneId = row.FileSharingZoneID,
            ZonePermissionId = row.ZonePermissionID ?? -1,
            ZoneTypeId = row.FileSharingZoneTypeID ?? -1,
            ZoneTypeName = row.ZoneTypeName ?? string.Empty,
            Description = row.Description ?? string.Empty,
            PrimaryFieldName = row.PrimaryObjectIDFieldName ?? string.Empty,
            PrimaryIdValue = row.PrimaryObjectID ?? string.Empty,
            SecondaryFieldName = row.SecondaryObjectIDFieldName ?? string.Empty,
            SecondaryIdValue = row.SecondaryObjectID ?? string.Empty,
            FileCount = row.FileCount,
            LastUploadDateUtc = row.LastUploadDateUTC,
            NotificationOptIn = row.NotificationOptIn ?? false
        };
    }

    private static DateTime ReadDateTime(IDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal))
        {
            return DateTime.MinValue;
        }

        object raw = reader.GetValue(ordinal);
        return raw switch
        {
            DateTimeOffset dto => dto.ToLocalTime().DateTime,
            DateTime dt => dt,
            _ => DateTime.MinValue
        };
    }

    private sealed class CanAdministerRow
    {
        public int AmsUserID { get; init; }
        public int FileSharingZoneID { get; init; }
        public bool CanAdminister { get; init; }
    }

    private sealed class ZoneRow
    {
        public string? Description { get; init; }
        public int FileCount { get; init; }
        public int FileSharingZoneID { get; init; }
        public int? FileSharingZoneTypeID { get; init; }
        public DateTime? LastUploadDateUTC { get; init; }
        public bool? NotificationOptIn { get; init; }
        public string? PrimaryObjectID { get; init; }
        public string? PrimaryObjectIDFieldName { get; init; }
        public string? SecondaryObjectID { get; init; }
        public string? SecondaryObjectIDFieldName { get; init; }
        public string? ZoneTypeName { get; init; }
        public int? ZonePermissionID { get; init; }
    }
}
