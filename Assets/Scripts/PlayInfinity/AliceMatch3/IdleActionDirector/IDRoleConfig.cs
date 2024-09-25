using System.Collections.Generic;
using System.Xml;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDRoleConfig
	{
		public bool isSet;

		public Dictionary<RoleType, IDRoleAnim> roles;

		public XmlElement roleElem;
	}
}
