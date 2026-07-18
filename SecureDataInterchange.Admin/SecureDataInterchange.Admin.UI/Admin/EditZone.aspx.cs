using SecureDataInterchange.Admin.Admin.access;
using SecureDataInterchange.Admin.Common;
using SecureDataInterchange.Admin.Common.UserControls;
using Snsc.FileTransfers.SecureDataInterchange.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace SecureDataInterchange.Admin.Admin
{
    public partial class EditZone : System.Web.UI.Page
    {
        private AdminMaster master;
        private int zoneID;

        protected void btnRemoveZone_Click(object sender, EventArgs e)
        {
            AdministrationData.DeleteFileSharingZoneByID(zoneID);
            GoBack();
        }

        protected void btnSaveZone_Click(object sender, EventArgs e)
        {
            ucEditUserZones.SaveZoneChages(zoneID);
            AddSelectedUsersToZone();
            BindAspNetUsers();
            ucUserList.ClearSelectedUsers();

            this.lblMessage.Visible = true;
            this.lblMessage.Text = "Zone saved";
        }

        protected void gvAspNetUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Remove", StringComparison.OrdinalIgnoreCase))
            {
                GridViewRow gvr = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                string user = gvr.Cells[1].Text;
                int zonePermissionID = Convert.ToInt32(e.CommandArgument);

                AdministrationData.DeleteZonePermission.DeleteZonePermissionByID(zonePermissionID, user, zoneID);

                BindAspNetUsers();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ucEditUserZones.SetMode(EditUserZones.Mode.NewZone);
            this.ucUserList.Mode = UserList.GridMode.CheckUserAllowed;

            try { zoneID = Convert.ToInt32(Request.QueryString["ZoneID"]); }
            catch { Response.Redirect("ViewZones.aspx"); }

            if (!IsPostBack)
            {
                ucEditUserZones.Load += new EventHandler(ucEditUserZones_Load);
            }

            master = (AdminMaster)this.Page.Master;
            BindAspNetUsers();

            this.lblMessage.Visible = false;
            if (!IsPostBack && Request.QueryString["saved"] == "1")
            {
                this.lblMessage.Text = "Zone saved";
                this.lblMessage.Visible = true;
            }
        }

        private void AddSelectedUsersToZone()
        {
            List<string> existingUserNames = GetExistingZoneUserNames();

            foreach (MembershipUser user in ucUserList.SelectedUsers)
            {
                if (existingUserNames.Contains(user.UserName))
                {
                    continue;
                }

                Guid userKey = new Guid(user.ProviderUserKey.ToString());
                AdministrationData.InsertFileSharingZonePermission(zoneID, userKey, user.UserName);
            }
        }

        private List<string> GetExistingZoneUserNames()
        {
            if (master == null || master.UserAdministrationData == null)
            {
                return new List<string>();
            }

            return master.UserAdministrationData
                .GetAspNetUsersByZoneID(zoneID)
                .Select(user => user.UserName)
                .ToList();
        }

        private void BindAspNetUsers()
        {
            if (master != null && master.UserAdministrationData != null)
            {
                gvAspNetUsers.DataSource = master.UserAdministrationData.GetAspNetUsersByZoneID(zoneID).OrderBy(x => x.UserName);
                gvAspNetUsers.DataBind();
            }
        }

        private void GoBack()
        {
            Response.Redirect(hlBack.NavigateUrl);
        }

        private void ucEditUserZones_Load(object sender, EventArgs e)
        {
            //The UC gets loaded AFTER this page gets loaded, and if i try to set
            //the uc's zone in this pages load, it will get overridden by the UC's page_load logic
            //so i just tap into the event, wait for the uc to load, then set the zone i want
            if (!IsPostBack)
            {
                ucEditUserZones.SetZoneIDForEdit(zoneID);
                if (Request.UrlReferrer != null)
                {
                    this.hlBack.NavigateUrl = Request.UrlReferrer.AbsoluteUri;
                }
            }
        }
    }
}
