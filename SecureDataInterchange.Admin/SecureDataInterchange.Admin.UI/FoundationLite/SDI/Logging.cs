using System;
using System.Data;
using System.Data.SqlClient;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.Foundation.SDI
{
    [Serializable]
    public class Logging
    {
        public long? LogID { get; private set; }
        public int LogSubTypeID { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }
        public int? PerformedByAmsUserID { get; set; }
        public Guid? PerformedByAspNetUserID { get; set; }
        public int? LogSourceId { get; set; }

        public long Execute()
        {
            using (var connection = (SqlConnection)SdiDb.CreateOpenConnection(DatabaseTarget.Core))
            {
                using (var cmd = new SqlCommand("SecureShare.up_sdii_sdiLogging", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@LogSubTypeID", SqlDbType.Int).Value = (object)LogSubTypeID;
                    cmd.Parameters.Add("@Value1", SqlDbType.VarChar, 100).Value = (object)Value1 ?? DBNull.Value;
                    cmd.Parameters.Add("@Value2", SqlDbType.VarChar, 100).Value = (object)Value2 ?? DBNull.Value;
                    cmd.Parameters.Add("@PerformedByAmsUserID", SqlDbType.Int).Value = PerformedByAmsUserID.HasValue ? (object)PerformedByAmsUserID.Value : DBNull.Value;
                    cmd.Parameters.Add("@PerformedByAspNetUserID", SqlDbType.UniqueIdentifier).Value = PerformedByAspNetUserID.HasValue ? (object)PerformedByAspNetUserID.Value : DBNull.Value;
                    cmd.Parameters.Add("@LogSourceId", SqlDbType.Int).Value = LogSourceId.HasValue ? (object)LogSourceId.Value : DBNull.Value;

                    SqlParameter logIdParam = cmd.Parameters.Add("@LogID", SqlDbType.BigInt);
                    logIdParam.Direction = ParameterDirection.Output;

                    cmd.ExecuteNonQuery();

                    LogID = logIdParam.Value == DBNull.Value ? (long?)null : Convert.ToInt64(logIdParam.Value);
                }
            }

            return LogID ?? 0;
        }

        public static void InsertNewLog(Enumerations.SdiLogSubType subType, string value1, string value2, int? amsUserID, Guid? aspNetUserID, Enumerations.SdiLogSource source)
        {
            var log = new Logging
            {
                LogSubTypeID = (int)subType,
                LogSourceId = (int)source,
                Value1 = value1,
                Value2 = string.IsNullOrEmpty(value2) ? null : value2,
                PerformedByAmsUserID = amsUserID,
                PerformedByAspNetUserID = aspNetUserID
            };

            log.Execute();
        }
    }
}
