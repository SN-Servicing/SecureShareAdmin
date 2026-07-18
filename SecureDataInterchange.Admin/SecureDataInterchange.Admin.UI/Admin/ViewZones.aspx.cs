using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Snsc.FileTransfers.SecureDataInterchange.Business;

namespace SecureDataInterchange.Admin.Admin.access
{
    public partial class ViewSharedFiles : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ucUserZones.SetMode(EditUserZones.Mode.ViewAmsUserZones);
        }


    }
}
