using System;

namespace Snsc.Foundation.Providers
{
    public class SecureShareMembershipUser : System.Web.Security.MembershipUser
    {
        public SecureShareMembershipUser(string name,
            string providerName,
            object providerUserKey,
            string email,
            string passwordQuestion,
            string comment,
            bool isApproved,
            bool isLockedOut,
            DateTime creationDate,
            DateTime lastLoginDate,
            DateTime lastActivityDate,
            DateTime lastPasswordChangedDate,
            DateTime lastLockoutDate) : base(providerName,
                                          email,
                                          providerUserKey,
                                          email,
                                          passwordQuestion,
                                          comment,
                                          isApproved,
                                          isLockedOut,
                                          creationDate,
                                          lastLoginDate,
                                          lastActivityDate,
                                          lastPasswordChangedDate,
                                          lastLockoutDate)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}