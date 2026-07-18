using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace SecureDataInterchange.Admin
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // User identity will be initialized in Application_AcquireRequestState
            // after authentication and session are both available
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Session is not available yet at this stage
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // Initialize user identity (retrieve AmsUserID from database)
            // This event fires AFTER session is available
            if (HttpContext.Current != null && 
                HttpContext.Current.Session != null && 
                HttpContext.Current.User != null && 
                HttpContext.Current.User.Identity.IsAuthenticated)
            {
                Snsc.Security.UserIdentityHelper.InitializeUserIdentity();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            System.Exception ex = Server.GetLastError();
            if (ex != null && ex.GetType() == typeof(HttpUnhandledException))
            {
                Server.Transfer("~/common/Error.aspx");
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}