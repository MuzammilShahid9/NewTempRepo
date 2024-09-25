using PlayInfinity.GameEngine.Common;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	private GameObject currentTutorial;

	private static TutorialManager instance;

	public static TutorialManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
	}

	public void ShowTutorial()
	{
		if (base.transform.childCount > 0)
		{
			Object.Destroy(base.transform.GetChild(0).gameObject);
		}
		DebugUtils.Log(DebugType.Other, "Current tutorial progress is: " + UserDataManager.Instance.GetService().tutorialProgress);
		int level = UserDataManager.Instance.GetService().level;
		DebugUtils.Log(DebugType.Other, "Current level is:" + level);
		int tutorialProgress = UserDataManager.Instance.GetService().tutorialProgress;
		if (level == 2 && tutorialProgress == 3)
		{
			GlobalVariables.ShowingTutorial = true;
			currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial1")) as GameObject;
			currentTutorial.SetActive(true);
			currentTutorial.name = "1";
			currentTutorial.transform.SetParent(base.transform, false);
		}
		else if (level == 2 && tutorialProgress == 4)
		{
			GlobalVariables.ShowingTutorial = true;
			currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial2")) as GameObject;
			currentTutorial.SetActive(true);
			currentTutorial.name = "2";
			currentTutorial.transform.SetParent(base.transform, false);
		}
		else if (tutorialProgress == 7)
		{
			GlobalVariables.ShowingTutorial = true;
			currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial3")) as GameObject;
			currentTutorial.SetActive(true);
			currentTutorial.name = "3";
			currentTutorial.transform.SetParent(base.transform, false);
		}
	}

	public void ShowBoosterTutorial()
	{
		if (UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[0])
		{
			GlobalVariables.ShowingTutorial = true;
			currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial4")) as GameObject;
			currentTutorial.SetActive(true);
			currentTutorial.name = "4";
			currentTutorial.transform.SetParent(base.transform, false);
		}
		else if (UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[1])
		{
			GlobalVariables.ShowingTutorial = true;
			currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial5")) as GameObject;
			currentTutorial.SetActive(true);
			currentTutorial.name = "5";
			currentTutorial.transform.SetParent(base.transform, false);
		}
		else if (UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[2])
		{
			GlobalVariables.ShowingTutorial = true;
			currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial6")) as GameObject;
			currentTutorial.SetActive(true);
			currentTutorial.name = "6";
			currentTutorial.transform.SetParent(base.transform, false);
		}
	}

	public void ShowEnterGameBtnMask()
	{
		Object.Destroy(base.transform.GetChild(0).gameObject);
		GlobalVariables.ShowingTutorial = true;
		currentTutorial = Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/Tutorial7")) as GameObject;
		currentTutorial.SetActive(true);
		currentTutorial.name = "7";
		currentTutorial.transform.SetParent(base.transform, false);
	}

	public void FinishBoosterTutorial()
	{
		GlobalVariables.ShowingTutorial = false;
		if (!UserDataManager.Instance.GetService().boosterTutorialShow[0] && UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[0])
		{
			UserDataManager.Instance.GetService().boosterTutorialShow[0] = true;
		}
		else if (!UserDataManager.Instance.GetService().boosterTutorialShow[1] && UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[1])
		{
			UserDataManager.Instance.GetService().boosterTutorialShow[1] = true;
		}
		else if (!UserDataManager.Instance.GetService().boosterTutorialShow[2] && UserDataManager.Instance.GetService().level == GeneralConfig.ItemUnlockLevel[2])
		{
			UserDataManager.Instance.GetService().boosterTutorialShow[2] = true;
		}
		UserDataManager.Instance.Save();
		Object.Destroy(base.transform.GetChild(0).gameObject);
	}

	public void FinishTutorial()
	{
		GlobalVariables.ShowingTutorial = false;
		if (base.transform.GetChild(0).gameObject.name == "1")
		{
			UserDataManager.Instance.GetService().tutorialProgress = 4;
		}
		else if (base.transform.GetChild(0).gameObject.name == "2")
		{
			UserDataManager.Instance.GetService().tutorialProgress = 5;
		}
		else if (base.transform.GetChild(0).gameObject.name == "3")
		{
			UserDataManager.Instance.GetService().tutorialProgress = 8;
		}
		UserDataManager.Instance.Save();
		Object.Destroy(base.transform.GetChild(0).gameObject);
	}
}
