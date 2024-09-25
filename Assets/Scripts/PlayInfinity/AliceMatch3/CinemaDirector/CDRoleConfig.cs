using System.Collections.Generic;
using System.Xml;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDRoleConfig
	{
		public bool isSet;

		public Dictionary<RoleType, CDRoleAnim> roles;

		public XmlElement roleElem;
	}
}
