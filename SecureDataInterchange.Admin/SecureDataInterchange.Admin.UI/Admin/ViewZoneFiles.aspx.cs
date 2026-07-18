using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snsc.FileTransfers.SecureDataInterchange.Business;

namespace SecureDataInterchange.Admin.Admin.access
{
    public partial class ViewZoneFiles : System.Web.UI.Page
    {
        #region Page Properties

        /// <summary>
        /// Gets the index of the zone.
        /// </summary>
        /// <value>The index of the zone.</value>
        private int ZoneIndex
        {
            get
            {
                int index;
                string qsZone = Request.QueryString["ZoneID"];
                if (int.TryParse(qsZone, out index))
                {
                    return index;
                }
                return -1;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var labelText = "An invalid zone was specified.";
                FileSharingZoneInfo zone = GetZone();
                if (zone != null)
                {
                    labelText = string.Format("Zone: {0}<br>{1}: {2}", zone.Description, zone.PrimaryFieldName, zone.PrimaryIDValue);
                    this.hlEditZone.NavigateUrl = "EditZone.aspx?ZoneID=" + ZoneIndex.ToString();
                }
                else
                {
                    //no valid zone... what are they doing on this page?
                    Response.Redirect("ViewZones.aspx");
                }
                lblZone.Text = labelText;

                BindFiles();
            }

            gvFiles.DataKeyNames = new string[] {"ID" };
        }

        #region Button Events

        protected void btnZoneList_Click(object sender, EventArgs e)
        {
            Response.Redirect("FileSharingZones.aspx");
        }

        #endregion

        #region Methods

        private void BindFiles()
        {
            FileSharingZoneInfo zone = GetZone();
            gvFiles.DataSource = zone != null ? zone.Files : null;
            gvFiles.DataBind();
        }

        private FileSharingZoneInfo GetZone()
        {
            //Guid userKey = (Guid)System.Web.Security.Membership.GetUser().ProviderUserKey;
            int userID = Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID ?? 0;
            UserZoneInfo uzi = UserZoneInfo.GetZoneInfoForInternalUser(userID);
            FileSharingZoneInfo zone = uzi.Zones.Where(z => z.ZoneID == ZoneIndex).FirstOrDefault();
            return zone;
        }


        #endregion

        protected void gvFiles_RowDataBound(object sender, GridViewRowEventArgs e)
        {
          
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewZones.aspx");
        }

        protected void ibTrash_Click(object sender, System.EventArgs e)
        {
            ImageButton ib = sender as ImageButton;

            int sharedFileID = Convert.ToInt32(ib.CommandArgument);
            Snsc.Foundation.SDI.Logging.InsertNewLog(Snsc.Foundation.Enumerations.SdiLogSubType.FileDelete, 
                sharedFileID.ToString(), null, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID, null,
                Snsc.Foundation.Enumerations.SdiLogSource.SdiInternalAdmin);

            SharedFileInfo.DeleteFileByID(sharedFileID);

            BindFiles();
        }
    }
}
