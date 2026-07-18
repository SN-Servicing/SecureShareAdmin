using System;

namespace Snsc.FileTransfers.SecureDataInterchange.Business
{ 
	[Serializable]
	public class ZoneType
	{
		public int FileSharingZoneTypeID { get; set; }
		public string Name { get; set; }
		public string PrimaryObjectIDFieldName { get; set; }
		public string SecondaryObjectIDFieldName { get; set; }
	}
}
