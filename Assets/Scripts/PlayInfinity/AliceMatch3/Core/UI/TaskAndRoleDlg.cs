using System;
using System.Collections;
using DG.Tweening;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.Core.UI
{
	public class TaskAndRoleDlg : BaseDialog
	{
		[Serializable]
		public class ImageArray
		{
			public Sprite[] spriteArray;
		}

		private static TaskAndRoleDlg instance;

		public GameObject taskPanel;

		public GameObject rolePanel;

		public GameObject friendListPanel;

		public GameObject roleDetailPanel;

		public GameObject flyScrollStartPos;

		public GameObject flyScrollMidPos;

		public GameObject flyImage;

		public GameObject scrollEffect;

		public Button[] buttonArray;

		public ImageArray[] buttonImageArray;

		private Vector3 scrollStartPos;

		public static TaskAndRoleDlg Instance
		{
			get
			{
				return instance;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			instance = this;
			scrollStartPos = flyScrollStartPos.transform.Find("Image").transform.localPosition;
			friendListPanel.SetActive(true);
		}

		public override void Show(object obj)
		{
			base.Show(obj);
			TaskBtnClick();
			CastleSceneUIManager.Instance.ChangeUpButtonFrontSortingLayer();
			if (UserDataManager.Instance.GetService().tutorialProgress == 3)
			{
				StartCoroutine(ShowTutorialDlg());
			}
		}

		public void TaskBtnClick()
		{
			taskPanel.SetActive(true);
			rolePanel.SetActive(false);
			friendListPanel.SetActive(false);
			roleDetailPanel.SetActive(false);
			TriggleBtnBg(0);
		}

		public void RoleBtnClick()
		{
			taskPanel.SetActive(false);
			rolePanel.SetActive(true);
			friendListPanel.SetActive(false);
			roleDetailPanel.SetActive(false);
			TriggleBtnBg(1);
		}

		public void FriendListBtnClick()
		{
			taskPanel.SetActive(false);
			rolePanel.SetActive(false);
			friendListPanel.SetActive(true);
			roleDetailPanel.SetActive(false);
			TriggleBtnBg(2);
		}

		public void ShowRoleDetail(RoleConfigData roleData)
		{
			rolePanel.SetActive(false);
			roleDetailPanel.gameObject.SetActive(true);
			roleDetailPanel.GetComponent<RoleDetailPanelManager>().Enter(roleData);
		}

		public void ReturnBtnClick()
		{
			roleDetailPanel.gameObject.SetActive(false);
			rolePanel.SetActive(true);
		}

		public void TriggleBtnBg(int index)
		{
			for (int i = 0; i < buttonArray.Length; i++)
			{
				if (i == index)
				{
					Sprite sprite = buttonImageArray[i].spriteArray[0];
					buttonArray[i].GetComponent<Image>().sprite = sprite;
					buttonArray[i].GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
				}
				else
				{
					Sprite sprite2 = buttonImageArray[i].spriteArray[1];
					buttonArray[i].GetComponent<Image>().sprite = sprite2;
					buttonArray[i].GetComponent<RectTransform>().sizeDelta = new Vector2(sprite2.textureRect.width, sprite2.textureRect.height);
				}
			}
		}

		public void CloseTaskPanel()
		{
			Close();
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.TaskAndRoleDlg);
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, true);
			CastleSceneUIManager.Instance.ChangeUpButtonBehindSortingLayer();
		}

		public void StartFlyIcon(int taskID, int taskStep, Vector3 targetPosition, int costScrollNum)
		{
			StartCoroutine(FlyIcon(taskID, taskStep, targetPosition, costScrollNum));
		}

		public void HideSelf()
		{
			Close();
			CastleSceneUIManager.Instance.ChangeUpButtonBehindSortingLayer();
		}

		private IEnumerator FlyIcon(int taskID, int taskStep, Vector3 targetPosition, int costScrollNum)
		{
			Vector3[] array = new Vector3[3]
			{
				flyScrollStartPos.transform.position,
				flyScrollMidPos.transform.position,
				targetPosition
			};
			GameObject[] flyScroll = new GameObject[costScrollNum];
			for (int i = 0; i < costScrollNum; i++)
			{
				GameObject coin = UnityEngine.Object.Instantiate(flyImage.gameObject);
				AudioManager.Instance.PlayAudioEffect("scroll_fly");
				flyScroll[i] = coin;
				coin.transform.SetParent(flyScrollStartPos.gameObject.transform, true);
				coin.transform.localScale = new Vector3(1f, 1f, 1f);
				coin.transform.localPosition = scrollStartPos;
				coin.SetActive(true);
				Transform transform = coin.transform.Find("juanzhou").transform;
				transform.localScale = new Vector3(40f, 40f, 1f);
				Sequence sequence = DOTween.Sequence();
				StartCoroutine(FlyIconEnumerator(coin, targetPosition, 0.8f));
				sequence.Append(transform.transform.DOScale(new Vector3(30f, 30f, 1f), 0.82f));
				sequence.OnComplete(delegate
				{
					StartCoroutine(DelayDestoryScroll(coin));
				});
				yield return new WaitForSeconds(0.2f);
			}
			yield return new WaitForSeconds(1.5f);
			CloseTaskPanel();
			CastleSceneUIManager.Instance.ChangeUpButtonBehindSortingLayer();
			PlotManager.Instance.DelayStartPlot(taskID, taskStep, 0.2f);
		}

		private IEnumerator FlyIconEnumerator(GameObject scroll, Vector3 targetPosition, float flyTime)
		{
			float timer = 0f;
			Vector3[] path = new Vector3[3]
			{
				flyScrollStartPos.transform.position,
				flyScrollMidPos.transform.position,
				targetPosition
			};
			while (timer <= flyTime)
			{
				Vector3 position = (flyTime - timer) / flyTime * ((flyTime - timer) / flyTime * path[0] + timer / flyTime * path[1]) + timer / flyTime * ((flyTime - timer) / flyTime * path[1] + timer / flyTime * path[2]);
				scroll.transform.position = position;
				timer += Time.deltaTime;
				yield return null;
			}
			scroll.transform.position = targetPosition;
			AudioManager.Instance.PlayAudioEffect("scroll_collect");
			yield return null;
		}

		private IEnumerator ShowTutorialDlg()
		{
			yield return new WaitForSeconds(hidingAnimation.length);
			TutorialManager.Instance.ShowTutorial();
		}

		private IEnumerator CreatScrollEffect(Vector3 tempPosition, GameObject scroll)
		{
			yield return new WaitForSeconds(0.8f);
			GameObject go = UnityEngine.Object.Instantiate(scrollEffect, base.transform);
			go.transform.position = tempPosition;
			yield return new WaitForSeconds(0.1f);
			UnityEngine.Object.Destroy(go);
			UnityEngine.Object.Destroy(scroll);
		}

		private IEnumerator DelayDestoryScroll(GameObject scroll)
		{
			scroll.transform.Find("juanzhou").gameObject.SetActive(false);
			yield return new WaitForSeconds(0.08f);
			scroll.gameObject.SetActive(false);
		}
	}
}
