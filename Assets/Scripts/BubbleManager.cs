using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour
{
	private Role followTarget;

	private Vector2 positionOffest;

	private Vector3 arrowStartPosition = new Vector3(15f, 0f, 0f);

	private float roleHeight = 1.45f;

	public Text contentText;

	public Image textImag;

	public Image arrowImg;

	public float maxWidth = 250f;

	public void Enter(Role target, string content)
	{
		base.transform.GetComponent<Canvas>().sortingLayerName = "UI";
		maxWidth = GeneralConfig.BubbleMaxLength;
		base.transform.SetAsFirstSibling();
		followTarget = target;
		roleHeight = followTarget.roleHeight;
		contentText.text = content;
		SetPosition();
		SetArrowPosition();
		StartCoroutine(DealSize());
		StartCoroutine(DealBubbleHide());
	}

	private IEnumerator DealSize()
	{
		yield return null;
		string text = contentText.text;
		int fontSize = contentText.fontSize;
		if (contentText.preferredWidth < maxWidth)
		{
			contentText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(contentText.preferredWidth, contentText.transform.GetComponent<RectTransform>().sizeDelta.y);
			base.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(contentText.preferredWidth + 20f, (contentText.preferredWidth / maxWidth + 1f) * (float)(fontSize + 2) + 30f);
			textImag.GetComponent<RectTransform>().sizeDelta = new Vector2(contentText.preferredWidth + 20f, contentText.transform.GetComponent<RectTransform>().sizeDelta.y + 3f);
		}
		else
		{
			contentText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, (contentText.preferredWidth / maxWidth + 1f) * (float)(fontSize + 2));
			base.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + 20f, (contentText.preferredWidth / maxWidth + 1f) * (float)(fontSize + 2) + 30f);
			textImag.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + 20f, (contentText.preferredWidth / maxWidth + 1f) * (float)(fontSize + 2) + 3f);
		}
		yield return null;
		if (contentText.transform.GetComponent<RectTransform>().sizeDelta.x >= maxWidth)
		{
			contentText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth, contentText.preferredHeight + 5f);
			base.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + 20f, contentText.preferredHeight + 30f);
			textImag.GetComponent<RectTransform>().sizeDelta = new Vector2(maxWidth + 20f, contentText.preferredHeight + 3f);
		}
	}

	private IEnumerator DealBubbleHide()
	{
		yield return new WaitForSeconds(GeneralConfig.BubbleShowTime);
		base.transform.gameObject.SetActive(false);
		StopAllCoroutines();
		IdleBubbleManager.Instance.FinishStep(followTarget.roleType);
	}

	public void Hide()
	{
		base.transform.gameObject.SetActive(false);
		StopAllCoroutines();
		followTarget.FinishStep();
		DestoryBubble();
	}

	public void DestoryBubble()
	{
		StopAllCoroutines();
		Object.Destroy(base.transform.gameObject);
	}

	private void Update()
	{
		SetPosition();
		SetArrowPosition();
	}

	public void SetPosition()
	{
		if (!(followTarget != null))
		{
			return;
		}
		Vector3 position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y + roleHeight, followTarget.transform.position.z);
		Vector3 startPosition = CameraControl.Instance.camera3D.cam.WorldToScreenPoint(position);
		float x = base.transform.GetComponent<RectTransform>().sizeDelta.x;
		float y = base.transform.GetComponent<RectTransform>().sizeDelta.y;
		Vector2 sizeDelta = base.transform.parent.GetComponent<RectTransform>().sizeDelta;
		Vector2 vector = CalculatePosition(startPosition, sizeDelta);
		startPosition = new Vector3(vector.x, vector.y, startPosition.z);
		Vector2 vector2 = new Vector2(vector.x, vector.y);
		if (startPosition.x + x > sizeDelta.x / 2f)
		{
			startPosition.x = sizeDelta.x / 2f - x;
		}
		else if (startPosition.x < 0f - sizeDelta.x / 2f)
		{
			startPosition.x = 0f - sizeDelta.x / 2f;
		}
		if (startPosition.y + y > sizeDelta.y / 2f)
		{
			startPosition.y = sizeDelta.y / 2f - y;
		}
		else if (startPosition.y < 0f - sizeDelta.y / 2f)
		{
			startPosition.y = 0f - sizeDelta.y / 2f;
		}
		if (CastleSceneUIManager.Instance.taskBtn.IsActive())
		{
			Vector3 localPosition = CastleSceneUIManager.Instance.taskBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x < localPosition.x && startPosition.y < localPosition.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.y = localPosition.y;
				}
				else
				{
					startPosition.x = localPosition.x;
				}
			}
		}
		if (CastleSceneUIManager.Instance.glodBtn.IsActive())
		{
			Vector3 localPosition2 = CastleSceneUIManager.Instance.glodBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x < localPosition2.x && startPosition.y + y > localPosition2.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.x = localPosition2.x;
				}
				else
				{
					startPosition.y = localPosition2.y - y;
				}
			}
		}
		if (CastleSceneUIManager.Instance.scrollBtn.IsActive())
		{
			Vector3 localPosition3 = CastleSceneUIManager.Instance.scrollBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x + x > localPosition3.x && startPosition.y + y > localPosition3.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.y = localPosition3.y - y;
				}
				else
				{
					startPosition.x = localPosition3.x - x;
				}
			}
		}
		if (CastleSceneUIManager.Instance.enterGameBtn.IsActive())
		{
			Vector3 localPosition4 = CastleSceneUIManager.Instance.enterGameBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x + x > localPosition4.x && startPosition.y < localPosition4.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.y = localPosition4.y;
				}
				else
				{
					startPosition.x = localPosition4.x - x;
				}
			}
		}
		if (CastleSceneUIManager.Instance.inboxBtn.IsActive())
		{
			Vector3 localPosition5 = CastleSceneUIManager.Instance.inboxBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x + x > localPosition5.x && startPosition.y + y > localPosition5.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.y = localPosition5.y - y;
				}
				else
				{
					startPosition.x = localPosition5.x - x;
				}
			}
		}
		if (CastleSceneUIManager.Instance.BankBtn.IsActive())
		{
			Vector3 localPosition6 = CastleSceneUIManager.Instance.BankBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x + x > localPosition6.x && startPosition.y + y > localPosition6.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.y = localPosition6.y - y;
				}
				else
				{
					startPosition.x = localPosition6.x - x;
				}
			}
		}
		if (CastleSceneUIManager.Instance.SaleBtn.IsActive())
		{
			Vector3 localPosition7 = CastleSceneUIManager.Instance.SaleBtn.transform.GetComponent<RectTransform>().localPosition;
			if (startPosition.x < localPosition7.x && startPosition.y + y > localPosition7.y)
			{
				if (Mathf.Abs(Camera.main.WorldToScreenPoint(position).x) / sizeDelta.x * sizeDelta.y > Mathf.Abs(Camera.main.WorldToScreenPoint(position).y))
				{
					startPosition.x = localPosition7.x;
				}
				else
				{
					startPosition.y = localPosition7.y - y;
				}
			}
		}
		positionOffest = new Vector2(vector2.x - startPosition.x, vector2.y - startPosition.y);
		base.transform.localPosition = new Vector3(startPosition.x, startPosition.y, startPosition.z);
	}

	public void SetArrowPosition()
	{
		Vector2 vector = positionOffest;
		Vector2 sizeDelta = textImag.GetComponent<RectTransform>().sizeDelta;
		if (positionOffest.x < arrowStartPosition.x)
		{
			vector.x = arrowStartPosition.x;
		}
		else if (positionOffest.x > sizeDelta.x - arrowStartPosition.x - arrowImg.GetComponent<RectTransform>().sizeDelta.x)
		{
			vector.x = sizeDelta.x - arrowStartPosition.x - arrowImg.GetComponent<RectTransform>().sizeDelta.x;
		}
		if (positionOffest.y < sizeDelta.y)
		{
			vector.y = 0f;
		}
		else if (positionOffest.y >= sizeDelta.y)
		{
			vector.y = sizeDelta.y + arrowImg.GetComponent<RectTransform>().sizeDelta.y * 2f;
		}
		arrowImg.transform.localPosition = new Vector3(vector.x, vector.y, arrowStartPosition.z);
		if (vector.y == 0f)
		{
			if (vector.x > sizeDelta.x / 2f)
			{
				arrowImg.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			}
			else
			{
				arrowImg.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
		else if (vector.x > textImag.GetComponent<RectTransform>().sizeDelta.x / 2f)
		{
			arrowImg.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
		}
		else
		{
			arrowImg.transform.localRotation = Quaternion.Euler(0f, 180f, 180f);
		}
	}

	public Vector2 CalculatePosition(Vector3 startPosition, Vector2 ScreenSize)
	{
		float x = startPosition.x;
		float y = startPosition.y;
		float num = 0f;
		float x2 = x / (float)Screen.width * ScreenSize.x - ScreenSize.x / 2f;
		num = y / (float)Screen.height * ScreenSize.y - ScreenSize.y / 2f;
		return new Vector2(x2, num);
	}
}
