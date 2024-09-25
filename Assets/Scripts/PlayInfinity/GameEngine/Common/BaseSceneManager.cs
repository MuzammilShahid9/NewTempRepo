using PlayInfinity.Pandora.Core.UI;
using UnityEngine;

namespace PlayInfinity.GameEngine.Common
{
	public class BaseSceneManager : MonoBehaviour
	{
		public BaseUIManager sceneUI;

		public GameObject roleNode;

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		public void HideUI()
		{
			sceneUI.HideUI();
		}

		public void HideRoles()
		{
			roleNode.SetActive(false);
		}

		protected virtual void OnEnable()
		{
			bool flag = sceneUI != null;
		}

		protected virtual void OnDisable()
		{
			if (sceneUI != null)
			{
				sceneUI.HideUI();
			}
		}

		protected virtual void OnApplicationPause(bool isPause)
		{
		}

		protected virtual void OnApplicationQuit()
		{
		}
	}
}
