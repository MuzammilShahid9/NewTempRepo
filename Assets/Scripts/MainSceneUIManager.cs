using System.Collections;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneUIManager : MonoBehaviour
{
	public AsyncOperation asyncCastleScene;

	public GameObject ConnectBtn;

	private void Start()
	{
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(21u, PressEsc);
		//Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(22u, FacebookStatusChanged);
		UpdateLoginBtn();
		Button[] componentsInChildren = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].onClick.AddListener(delegate
			{
				AudioManager.Instance.PlayAudioEffect("general_button");
			});
		}
	}

	private IEnumerator LoadCastleScene()
	{
		asyncCastleScene = SceneManager.LoadSceneAsync("_Scenes/CastleScene");
		asyncCastleScene.allowSceneActivation = false;
		while (!asyncCastleScene.isDone)
		{
			float progress = asyncCastleScene.progress;
			float num = 0.9f;
			yield return null;
		}
		yield return asyncCastleScene;
	}

	public void PlayBtnClick()
	{
		
		if (UserDataManager.Instance.GetService().tutorialProgress == 2)
		{
			InitGame.Instance.asyncGameScene.allowSceneActivation = true;
			return;
		}
		SceneTransition.Instance.PlayFadeOutAnim();
		Timer.Schedule(this, 0.5f, delegate
		{
			InitGame.Instance.asyncMainScene.allowSceneActivation = true;
		});
	}

	// public void ConnectFaceBook()
	// {
	// 	DebugUtils.Log(DebugType.Other, "BtnLoginFacebookClicked");
	// 	FacebookUtilities.Instance.LoginWithFacebook();
	// }

	private void PressEsc(uint iMessageType, object arg)
	{
		if (!DialogManagerTemp.Instance.IsDialogShowing())
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.QuitGameDlg);
		}
	}

	private void UpdateLoginBtn()
	{
		// if (FacebookUtilities.Instance.CheckFacebookLogin())
		// {
		// 	ConnectBtn.SetActive(false);
		// }
		// else
		// {
		// 	ConnectBtn.SetActive(true);
		// }
	}

	// private void FacebookStatusChanged(uint iMessageType, object arg)
	// {
	// 	UpdateLoginBtn();
	// }

	private void OnDestroy()
	{
		Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(21u, PressEsc);
		//Singleton<MessageDispatcher>.Instance().UnRegisterMessageHandler(22u, FacebookStatusChanged);
	}
}
