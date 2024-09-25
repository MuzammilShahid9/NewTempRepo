using UnityEngine;
using UnityEngine.Rendering;

public class WallPaperEffectChange : MonoBehaviour
{
	public MeshRenderer[] meshRenderer1;

	public MeshRenderer[] meshRenderer2;

	public SpriteRenderer[] spriteArray;

	public SpriteRenderer[] spriteArray2;

	public SpriteMask mask;

	public bool isFlip;

	private float timer;

	private float time = 0.1f;

	private Animator[] childrenAni;

	public float waitTime = 0.25f;

	private bool isSend;

	private bool isAnimFinishSend;

	private bool isFinish;

	private ItemChangeWallPaper currItemAnim;

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
				base.transform.parent.parent.parent.GetComponent<ItemChangeWallPaper>().FinishOneAni();
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
		currItemAnim = tempTransform.GetComponent<ItemChangeWallPaper>();
		if (currItemAnim.selectImage == -1)
		{
			return;
		}
		if (currItemAnim.changeSpriteArray[index] != null)
		{
			SortingGroup component = base.transform.GetComponent<SortingGroup>();
			if (component != null)
			{
				component.sortingOrder = count;
			}
			Sprite sprite = currItemAnim.changeSpriteArray[index].changeSprite[currItemAnim.selectImage + 1];
			for (int i = 0; i < spriteArray.Length; i++)
			{
				spriteArray[i].sprite = sprite;
				spriteArray[i].sortingOrder = count;
				if (currItemAnim.isFlip)
				{
					spriteArray[i].flipX = true;
				}
			}
			if (mask != null)
			{
				mask.frontSortingOrder = count;
				mask.backSortingOrder = count - 1;
			}
			if (meshRenderer1.Length != 0)
			{
				for (int j = 0; j < meshRenderer1.Length; j++)
				{
					meshRenderer1[j].GetComponent<SortingGroup>().sortingOrder = count;
					meshRenderer1[j].material.mainTexture = currItemAnim.changeTextureMeshArray1[currItemAnim.selectImage].texture;
				}
			}
		}
		if (currItemAnim.changeSpriteArray1.Length < index || currItemAnim.changeSpriteArray1[index] == null)
		{
			return;
		}
		Sprite sprite2 = currItemAnim.changeSpriteArray1[index].changeSprite[currItemAnim.selectImage];
		for (int k = 0; k < spriteArray2.Length; k++)
		{
			spriteArray2[k].sprite = sprite2;
			spriteArray2[k].sortingOrder = count;
			if (currItemAnim.isFlip)
			{
				spriteArray[k].flipX = true;
			}
		}
	}

	public Sprite GetResourceImage(string spriteName)
	{
		return Resources.Load("Wallpaper/Antechamber/" + spriteName, typeof(Sprite)) as Sprite;
	}
}
