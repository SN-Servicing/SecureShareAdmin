using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snsc.FileTransfers.SecureDataInterchange.Business;

namespace SecureDataInterchange.Admin.Admin.access
{
    public partial class EditUserZones : System.Web.UI.UserControl
    {
        #region Page Fields and Properties

        private UserZoneInfo UserZones { get; set; }
        private SecureDataInterchange.Admin.Common.AdminMaster master;
        private int _rowCount = -1;
        private Mode _mode;
        private List<FileSharingZoneInfo> _businessObject = null;

        public bool AddingNewZone 
        {
            get
            {
                return this.rdoSelectZoneType.SelectedIndex == 1;
            }
        }

        public int SelectedZonesCount
        {
            get
            {
                int counter = 0;
                foreach (System.Web.UI.WebControls.GridViewRow gvr in gvAvailableZones.Rows)
                {
                    if (((System.Web.UI.WebControls.CheckBox)gvr.Cells[0].FindControl("chkInclude")).Checked)
                    {
                        counter++;
                    }
                }
                return counter;
            }
        }

        public enum Mode
        {
            NewZone,
            ViewExternalUserZones, 
            ViewAmsUserZones
        }

        #endregion

        protected void Page_Init(object sender, EventArgs e)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadMaster();

            if (!IsPostBack)
            {
                //if (this._mode != Mode.NewZone)
                    rdoSelectZoneType_SelectedIndexChanged(rdoSelectZoneType, EventArgs.Empty);
            }
            else // this is a post back
            {
                this.UserZones = ViewState["uzi"] as UserZoneInfo;
            }
        }

        #region Data Binding

        private void BindZoneTypes()
        {
            if (master == null || master.UserAdministrationData == null)
            {
                return;
            }

            ddlZoneType.DataSource = master.UserAdministrationData.ZoneTypes.OrderBy(zt => zt.Name);
            ddlZoneType.DataBind();

            ddlZoneTypeFilter.DataSource = master.UserAdministrationData.ZoneTypes.OrderBy(zt => zt.Name);
            ddlZoneTypeFilter.DataBind();
            ddlZoneTypeFilter.Items.Insert(0, new ListItem("*** ALL ***", "0"));
            ddlZoneTypeFilter.SelectedIndex = 0;
        }

        private List<FileSharingZoneInfo> BuildZoneList()
        {
            if (master == null || master.UserAdministrationData == null)
            {
                return new List<FileSharingZoneInfo>();
            }

            string[] searchTokens = ParseSearchTokens(txtZoneSearch.Text);
            if (searchTokens.Length == 0)
            {
                return new List<FileSharingZoneInfo>();
            }

            IEnumerable<FileSharingZoneInfo> zones = master.UserAdministrationData.UserZoneInfo.Zones;
            int zoneTypeFilter = (ddlZoneTypeFilter.SelectedIndex > 0)
                ? Convert.ToInt16(ddlZoneTypeFilter.SelectedValue)
                : -1;

            if (zoneTypeFilter > -1)
            {
                zones = zones.Where(zone => zone.ZoneTypeID == zoneTypeFilter);
            }

            return zones
                .Select(zone => new { Zone = zone, Hits = CountSearchHits(zone, searchTokens) })
                .Where(ranked => ranked.Hits > 0)
                .OrderByDescending(ranked => ranked.Hits)
                .ThenBy(ranked => ranked.Zone.Description)
                .Select(ranked => ranked.Zone)
                .ToList();
        }

        private static string[] ParseSearchTokens(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                return new string[0];
            }

            return searchText.ToLower().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static int CountSearchHits(FileSharingZoneInfo zone, string[] searchTokens)
        {
            int hitCount = 0;
            foreach (string token in searchTokens)
            {
                if (TokenMatchesZone(zone, token))
                {
                    hitCount++;
                }
            }
            return hitCount;
        }

        private static bool TokenMatchesZone(FileSharingZoneInfo zone, string token)
        {
            return FieldContainsToken(zone.Description, token)
                || FieldContainsToken(zone.PrimaryIDValue, token)
                || FieldContainsToken(zone.SecondaryIDValue, token);
        }

        private static bool FieldContainsToken(string fieldValue, string token)
        {
            return !string.IsNullOrEmpty(fieldValue) && fieldValue.ToLower().Contains(token);
        }

        private void BindZoneList()
        {
            _businessObject = BuildZoneList();
            _rowCount = _businessObject.Count;
            gvAvailableZones.DataSource = _businessObject;
            gvAvailableZones.DataBind();
        }

        #endregion

