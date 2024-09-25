using UnityEngine;
using UnityEngine.UI;

public class PlotRecorder : MonoBehaviour
{
	public InputField chapterInput;

	public Text chapterInfo;

	public GameObject recordChar;

	public Camera cam;

	public Button btnFlashMode;

	public Button btnWalkMode;

	private string modeStr;

	private bool lookAtTarget;

	private Vector3 lookTarget;

	private void Start()
	{
		base.gameObject.SetActive(false);
		recordChar = GameObject.Find("all_nvpu");
	}

	public void OnChapterValueChange()
	{
		DebugUtils.Log(DebugType.Plot, "OnChapterValueChange " + chapterInput.text);
	}

	public void BtnGenerateClicked()
	{
		DebugUtils.Log(DebugType.Plot, "BtnSaveClicked ");
		DebugUtils.Log(DebugType.Plot, "Pos " + recordChar.transform.position.ToString());
		DebugUtils.Log(DebugType.Plot, "Rotation " + recordChar.transform.rotation.ToString());
		string text = modeStr + "(" + recordChar.transform.position.x.ToString("0.00") + "," + recordChar.transform.position.y.ToString("0.00") + "," + recordChar.transform.position.z.ToString("0.00") + ");(" + recordChar.transform.rotation.x.ToString("0.00") + "," + recordChar.transform.rotation.y.ToString("0.00") + "," + recordChar.transform.rotation.z.ToString("0.00") + "," + recordChar.transform.rotation.w.ToString("0.00") + ")";
		DebugUtils.Log(DebugType.Plot, "CopyStr: " + text);
		chapterInfo.text = text;
	}

	public void BtnCopyClicked()
	{
		DebugUtils.Log(DebugType.Other, "BtnLoadClicked ");
		chapterInfo.text = chapterInput.text;
	}

	public void BtnFlashModeClicked()
	{
		DebugUtils.Log(DebugType.Other, "BtnFlashModeClicked ");
		modeStr = "F";
		btnFlashMode.GetComponent<Image>().color = new Vector4(1f, 0f, 0f, 1f);
		btnWalkMode.GetComponent<Image>().color = new Vector4(1f, 1f, 1f, 1f);
	}

	public void BtnWalkModeClicked()
	{
		DebugUtils.Log(DebugType.Other, "BtnWalkModeClicked ");
		modeStr = "W";
		btnWalkMode.GetComponent<Image>().color = new Vector4(1f, 0f, 0f, 1f);
		btnFlashMode.GetComponent<Image>().color = new Vector4(1f, 1f, 1f, 1f);
	}

	public void BtnCopyCamClicked()
	{
		string text = cam.transform.position.ToString() + ";" + cam.orthographicSize.ToString("F2");
		DebugUtils.Log(DebugType.Other, "BtnCopyCamClicked " + text);
		chapterInfo.text = text;
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
		{
			lookTarget = hitInfo.point;
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			lookAtTarget = true;
		}
		if (lookAtTarget)
		{
			recordChar.transform.LookAt(lookTarget);
		}
		lookAtTarget = false;
	}
}
