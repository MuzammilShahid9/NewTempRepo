using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Item : MonoBehaviour
{
	public Sprite[] imageArray;

	public string[] iconImageArray;

	public string[] nameArray;

	public Sprite ssss;

	public int selectImage;

	public int tempSelectIndex;

	public int itemID;

	public Vector3 StartScale;

	public ItemType itemType;

	public ItemConfigData configData;

	public PolygonCollider2D collider;

	private SpriteRenderer render;

	private Tilemap tileMap;

	private bool itemPress;

	private float pressTime;

	private float showArrowTime;

	private float pressTimer;

	private Sprite defaultSprite;

	private Vector3 startPosition;

	private Vector3 mouseStartPosition;

	private SelectItemImage UpArrow;

	private void Start()
	{
		pressTime = GeneralConfig.ItemPressShowChangeTime;
		showArrowTime = GeneralConfig.ItemPressShowArrowTime;
		startPosition = base.transform.position;
		tempSelectIndex = -1;
		configData = ItemManager.Instance.GetItemConfInfo(itemID);
		itemType = (ItemType)configData.ItemType;
		if (itemType == ItemType.Tile)
		{
			tileMap = base.transform.GetComponent<Tilemap>();
		}
		else
		{
			render = GetComponent<SpriteRenderer>();
			if (render != null)
			{
				defaultSprite = render.sprite;
			}
		}
		iconImageArray = configData.IconImage.Split(';');
		string[] itemImageData = ItemManager.Instance.GetItemImageData(itemID);
		nameArray = ItemManager.Instance.GetItemImageNameData(itemID);
		StartScale = base.transform.localScale;
		imageArray = new Sprite[3];
		for (int i = 0; i < 3; i++)
		{
			if (itemImageData != null && i < itemImageData.Length && itemImageData[i] != "")
			{
				imageArray[i] = InitGame.Instance.GetAtlasImage(itemImageData[i]);
			}
		}
		UnlockitemDeal();
		ItemManager.Instance.AddItemToList(this);
	}

	public void UnlockitemDeal()
	{
		if (selectImage != -1)
		{
			if (itemType == ItemType.Image)
			{
				if (imageArray[selectImage] != null)
				{
					base.transform.GetComponent<ItemAnim>().ActiveSelectObject(selectImage);
				}
			}
			else
			{
				Showtile(selectImage);
			}
		}
		if (configData.CleanFlag == 1 && selectImage != -1)
		{
			base.gameObject.SetActive(false);
		}
		if (configData.EffectAlwayShow == 1 && selectImage != -1 && itemType != ItemType.Tile)
		{
			string[] array = configData.Effect.Split('|');
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(array[0]) as GameObject);
			float x = 0f;
			float y = 0f;
			if (array.Length >= 2)
			{
				string[] array2 = array[1].Substring(0, array[1].Length - 1).Split('*');
				x = Convert.ToSingle(array2[0]);
				y = Convert.ToSingle(array2[1]);
				Convert.ToSingle(array2[2]);
			}
			if (base.transform.parent != null)
			{
				gameObject.transform.SetParent(base.transform.parent);
				gameObject.transform.localPosition = new Vector3(x, y, 0f);
			}
			gameObject.GetComponent<ChangeTexture>();
		}
	}

	public void getData(string[] imageDataArray)
	{
		imageArray = new Sprite[3];
		for (int i = 0; i < 3; i++)
		{
			if (i < imageDataArray.Length && imageDataArray[i] != "")
			{
				imageArray[i] = InitGame.Instance.GetAtlasImage(imageDataArray[i]);
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		DebugUtils.Log(DebugType.Other, other.ToString());
	}

	private void OnMouseDown()
	{
	}

	public void DealPress()
	{
		if (!CastleSceneUIManager.Instance.GetSelectItemUIStatu())
		{
			DebugUtils.Log(DebugType.Other, "ui未展示");
		}
		itemPress = true;
	}

	private void OnMouseUp()
	{
		if (pressTimer < showArrowTime && itemType == ItemType.Image && configData.IsShake)
		{
			StartCoroutine(ShowItemImage());
		}
		HideArrow();
		StartCoroutine(UnlockCamera());
		itemPress = false;
		pressTimer = 0f;
	}

	private void HideArrow()
	{
		if (UpArrow != null)
		{
			UpArrow.gameObject.SetActive(false);
			UnityEngine.Object.Destroy(UpArrow);
			UpArrow = null;
		}
	}

	private void CreateArrow(Vector3 mousePosition)
	{
		UpArrow = UnityEngine.Object.Instantiate(Resources.Load("Prefabs/UI/SelectItemImage") as GameObject).GetComponent<SelectItemImage>();
		UpArrow.transform.SetParent(CastleSceneUIManager.Instance.transform);
		UpArrow.transform.localScale = new Vector3(1f, 1f, 1f);
		UpArrow.transform.localPosition = new Vector3(mousePosition.x - (float)(Screen.width / 2), mousePosition.y - (float)(Screen.height / 2), mousePosition.z);
	}

	private void Update()
	{
		if (itemPress)
		{
			pressTimer += Time.deltaTime;
		}
		if (pressTimer >= pressTime && itemPress && mouseStartPosition == Input.mousePosition)
		{
			HideArrow();
			itemPress = false;
			if (itemType == ItemType.Image)
			{
				StartCoroutine(ShowItemImage());
			}
			if (imageArray != null)
			{
				CastleSceneUIManager.Instance.ShowChangeItemUI(this);
			}
			FininshTutorial();
		}
		else if (pressTimer >= showArrowTime && itemPress && UpArrow == null && mouseStartPosition == Input.mousePosition)
		{
			GlobalVariables.UnderChangeItemControl = true;
			CreateArrow(Input.mousePosition);
		}
		else if (pressTimer >= showArrowTime && itemPress && UpArrow != null && mouseStartPosition == Input.mousePosition)
		{
			UpArrow.Enter(pressTimer / pressTime);
		}
		else
		{
			HideArrow();
		}
	}

	public void FininshTutorial()
	{
		if (GlobalVariables.ShowingTutorial)
		{
			TutorialManager.Instance.FinishTutorial();
		}
	}

	public void ShowImage(int index)
	{
		if (itemType == ItemType.Image)
		{
			if (index < imageArray.Length && index != -1)
			{
				render.sprite = imageArray[index];
				base.transform.localScale = StartScale;
			}
			else
			{
				render.sprite = defaultSprite;
				base.transform.localScale = StartScale;
			}
			StartCoroutine(ShowItemImage());
		}
		else if (itemType == ItemType.Tile)
		{
			Showtile(index);
		}
	}

	public void Showtile(int index)
	{
		string[] array = configData.Tile.Split(';');
		TileBase tile = Resources.Load("Wallpaper/Tile/" + array[index + 1], typeof(TileBase)) as TileBase;
		Resources.Load("Wallpaper/Tile/" + array[tempSelectIndex + 1], typeof(TileBase));
		BoundsInt cellBounds = tileMap.cellBounds;
		for (int i = cellBounds.xMin; i < cellBounds.xMax; i++)
		{
			for (int j = cellBounds.yMin; j < cellBounds.yMax; j++)
			{
				TileBase tile2 = tileMap.GetTile(new Vector3Int(i, j, 0));
				if (tile2 != null && tile2.name == array[tempSelectIndex + 1])
				{
					tileMap.SetTile(new Vector3Int(i, j, 0), tile);
				}
			}
		}
		tempSelectIndex = index;
	}

	public void StartAnim(string animStep)
	{
		StopAllCoroutines();
		StartCoroutine(DoAnimator(animStep));
	}

	public void StopAllAni()
	{
		StopAllCoroutines();
		base.transform.position = startPosition;
		base.transform.rotation = Quaternion.identity;
		base.transform.localScale = StartScale;
	}

	private IEnumerator ShowItemImage()
	{
		base.transform.DOMoveY(startPosition.y + 0.1f, 0.2f);
		base.transform.DOScale(new Vector3(0.95f * StartScale.x, 1.05f * StartScale.y, 1f * StartScale.z), 0.2f);
		yield return new WaitForSeconds(0.2f);
		base.transform.DOMoveY(startPosition.y, 0.2f);
		base.transform.DOScale(StartScale, 0.2f);
	}

	private IEnumerator UnlockCamera()
	{
		yield return new WaitForSeconds(0.1f);
		GlobalVariables.UnderChangeItemControl = false;
	}

	private IEnumerator DoAnimator(string animStep)
	{
		yield return null;
		string[] animStepArray = animStep.Split(';');
		for (int i = 0; i < animStepArray.Length; i++)
		{
			if (!(animStepArray[i] != ""))
			{
				continue;
			}
			float maxWaitTime = 0f;
			string[] stepDetail = animStepArray[i].Split('|');
			for (int j = 0; j < stepDetail.Length; j++)
			{
				if (!(stepDetail[j] != ""))
				{
					continue;
				}
				float num = 0f;
				Vector3 vector = new Vector3(0f, 0f, 0f);
				if (stepDetail[j].Substring(0, 1) == "S" || stepDetail[j].Substring(0, 1) == "M" || stepDetail[j].Substring(0, 1) == "R")
				{
					stepDetail[j].Substring(0, 1);
					num = Convert.ToSingle(stepDetail[j].Split(')')[1]);
					if (num > maxWaitTime)
					{
						maxWaitTime = num;
					}
					string[] array = stepDetail[j].Split('(')[1].Split(')')[0].Split(',');
					vector = new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
				}
				else if (stepDetail[j].Substring(0, 1) == "D")
				{
					num = Convert.ToSingle(stepDetail[j].Substring(1));
					if (num > maxWaitTime)
					{
						maxWaitTime += num;
					}
				}
				if (stepDetail[j].Substring(0, 1) == "S")
				{
					ShortcutExtensions.DOScale(endValue: new Vector3(vector.x * StartScale.x, vector.y * StartScale.y, vector.z * StartScale.z), target: base.transform, duration: num);
				}
				else if (stepDetail[j].Substring(0, 1) == "M")
				{
					base.transform.DOMove(startPosition + vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "R")
				{
					base.transform.DORotate(vector, num);
				}
				else if (stepDetail[j].Substring(0, 1) == "D")
				{
					yield return new WaitForSeconds(Convert.ToSingle(stepDetail[j].Substring(1)));
				}
			}
			yield return new WaitForSeconds(maxWaitTime);
		}
		PlotItemAniManager.Instance.FinishStep();
	}
}
