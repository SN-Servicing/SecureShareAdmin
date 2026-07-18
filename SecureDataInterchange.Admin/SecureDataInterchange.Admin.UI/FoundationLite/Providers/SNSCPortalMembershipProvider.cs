using System;
using System.Data;

namespace Snsc.Foundation.Providers
{
    [Serializable]
    public class SNSCPortalMembershipProvider : System.Web.Security.MembershipProvider
    {
        private const string PASSWORDCOMMENT = "forcepasswordchange";

        private string _applicationName = "Snsc";

        private string _providerName = "AspNetSqlMembershipProvider";

        public override string ApplicationName
        {
            get
            {
                return _applicationName.Length == 0 ? "Snsc" : _applicationName;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordReset
        {
            get { return Convert.ToBoolean(GetWebConfigMembershipSectionProperty("enablePasswordReset")); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return Convert.ToBoolean(GetWebConfigMembershipSectionProperty("enablePasswordRetrieval")); }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return Convert.ToInt32(GetWebConfigMembershipSectionProperty("maxInvalidPasswordAttempts")); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return Convert.ToInt32(GetWebConfigMembershipSectionProperty("minRequiredNonAlphanumericCharacters")); }
        }

        public override int MinRequiredPasswordLength
        {
            get { return Convert.ToInt32(GetWebConfigMembershipSectionProperty("minRequiredPasswordLength")); }
        }

        public override int PasswordAttemptWindow
        {
            get { return Convert.ToInt32(GetWebConfigMembershipSectionProperty("passwordAttemptWindow")); }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get
            {
                System.Web.Security.MembershipPasswordFormat retFormat = System.Web.Security.MembershipPasswordFormat.Hashed;
                string format = GetWebConfigMembershipSectionProperty("passwordFormat").ToString();

                switch (format.ToLower())
                {
                    case "hashed":
                        retFormat = System.Web.Security.MembershipPasswordFormat.Hashed;
                        break;

                    case "encrypted":
                        retFormat = System.Web.Security.MembershipPasswordFormat.Encrypted;
                        break;

                    case "clear":
                        retFormat = System.Web.Security.MembershipPasswordFormat.Clear;
                        break;
                }

                return retFormat;
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return GetWebConfigMembershipSectionProperty("passwordStrengthRegularExpression").ToString(); }
        }

        //default provider
        public string ProviderName
        {
            get
            {
                return _providerName;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return Convert.ToBoolean(GetWebConfigMembershipSectionProperty("requiresQuestionAndAnswer")); }
        }

        public override bool RequiresUniqueEmail
        {
            get { return Convert.ToBoolean(GetWebConfigMembershipSectionProperty("requiresUniqueEmail")); }
        }

        public int ResetPasswordTimeoutDays
        {
            get { return Convert.ToInt32(GetWebConfigMembershipSectionProperty("resetPasswordTimeoutDays")); }
        }

        /// <summary>
        /// Encrypts the provided password with SHA1 into a hash
        /// suitable for text storage.
        /// </summary>
        /// <param name="password">Unencrypted password</param>
        /// <returns>Encrypted Password</returns>
        public static string GetHashedPassword(string password)
        {
            string salt = CreateSalt();
            string saltAndStr = String.Concat(password, salt);
            string hashedString = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndStr, "SHA1");

            return String.Concat(hashedString, salt);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();

            //bool retVal = false;

            //Snsc.Database.Core.aspnet_Membership_SetPassword spew = new Snsc.Database.Core.aspnet_Membership_SetPassword();
            //spew.VersionInfo43C92D75AF4C8C57CF77981B88BDDF4();
            //spew.Parameters.ApplicationName_.Value = ApplicationName;
            //spew.Parameters.UserName_.Value = username;
            //spew.Parameters.NewPassword_.Value = GetHashedPassword(newPassword);
            //spew.Parameters.PasswordFormat_.Value = this.PasswordFormat;
            //spew.Parameters.PasswordSalt_.Value = "";
            //spew.Parameters.CurrentTimeUtc_.Value = DateTime.Now.ToUniversalTime();

            //Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.NonQuery);
            //csla = csla.Execute();
            //retVal = true;

            ////remove the 'forcechangepassword' flag
            //System.Web.Security.MembershipUser mu = System.Web.Security.Membership.GetUser(username);
            //mu.Comment = "";
            //System.Web.Security.Membership.UpdateUser(mu);

            //return retVal;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            // todo...
            var userId = String.Empty;
            if (passwordQuestion == null) passwordQuestion = string.Empty;
            if (passwordAnswer == null) passwordAnswer = string.Empty;

            var perms = Convert.ToInt32(passwordQuestion);

            var fname = String.Empty;
            var lname = String.Empty;
            var split = passwordAnswer.Split('|');
            if (split.Length > 0)
            {
                fname = split[0];
                lname = split[1];
            }

            Snsc.Database.Core.SecureShare_CreateExternalUser_Admin spew = new Snsc.Database.Core.SecureShare_CreateExternalUser_Admin();
            spew.VersionInfo7FF663188C8A3613F27BC1C451C7439C();
            spew.Parameters.UserName_.Value = username;
            spew.Parameters.FirstName_.Value = fname;
            spew.Parameters.LastName_.Value = lname;
            spew.Parameters.UserAccess_.Value = perms;

            try
            {
                Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.NonQuery);
                csla = csla.Execute();
                //spew = csla.StoredProcedureExecObject as Snsc.Database.Core.SecureShare_CreateExternalUser_Admin;

                spew = csla.StoredProcedureExecObject as Snsc.Database.Core.SecureShare_CreateExternalUser_Admin;

                userId = spew.Parameters.ExternalUserId_.Value.ToString();
                if (string.IsNullOrEmpty(userId))
                    userId = Guid.NewGuid().ToString();

                switch (spew.Parameters.ReturnValue__.Value.ToString())
                {
                    case "0": //good, success
                        status = System.Web.Security.MembershipCreateStatus.Success;
                        break;

                    case "6":
                        status = System.Web.Security.MembershipCreateStatus.DuplicateUserName;
                        break;

                    case "7":
                        status = System.Web.Security.MembershipCreateStatus.DuplicateEmail;
                        break;

                    case "8":
                        status = System.Web.Security.MembershipCreateStatus.UserRejected;
                        break;

                    default:
                        status = System.Web.Security.MembershipCreateStatus.ProviderError;
                        break;
                }
            }
            catch
            {
                status = System.Web.Security.MembershipCreateStatus.ProviderError;
            }

