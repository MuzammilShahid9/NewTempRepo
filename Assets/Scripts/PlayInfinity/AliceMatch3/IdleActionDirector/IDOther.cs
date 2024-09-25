using UnityEngine;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDOther : MonoBehaviour
	{
		private static IDOther instance;

		public static IDOther Instance
		{
			get
			{
				return instance;
			}
		}

		private void Awake()
		{
			instance = this;
		}

		public void Load(IDAction action)
		{
		}
	}
}
