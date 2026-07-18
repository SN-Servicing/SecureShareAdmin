using System;
using System.Collections.Generic;
using System.Text;
using Snsc.FileTransfers.SecureDataInterchange.Business.Data;

namespace Snsc.Foundation.Providers
{
    [Serializable]
    public class SNSCMembershipProvider : System.Web.Security.SqlMembershipProvider
    {
        Snsc.Foundation.Providers.SNSCPortalMembershipProvider mp = new SNSCPortalMembershipProvider();

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            mp.Initialize(name, config);
            base.Initialize(name, config);

            System.Reflection.FieldInfo fi = GetType().BaseType.GetField("_sqlConnectionString", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            fi.SetValue(this, SdiDb.GetConnectionString(DatabaseTarget.Core));
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            return mp.GetUser(username, userIsOnline);
            //return base.GetUser(username, userIsOnline);
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            return mp.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
            //return base.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return mp.ChangePassword(username, oldPassword, newPassword);
            //return base.ChangePassword(username, oldPassword, newPassword);
        }

        public override string ResetPassword(string username, string passwordAnswer)
        {
            return mp.ResetPassword(username, passwordAnswer);
            //return base.ResetPassword(username, passwordAnswer);
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            return mp.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
            //return base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
        }

        public override bool ValidateUser(string username, string password)
        {
            return mp.ValidateUser(username, password);
            //return base.ValidateUser(username, password);
        }

    }
}
