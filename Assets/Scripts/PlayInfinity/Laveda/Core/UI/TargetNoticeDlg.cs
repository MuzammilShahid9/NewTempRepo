using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class TargetNoticeDlg : BaseDialog
	{
		private static TargetNoticeDlg instance;

		public GameObject targetItem1;

		public GameObject targetItem2;

		public Image firstTargetImage;

		public Text firstTargetText;

		public Image secondTargetImage;

		public Text secondTargetText;

		public Transform BG;

		private Vector3 pos;

		private bool isCanCheck;

		public static TargetNoticeDlg Instance
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
			targetItem1.transform.localScale = Vector3.zero;
			targetItem2.transform.localScale = Vector3.zero;
			pos = BG.localPosition;
		}

		public void Enter(int[] tarIDArray)
		{
			targetItem1.transform.localScale = Vector3.zero;
			targetItem2.transform.localScale = Vector3.zero;
			Vector3 zero = Vector3.zero;
			Sequence s = DOTween.Sequence();
			AudioManager.Instance.PlayAudioEffect("board_moving");
			s.Append(BG.DOLocalMove(zero, 0.7f).SetEase(Ease.OutBack).OnComplete(delegate
			{
				int[] targetList = GameLogic.Instance.levelData.targetList;
				if (tarIDArray.Length < 3)
				{
					targetItem2.SetActive(false);
					int num = targetList[tarIDArray[1]];
					firstTargetImage.sprite = GetHaveElementAndCellTool.GetPicture(tarIDArray[1] + 1);
					firstTargetText.text = string.Concat(num);
				}
				else
				{
					targetItem2.SetActive(true);
					int num2 = targetList[tarIDArray[1]];
					firstTargetImage.sprite = GetHaveElementAndCellTool.GetPicture(tarIDArray[1] + 1);
					firstTargetText.text = string.Concat(num2);
					num2 = targetList[tarIDArray[2]];
					secondTargetImage.sprite = GetHaveElementAndCellTool.GetPicture(tarIDArray[2] + 1);
					secondTargetText.text = string.Concat(num2);
				}
				Sequence sequence = DOTween.Sequence();
				sequence.Append(targetItem1.transform.DOScale(Vector3.one, 0.58f).SetEase(Ease.OutBack));
				sequence.Join(targetItem2.transform.DOScale(Vector3.one, 0.58f).SetEase(Ease.OutBack));
				sequence.OnComplete(delegate
				{
					StartCoroutine("HideSelf");
					isCanCheck = true;
				});
			}));
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			base.gameObject.SetActive(true);
		}

		private IEnumerator HideSelf()
		{
			yield return new WaitForSeconds(3f);
			isCanCheck = false;
			Close();
		}

		public void Close(bool isAnim = true)
		{
			if (isAnim)
			{
				AudioManager.Instance.PlayAudioEffect("board_moving_up");
				BG.DOLocalMove(pos, 0.7f).SetEase(Ease.InBack).OnComplete(delegate
				{
					DialogManagerTemp.Instance.CloseDialog(DialogType.TargetNoticeDlg);
					Singleton<MessageDispatcher>.Instance().SendMessage(9u, null);
				});
			}
			else
			{
				BG.transform.localPosition = pos;
				DialogManagerTemp.Instance.CloseDialog(DialogType.TargetNoticeDlg);
				Singleton<MessageDispatcher>.Instance().SendMessage(9u, null);
			}
		}

		public override void Show(object obj)
		{
			DebugUtils.Assert(obj != null, "target notice data is null!");
			base.gameObject.SetActive(true);
			isCanOpen = false;
			isCanClose = true;
			isAniming = false;
			Enter((int[])obj);
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

		public override void PressEsc(uint iMessageType, object arg)
		{
			if (isCanCheck)
			{
				Close();
			}
		}
	}
}
