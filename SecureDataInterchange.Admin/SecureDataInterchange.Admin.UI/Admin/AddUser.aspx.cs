using Snsc.Foundation.InvestorPortal;
using Snsc.Foundation.Providers;
using System;
using System.Web.Security;

namespace SecureDataInterchange.Admin.Admin.access
{
    public partial class AddUser : System.Web.UI.Page
    {
        private int PermissionSet
        {
            get
            {
                var actualSet = loanlevel.Checked ? (ExternalUserAccess.LoanAccess ^ ExternalUserAccess.LoanDocuments) : 0;
                actualSet = actualSet ^ (sdi.Checked ? (ExternalUserAccess.SDIUpload ^ ExternalUserAccess.SDIDownload) : 0);
                actualSet = actualSet ^ (requests.Checked ? (ExternalUserAccess.SecureRequests) : 0);
                return (int)actualSet;
            }
        }

        protected void AddNewUser()
        {
            //string pw = System.Web.Security.Membership.GeneratePassword(6, 1);
            MembershipCreateStatus mcs;
            var wim = new WebIntegrationManager();

            // get the permission set from the form and pass it in as the passwordquestion ... yay!!!
            // as well as the username (first | last) in answer.. yay x2 !!
            // Some of the permissions are not included today...

            var mp = new SNSCPortalMembershipProvider();
            var providedUserName = String.Empty;
            providedUserName = firstname.Text + '|' + lastname.Text;

            MembershipUser newUser = mp.CreateUser(email.Text, String.Empty, email.Text, PermissionSet.ToString(), providedUserName, true, null, out mcs);
            //MembershipUser newUser = Membership.CreateUser(email.Text, String.Empty, email.Text, permissionSet, email.Text, true, null, out mcs);

            if (mcs != MembershipCreateStatus.Success)
            {
                throw new Exception(mcs.ToString());
            }
            else
            {
                Snsc.Foundation.SDI.Logging.InsertNewLog(Snsc.Foundation.Enumerations.SdiLogSubType.UserCreation,
                    newUser.UserName, null, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, null,
                    Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);

                //newUser.Comment = "forcepasswordchange";
                //Membership.UpdateUser(newUser);
                ucEditUserZones.SaveUserZoneChanges(newUser);

                // todo: Send Password similar/refactor that one... to one from "New" My Zone that directs to site
                wim.SendPasswordResetEmailLink(newUser.UserName);
                //SendNewAccountEmail(newUser, pw);
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                if (PermissionSet == 0)
                {
                    this.lblError.Text = "You must supply permissions";
                    return;
                }

                this.lblError.Text = "";
                // if the user doesn't have SDI Access ... no need to enforce this.
                var hasSDI = (((int)ExternalUserAccess.SDIDownload & PermissionSet) == (int)ExternalUserAccess.SDIDownload);
                if (hasSDI && (!ucEditUserZones.AddingNewZone && ucEditUserZones.SelectedZonesCount < 1)) //using current zone
                {
                    this.lblError.Text = "No zones selected";
                    return;
                }

                AddNewUser();

                Response.Redirect("ViewUsers.aspx");
            }
            catch (Exception ex)
            {
                lblError.Text = "Insert Failure: " + ex.Message;
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewUsers.aspx");
        }

        private void Page_Load()
        {
        }

        private void SendNewAccountEmail(MembershipUser user, string password)
        {
            string msg = System.IO.File.ReadAllText(Server.MapPath("~/Common/Templates") + "/NewAccount.htm");
            msg = msg.Replace("##PASSWORD##", password);

            System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("TASC@snsc.com", user.Email);
            mm.Subject = "New SNSC SDI Account";
            mm.Body = msg;
            mm.IsBodyHtml = true;

            Snsc.Enterprise.Net.RemotingSmtpClient.SendEmail(mm);
        }
    }
}