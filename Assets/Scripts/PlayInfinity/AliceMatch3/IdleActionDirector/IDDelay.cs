using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.IdleActionDirector
{
	public class IDDelay : MonoBehaviour
	{
		private static IDDelay instance;

		public InputField textIDInput;

		public static IDDelay Instance
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

		public void Load(IDAction action)
		{
			if (action != null)
			{
				textIDInput.text = action.delayConfig.delayTime.ToString();
			}
		}

		public void OnDelayTimeChange()
		{
			IDStepManager.Instance.currentAction.delayConfig.delayTime = float.Parse(textIDInput.text);
			IDStepManager.Instance.UpdateActionInfo();
		}
	}
}
