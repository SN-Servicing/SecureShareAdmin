/********************************************
Copyright  2015, SN Servicing, Inc.
********************************************/

using System;
using System.Web;

namespace Snsc.Security
{
    /// <summary>
    /// Minimal implementation of SnscPrincipal for SecureShare Admin.
    /// Provides access to current user identity information.
    /// </summary>
    public static class SnscPrincipal
    {
        /// <summary>
        /// Gets the identity of the current authenticated user.
        /// </summary>
        public static SNIdentityInfo SNIdentity
        {
            get { return new SNIdentityInfo(); }
        }

        /// <summary>
        /// Provides identity information for the current user.
        /// </summary>
        public class SNIdentityInfo
        {
            /// <summary>
            /// Gets the AMS User ID for the current authenticated user.
            /// Returns null if user is not authenticated or AMS User ID is not available.
            /// </summary>
            public int? AmsUserID
            {
                get
                {
                    if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                    {
                        // Try to get from session or user context
                        if (HttpContext.Current.Session != null && HttpContext.Current.Session["AmsUserID"] != null)
                        {
                            return Convert.ToInt32(HttpContext.Current.Session["AmsUserID"]);
                        }
                    }
                    return null;
                }
            }

            /// <summary>
            /// Checks if the current user is in the specified role.
            /// </summary>
            /// <param name="roleName">The role name to check.</param>
            /// <returns>True if user is in the role, otherwise false.</returns>
            public bool IsInRole(string roleName)
            {
                if (HttpContext.Current != null && HttpContext.Current.User != null)
                {
                    return HttpContext.Current.User.IsInRole(roleName);
                }
                return false;
            }
        }
    }
}