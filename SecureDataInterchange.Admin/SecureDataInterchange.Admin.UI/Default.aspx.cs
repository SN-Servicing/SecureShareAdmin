using System;

namespace SecureDataInterchange.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/ViewUsers.aspx");
        }
    }
}