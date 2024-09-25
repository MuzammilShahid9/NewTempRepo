using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial3 : MonoBehaviour
{
	public GameObject arrow;

	public Image maskImage;

	public ItemAnim currItemAnim;

	public GameObject dialog;

	public GameObject hand;

	private Vector3 arrowStartPosition;

	private void Start()
	{
		base.transform.GetComponent<Canvas>().worldCamera = GameObject.Find("UICamera").transform.GetComponent<Camera>();
		arrowStartPosition = arrow.transform.localPosition;
		arrow.transform.DOLocalMoveY(arrowStartPosition.y + 20f, 0.5f).SetLoops(-1, LoopType.Yoyo);
		PlotManager.Instance.GetPlotUnlockItemInfo(GeneralConfig.ChangeItemTutorialStartPlot);
		currItemAnim = CastleManager.Instance.GetItem(1, 2);
		currItemAnim.DealSpecialBoxColliderEnable();
		Vector3 position = currItemAnim.originalImage.transform.position;
		CastleSceneUIManager.Instance.HideArrow();
		maskImage.gameObject.SetActive(true);
		dialog.SetActive(false);
		hand.SetActive(false);
		StartCoroutine(DelayShowMask());
	}

	private IEnumerator DelayShowMask()
	{
		yield return new WaitForSeconds(1.1f);
		dialog.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		hand.SetActive(true);
	}

	private void OnDisable()
	{
		CastleSceneUIManager.Instance.ShowArrow();
		currItemAnim.DealSpecialBoxColliderDisable();
	}
}
