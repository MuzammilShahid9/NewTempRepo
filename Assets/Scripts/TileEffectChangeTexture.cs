using UnityEngine;

public class TileEffectChangeTexture : MonoBehaviour
{
	public MeshRenderer[] meshRenderer1;

	public MeshRenderer[] meshRenderer2;

	public SpriteRenderer[] spriteArray;

	public SpriteMask mask;

	private float timer;

	private float time = 0.1f;

	private Animator[] childrenAni;

	public float waitTime = 0.25f;

	private bool isSend;

	private bool isAnimFinishSend;

	private bool isFinish;

	private ItemTileChange currItemAnim;

	private void Start()
	{
		isSend = false;
		isAnimFinishSend = false;
		isFinish = false;
		childrenAni = base.transform.GetComponentsInChildren<Animator>();
		timer = 0f;
	}

	private void Update()
	{
		if (!isAnimFinishSend)
		{
			JudegeAnimationFinish();
			if (isFinish)
			{
				base.transform.parent.GetComponent<ItemTileChange>().FinishOneAni();
				isAnimFinishSend = true;
			}
		}
	}

	public void JudegeAnimationFinish()
	{
		bool flag = true;
		for (int i = 0; i < childrenAni.Length; i++)
		{
			if (childrenAni[i].GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				flag = false;
			}
		}
		if (flag)
		{
			isFinish = true;
		}
	}

	public void Enter(Transform tempTransform, int count, int index)
	{
		currItemAnim = tempTransform.GetComponent<ItemTileChange>();
		if (currItemAnim.isWallPaper)
		{
			if (currItemAnim.selectImage != -1 && currItemAnim.changeSpriteArray[index] != null)
			{
				Sprite sprite = currItemAnim.changeSpriteArray[index].changeSprite[currItemAnim.selectImage];
				for (int i = 0; i < spriteArray.Length; i++)
				{
					spriteArray[i].sprite = sprite;
					spriteArray[i].sortingOrder = count;
				}
				if (mask != null)
				{
					mask.frontSortingOrder = count;
					mask.backSortingOrder = count - 1;
				}
			}
		}
		else
		{
			if (currItemAnim.selectImage == -1)
			{
				return;
			}
			if (currItemAnim.changeTextureImageArray1.Length != 0)
			{
				Sprite sprite2 = currItemAnim.changeTextureImageArray1[currItemAnim.selectImage];
				for (int j = 0; j < spriteArray.Length; j++)
				{
					spriteArray[j].sprite = sprite2;
					spriteArray[j].sortingOrder = count;
				}
				if (mask != null)
				{
					mask.frontSortingOrder = count;
					mask.backSortingOrder = count - 1;
				}
			}
			if (currItemAnim.changeTextureMeshArray1.Length != 0)
			{
				Texture2D texture = currItemAnim.changeTextureMeshArray1[currItemAnim.selectImage].texture;
				for (int k = 0; k < meshRenderer1.Length; k++)
				{
					meshRenderer1[k].materials[0].mainTexture = texture;
				}
			}
		}
	}

	public Sprite GetResourceImage(string spriteName)
	{
		return Resources.Load("Wallpaper/Antechamber/" + spriteName, typeof(Sprite)) as Sprite;
	}
}
