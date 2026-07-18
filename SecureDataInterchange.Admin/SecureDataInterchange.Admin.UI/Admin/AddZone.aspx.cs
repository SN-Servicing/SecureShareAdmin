using SecureDataInterchange.Admin.Common;
using System;

namespace SecureDataInterchange.Admin.Admin
{
    public partial class AddZone : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ucEditUserZones.SetMode(SecureDataInterchange.Admin.Admin.access.EditUserZones.Mode.NewZone);
            ucUserList.Mode = SecureDataInterchange.Admin.Common.UserControls.UserList.GridMode.CheckUserAllowed;
            lblMessage.Visible = false;
        }

        #region Button Eventns

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            int zoneId = ucEditUserZones.SaveNewZone(ucUserList.SelectedUsers);
            if (zoneId != -1)
            {
                AdminMaster master = Page.Master as AdminMaster;
                if (master != null)
                {
                    master.ResetUserAdministrationData();
                }

                Response.Redirect("EditZone.aspx?ZoneID=" + zoneId + "&saved=1");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewZones.aspx");
        }

        #endregion

    }
}
