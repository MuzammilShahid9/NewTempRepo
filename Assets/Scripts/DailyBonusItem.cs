using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;
using UnityEngine.UI;

public class DailyBonusItem : MonoBehaviour
{
	public Image gou;

	public Image drop;

	public Image lockIamge;

	public GameObject red;

	public GameObject green;

	public GameObject showEffect;

	public GameObject idleEffect;

	public GameObject selectEffect;

	public GameObject tailEffect;

	public void ShowIcon()
	{
		if (selectEffect != null)
		{
			selectEffect.SetActive(false);
		}
		drop.GetComponent<Canvas>().sortingLayerName = "UI";
		drop.gameObject.SetActive(true);
		lockIamge.gameObject.SetActive(false);
		gou.gameObject.SetActive(true);
		gou.GetComponent<Canvas>().sortingLayerName = "UI";
		red.SetActive(false);
		green.SetActive(true);
	}

	public void GetReward()
	{
		red.SetActive(false);
		green.SetActive(true);
		drop.gameObject.SetActive(true);
		drop.DOFade(0f, 0f);
		drop.DOFade(1f, 1f).SetDelay(1f);
		lockIamge.DOFade(0f, 1f).OnComplete(delegate
		{
			AudioManager.Instance.PlayAudioEffect("DailyBonus");
			lockIamge.gameObject.SetActive(false);
			ShowEffect();
		});
	}

	public void HideIcon()
	{
		drop.gameObject.SetActive(false);
		lockIamge.DOFade(1f, 0f);
		lockIamge.gameObject.SetActive(true);
		gou.gameObject.SetActive(false);
		red.SetActive(true);
		green.SetActive(false);
		selectEffect.SetActive(false);
	}

	public void ShowEffect()
	{
		drop.GetComponent<Canvas>().sortingLayerName = "UI";
		if (showEffect != null)
		{
			showEffect.SetActive(true);
		}
		if (idleEffect != null)
		{
			idleEffect.SetActive(true);
		}
		if (selectEffect != null)
		{
			selectEffect.SetActive(false);
		}
	}

	public void ShowSelectEffect()
	{
		if (showEffect != null)
		{
			showEffect.SetActive(false);
		}
		if (idleEffect != null)
		{
			idleEffect.SetActive(false);
		}
		if (selectEffect != null)
		{
			selectEffect.SetActive(true);
		}
		StartCoroutine(DelayMoveItem());
	}

	private IEnumerator DelayMoveItem()
	{
		yield return new WaitForSeconds(selectEffect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
		GameObject go = Object.Instantiate(drop.gameObject, DailyBonusDlg.Instance.transform.parent);
		Object.Instantiate(tailEffect.gameObject, go.transform);
		go.transform.position = drop.transform.position;
		ShortcutExtensions.DOPath(path: new Vector3[3]
		{
			go.transform.position,
			DailyBonusDlg.Instance.itemMoveMidPos.transform.position,
			DailyBonusDlg.Instance.itemMoveEndPos.transform.position
		}, target: go.transform, duration: 0.7f, pathType: PathType.CatmullRom).OnComplete(delegate
		{
			CastleSceneUIManager.Instance.EnterGameBtnScale();
			Object.Destroy(go);
		});
	}

	public void SetImage(DropType type)
	{
		string text = "";
		switch (type)
		{
		case DropType.AreaBomb:
			text = "AreaBomb";
			break;
		case DropType.ColorBomb:
			text = "ColorBomb";
			break;
		case DropType.Glove:
			text = "Glove";
			break;
		case DropType.DoubleBee:
			text = "DoubleBee";
			break;
		case DropType.Hammer:
			text = "Hammer";
			break;
		case DropType.Spoon:
			text = "Spoon";
			break;
		}
		DebugUtils.Log(DebugType.Other, "Textures/Elements2/" + text);
		drop.sprite = Resources.Load<GameObject>("Textures/Elements2/" + text).GetComponent<SpriteRenderer>().sprite;
	}
}
