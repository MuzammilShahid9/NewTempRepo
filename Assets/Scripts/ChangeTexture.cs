using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
	private Animator[] childrenAni;

	public MeshRenderer[] meshRenderer1;

	public MeshRenderer[] meshRenderer2;

	public SpriteRenderer[] childrenSprite1;

	public SpriteRenderer[] childrenSprite2;

	private bool isSend;

	private bool finishSendPlot;

	private float timer;

	private float time = 0.1f;

	private ItemAnim currItemAnim;

	private void Start()
	{
		isSend = false;
		childrenAni = base.transform.GetComponentsInChildren<Animator>();
		timer = 0f;
	}

	public virtual void Update()
	{
		if (isSend || !finishSendPlot)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer > time)
		{
			timer = 0f;
			if (JudegeAnimationFinish() && currItemAnim != null)
			{
				currItemAnim.FinishAnim();
				isSend = true;
			}
		}
	}

	public bool JudegeAnimationFinish()
	{
		bool result = true;
		for (int i = 0; i < childrenAni.Length; i++)
		{
			if (childrenAni[i].GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
			{
				result = false;
			}
		}
		return result;
	}

	public virtual void Enter(Transform tempTransform, bool isFinishSend)
	{
		currItemAnim = tempTransform.GetComponent<ItemAnim>();
		if (!isFinishSend)
		{
			finishSendPlot = false;
		}
		else
		{
			finishSendPlot = true;
		}
		if (currItemAnim.selectImage == -1 || currItemAnim.changeTextureImageArray1.Length == 0)
		{
			return;
		}
		Sprite sprite = currItemAnim.changeTextureImageArray1[currItemAnim.selectImage];
		for (int i = 0; i < childrenSprite1.Length; i++)
		{
			childrenSprite1[i].sprite = sprite;
		}
		if (currItemAnim.changeTextureImageArray2.Length >= 2)
		{
			Sprite sprite2 = currItemAnim.changeTextureImageArray2[currItemAnim.selectImage];
			for (int j = 0; j < childrenSprite2.Length; j++)
			{
				childrenSprite2[j].sprite = sprite2;
			}
		}
		if (currItemAnim.changeTextureMeshArray1.Length == 0)
		{
			return;
		}
		Texture2D texture = currItemAnim.changeTextureMeshArray1[currItemAnim.selectImage].texture;
		for (int k = 0; k < meshRenderer1.Length; k++)
		{
			meshRenderer1[k].material.mainTexture = texture;
		}
		if (currItemAnim.changeTextureMeshArray2.Length >= 2)
		{
			Texture2D texture2 = currItemAnim.changeTextureMeshArray2[currItemAnim.selectImage].texture;
			for (int l = 0; l < meshRenderer2.Length; l++)
			{
				meshRenderer2[l].material.mainTexture = texture2;
			}
		}
	}
}
