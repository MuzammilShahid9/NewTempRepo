using System;
using System.Collections;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PlayInfinity.GameEngine.Common
{
	public class SceneTransManager : MonoBehaviour
	{
		private AsyncOperation asyncMainScene;

		public BaseSceneManager currentSceneManager;

		private static SceneTransManager instance;

		public GameObject BuLian;

		public GameObject[] EffectArray;

		public bool isReadySwitch;

		public static SceneTransManager Instance
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

		public void TransTo(SceneType sceneType)
		{
			isReadySwitch = true;
			StartCoroutine(_SwitchToScene1(sceneType));
		}

		public void TransToSwitch(SceneType type)
		{
			GlobalVariables.targetScene = type;
			StartCoroutine(_SwitchToScene(GlobalVariables.targetScene));
		}

		private IEnumerator _SwitchToScene(SceneType sceneType = SceneType.GameScene)
		{
			if (sceneType != 0 && GameSceneUIManager.Instance != null)
			{
				UnityEngine.Object.Destroy(GameSceneUIManager.Instance.gameObject);
			}
			if (sceneType != SceneType.CastleScene && CastleSceneUIManager.Instance != null)
			{
				CastleSceneUIManager.Instance.gameObject.SetActive(false);
				CastleSceneUIManager.Instance.Destroy();
				UnityEngine.Object.Destroy(CastleSceneUIManager.Instance.gameObject);
			}
			PoolManager.Ins.DeSpawnAllEffect();
			GC.Collect();
			bool isFirst = false;
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneType.ToString());
			while (!asyncLoad.isDone)
			{
				if (!isFirst && asyncLoad.progress > 0.9f)
				{
					isFirst = true;
				}
				yield return null;
			}
			GC.Collect();
			HideTip();
			yield return new WaitForSeconds(0.3f);
			Animation[] componentsInChildren = Instance.BuLian.GetComponentsInChildren<Animation>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Play("lianzi_on");
			}
			bool isPlay = false;
			float time = 0f;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float durtion)
			{
				if (time > 0.3f && !isPlay)
				{
					isPlay = true;
					AudioManager.Instance.PlayAudioEffect("curtain_open");
				}
				if (time > 1f)
				{
					DialogManagerTemp.Instance.CancelMaskAllDlg();
					return true;
				}
				time += durtion;
				return false;
			}));
			yield return asyncLoad;
		}

		private IEnumerator _SwitchToScene1(SceneType sceneType = SceneType.GameScene)
		{
			if (sceneType != 0 && GameSceneUIManager.Instance != null)
			{
				UnityEngine.Object.Destroy(GameSceneUIManager.Instance.gameObject);
			}
			if (sceneType != SceneType.CastleScene && CastleSceneUIManager.Instance != null)
			{
				CastleSceneUIManager.Instance.gameObject.SetActive(false);
				CastleSceneUIManager.Instance.Destroy();
				UnityEngine.Object.Destroy(CastleSceneUIManager.Instance.gameObject);
			}
			PoolManager.Ins.DeSpawnAllEffect();
			GC.Collect();
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneType.ToString());
			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			GC.Collect();
			isReadySwitch = false;
			yield return asyncLoad;
		}

		public void ChangeSceneWithEffect(Action action = null)
		{
			isReadySwitch = true;
			DialogManagerTemp.Instance.MaskAllDlg();
			Instance.BuLian.SetActive(true);
			Animation[] componentsInChildren = Instance.BuLian.GetComponentsInChildren<Animation>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Play("lianzi_off");
			}
			AudioManager.Instance.PlayAudioEffect("curtain_close");
			float time = 0f;
			bool isFirst = true;
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
			{
				if (time >= 1.5f)
				{
					isReadySwitch = false;
					if (action != null)
					{
						action();
					}
					return true;
				}
				if (isFirst && time > 0.5f)
				{
					isFirst = false;
					ShowTip();
				}
				time += duration;
				return false;
			}));
		}

		public void ShowTip()
		{
			int num = UnityEngine.Random.Range(0, EffectArray.Length);
			EffectArray[num].SetActive(true);
			EffectArray[num].GetComponent<CanvasGroup>().alpha = 1f;
			EffectArray[num].GetComponent<Animation>()["Tip"].time = 0f;
			EffectArray[num].GetComponent<Animation>().Play("Tip");
		}

		public void HideTip()
		{
			for (int i = 0; i < EffectArray.Length; i++)
			{
				if (EffectArray[i].activeSelf)
				{
					EffectArray[i].GetComponent<Animation>().Play("HideTip");
					GameObject obj = EffectArray[i];
					Timer.Schedule(this, 0.25f, delegate
					{
						obj.SetActive(false);
					});
				}
			}
		}
	}
}
