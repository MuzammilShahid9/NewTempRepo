using System.Xml;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDAction
	{
		public float tm;

		public CDCameraConfig camConfig;

		public CDRoleConfig roleConfig;

		public CDConversationConfig convConfig;

		public CDBuildConfig buildConfig;

		public CDAudioConfig audioConfig;

		public CDOtherConfig otherConfig;

		public XmlElement actionElem;
	}
}