            System.Web.Security.MembershipUser mu = new System.Web.Security.MembershipUser(ProviderName,
                username, new Guid(userId), email, passwordQuestion, "", isApproved, false,
                DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);

            return mu;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            System.Web.Security.MembershipUserCollection muc = new System.Web.Security.MembershipUserCollection();

            Snsc.Database.Core.SecureShare_GetExternalUsersByName_Admin spew = new Snsc.Database.Core.SecureShare_GetExternalUsersByName_Admin();
            spew.VersionInfoA994FE6E111B56A7688B8EFF3992A4F();
            spew.Parameters.PageSize_.Value = 10;
            spew.Parameters.PageIndex_.Value = 0;
            spew.Parameters.UserNameToMatch_.Value = usernameToMatch;

            Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.DataTable);
            csla = csla.Execute();

            DataTable dt = csla.ProcedureReturnValue as DataTable;
            //totalRecords = dt.Rows.Count;
            totalRecords = Convert.ToInt32(spew.Parameters.ReturnValue__.Value.ToString());

            foreach (DataRow dr in dt.Rows)
            {
                var mu = new SecureShareMembershipUser(dr["Name"].ToString(),
                             ProviderName,
                             dr["ExternalUserId"].ToString(),
                             dr["Email"].ToString(),
                             dr["PasswordQuestion"].ToString(),
                             dr["Comment"].ToString(),
                             Convert.ToBoolean(dr["IsApproved"]),
                             false,
                             DateTimeOffSetToDateTime(dr["CreateDate"].ToString()),
                             DateTimeOffSetToDateTime(dr["LastLoginDate"].ToString()),
                             DateTimeOffSetToDateTime(dr["LastActivityDate"].ToString()),
                             DateTimeOffSetToDateTime(dr["LastPasswordChangedDate"].ToString()),
                             DateTimeOffSetToDateTime(dr["LastLockoutDate"].ToString()));

                //System.Web.Security.MembershipUser mu = new System.Web.Security.MembershipUser(ProviderName,
                //                                            dr["UserName"].ToString(),
                //                                            dr["ExternalUserId"].ToString(),
                //                                            dr["Email"].ToString(),
                //                                            dr["PasswordQuestion"].ToString(),
                //                                            dr["Comment"].ToString(),
                //                                            Convert.ToBoolean(dr["IsApproved"]),
                //                                            false,
                //                                            DateTimeOffSetToDateTime(dr["CreateDate"].ToString()),
                //                                            DateTimeOffSetToDateTime(dr["LastLoginDate"].ToString()),
                //                                            DateTimeOffSetToDateTime(dr["LastActivityDate"].ToString()),
                //                                            DateTimeOffSetToDateTime(dr["LastPasswordChangedDate"].ToString()),
                //                                            DateTimeOffSetToDateTime(dr["LastLockoutDate"].ToString()));
                muc.Add(mu);
            }

