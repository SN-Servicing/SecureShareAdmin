using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snsc.FileTransfers.SecureDataInterchange.Business;

namespace SecureDataInterchange.Admin.Common
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        private AdministrationData _userAdministrationData;
        public AdministrationData UserAdministrationData
        {
            get
            {
                if (_userAdministrationData == null)
                {
                    _userAdministrationData = AdministrationData.GetAdministrationData(Snsc.Security.SnscPrincipal.SNIdentity.AmsUserID ?? 0);
                }
                return _userAdministrationData;
            }
            private set
            {
                _userAdministrationData = value;
            }
        }

        public void ResetUserAdministrationData()
        {
            _userAdministrationData = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (UserAdministrationData.ZoneTypes.Count < 1)
                {
                    //the 'other' way to check security
                    //bool userHasAccess = Snsc.Foundation.Security.SecurityManager.WebCheckRights("SecureDataInterchange", "1", "Modify");
                    this.MainContent.Visible = false;
                    this.pnlError.Visible = true;
                    this.lblError.Text = "You do not have the necessary AMS permissions required to view this site.";
                }
                else
                {
                    this.MainContent.Visible = true;
                    this.pnlError.Visible = false;
                }
            }
        }

    }
}
