using System;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{ 
	[Serializable]
	public class AspNetUser
	{
		public Guid UserId { get; set; }
		public string Name { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public DateTime LastLoginDate { get; set; }
        public int ZonePermissionID { get; set; }
	}
}
