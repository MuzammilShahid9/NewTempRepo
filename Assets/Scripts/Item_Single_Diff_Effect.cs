using System.Collections;
using UnityEngine;

public class Item_Single_Diff_Effect : GroupSingleItem
{
	public Sprite[] changeTextureImageArray1;

	public Sprite[] changeTextureMeshArray1;

	public Vector3 effectPositionOffset = new Vector3(0f, 0f, 0f);

	public override void Enter(int index, bool isSelect = false)
	{
		base.gameObject.SetActive(true);
		ShowImage(index, isSelect);
	}

	public override IEnumerator StartPlayEffect()
	{
		originalImage.gameObject.SetActive(false);
		for (int i = 0; i < imageArray.Length; i++)
		{
			if (imageArray[i] != null)
			{
				imageArray[i].SetActive(false);
			}
		}
		effectGameObject = Object.Instantiate(effect);
		effectGameObject.transform.SetParent(base.transform);
		effectGameObject.transform.localPosition = imageArray[selectImage].transform.localPosition + effectPositionOffset;
		effectGameObject.transform.localScale = imageArray[selectImage].transform.localScale;
		effectGameObject.GetComponent<ChangeTexture>().Enter(base.transform, false);
		yield return null;
	}
}
