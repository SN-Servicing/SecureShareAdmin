using System.Data;
using Dapper;
using Snsc.SecureShareAdmin.Data;
using Snsc.SecureShareAdmin.Domain;

namespace Snsc.SecureShareAdmin.Zones;

public sealed class ZoneFileCatalog
{
    private readonly AppDatabase _database;

    public ZoneFileCatalog(AppDatabase database)
    {
        _database = database;
    }

    public IReadOnlyList<SharedFile> GetFilesByZoneId(int zoneId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        IEnumerable<FileRow> rows = connection.Query<FileRow>(
            "SecureShare.up_SDIs_SharedFilesByZone",
            new { ZoneID = zoneId },
            commandType: CommandType.StoredProcedure);

        return rows.Select(row => new SharedFile
        {
            SharedFileId = row.SharedFileID,
            Name = row.Name ?? string.Empty,
            Description = row.Description ?? string.Empty,
            UploadedBy = row.UploadedBy ?? string.Empty,
            CreatedOnUtc = row.CreatedOnUTC,
            FileSizeBytes = row.FileSizeBytes
        }).ToList();
    }

    public void DeleteFile(int sharedFileId)
    {
        using IDbConnection connection = _database.CreateOpenConnection(DatabaseTarget.Core);
        connection.Execute(
            "SecureShare.up_SDId_sdiSharedFile",
            new { SharedFileID = sharedFileId },
            commandType: CommandType.StoredProcedure);
    }

    private sealed class FileRow
    {
        public int SharedFileID { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public string? UploadedBy { get; init; }
        public DateTime CreatedOnUTC { get; init; }
        public decimal? FileSizeBytes { get; init; }
    }
}

