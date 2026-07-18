using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{ 
	[Serializable]
	public class AspNetUserList : List<AspNetUser>
	{
        public static AspNetUserList GetAspNetUsersByZoneID(int zoneID)
        {
            var result = new AspNetUserList();
            using (var connection = SdiDb.CreateOpenConnection(DatabaseTarget.Core))
            {
                using (var reader = connection.ExecuteReader(
                    "SecureShare.up_SDIs_sdiUsersByZone",
                    new { ZoneID = zoneID },
                    commandType: CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        DateTime lastLoginDate = DateTime.MinValue;
                        int lastLoginOrdinal = reader.GetOrdinal("LastLoginDate");
                        if (!reader.IsDBNull(lastLoginOrdinal))
                        {
                            object raw = reader.GetValue(lastLoginOrdinal);
                            if (raw is DateTimeOffset dto)
                            {
                                lastLoginDate = dto.ToLocalTime().DateTime;
                            }
                            else if (raw is DateTime dt)
                            {
                                lastLoginDate = dt;
                            }
                        }

                        result.Add(new AspNetUser
                        {
                            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            LastLoginDate = lastLoginDate,
                            ZonePermissionID = reader.GetInt32(reader.GetOrdinal("ZonePermissionID"))
                        });
                    }
                }
            }

            return result;
        }
	}
}
