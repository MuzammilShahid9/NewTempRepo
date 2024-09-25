using UnityEngine;

public class SingleChangeTexture : ChangeTexture
{
	private Item_Single_Diff_Effect currSingleItemAnim;

	public override void Update()
	{
	}

	public override void Enter(Transform tempTransform, bool isFinishSend)
	{
		currSingleItemAnim = tempTransform.GetComponent<Item_Single_Diff_Effect>();
		if (currSingleItemAnim.selectImage == -1 || currSingleItemAnim.changeTextureImageArray1.Length == 0)
		{
			return;
		}
		Sprite sprite = currSingleItemAnim.changeTextureImageArray1[currSingleItemAnim.selectImage];
		for (int i = 0; i < childrenSprite1.Length; i++)
		{
			childrenSprite1[i].sprite = sprite;
		}
		if (currSingleItemAnim.changeTextureMeshArray1.Length != 0)
		{
			Texture2D texture = currSingleItemAnim.changeTextureMeshArray1[currSingleItemAnim.selectImage].texture;
			for (int j = 0; j < meshRenderer1.Length; j++)
			{
				meshRenderer1[j].material.mainTexture = texture;
			}
		}
	}
}
