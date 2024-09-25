using System.Collections;
using UnityEngine;

public class ItemSingleAnim : ItemAnim
{
	public Animator effectAnimator;

	public override void PlayEffect(float roleAnimWaitEffectTime)
	{
		StartCoroutine(WaitForPlayEffect(roleAnimWaitEffectTime));
	}

	private IEnumerator WaitForPlayEffect(float roleAnimWaitEffectTime)
	{
		PlotManager.Instance.PlotInsertRoleAction();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		originalImage.gameObject.SetActive(false);
		imageArray[0].SetActive(true);
		effectAnimator.enabled = true;
		yield return new WaitForSeconds(effectAnimator.GetCurrentAnimatorStateInfo(0).length);
		PlotItemAniManager.Instance.FinishStep();
		effectAnimator.enabled = false;
	}
}
