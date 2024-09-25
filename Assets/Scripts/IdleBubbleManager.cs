using UnityEngine;

public class IdleBubbleManager : MonoBehaviour
{
	private static IdleBubbleManager instance;

	private IdleBubbleConfigData[] idleBubbleConfigDataArray;

	private BubbleManager[] showBubbleArray;

	private int[] currStepArray;

	public GameObject bubble;

	public static IdleBubbleManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		idleBubbleConfigDataArray = new IdleBubbleConfigData[GeneralConfig.RoleNumber];
		currStepArray = new int[GeneralConfig.RoleNumber];
		showBubbleArray = new BubbleManager[GeneralConfig.RoleNumber];
	}

	public void ShowBubble(IdleBubbleConfigData currBubbleData, int currStep)
	{
		GameObject obj = Object.Instantiate(bubble, CastleSceneUIManager.Instance.transform);
		currStepArray[(int)currBubbleData.roleType] = currStep;
		idleBubbleConfigDataArray[(int)currBubbleData.roleType] = currBubbleData;
		obj.SetActive(true);
		BubbleManager component = obj.GetComponent<BubbleManager>();
		showBubbleArray[(int)currBubbleData.roleType] = component;
		Role role = RoleManager.Instance.roleDictionary[currBubbleData.roleType];
		string @string = LanguageConfig.GetString(currBubbleData.Key);
		component.Enter(role, @string);
		role.SetBubble(component);
	}

	public void FinishStep(RoleType roleType)
	{
		IdleDialogManager.Instance.FinishStep(roleType);
		showBubbleArray[(int)roleType].DestoryBubble();
	}

	public void StopIdle()
	{
		for (int i = 0; i < GeneralConfig.RoleNumber; i++)
		{
			if (showBubbleArray[i] != null)
			{
				showBubbleArray[i].DestoryBubble();
			}
		}
	}
}
