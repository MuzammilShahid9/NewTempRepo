using UnityEngine;

namespace PlayInfinity.Laveda.Core.RollTheBall.PlayInfinity.GameEngine.Libs.Common
{
	public class DontDestroy : MonoBehaviour
	{
		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
