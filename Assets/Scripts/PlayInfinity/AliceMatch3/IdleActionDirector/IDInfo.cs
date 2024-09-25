using System.Collections.Generic;
using System.Xml;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDInfo
	{
		public string startTask;

		public string endTask;

		public RoleType roleType;

		public List<IDAction> actions = new List<IDAction>();

		public XmlElement actionElem;
	}
}
