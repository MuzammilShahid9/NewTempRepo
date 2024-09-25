using System.Collections;
using DG.Tweening;
using PlayInfinity.AliceMatch3.CinemaDirector;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemAnim : MonoBehaviour
{
	public GameObject effectObj;

	public GameObject originalImage;

	public GameObject hideGameObject;

	public GameObject[] imageArray;

	public BoxCollider specialBoxCollider;

	public ItemType itemType;

	public Sprite[] iconImageArray;

	public Sprite[] changeTextureImageArray1;

	public Sprite[] changeTextureImageArray2;

	public Sprite[] changeTextureMeshArray1;

	public Sprite[] changeTextureMeshArray2;

	public int cameraPositon;

	public float cameraMoveTime;

	public int selectImage;

	public int tempSelectIndex;

	public bool clearFlag;

	public bool isShake;

	public bool isChangeSortIndex;

	public bool isPlaySingleAnim;

	public bool isFirstHideOriginal;

	public bool isImageAfterEffectShow;

	public bool isEffectNotInOriginal;

	public int sortIndexOffset;

	public Vector3 effectPositionOffset = new Vector3(0f, 0f, 0f);

	[HideInInspector]
	public GameObject effectGameObject;

	protected bool effectIsChild;

	protected bool isAnimFinish;

	protected BuildType buileType;

	protected Vector3 mouseStartPosition;

	protected Vector3 startPosition;

	protected Vector3 StartScale;

	protected bool itemPress;

	protected SelectItemLoading UpArrow;

	private float pressTimer;

	private float pressTime;

	private float showArrowTime;

	private SpriteRenderer sprite;

	[HideInInspector]
	public int roomID;

	[HideInInspector]
	public int itemID;

	public virtual void Awake()
	{
		buileType = BuildType.Build;
	}

	private void Start()
	{
		selectImage = -1;
		tempSelectIndex = -1;
		pressTime = GeneralConfig.ItemPressShowChangeTime;
		showArrowTime = GeneralConfig.ItemPressShowArrowTime;
		startPosition = base.transform.position;
		StartScale = base.transform.localScale;
		DealItemUnlock();
		sprite = base.transform.GetComponent<SpriteRenderer>();
		ChangeLayer();
	}

	private void Update()
	{
		if (itemPress)
		{
			pressTimer += Time.deltaTime;
		}
		if (pressTimer >= pressTime && itemPress && !JudgeTouchMove())
		{
			HideArrow();
			itemPress = false;
			if (itemType == ItemType.Image)
			{
				bool isShake2 = isShake;
			}
			if (imageArray != null)
			{
				GlobalVariables.UnderChangeItemControl = true;
				CastleSceneUIManager.Instance.ShowChangeItemUI(base.transform);
				if (effectGameObject != null)
				{
					Object.Destroy(effectGameObject);
				}
			}
			FininshTutorial();
		}
		else if (pressTimer >= showArrowTime && itemPress && UpArrow == null && !JudgeTouchMove())
		{
			CreateArrow(Input.mousePosition);
			GlobalVariables.UnderChangeItemControl = true;
		}
		else if (pressTimer >= showArrowTime && itemPress && UpArrow != null && !JudgeTouchMove())
		{
			UpArrow.Enter((pressTimer - showArrowTime) / (pressTime - showArrowTime));
		}
		else
		{
			HideArrow();
		}
	}

	private bool JudgeTouchMove()
	{
		if (Mathf.Abs(mouseStartPosition.x - Input.mousePosition.x) < 20f && Mathf.Abs(mouseStartPosition.y - Input.mousePosition.y) < 20f)
		{
			return false;
		}
		return true;
	}

	public void ChangeLayer()
	{
		if (isChangeSortIndex)
		{
			ChangeLayerWithTransform(base.transform);
		}
	}

	public void ChangeLayerWithTransform(Transform trans)
	{
		SpriteRenderer component = trans.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			float y = CameraControl.Instance.defaultCamera.WorldToScreenPoint(component.transform.position).y;
			component.sortingOrder = 10000 - (int)(y * 10f) + sortIndexOffset * CameraControl.Instance.defaultCamera.pixelHeight / 720;
		}
		foreach (Transform tran in trans)
		{
			if (tran != base.transform)
			{
				ChangeLayerWithTransform(tran);
			}
		}
	}

	public void FininshTutorial()
	{
		if (GlobalVariables.ShowingTutorial)
		{
			TutorialManager.Instance.FinishTutorial();
		}
	}

	public virtual bool JudgeIsSelectItem()
	{
		if (buileType == BuildType.Build)
		{
			return false;
		}
		return true;
	}

	public virtual void DealItemUnlock()
	{
		if (itemType == ItemType.Image)
		{
			selectImage = UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID);
		}
		if (selectImage != -1)
		{
			if (clearFlag)
			{
				Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != base.transform)
					{
						componentsInChildren[i].gameObject.SetActive(false);
					}
				}
			}
			else
			{
				ActiveSelectObject(selectImage);
				DealHideGameObject(true);
			}
		}
		else
		{
			for (int j = 0; j < imageArray.Length; j++)
			{
				imageArray[j].SetActive(false);
			}
			if (originalImage != null)
			{
				originalImage.SetActive(true);
			}
			DealHideGameObject(false);
		}
	}

	public virtual void PlayEffect(float roleAnimWaitEffectTime)
	{
		StartCoroutine(WaitForPlayEffect(roleAnimWaitEffectTime));
	}

	private IEnumerator WaitForPlayEffect(float roleAnimWaitEffectTime)
	{
		PlotManager.Instance.PlotInsertRoleAction();
		yield return new WaitForSeconds(roleAnimWaitEffectTime);
		isAnimFinish = false;
		effectIsChild = true;
		SpriteRenderer[] componentsInChildren = originalImage.transform.GetComponentsInChildren<SpriteRenderer>();
		DealHideGameObject(true);
		if (isFirstHideOriginal)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if ((!isPlaySingleAnim || j != 0) && isPlaySingleAnim)
			{
				continue;
			}
			effectGameObject = Object.Instantiate(effectObj);
			if (isEffectNotInOriginal)
			{
				effectGameObject.transform.SetParent(base.transform);
			}
			else
			{
				effectGameObject.transform.SetParent(componentsInChildren[j].transform);
			}
			effectGameObject.transform.localPosition = new Vector3(0f, 0f, 0f) + effectPositionOffset;
			ChangeTexture component = effectGameObject.GetComponent<ChangeTexture>();
			if (component != null && j == 0)
			{
				component.Enter(base.transform, true);
				continue;
			}
			ParitcleSystem component2 = effectGameObject.GetComponent<ParitcleSystem>();
			if (component2 != null)
			{
				component2.Enter(base.transform);
			}
			else
			{
				component.Enter(base.transform, false);
			}
		}
		StartCoroutine(WaitForChangeImage());
	}

	public virtual IEnumerator WaitForChangeImage()
	{
		yield return new WaitUntil(() => isAnimFinish);
		SpriteRenderer component = originalImage.GetComponent<SpriteRenderer>();
		if (component != null)
		{
			component.enabled = false;
		}
		if (isImageAfterEffectShow)
		{
			ActiveSelectObject(0);
			if (isEffectNotInOriginal)
			{
				Object.Destroy(effectGameObject);
			}
		}
		PlotItemAniManager.Instance.FinishStep();
	}

	public void ActiveSelectObject(int index)
	{
		if (imageArray.Length != 0)
		{
			for (int i = 0; i < imageArray.Length; i++)
			{
				imageArray[i].SetActive(false);
			}
			originalImage.SetActive(false);
			imageArray[index].SetActive(true);
		}
	}

	public void Enter(int roomIndex, int itemIndex)
	{
		roomID = roomIndex;
		itemID = itemIndex;
		DealItemUnlock();
	}

	private void CreateArrow(Vector3 mousePosition)
	{
		if (CastleSceneUIManager.Instance != null)
		{
			UpArrow = Object.Instantiate(Resources.Load("Prefabs/UI/SelectItemLoading") as GameObject).GetComponent<SelectItemLoading>();
			UpArrow.transform.SetParent(CastleSceneUIManager.Instance.transform);
			Vector2 sizeDelta = CastleSceneUIManager.Instance.GetComponent<RectTransform>().sizeDelta;
			UpArrow.transform.localScale = new Vector3(1f, 1f, 1f);
			UpArrow.transform.localPosition = new Vector3((mousePosition.x / (float)Screen.width - 0.5f) * sizeDelta.x, (mousePosition.y / (float)Screen.height - 0.5f) * sizeDelta.y + 100f, mousePosition.z);
		}
	}

	public void FinishAnim()
	{
		isAnimFinish = true;
	}

	public virtual void DealHideGameObject(bool isHide)
	{
		if (!(hideGameObject == null))
		{
			if (isHide)
			{
				hideGameObject.SetActive(false);
			}
			else
			{
				hideGameObject.SetActive(true);
			}
		}
	}

	public virtual void StopAllAni()
	{
		StopAllCoroutines();
	}

	public virtual void ShowImage(int index)
	{
	}

	public virtual void SelectShowImage(int index, bool notShowSelectAnim = false)
	{
	}

	public virtual void FinishBuild()
	{
	}

	public virtual void PlayStageEffect(int stage, float roleAnimWaitEffectTime)
	{
	}

	private void OnMouseDown()
	{
		MouseDownEvent();
	}

	public void MouseDownEvent()
	{
		if (!GlobalVariables.UnderChangeItemControl)
		{
			MouseDown();
		}
		else if (PlotManager.Instance.isPlotFinish && DialogManagerTemp.Instance.IsDialogOpening() <= 0)
		{
			bool flag = false;
			flag = CameraControl.Instance.JudgeClickOnUI();
			if (UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID) != -1 && !flag)
			{
				CastleSceneUIManager.Instance.ShowChangeItemUI(base.transform);
			}
		}
	}

	public void MouseDown()
	{
		DebugUtils.Log(DebugType.Other, "点击家具:roomId:" + roomID + "itemId:" + itemID);
		mouseStartPosition = Input.mousePosition;
		PlotManager.Instance.GetPlotUnlockItemInfo(GeneralConfig.ChangeItemTutorialStartPlot);
		if (UserDataManager.Instance.GetItemUnlockInfo(roomID, itemID) != -1 && PlotManager.Instance.isPlotFinish && DialogManagerTemp.Instance.IsDialogOpening() <= 0)
		{
			if (!EventSystem.current.IsPointerOverGameObject())
			{
				DealPress();
			}
			else if (EventSystem.current.IsPointerOverGameObject() && UserDataManager.Instance.GetService().tutorialProgress == 7 && roomID == 2 && itemID == 3)
			{
				DealPress();
			}
		}
	}

	public void DealPress()
	{
		if (!CastleSceneUIManager.Instance.GetSelectItemUIStatu())
		{
			DebugUtils.Log(DebugType.Plot, "ui未展示");
		}
		itemPress = true;
	}

	public void DealSpecialBoxColliderEnable()
	{
		if (specialBoxCollider != null)
		{
			specialBoxCollider.enabled = true;
		}
	}

	public void DealSpecialBoxColliderDisable()
	{
		if (specialBoxCollider != null)
		{
			specialBoxCollider.enabled = false;
		}
	}

	private void OnMouseUp()
	{
		MouseUp();
	}

	public void MouseUp()
	{
		if (pressTimer < showArrowTime)
		{
			if (itemType == ItemType.Image && !isShake)
			{
			}
		}
		else if (pressTimer >= showArrowTime)
		{
			CameraControl.Instance.RestortCameraDealyMove();
		}
		if (!CastleSceneUIManager.Instance.GetSelectItemUIStatu())
		{
			GlobalVariables.UnderChangeItemControl = false;
		}
		HideArrow();
		itemPress = false;
		pressTimer = 0f;
	}

	private void HideArrow()
	{
		if (UpArrow != null)
		{
			UpArrow.gameObject.SetActive(false);
			Object.Destroy(UpArrow.gameObject);
			UpArrow = null;
		}
	}

	private IEnumerator ShowItemImage()
	{
		base.transform.DOMoveY(startPosition.y + 0.1f, 0.2f);
		base.transform.DOScale(new Vector3(0.95f * StartScale.x, 1.05f * StartScale.y, 1f * StartScale.z), 0.2f);
		yield return new WaitForSeconds(0.2f);
		base.transform.DOMoveY(startPosition.y, 0.2f);
		base.transform.DOScale(StartScale, 0.2f);
	}
}
