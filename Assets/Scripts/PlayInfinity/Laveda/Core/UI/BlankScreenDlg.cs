using System;
using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.Laveda.Core.UI
{
	public class BlankScreenDlg : BaseDialog
	{
		public Image blankImage;

		public LocalizationText contentText;

		private static BlankScreenDlg instance;

		public static BlankScreenDlg Instance
		{
			get
			{
				return instance;
			}
		}

		protected override void Awake()
		{
			instance = this;
		}

		protected override void Start()
		{
			base.Start();
		}

		public override void Show(object obj)
		{
			base.gameObject.SetActive(true);
			if (obj != null)
			{
				StartCoroutine(FinishProcess((string)obj));
			}
		}

		public void Close(bool isAnim = true)
		{
			DialogManagerTemp.Instance.CloseDialog(DialogType.BlankScreenDlg, true, false);
		}

		public void Exit()
		{
			blankImage.DOFade(0f, 0.1f);
			base.gameObject.SetActive(false);
			StopAllCoroutines();
		}

		private IEnumerator FinishProcess(string content)
		{
			blankImage.DOFade(1f, 1.1f);
			contentText.text = "";
			yield return new WaitForSeconds(1.1f);
			float time = Convert.ToSingle(content);
			contentText.text = LanguageConfig.GetString("BlackScreenText");
			contentText.color = new Color(1f, 1f, 1f, 0f);
			contentText.DOFade(1f, 1f);
			yield return new WaitForSeconds(2f);
			contentText.DOFade(0f, 1f);
			yield return new WaitForSeconds(time);
			blankImage.DOFade(0f, 2.5f);
			yield return new WaitForSeconds(2.5f);
			PlotBlankScreenManager.Instance.FinishStep();
			Close();
		}
	}
}
