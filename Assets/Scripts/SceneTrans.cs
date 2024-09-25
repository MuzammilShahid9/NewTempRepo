using System.Collections;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrans : MonoBehaviour
{
	private AsyncOperation asyncMainScene;

	private void Start()
	{
		StartCoroutine(LoadGameScene());
	}

	private IEnumerator LoadGameScene()
	{
		DebugUtils.Log(DebugType.Other, GlobalVariables.targetScene);
		SceneType targetScene = GlobalVariables.targetScene;
		asyncMainScene = SceneManager.LoadSceneAsync(targetScene.ToString());
		asyncMainScene.allowSceneActivation = false;
		yield return new WaitForSeconds(2f);
		while (!asyncMainScene.isDone)
		{
			if (asyncMainScene.progress == 0.9f)
			{
				asyncMainScene.allowSceneActivation = true;
				Animation[] componentsInChildren = SceneTransManager.Instance.BuLian.GetComponentsInChildren<Animation>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].Play("lianzi_on");
				}
				float time = 0f;
				UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float durtion)
				{
					if (time > 0.3f)
					{
						AudioManager.Instance.PlayAudioEffect("curtain_open");
						return true;
					}
					time += durtion;
					return false;
				}));
			}
			yield return null;
		}
		yield return asyncMainScene;
	}
}
