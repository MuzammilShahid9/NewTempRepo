using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDBuild : MonoBehaviour
	{
		private static CDBuild instance;

		public ToggleGroup toggleGroup;

		public Toggle selectToggle;

		public Toggle buildToggle;

		public InputField roomID;

		public InputField itemID;

		public InputField stageID;

		public InputField delayTime;

		public static CDBuild Instance
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
			selectToggle.isOn = true;
			buildToggle.isOn = false;
		}

		public void Load(CDAction action)
		{
			roomID.text = action.buildConfig.roomID.ToString();
			itemID.text = action.buildConfig.itemID.ToString();
			stageID.text = action.buildConfig.stageID.ToString();
			delayTime.text = action.buildConfig.delayTime.ToString();
		}

		public void OnRoomIdChange()
		{
			CDActionManager.Instance.currentAction.buildConfig.roomID = int.Parse(roomID.text);
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnItemIdChange()
		{
			CDActionManager.Instance.currentAction.buildConfig.itemID = int.Parse(itemID.text);
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnStageIdChange()
		{
			CDActionManager.Instance.currentAction.buildConfig.stageID = int.Parse(stageID.text);
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnDelayTimeChange()
		{
			CDActionManager.Instance.currentAction.buildConfig.delayTime = float.Parse(delayTime.text);
			CDActionManager.Instance.UpdateActionInfo();
		}
	}
}
