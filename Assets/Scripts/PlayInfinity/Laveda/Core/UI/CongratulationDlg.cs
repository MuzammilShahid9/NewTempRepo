using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

namespace PlayInfinity.Laveda.Core.UI
{
	public class CongratulationDlg : BaseDialog
	{
		private static CongratulationDlg instance;

		public Transform BG;

		private Vector3 pos;

		private bool isCanCheck;

		public static CongratulationDlg Instance
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
			pos = BG.localPosition;
		}

		public override void Show(object obj)
		{
			isCanCheck = false;
			Vector3 zero = Vector3.zero;
			Sequence s = DOTween.Sequence();
			AudioManager.Instance.PlayAudioEffect("congratulations");
			AudioManager.Instance.PlayAudioMusic("music_game_win");
			isCanOpen = false;
			isCanClose = true;
			isAniming = false;
			GameSceneUIManager.Instance.btnSetting.enabled = false;
			GameSceneUIManager.Instance.settingPanel.SetActive(false);
			s.Append(BG.DOLocalMove(zero, 0.7f).SetEase(Ease.OutBack).OnComplete(delegate
			{
				isCanCheck = true;
			}));
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			base.gameObject.SetActive(true);
			StartCoroutine("HideSelf");
		}

		public void Close(bool isAnim = true)
		{
			if (isAnim)
			{
				BG.DOLocalMove(pos, 0.7f).SetEase(Ease.InBack).OnComplete(delegate
				{
					DialogManagerTemp.Instance.CloseDialog(DialogType.CongratulationDlg);
					if (UserDataManager.Instance.GetProgress() >= 7 && (GetHaveElementAndCellTool.GetBombCellList(GameLogic.Instance.currentBoard).Count != 0 || GameLogic.Instance.levelData.move - GameLogic.Instance.TotleMoveCount != 0))
					{
						float time = 0f;
						UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
						{
							if (time > 2.5f)
							{
								GameSceneUIManager.Instance.isCanSkipFinish = true;
								return true;
							}
							time += duration;
							return false;
						}));
					}
				});
			}
			else
			{
				BG.transform.localPosition = pos;
				DialogManagerTemp.Instance.CloseDialog(DialogType.CongratulationDlg);
			}
		}

		private IEnumerator HideSelf()
		{
			yield return new WaitForSeconds(1f);
			isCanCheck = false;
			Close();
		}

		public override void PressEsc(uint iMessageType, object arg)
		{
			if (isCanCheck)
			{
				Close();
			}
		}

		private void Update()
		{
			if (isCanCheck && Input.GetMouseButtonDown(0))
			{
				isCanCheck = false;
				StopCoroutine("HideSelf");
				Close();
			}
		}
	}
}
