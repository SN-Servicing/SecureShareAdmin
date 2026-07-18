using Dapper;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Snsc.Security
{
    /// <summary>
    /// Helper class to retrieve and cache user identity information from the LmsSystem database.
    /// </summary>
    public static class UserIdentityHelper
    {
        private const string SESSION_KEY_AMSUSER_ID = "AmsUserID";
        private const string SESSION_KEY_USER_INITIALIZED = "UserInitialized";

        /// <summary>
        /// Clears the cached user identity information from the session.
        /// </summary>
        public static void ClearUserIdentity()
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session.Remove(SESSION_KEY_AMSUSER_ID);
                HttpContext.Current.Session.Remove(SESSION_KEY_USER_INITIALIZED);
            }
        }

        /// <summary>
        /// Initializes the current user's identity by retrieving their AmsUserID from the database
        /// based on their Windows NT username and storing it in the session.
        /// </summary>
        public static void InitializeUserIdentity()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null)
                return;

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return;

            // Check if already initialized
            if (HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SESSION_KEY_USER_INITIALIZED] != null)
                return;

            try
            {
                string ntUserName = HttpContext.Current.User.Identity.Name;
                // strip off the domain if present (DOMAIN\username)
                if (ntUserName.Contains("\\"))
                {
                    ntUserName = ntUserName.Substring(ntUserName.IndexOf("\\") + 1);
                }
                if (string.IsNullOrEmpty(ntUserName))
                    return;

                int? amsUserID = GetAmsUserIdByNTUserName(ntUserName);

                if (HttpContext.Current.Session != null)
                {
                    HttpContext.Current.Session[SESSION_KEY_AMSUSER_ID] = amsUserID;
                    HttpContext.Current.Session[SESSION_KEY_USER_INITIALIZED] = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(
                    string.Format("[UserIdentityHelper] Failed to initialize user identity: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the AmsUserID for the given NT username from the LmsSystem database.
        /// </summary>
        /// <param name="ntUserName">The Windows NT username (DOMAIN\username format)</param>
        /// <returns>The AmsUserID if found, otherwise null</returns>
        private static int? GetAmsUserIdByNTUserName(string ntUserName)
        {
            string connectionString = SdiDb.GetConnectionString(DatabaseTarget.LmsSystem);

            using (IDbConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Try to find stored procedure - adjust the name based on your actual stored proc
                // Common patterns: up_GetUserConfig, sp_GetUserByNTName, up_GetUserByNTUserName
                var results = connection.Query<int?>(
                    "SELECT UserID FROM [User] WHERE NTLoginName = @NTUserName",
                    new { NTUserName = ntUserName });

                var resultList = results.ToList();
                if (resultList.Count > 0)
                    return resultList[0];

                return null;
            }
        }
    }
}