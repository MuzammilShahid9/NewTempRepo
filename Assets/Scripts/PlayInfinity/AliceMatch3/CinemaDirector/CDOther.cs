using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDOther : MonoBehaviour
	{
		private static CDOther instance;

		public Toggle stageFinalToggle;

		public Toggle blackScreenToggle;

		public Toggle catRenameToggle;

		public Toggle castleRenameToggle;

		public Toggle unlockRoomToggle;

		public InputField unlockRoomIDInput;

		public static CDOther Instance
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

		public void Load(CDAction action)
		{
			stageFinalToggle.isOn = action.otherConfig.isChapterEnd;
			blackScreenToggle.isOn = action.otherConfig.isBlackScreen;
			catRenameToggle.isOn = action.otherConfig.isCatRename;
			castleRenameToggle.isOn = action.otherConfig.isCastleRename;
			unlockRoomToggle.isOn = action.otherConfig.unlockRoomID != 0;
			if (action.otherConfig.unlockRoomID != 0)
			{
				unlockRoomIDInput.text = action.otherConfig.unlockRoomID.ToString();
			}
		}

		public void OnStageFinalValueChanged()
		{
			CDActionManager.Instance.currentAction.otherConfig.isChapterEnd = stageFinalToggle.isOn;
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnBlackScreenValueChanged()
		{
			CDActionManager.Instance.currentAction.otherConfig.isBlackScreen = blackScreenToggle.isOn;
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnCatRenameValueChanged()
		{
			CDActionManager.Instance.currentAction.otherConfig.isCatRename = catRenameToggle.isOn;
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnCastleRenameValueChanged()
		{
			CDActionManager.Instance.currentAction.otherConfig.isCastleRename = castleRenameToggle.isOn;
			CDActionManager.Instance.UpdateActionInfo();
		}

		public void OnUnlockRoomIdToggleValueChanged()
		{
			if (unlockRoomIDInput.text != "" && unlockRoomToggle.isOn)
			{
				CDActionManager.Instance.currentAction.otherConfig.unlockRoomID = int.Parse(unlockRoomIDInput.text);
				CDActionManager.Instance.UpdateActionInfo();
			}
			else
			{
				CDActionManager.Instance.currentAction.otherConfig.unlockRoomID = 0;
				CDActionManager.Instance.UpdateActionInfo();
			}
		}

		public void OnUnlockRoomIdInputValueChanged()
		{
			if (unlockRoomIDInput.text != "" && unlockRoomToggle.isOn)
			{
				CDActionManager.Instance.currentAction.otherConfig.unlockRoomID = int.Parse(unlockRoomIDInput.text);
				CDActionManager.Instance.UpdateActionInfo();
			}
			else
			{
				CDActionManager.Instance.currentAction.otherConfig.unlockRoomID = 0;
				CDActionManager.Instance.UpdateActionInfo();
			}
		}
	}
}
