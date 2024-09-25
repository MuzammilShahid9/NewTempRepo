using System.Collections;
using UnityEngine;

public class GroupSingleSelectAnimation : GroupSingleItem
{
	public Animation anim;

	public SkinnedMeshRenderer render;

	public Sprite[] changeTextureMeshArray1;

	public override void Enter(int index, bool isSelect = false)
	{
		base.gameObject.SetActive(true);
		ShowImage(index, isSelect);
	}

	public override IEnumerator StartPlayEffect()
	{
		ItemAnim component = base.transform.parent.GetComponent<ItemAnim>();
		Texture2D texture = changeTextureMeshArray1[component.selectImage].texture;
		render.material.mainTexture = texture;
		if (!notHide)
		{
			originalImage.gameObject.SetActive(false);
		}
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (imageArray[i] != null)
			{
				imageArray[i].SetActive(false);
			}
		}
		if (effect != null)
		{
			effect.gameObject.SetActive(true);
		}
		yield return new WaitForSeconds(anim.clip.length);
		if (!isEffectAlwayShow)
		{
			imageArray[selectImage].SetActive(true);
			if (effect != null)
			{
				effect.gameObject.SetActive(false);
			}
			ShowImage(selectImage);
			PlotItemAniManager.Instance.FinishStep();
		}
	}
}
