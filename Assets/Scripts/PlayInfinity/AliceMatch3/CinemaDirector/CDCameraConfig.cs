using System.Xml;
using UnityEngine;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDCameraConfig
	{
		public bool isSet;

		public Vector3 pos;

		public float size;

		public float tm;

		public bool isFollow;

		public RoleType followRole;

		public XmlElement camElem;
	}
}
