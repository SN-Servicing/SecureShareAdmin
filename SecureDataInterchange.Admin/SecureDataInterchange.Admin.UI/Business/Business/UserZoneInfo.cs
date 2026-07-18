using System;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{
	/// <summary>
	/// Root business object used to retrieve information about a user's zones.
	/// </summary>
	[Serializable]
	public class UserZoneInfo
	{
		public Guid UserID { get; private set; }
		public FileSharingZoneInfoList Zones { get; private set; }

		/// <summary>
		/// Gets the zone info for external user.
		/// </summary>
		/// <param name="userID">The user ID.</param>
		/// <returns></returns>
		public static UserZoneInfo GetZoneInfoForExternalUser(Guid userID)
		{
			return new UserZoneInfo
			{
				UserID = userID,
				Zones = FileSharingZoneInfoList.GetFileSharingZoneInfoList(userID)
			};
		}

        public static UserZoneInfo GetZoneInfoForInternalUser(int AmsUserID)
        {
			return new UserZoneInfo
			{
				UserID = Guid.Empty,
				Zones = FileSharingZoneInfoList.GetFileSharingZoneInfoListForInternalUser(AmsUserID)
			};
        }

		private UserZoneInfo()
		{
			this.UserID = Guid.Empty;
			this.Zones = new FileSharingZoneInfoList();
		}
	}
}


