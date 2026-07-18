using Snsc.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Snsc.Foundation.InvestorPortal
{
    public class ExternalUser
    {
        // Add more if required.
        public Guid ExternalUserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UserMasterId { get; set; }
    }

    [Flags]
    public enum ExternalUserAccess
    {
        LoanAccess = 1, 
        SDIUpload = 2,
        SDIDownload = 4,
	    SecureRequests = 8,
	    ExpressAccess = 16,
    	LoanDocuments = 32
    }


    public class WebIntegrationManager
    {
        private string GetNewAccountEmail()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetCallingAssembly();
            using (System.IO.StreamReader strm = new System.IO.StreamReader(asm.GetManifestResourceStream("Snsc.Foundation.SDI.NewAccount.htm")))
            {
                return strm.ReadToEnd();
            }
        }

        public void SendPasswordResetEmailLink(string userName)
        {
            string msg = GetNewAccountEmail();

            var site = "https://{0}/Account/ForgotPassword";
            switch (Snsc.Configuration.ConfigurationManager.Deployment.EnvironmentName.ToLower())
            {
                case "production":
                    {
                        site = string.Format(site, "investorreporting.snsc.com");
                        break;
                    }
                case "qaregression":
                    {
                        site = string.Format(site, "investorreporting-stg.snsc.com");
                        break;
                    }
                default:
                    {
                        site = String.Empty;
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(site))
            {
                msg = msg.Replace("##SITE##", site);
                System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("TASC@snsc.com", userName);
                mm.Subject = "New SNSC SDI Account";
                mm.Body = msg;
                mm.IsBodyHtml = true;

                Snsc.Enterprise.Net.RemotingSmtpClient.SendEmail(mm);
            }
        }

        public ExternalUser GetExternalUser(string userName)
        {
            Snsc.Database.Core.SecureShare_GetExternalUser spew = new Snsc.Database.Core.SecureShare_GetExternalUser();
            spew.VersionInfo74EDF1772145DF672EB351E01FE75294();
            spew.Parameters.UserName_.Value = userName;
            Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.DataTable);
            csla = csla.Execute();

            DataTable dt = csla.ProcedureReturnValue as DataTable;
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                return new ExternalUser
                {
                    ExternalUserId = new Guid(dr["ExternalUserId"].ToString()),
                    UserName = dr["UserName"].ToString(),
                    UserMasterId = Convert.ToInt32(dr["UserMasterId"]),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString()
                };
            }
            return null;
        }

        public ExternalUser CreateExternalUser(string userName, string firstName, string lastName, ExternalUserAccess userAccess)
        {
            Snsc.Database.Core.SecureShare_CreateExternalUser spew = new Snsc.Database.Core.SecureShare_CreateExternalUser();
            spew.VersionInfo23FCD5A4228AA1E62EFCF937DA3EB258();
            spew.Parameters.UserName_.Value = userName;
            spew.Parameters.FirstName_.Value = firstName;
            spew.Parameters.LastName_.Value = lastName;
            spew.Parameters.UserAccess_.Value = (int)userAccess;
            Snsc.Base.Business.CslaExecutor csla = Snsc.Base.Business.CslaExecutor.NewCslaExecutor(spew, Snsc.Base.Business.DatabaseCommandExecutionStyle.NonQuery);
            csla = csla.Execute();

            var eu = GetExternalUser(userName);
            return eu;
        }
    }
}