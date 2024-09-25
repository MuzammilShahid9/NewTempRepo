using System.Xml;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDAudioConfig
	{
		public bool isSet;

		public bool isMusicSet;

		public bool isMusicLoop;

		public bool isMusicStop;

		public float musicMinTime;

		public float musicMaxTime;

		public bool isEffectSet;

		public bool isEffectLoop;

		public bool isEffectStop;

		public float effectMinTime;

		public float effectMaxTime;

		public string musicName;

		public string effectName;

		public XmlElement audioElem;
	}
}
