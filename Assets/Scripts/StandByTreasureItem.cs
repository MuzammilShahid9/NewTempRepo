using Spine.Unity;
using UnityEngine;

public class StandByTreasureItem : MonoBehaviour
{
	public SkeletonAnimation Ani;

	public Transform Peral;

	public void Play(string name, bool isLoop)
	{
		Ani.state.SetAnimation(0, name, isLoop);
	}

	public void ShowPearl()
	{
		Peral.localScale = Vector3.zero;
		Peral.gameObject.SetActive(true);
		float time = 0f;
		UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			Peral.localScale = Vector3.one * time / 0.2f;
			if (time > 0.2f)
			{
				return true;
			}
			time += duration;
			return false;
		}));
	}

	public void HidePearl()
	{
		float time = 0f;
		Vector3 localScale = Peral.localScale;
		UpdateManager.Instance.AddUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (time > 0.27f)
			{
				Peral.gameObject.SetActive(false);
				return true;
			}
			time += duration;
			return false;
		}));
	}

	public void PeralActive(bool isActive)
	{
		Peral.gameObject.SetActive(isActive);
	}
}
