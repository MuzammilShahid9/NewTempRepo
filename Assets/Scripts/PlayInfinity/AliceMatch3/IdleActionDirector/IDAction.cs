using System.Xml;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDAction
	{
		public float tm;

		public IDRoleConfig roleConfig;

		public IDDelayConfig delayConfig;

		public IDConversationConfig convConfig;

		public IDBuildConfig buildConfig;

		public IDAudioConfig audioConfig;

		public IDOtherConfig otherConfig;

		public XmlElement actionElem;
	}
}
