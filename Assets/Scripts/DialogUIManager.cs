using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogUIManager : MonoBehaviour
{
	public enum ShowType
	{
		Left,
		Right
	}

	private static DialogUIManager instance;

	private Dialog currDialog;

	private bool isDialogStart;

	private bool underControl;

	private bool isShowSkipButton;

	private float showStringIndex;

	private bool showStringFinish;

	private bool isLeftDialogShow;

	private bool isRightDialogShow;

	private string leftImageName;

	private string rightImageName;

	private ShowType showType;

	private Tweener leftArrowTweener;

	private Tweener rightArrowTweener;

	private Vector3 leftArrowStartPosition;

	private Vector3 rightArrowStartPosition;

	private Vector3 leftRoleImageStartPosition;

	private Vector3 rightRoleImageStartPosition;

	private string currContent;

	public GameObject leftDialog;

	public GameObject rightDialog;

	public GameObject leftArrow;

	public GameObject rightArrow;

	public GameObject leftStartPosition;

	public GameObject leftEndPosition;

	public GameObject rightStartPosition;

	public GameObject rightEndPosition;

	public Image leftRoleImage;

	public Image rightRoleImage;

	public Image leftBg;

	public Image rightBg;

	public Text leftRoleName;

	public Text rightRoleName;

	public LocalizationText leftContent;

	public LocalizationText rightContent;

	public float showSpeed = 1f;

	public float offsetY = 20f;

	public float animTime = 0.5f;

	public float textMaxLenght;

	public static DialogUIManager Instance
	{
		get
		{
			return instance;
		}
	}

	public void ShowDialog(Dialog currDia)
	{
		underControl = true;
		showStringFinish = false;
		base.transform.gameObject.SetActive(true);
		showStringIndex = 0f;
		currDialog = currDia;
		AdaptLanguage();
		CastleSceneUIManager.Instance.gameObject.SetActive(true);
		AudioManager.Instance.PlayAudioEffect("new_task");
		if (currDialog.leftOrRight == 0)
		{
			textMaxLenght = leftContent.GetComponent<RectTransform>().rect.width;
			StartCoroutine(ShowLeftDialog());
		}
		else
		{
			textMaxLenght = rightContent.GetComponent<RectTransform>().rect.width;
			StartCoroutine(ShowRightDialog());
		}
	}

	public void AdaptLanguage()
	{
		if (LanguageConfig.GetCurrentLanguage() != SystemLanguage.Chinese && LanguageConfig.GetCurrentLanguage() != SystemLanguage.Korean && LanguageConfig.GetCurrentLanguage() != SystemLanguage.Japanese)
		{
			showSpeed = 200f;
		}
		else
		{
			showSpeed = 70f;
		}
		if (LanguageConfig.GetCurrentLanguage() != SystemLanguage.Chinese && LanguageConfig.GetCurrentLanguage() != SystemLanguage.Russian && LanguageConfig.GetCurrentLanguage() != SystemLanguage.Korean && LanguageConfig.GetCurrentLanguage() != SystemLanguage.Japanese)
		{
			leftContent.lineSpacing = 0.65f;
			rightContent.lineSpacing = 0.65f;
		}
		else
		{
			leftContent.lineSpacing = 1f;
			rightContent.lineSpacing = 1f;
		}
	}

	public void HideDialog()
	{
		isDialogStart = false;
		if (!PlotDialogManager.Instance.JudgeNextHaveDialog() && !PlotManager.Instance.JudgeNextStepHaveDialog())
		{
			isShowSkipButton = false;
			isLeftDialogShow = false;
			isRightDialogShow = false;
			leftImageName = "";
			rightImageName = "";
			leftRoleImage.transform.localPosition = leftRoleImageStartPosition;
			rightRoleImage.transform.localPosition = rightRoleImageStartPosition;
			leftDialog.SetActive(false);
			rightDialog.SetActive(false);
			CastleSceneUIManager.Instance.HideMask();
		}
	}

	private IEnumerator ShowLeftDialog()
	{
		yield return null;
		if (isRightDialogShow)
		{
			StartCoroutine(HideRightDialog());
		}
		leftDialog.SetActive(true);
		showType = ShowType.Left;
		leftArrowTweener.Play();
		rightArrowTweener.Pause();
		leftRoleName.text = currDialog.roleName;
		currContent = LanguageConfig.GetString(currDialog.content);
		leftContent.KeyString = "";
		leftContent.text = "";
		if (!isLeftDialogShow)
		{
			Sprite sprite = Resources.Load("Textures/Role/" + currDialog.Image, typeof(Sprite)) as Sprite;
			leftRoleImage.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
			leftRoleImage.sprite = sprite;
			leftRoleName.transform.parent.gameObject.SetActive(false);
			leftBg.gameObject.SetActive(true);
			leftBg.transform.localScale = new Vector3(0f, 1f, 1f);
			leftBg.transform.DOScaleX(1f, 0.3f);
			leftRoleImage.color = new Color(1f, 1f, 1f, 1f);
			leftRoleImage.transform.localScale = new Vector3(1f, 1f, 1f);
			leftRoleImage.transform.position = new Vector3(leftStartPosition.transform.position.x, leftRoleImage.transform.position.y, leftRoleImage.transform.position.z);
			leftRoleImage.transform.DOLocalMoveX(leftEndPosition.transform.localPosition.x, 0.3f);
			yield return new WaitForSeconds(0.3f);
			leftRoleName.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			if (leftImageName != currDialog.Image)
			{
				GameObject otherImage = Object.Instantiate(leftRoleImage.gameObject, leftRoleImage.transform.parent);
				otherImage.transform.SetAsFirstSibling();
				Sprite sprite2 = Resources.Load("Textures/Role/" + currDialog.Image, typeof(Sprite)) as Sprite;
				otherImage.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite2.textureRect.width, sprite2.textureRect.height);
				otherImage.GetComponent<Image>().sprite = sprite2;
				otherImage.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				otherImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
				leftRoleImage.DOFade(0f, 0.1f);
				leftRoleImage.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f);
				otherImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f);
				otherImage.GetComponent<Image>().DOFade(1f, 0.25f);
				yield return new WaitForSeconds(0.1f);
				Object.Destroy(leftRoleImage.gameObject);
				leftRoleImage = otherImage.GetComponent<Image>();
			}
			else
			{
				leftRoleImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
				leftRoleImage.DOColor(new Color(1f, 1f, 1f, 1f), 0.1f);
				yield return new WaitForSeconds(0.1f);
			}
			if (!leftBg.gameObject.activeInHierarchy)
			{
				leftBg.gameObject.SetActive(true);
				leftBg.transform.localScale = new Vector3(0f, 1f, 1f);
				leftBg.transform.DOScaleX(1f, 0.3f);
				leftRoleName.transform.parent.gameObject.SetActive(false);
				yield return new WaitForSeconds(0.3f);
				leftRoleName.transform.parent.gameObject.SetActive(true);
			}
		}
		leftImageName = currDialog.Image;
		isLeftDialogShow = true;
		isDialogStart = true;
	}

	private IEnumerator ShowRightDialog()
	{
		yield return null;
		if (isLeftDialogShow)
		{
			StartCoroutine(HideLeftDialog());
		}
		showType = ShowType.Right;
		rightDialog.SetActive(true);
		rightArrowTweener.Play();
		leftArrowTweener.Pause();
		rightRoleName.text = currDialog.roleName;
		currContent = LanguageConfig.GetString(currDialog.content);
		rightContent.KeyString = "";
		rightContent.text = "";
		if (!isRightDialogShow)
		{
			Sprite sprite = Resources.Load("Textures/Role/" + currDialog.Image, typeof(Sprite)) as Sprite;
			rightRoleImage.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
			rightRoleImage.sprite = sprite;
			rightRoleName.transform.parent.gameObject.SetActive(false);
			rightBg.gameObject.SetActive(true);
			rightBg.transform.localScale = new Vector3(0f, 1f, 1f);
			rightBg.transform.DOScaleX(1f, 0.3f);
			rightRoleImage.color = new Color(1f, 1f, 1f, 1f);
			rightRoleImage.transform.localScale = new Vector3(1f, 1f, 1f);
			rightRoleImage.transform.position = new Vector3(rightStartPosition.transform.position.x, rightRoleImage.transform.position.y, rightRoleImage.transform.position.z);
			rightRoleImage.transform.DOLocalMoveX(rightEndPosition.transform.localPosition.x, 0.3f);
			yield return new WaitForSeconds(0.3f);
			rightRoleName.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			if (rightImageName != currDialog.Image)
			{
				GameObject otherImage = Object.Instantiate(rightRoleImage.gameObject, rightRoleImage.transform.parent);
				otherImage.transform.SetAsFirstSibling();
				Sprite sprite2 = Resources.Load("Textures/Role/" + currDialog.Image, typeof(Sprite)) as Sprite;
				otherImage.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite2.textureRect.width, sprite2.textureRect.height);
				otherImage.GetComponent<Image>().sprite = sprite2;
				otherImage.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
				otherImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
				rightRoleImage.DOFade(0f, 0.1f);
				rightRoleImage.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f);
				otherImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f);
				otherImage.GetComponent<Image>().DOFade(1f, 0.25f);
				yield return new WaitForSeconds(0.1f);
				Object.Destroy(rightRoleImage.gameObject);
				rightRoleImage = otherImage.GetComponent<Image>();
			}
			else
			{
				rightRoleImage.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
				rightRoleImage.DOColor(new Color(1f, 1f, 1f, 1f), 0.1f);
				yield return new WaitForSeconds(0.1f);
			}
			if (!rightBg.gameObject.activeInHierarchy)
			{
				rightBg.gameObject.SetActive(true);
				rightBg.transform.localScale = new Vector3(0f, 1f, 1f);
				rightBg.transform.DOScaleX(1f, 0.3f);
				rightRoleName.transform.parent.gameObject.SetActive(false);
				yield return new WaitForSeconds(0.3f);
				rightRoleName.transform.parent.gameObject.SetActive(true);
			}
		}
		rightImageName = currDialog.Image;
		isRightDialogShow = true;
		isDialogStart = true;
	}

	private IEnumerator HideRightDialog()
	{
		yield return null;
		rightDialog.transform.SetAsFirstSibling();
		rightBg.gameObject.SetActive(false);
		rightRoleImage.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.25f);
		rightRoleImage.DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.25f);
	}

	private IEnumerator HideLeftDialog()
	{
		yield return null;
		leftDialog.transform.SetAsFirstSibling();
		leftBg.gameObject.SetActive(false);
		leftRoleImage.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.25f);
		leftRoleImage.DOColor(new Color(0.8f, 0.8f, 0.8f, 1f), 0.25f);
	}

	public void ForceHide()
	{
		underControl = false;
		isDialogStart = false;
		isShowSkipButton = false;
		leftDialog.SetActive(false);
		rightDialog.SetActive(false);
		isLeftDialogShow = false;
		isRightDialogShow = false;
		leftImageName = "";
		rightImageName = "";
		leftRoleImage.transform.localPosition = leftRoleImageStartPosition;
		rightRoleImage.transform.localPosition = rightRoleImageStartPosition;
	}

	private void Awake()
	{
		instance = this;
		leftArrowStartPosition = leftArrow.transform.localPosition;
		rightArrowStartPosition = rightArrow.transform.localPosition;
		leftArrowTweener = leftArrow.transform.DOLocalMoveY(leftArrowStartPosition.y + offsetY, animTime).SetLoops(-1, LoopType.Yoyo);
		rightArrowTweener = rightArrow.transform.DOLocalMoveY(rightArrowStartPosition.y + offsetY, animTime).SetLoops(-1, LoopType.Yoyo);
		leftRoleImageStartPosition = leftRoleImage.transform.localPosition;
		rightRoleImageStartPosition = rightRoleImage.transform.localPosition;
		leftArrowTweener.Pause();
		rightArrowTweener.Pause();
	}

	private void Update()
	{
		if (isDialogStart && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			if (underControl && showStringFinish)
			{
				HideDialog();
				isDialogStart = false;
				underControl = false;
				PlotDialogManager.Instance.FinishStep();
			}
			else if (underControl && !showStringFinish)
			{
				showStringFinish = true;
				if (showType == ShowType.Left)
				{
					leftContent.text = currContent;
				}
				else if (showType == ShowType.Right)
				{
					rightContent.text = currContent;
				}
			}
		}
		if (!showStringFinish && isDialogStart)
		{
			ShowString();
		}
		if (showStringFinish && isDialogStart && !isShowSkipButton)
		{
			CastleSceneUIManager.Instance.ShowSkipButton();
			isShowSkipButton = true;
		}
	}

	private void ShowString()
	{
		showStringIndex += showSpeed * Time.deltaTime;
		int num = (int)showStringIndex;
		DealShowString(num);
		string text = "";
		text = ((num < currContent.Length) ? currContent.Substring(0, num) : currContent);
		if (showType == ShowType.Left)
		{
			leftContent.text = text;
		}
		else if (showType == ShowType.Right)
		{
			rightContent.text = text;
		}
		if (showStringIndex > (float)currContent.Length)
		{
			showStringFinish = true;
		}
	}

	public float GetContentLength(string contentString)
	{
		float num = 0f;
		Font font = leftContent.font;
		font.RequestCharactersInTexture(contentString, leftContent.fontSize, leftContent.fontStyle);
		CharacterInfo info = default(CharacterInfo);
		char[] array = contentString.ToCharArray();
		foreach (char ch in array)
		{
			font.GetCharacterInfo(ch, out info, leftContent.fontSize);
			num += (float)info.advance;
		}
		return num;
	}

	public void DealShowString(int showIndex)
	{
		int num = currContent.LastIndexOf("\n");
		string text = "";
		string text2 = "";
		if (num == -1)
		{
			text = currContent;
		}
		else
		{
			text = currContent.Substring(num + 1);
			text2 = currContent.Substring(0, num);
		}
		string[] array = text.Split(' ');
		if (array.Length <= 1)
		{
			return;
		}
		string text3 = array[0];
		bool flag = false;
		int num2 = 0;
		for (int i = 1; i < array.Length; i++)
		{
			string text4 = text3 + " " + array[i];
			GetContentLength(text4);
			if (GetContentLength(text4) >= (float)((int)textMaxLenght - 10))
			{
				flag = true;
				num2 = i;
				text3 += "\n";
				break;
			}
			text3 = text3 + " " + array[i];
			if (text4.Length >= showIndex)
			{
				break;
			}
		}
		if (flag)
		{
			string text5 = array[0];
			for (int j = 1; j < array.Length; j++)
			{
				text5 = ((j == num2) ? (text5 + "\n" + array[j]) : (text5 + " " + array[j]));
			}
			if (text2 != "")
			{
				currContent = text2 + "\n" + text5;
			}
			else
			{
				currContent = text5;
			}
		}
	}
}