        #region Control Events

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindZoneList();
        }

        protected void ddlZoneTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectZoneType();
            BindZoneList();
        }

        protected void ddlZoneType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectZoneType();
        }

        protected void gvAvailableZones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.EmptyDataRow &&
                e.Row.RowType != DataControlRowType.Pager)
            {
                //if the secondary value is null/empty, hide that row
                if (_rowCount <= 0) return;

                if (_mode == Mode.ViewAmsUserZones) e.Row.Cells[0].Visible = false; //hide chk column
                if (_mode != Mode.ViewAmsUserZones)
                {
                    this.pnlLegend.Visible = false;
                    e.Row.Cells[e.Row.Cells.Count - 1].Visible = false; //hide email column
                }

                if (e.Row.DataItem != null)
                {
                    FileSharingZoneInfo z = e.Row.DataItem as FileSharingZoneInfo;

                    if (z != null)
                    {
                        HyperLink hl = (HyperLink)e.Row.FindControl("hlDescription");
                        CheckBox chk = (CheckBox)e.Row.FindControl("chkInclude");

                        // Navigate directly to Edit Zone page (file upload/download features removed)
                        if (this._mode == Mode.ViewAmsUserZones)
                        {
                            hl.NavigateUrl = string.Format("~/Admin/EditZone.aspx?ZoneID={0}", z.ZoneID);
                        }
                        else
                        {
                            hl.NavigateUrl = null;
                        }

                        if (z.SecondaryIDValue == "")
                        {
                            Label lbl = e.Row.FindControl("lblSecondary") as Label;
                            lbl.Visible = false;
                        }

                        if (UserZones != null && UserZones.Zones.SingleOrDefault(x => x.ZoneID == z.ZoneID) != null)
                        {
                            e.Row.Enabled = false;
                        }


                        //set email icon
                        ImageButton ibtn = e.Row.FindControl("btnEmail") as ImageButton;
                        ibtn.ImageUrl = (z.NotificationOptIn) ?
                            "~/Common/images/email.png" :
                            "~/Common/images/cross.png";

                    }
                }
            }
        }

        protected void rdoSelectZoneType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.rdoSelectZoneType.SelectedIndex == 0) //current
            {
                this.mvZone.SetActiveView(this.mvvAddCurrentZone);
                BindZoneTypes();
                BindZoneList();
            }
            else //new zone
            {
                this.mvZone.SetActiveView(mvvNewZone);
                BindZoneTypes();
                if (this.ddlZoneType.Items.Count > 0) this.ddlZoneType.SelectedIndex = 0;
                SelectZoneType();

                this.lblPrimaryObjectIDError.Visible = false;
                this.lblPrimaryObjectIDError.Text = "";

                this.txtPrimaryText.Text = "";
                this.txtDescription.Text = "";
                this.txtSecondaryText.Text = "";
            }
        }


        #endregion

        #region Methods

        public void SetMode(Mode mode)
        {
            _mode = mode;

            if (mode == Mode.NewZone && !IsPostBack)
            {
                this.rdoSelectZoneType.Visible = false;
                this.lblSelectZoneType.Visible = false;
                this.pnlLegend.Visible = false;

                rdoSelectZoneType.SelectedIndex = 1;
                rdoSelectZoneType_SelectedIndexChanged(rdoSelectZoneType, EventArgs.Empty);
            }
            if (mode == Mode.ViewAmsUserZones && !IsPostBack) //dont show the rdo option
            {
                this.rdoSelectZoneType.Visible = false;
                this.lblSelectZoneType.Visible = false;
                this.pnlLegend.Visible = true;
            }
        }

        public void SetZoneIDForEdit(int zoneID)
        {
            if (master != null)
            {
                FileSharingZoneInfo zi = master.UserAdministrationData.UserZoneInfo.Zones.SingleOrDefault(zz => zz.ZoneID == zoneID);
                if (zi != null)
                {
                    this.ddlZoneType.SelectedValue = zi.ZoneTypeID.ToString();
                    SelectZoneType();
                    txtDescription.Text = zi.Description;
                    txtPrimaryText.Text = zi.PrimaryIDValue;
                    txtSecondaryText.Text = zi.SecondaryIDValue;
                }



            }

        }

        public int SaveZoneChages(int zoneID)
        {
            if (AdministrationData.UpdateFileSharingZoneInfo(zoneID,
                Convert.ToInt32(this.ddlZoneType.SelectedValue),
                txtDescription.Text,
                txtPrimaryText.Text,
                txtSecondaryText.Text) != -1)
            {
                //all ok
                this.lblPrimaryObjectIDError.Visible = false;
                return 0;
            }
            else //this is a violation of the unique constraint
            {
                this.lblPrimaryObjectIDError.Visible = true;
                this.lblPrimaryObjectIDError.Text = string.Format("{0} {1} already exists.", lblPrimaryText.Text, txtPrimaryText.Text);
                return -1; //constraint error
            }
        }

        private void LoadMaster()
        {
            master = (SecureDataInterchange.Admin.Common.AdminMaster)this.Page.Master;
        }

        private void SelectZoneType()
        {
            if (master == null)
            {
                return;
            }

            ZoneType zoneType = ddlZoneType.Visible
                ? master.UserAdministrationData.ZoneTypes.SingleOrDefault(x => x.FileSharingZoneTypeID.ToString() == ddlZoneType.SelectedValue.ToString())
                : master.UserAdministrationData.ZoneTypes.SingleOrDefault(x => x.FileSharingZoneTypeID.ToString() == ddlZoneTypeFilter.SelectedValue.ToString());

            if (zoneType == null)
            {
                return;
            }

            this.lblPrimaryText.Text = zoneType.PrimaryObjectIDFieldName;

            bool hasSecondaryField = !string.IsNullOrEmpty(zoneType.SecondaryObjectIDFieldName);
            if (hasSecondaryField)
            {
                this.lblSecondaryText.Text = zoneType.SecondaryObjectIDFieldName;
            }

            this.lblSecondaryText.Visible = hasSecondaryField;
            this.txtSecondaryText.Visible = hasSecondaryField;
            this.revSecondaryValue.Visible = hasSecondaryField;
            this.rfvSecondaryValue.Visible = hasSecondaryField;
        }

        public void SetUserZoneInfo(UserZoneInfo uzi)
        {
            this.UserZones = uzi;
            BindZoneList();

            if (this.ViewState["uzi"] == null)
                this.ViewState.Add("uzi", uzi);
            else
                this.ViewState["uzi"] = uzi;
        }

        public int SaveNewZone(System.Web.Security.MembershipUserCollection muc)
        {
            int zoneType = Convert.ToInt32(ddlZoneType.SelectedValue);
            int? zone;

            zone = AdministrationData.InsertFileSharingZone(
                txtPrimaryText.Text, txtSecondaryText.Text, zoneType, txtDescription.Text);

            if (zone != -1)
            {
                foreach (System.Web.Security.MembershipUser mu in muc)
                {
                    AdministrationData.InsertFileSharingZonePermission(zone.Value, new Guid(mu.ProviderUserKey.ToString()), mu.UserName);
                }
            }
            else //this is a violation of the unique constraint
            {
                this.lblPrimaryObjectIDError.Visible = true;
                this.lblPrimaryObjectIDError.Text = string.Format("{0} {1} already exists.", lblPrimaryText.Text, txtPrimaryText.Text);
                return -1; //constraint error
            }

            return zone.Value;
        }

        public int SaveUserZoneChanges(System.Web.Security.MembershipUser user)
        {
            //now add users zone permissions
            Guid userID = new Guid(user.ProviderUserKey.ToString());

            if (rdoSelectZoneType.SelectedIndex == 0) //current zone
            {
                foreach (System.Web.UI.WebControls.GridViewRow gvr in gvAvailableZones.Rows)
                {
                    if (((System.Web.UI.WebControls.CheckBox)gvr.Cells[0].FindControl("chkInclude")).Checked)
                    {
                        int fileSharingZoneID = Convert.ToInt32(((Label)gvr.FindControl("lblID")).Text);
                        AdministrationData.InsertFileSharingZonePermission(fileSharingZoneID, userID, user.UserName);
                    }
                }
            }
            else //new zone
            {
                int zoneType = Convert.ToInt32(ddlZoneType.SelectedValue);
                int? zone;

                zone = AdministrationData.InsertFileSharingZone(
                    txtPrimaryText.Text, txtSecondaryText.Text, zoneType, txtDescription.Text);

                if (zone != -1)
                {
                    AdministrationData.InsertFileSharingZonePermission(zone.Value, userID, user.UserName);
                }
                else //this is a violation of the unique constraint
                {
                    this.lblPrimaryObjectIDError.Visible = true;
                    this.lblPrimaryObjectIDError.Text = string.Format("{0} {1} already exists.", lblPrimaryText.Text, txtPrimaryText.Text);
                    return -1; //constraint error
                }
            }

            this.txtDescription.Text = "";
            this.txtPrimaryText.Text = "";
            this.txtSecondaryText.Text = "";
            return 0; //all fine
        }

        #endregion

        protected void gvAvailableZones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandArgument.ToString() == "emailnotification")
            {
                GridViewRow gvr = ((ImageButton)e.CommandSource).NamingContainer as GridViewRow;
                Label lblEmailNotification = gvr.FindControl("lblEmailNotification") as Label;
                Label lblZoneID = gvr.FindControl("lblZoneID") as Label;
                int amsUserID = Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID ?? 0;

                if (lblEmailNotification != null && lblZoneID != null)
                {
                    Snsc.FileTransfers.SecureDataInterchange.Business.FileSharingZoneInfo.ChangeEmailNotificationAmsUser(
                        Convert.ToInt32(lblZoneID.Text),
                        !Convert.ToBoolean(lblEmailNotification.Text),
                        amsUserID);

                    BindZoneList();
                }
            }
        }

        protected void btnYesMail_Click(object sender, EventArgs e)
        {
            BindZoneList();
            if (_businessObject != null)
            foreach (FileSharingZoneInfo fszi in _businessObject)
            {
                FileSharingZoneInfo.ChangeEmailNotificationAmsUser(fszi.ZoneID, true, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID);

            }

            master.ResetUserAdministrationData();
            BindZoneList();
            
        }

        protected void btnNoMail_Click(object sender, EventArgs e)
        {
            BindZoneList();
            if (_businessObject != null)
                foreach (FileSharingZoneInfo fszi in _businessObject)
                {
                    FileSharingZoneInfo.ChangeEmailNotificationAmsUser(fszi.ZoneID, false, Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID);
                }

            master.ResetUserAdministrationData();
            BindZoneList();
            
        }

    }
}