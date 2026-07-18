using System;

namespace SecureDataInterchange.Admin.Common
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Server.GetLastError() != null)
            {
                System.Exception ex = Server.GetLastError().GetBaseException();

                if (ex != null && ex.Message != null)
                {
                    //log error
                    Snsc.Logging.LogService _logSvc = new Snsc.Logging.LogService();
                    _logSvc.Log("Secure Data Interchange", "Admin Error", string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
                    Server.ClearError();
                }
            }
        }
    }
}