using Snsc.FileTransfers.SecureDataInterchange.Business;
using Snsc.Foundation.InvestorPortal;
using Snsc.Foundation.Providers;
using System;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureDataInterchange.Admin.Admin.access
{
    public partial class EditUser : Page
    {
        private SecureDataInterchange.Admin.Common.AdminMaster master;
        private MembershipUser user;
        private string username;
        private UserZoneInfo uzi;

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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewUsers.aspx");
        }

        protected void btnCancelEdit_Click(object sender, EventArgs e)
        {
            this.btnEditUser.Visible = true;
            this.btnSaveEdit.Visible = false;
            this.btnCancelEdit.Visible = false;

            this.pnlCurrentZones.Visible = true;
            this.pnlEditZones.Visible = false;
        }

        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            //Membership.DeleteUser(username, false); // DC: My apps will NEVER delete the related data.
            Membership.DeleteUser(username, true); // DC: except during testing, of course!
            Snsc.Foundation.SDI.Logging.InsertNewLog(Snsc.Foundation.Enumerations.SdiLogSubType.UserDelete,
                user.UserName, null, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, null,
                Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);

            Response.Redirect("ViewUsers.aspx");
        }

        protected void btnEditUser_Click(object sender, EventArgs e)
        {
            this.btnEditUser.Visible = false;
            this.btnSaveEdit.Visible = true;
            this.btnCancelEdit.Visible = true;

            this.pnlCurrentZones.Visible = false;
            this.pnlEditZones.Visible = true;

            ucEditUserZones.SetUserZoneInfo(uzi);
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            string newPassword = user.ResetPassword();
            SendNewAccountEmail(user, newPassword);
            Snsc.Foundation.SDI.Logging.InsertNewLog(Snsc.Foundation.Enumerations.SdiLogSubType.UserEdit,
                user.UserName, "Password Change", Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, null,
                Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            // Save User.
            var mp = new SNSCPortalMembershipProvider();
            var providedUserName = String.Empty;
            providedUserName = firstname.Text + '|' + lastname.Text;

            var reasonId = DisabledReasonListBox.SelectedValue;

            // cobble shit back into the "Comment"
            user.Comment = PermissionSet.ToString() + "~" + providedUserName + "~" + reasonId.ToString();
            mp.UpdateUser(user);

            if (ucEditUserZones.SaveUserZoneChanges(this.user) != -1)
            {
                btnCancelEdit_Click(this.btnCancelEdit, System.EventArgs.Empty);
                RefreshZones();
            }
        }

        protected void gvCurrentZones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Remove", StringComparison.OrdinalIgnoreCase))
            {
                GridViewRow row = ((Control)e.CommandSource).NamingContainer as GridViewRow;
                if (row != null)
                {
                    int zoneID = Convert.ToInt32(gvCurrentZones.DataKeys[row.RowIndex].Value);
                    FileSharingZoneInfo zone = uzi.Zones.SingleOrDefault(x => x.ZoneID == zoneID);
                    if (zone != null)
                    {
                        AdministrationData.DeleteZonePermission.DeleteZonePermissionByID(zone.ZonePermissionID, username, zoneID);
                        RefreshZones();
                    }
                }

                return;
            }

            if (e.CommandArgument.ToString() == "emailnotification")
            {
                GridViewRow gvr = ((ImageButton)e.CommandSource).NamingContainer as GridViewRow;
                Label lblEmailNotification = gvr.FindControl("lblEmailNotification") as Label;
                Label lblZonePermissionID = gvr.FindControl("lblZonePermissionID") as Label;
                int amsUserID = Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID ?? 0;

                if (lblEmailNotification != null && lblZonePermissionID != null)
                {
                    Snsc.FileTransfers.SecureDataInterchange.Business.FileSharingZoneInfo.ChangeEmailNotification(
                        Convert.ToInt32(lblZonePermissionID.Text),
                        !Convert.ToBoolean(lblEmailNotification.Text),
                        null, amsUserID);

                    this.RefreshZones();
                }
            }
        }

        protected void gvCurrentZones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if the secondary value is null/empty, hide that row
            if (e.Row.DataItem != null)
            {
                FileSharingZoneInfo fszi = (FileSharingZoneInfo)e.Row.DataItem;

                if (fszi != null)
                {
                    if (fszi.SecondaryIDValue == "")
                    {
                        Label lbl = e.Row.FindControl("lblSecondary") as Label;
                        lbl.Visible = false;
                    }

                    //make sure this admin user has access to each zone that the external user has access to
                    //and disable the row if the admin user should not be able to change that zone
                    int zoneTypeID = ((FileSharingZoneInfo)e.Row.DataItem).ZoneTypeID;
                    if (master.UserAdministrationData.ZoneTypes.SingleOrDefault(x => x.FileSharingZoneTypeID == zoneTypeID) == null)
                    {
                        e.Row.Enabled = false;
                    }

                    //set email icon
                    ImageButton ibtn = e.Row.FindControl("btnEmail") as ImageButton;
                    ibtn.ImageUrl = (fszi.NotificationOptIn) ?
                        "~/Common/images/email.png" :
                        "~/Common/images/cross.png";
                }
            }
        }

        protected void UserInfo_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            //Need to handle the update manually because MembershipUser does not have a
            //parameterless constructor

            user.Email = (string)e.NewValues["Email"];
            user.Comment = (string)e.NewValues["Comment"];
            user.IsApproved = (bool)e.NewValues["IsApproved"];

            if (user.IsLockedOut && ((bool)e.NewValues["IsLockedOut"]) == true)
            {
                user.UnlockUser();
            }

            try
            {
                // Update user info:
                Membership.UpdateUser(user);
                Snsc.Foundation.SDI.Logging.InsertNewLog(Snsc.Foundation.Enumerations.SdiLogSubType.UserEdit,
                    user.UserName, "Membership Edit", Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, null,
                    Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);

                lblMessage.Text = "Update Successful.";

                e.Cancel = true;
                UserInfo.ChangeMode(DetailsViewMode.ReadOnly);
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Update Failed: " + ex.Message;

                e.Cancel = true;
                UserInfo.ChangeMode(DetailsViewMode.ReadOnly);
            }
        }

        private void Page_Load()
        {
            if (!IsPostBack)
            {
                ucEditUserZones.SetUserZoneInfo(uzi);
            }

            master = (SecureDataInterchange.Admin.Common.AdminMaster)Page.Master;
            username = Request.QueryString["username"];
            if (username == null || username == "")
            {
                Response.Redirect("ViewUsers.aspx");
            }
            user = Membership.GetUser(username);
            if (user == null)
            {
                Response.Redirect("ViewUsers.aspx");
            }

            if (!IsPostBack)
            {
                // set specific fields that are not bound.
                var wim = new WebIntegrationManager();

                // PasswordQuestion field is overloaded to have access permissions and account status
                // crammed into it using a special encoding technique called "pipe delimited" (sigh)
                var passwordQuestionSplitByPipes = user.PasswordQuestion.Split('|');
                if (passwordQuestionSplitByPipes.Length > 0)
                {
                    var tempperms = Convert.ToInt32(passwordQuestionSplitByPipes[0]);
                    if (((int)ExternalUserAccess.LoanAccess & tempperms) == (int)ExternalUserAccess.LoanAccess)
                    {
                        loanlevel.Checked = true;
                    }
                    if (((int)ExternalUserAccess.SDIUpload & tempperms) == (int)ExternalUserAccess.SDIUpload)
                    {
                        sdi.Checked = true;
                    }
                    if (((int)ExternalUserAccess.SecureRequests & tempperms) == (int)ExternalUserAccess.SecureRequests)
                    {
                        requests.Checked = true;
                    }

                    // set the value of the disabled reason listbox
                    // It's embedded as the second chunk of the password question field
                    string userDisabledReason = passwordQuestionSplitByPipes[1];
                    DisabledReasonListBox.SelectedValue = userDisabledReason;
                }

                // Similarly, the user's first & last names are also "encoded" into
                // the "Comment" field
                var commentSplitByPipes = user.Comment.Split('|');
                if (commentSplitByPipes.Length > 0)
                {
                    firstname.Text = commentSplitByPipes[0];
                    lastname.Text = commentSplitByPipes[1];
                }
            }

            Guid userKey = new Guid(user.ProviderUserKey.ToString());
            uzi = UserZoneInfo.GetZoneInfoForExternalUser(userKey);

            lblMessage.Text = "";
            this.gvCurrentZones.DataKeyNames = new string[] { "ZoneID" };
            BindCurrentZones();
        }

        private void RefreshZones()
        {
            Guid userKey = new Guid(user.ProviderUserKey.ToString());
            uzi = UserZoneInfo.GetZoneInfoForExternalUser(userKey);
            BindCurrentZones();
        }

        private void BindCurrentZones()
        {
            gvCurrentZones.DataSource = uzi != null ? uzi.Zones : null;
            gvCurrentZones.DataBind();
        }

        private void SendNewAccountEmail(MembershipUser user, string password)
        {
            string msg = System.IO.File.ReadAllText(Server.MapPath("~/Common/Templates") + "/ResetPassword.htm");
            msg = msg.Replace("##PASSWORD##", password);

            System.Net.Mail.MailMessage mm = new System.Net.Mail.MailMessage("TASC@snsc.com", user.Email);
            mm.Subject = "SNSC SDI Account";
            mm.Body = msg;
            mm.IsBodyHtml = true;

            Snsc.Enterprise.Net.RemotingSmtpClient.SendEmail(mm);
        }
    }
}