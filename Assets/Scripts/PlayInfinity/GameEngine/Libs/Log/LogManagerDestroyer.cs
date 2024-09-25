using UnityEngine;

namespace PlayInfinity.GameEngine.Libs.Log
{
	internal class LogManagerDestroyer : MonoBehaviour
	{
		private void Awake()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnApplicationQuit()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
