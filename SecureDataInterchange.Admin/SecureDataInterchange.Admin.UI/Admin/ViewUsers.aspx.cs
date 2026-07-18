using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace SecureDataInterchange.Admin.Admin
{
    public partial class ViewUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.ucUserList.Mode = SecureDataInterchange.Admin.Common.UserControls.UserList.GridMode.LinkToEditUserAllowed;

        }


    }
}