            return muc;
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            System.Web.Security.MembershipUser mu = null;

            Snsc.Database.Core.SecureShare_GetExternalUser spew = new Snsc.Database.Core.SecureShare_GetExternalUser();
            spew.VersionInfo74EDF1772145DF672EB351E01FE75294();
            spew.Parameters.ExternalUserId_.Value = providerUserKey;

            Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.DataTable);
            csla = csla.Execute();

            DataTable dt = csla.ProcedureReturnValue as DataTable;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var theDate = DateTimeOffset.MinValue;
                if (DateTimeOffset.TryParse(dr["LastLoginDate"].ToString(), out theDate))

                    mu = new System.Web.Security.MembershipUser(
                                                    ProviderName,
                                                    dr["UserName"].ToString(),
                                                    providerUserKey.ToString(),
                                                    dr["UserName"].ToString(), // UserName == Email
                                                    dr["ExternalUserAccessMaskId"].ToString() + "|" + dr["ExternalUserDisabledReasonId"].ToString(), // PasswordQuestion OR now.. other use :)
                                                    dr["FirstName"].ToString() + "|" + dr["LastName"].ToString(),
                                                    true, // IsApproved
                                                    false, // IsLockedOut
                                                    DateTimeOffSetToDateTime(dr["CreateDate"].ToString()),
                                                    DateTimeOffSetToDateTime(dr["LastLoginDate"].ToString()),
                                                    DateTimeOffSetToDateTime(dr["LastActivityDate"].ToString()),
                                                    DateTimeOffSetToDateTime(dr["LastPasswordChangedDate"].ToString()),
                                                    DateTimeOffSetToDateTime(dr["LastLockoutDate"].ToString())
                                                    );
            }

            return mu;
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            System.Web.Security.MembershipUser mu = null;

            Snsc.Database.Core.SecureShare_GetExternalUser spew = new Snsc.Database.Core.SecureShare_GetExternalUser();
            spew.VersionInfo74EDF1772145DF672EB351E01FE75294();
            spew.Parameters.UserName_.Value = username;

            Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.DataTable);
            csla = csla.Execute();

            DataTable dt = csla.ProcedureReturnValue as DataTable;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                mu = new System.Web.Security.MembershipUser(
                                                ProviderName,
                                                username,
                                                dr["ExternalUserId"],
                                                dr["UserName"].ToString(),
                                                dr["ExternalUserAccessMaskId"].ToString() + "|" + dr["ExternalUserDisabledReasonId"].ToString(), // PasswordQuestion OR now.. other use :)
                                                dr["FirstName"].ToString() + "|" + dr["LastName"].ToString(),
                                                true, // IsApproved
                                                false, // IsLockedOut
                                                DateTimeOffSetToDateTime(dr["CreateDate"].ToString()),
                                                DateTimeOffSetToDateTime(dr["LastLoginDate"].ToString()),
                                                DateTimeOffSetToDateTime(dr["LastActivityDate"].ToString()),
                                                DateTimeOffSetToDateTime(dr["LastPasswordChangedDate"].ToString()),
                                                DateTimeOffSetToDateTime(dr["LastLockoutDate"].ToString())
                                                );
            }

            return mu;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            _providerName = name;
            _applicationName = config["applicationName"];
            base.Initialize(name, config);
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();

            //string newPassword = System.Web.Security.Membership.GeneratePassword(9, 0);

            //Snsc.Database.Core.aspnet_Membership_ResetPassword spew = new Snsc.Database.Core.aspnet_Membership_ResetPassword();
            //spew.VersionInfoCFF25C591C76E1379FBE89A7AA9384();
            //spew.Parameters.ApplicationName_.Value = ApplicationName;
            //spew.Parameters.UserName_.Value = username;
            //spew.Parameters.NewPassword_.Value = GetHashedPassword(newPassword);
            //spew.Parameters.PasswordFormat_.Value = this.PasswordFormat;
            //spew.Parameters.PasswordSalt_.Value = "";
            //spew.Parameters.CurrentTimeUtc_.Value = DateTime.Now.ToUniversalTime();
            //spew.Parameters.MaxInvalidPasswordAttempts_.Value = this.MaxInvalidPasswordAttempts;
            //spew.Parameters.PasswordAttemptWindow_.Value = this.PasswordAttemptWindow;
            //spew.Parameters.PasswordAnswer_.Value = answer;

            //Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.NonQuery);
            //csla = csla.Execute();

            ////set the 'forcechangepassword' flag
            //System.Web.Security.MembershipUser mu = GetUser(username,false);
            //mu.Comment = PASSWORDCOMMENT;
            //UpdateUser(mu);

            //return newPassword;
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            // Update by ... UserName
            // todo...
            // Create UpdateUser_Admin
            // Pass (use "other fields" as needed...)
            // Update (FirstName, LastName, Perms, Deactivation)
            // Get values out of comment split ~
            var split = user.Comment.Split('~');
            var perms = 0;
            var fname = String.Empty;
            var lname = String.Empty;
            var enabledflag = 0;
            if (split.Length > 0)
            {
                // permissions at front
                perms = Convert.ToInt32(split[0]);

                // split name now
                var splitname = split[1].Split('|');
                if (splitname.Length > 0)
                {
                    fname = splitname[0];
                    lname = splitname[1];
                }
                enabledflag = Convert.ToInt32(split[2]);
            }

            Snsc.Database.Core.SecureShare_UpdateExternalUser_Admin spew = new Snsc.Database.Core.SecureShare_UpdateExternalUser_Admin();
            spew.VersionInfoE68FA35B08F17139AF753F57ADFA2A2();
            spew.Parameters.UserName_.Value = user.UserName;
            spew.Parameters.FirstName_.Value = fname;
            spew.Parameters.LastName_.Value = lname;
            spew.Parameters.UserAccess_.Value = perms;
            spew.Parameters.ExternalUserDisabledReasonId_.Value = enabledflag;

            Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.NonQuery);
            csla = csla.Execute();
        }

        public override bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();

            //string dbPassword = string.Empty;
            //Snsc.Database.Core.aspnet_Membership_GetPassword spew = new Snsc.Database.Core.aspnet_Membership_GetPassword();

            //spew.VersionInfoD64AF958A7532CA85F8DAE1C8A4A664();
            //spew.Parameters.UserName_.Value = username;
            //spew.Parameters.ApplicationName_.Value = ApplicationName;
            //spew.Parameters.CurrentTimeUtc_.Value = DateTime.Now.ToUniversalTime();
            //spew.Parameters.MaxInvalidPasswordAttempts_.Value = this.MaxInvalidPasswordAttempts;
            //spew.Parameters.PasswordAttemptWindow_.Value = this.PasswordAttemptWindow;

            //Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.DataTable);
            //csla = csla.Execute();
            //DataTable dt = csla.ProcedureReturnValue as DataTable ;

            //if (dt.Rows.Count > 0)
            //{
            //    dbPassword = dt.Rows[0][0].ToString(); //password column | no column name provided
            //}

            //bool valid = false;
            //System.Web.Security.MembershipUser mu = GetUser(username, true);
            //if (mu != null)
            //{
            //    if (mu.Comment.Contains(PASSWORDCOMMENT) && mu.LastPasswordChangedDate.AddDays(this.ResetPasswordTimeoutDays) < DateTime.Now)
            //    {
            //        //they have not logged in since their password was reset (in the required time frame)
            //        //so they cannot get in... they must reset their pw again
            //        valid = false;
            //    }
            //    else
            //    {
            //        valid = ValidatePassword(password, dbPassword);
            //    }
            //}

            //return valid;
        }

        internal static string CreateSalt()
        {
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            byte[] buff = new byte[10];
            rng.GetBytes(buff);
            string salt = Convert.ToBase64String(buff);
            return salt;
        }

        private static bool ValidatePassword(string enteredPwd, string databasePwd)
        {
            //this is borrower web password logic (snsc.servicing)
            bool passwordMatched = false;
            string salt;
            int length = databasePwd.Length;

            if (length <= 16)
                passwordMatched = false;
            else
            {
                salt = databasePwd.Substring((databasePwd.Length - 16));

                string saltAndStr = String.Concat(enteredPwd, salt);
                string hashedString = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndStr, "SHA1");

                if (databasePwd != String.Concat(hashedString, salt))
                    passwordMatched = false;
                else
                    passwordMatched = true;
            }
            return passwordMatched;
        }

        private DateTime DateTimeOffSetToDateTime(string theDate)
        {
            var newDate = DateTimeOffset.UtcNow;
            DateTimeOffset.TryParse(theDate, out newDate); // Not using if because will just return UtcNow anyhow.
            return newDate.DateTime;
        }

        private object GetWebConfigMembershipSectionProperty(string propertyName)
        {
            System.Web.Configuration.MembershipSection ms = System.Web.Configuration.WebConfigurationManager.GetSection("system.web/membership") as System.Web.Configuration.MembershipSection;
            return ms.Providers[ProviderName].Parameters[propertyName];
        }
    }
}