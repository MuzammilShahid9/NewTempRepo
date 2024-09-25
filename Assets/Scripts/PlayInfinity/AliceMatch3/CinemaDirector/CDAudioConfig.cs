using System.Xml;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDAudioConfig
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

		public MusicClip musicName;

		public EffectClip effectName;

		public XmlElement audioElem;
	}
}
