using UnityEngine;
using UnityEngine.UI;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class CDAudio : MonoBehaviour
	{
		private static CDAudio instance;

		public Dropdown selectMusic;

		public Dropdown selectEffect;

		public Toggle musicToggle;

		public Toggle effectToggle;

		public Toggle musicLoopToggle;

		public Toggle effectLoopToggle;

		public Toggle musicStopToggle;

		public Toggle effectStopToggle;

		public InputField musicTimeMinInput;

		public InputField effectTimeMinInput;

		public InputField musicTimeMaxInput;

		public InputField effectTimeMaxInput;

		public int currentMusic;

		public int currentEffect;

		public static CDAudio Instance
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
			musicToggle.isOn = action.audioConfig.isMusicSet;
			effectToggle.isOn = action.audioConfig.isEffectSet;
			LoadMusicData();
			LoadEffectData();
			LoadMusicType();
			LoadEffectType();
		}

		private void LoadMusicType()
		{
			selectMusic.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.audioConfig.Count; i++)
			{
				if (CDConfigManager.Instance.audioConfig[i].Music != null && CDConfigManager.Instance.audioConfig[i].Music != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.audioConfig[i].Music;
					selectMusic.options.Add(optionData);
				}
			}
			if (selectMusic.options.Count > 0)
			{
				selectMusic.value = 0;
				selectMusic.captionText.text = selectMusic.options[0].text;
			}
		}

		private void LoadMusicData()
		{
			CDAudioConfig audioConfig = CDActionManager.Instance.currentAction.audioConfig;
			musicLoopToggle.isOn = audioConfig.isMusicLoop;
			musicStopToggle.isOn = audioConfig.isMusicStop;
			musicTimeMinInput.text = audioConfig.musicMinTime.ToString();
			musicTimeMaxInput.text = audioConfig.musicMaxTime.ToString();
		}

		private void LoadEffectData()
		{
			CDAudioConfig audioConfig = CDActionManager.Instance.currentAction.audioConfig;
			effectLoopToggle.isOn = audioConfig.isEffectLoop;
			effectStopToggle.isOn = audioConfig.isEffectStop;
			effectTimeMinInput.text = audioConfig.effectMinTime.ToString();
			effectTimeMaxInput.text = audioConfig.effectMaxTime.ToString();
		}

		private void LoadEffectType()
		{
			selectEffect.ClearOptions();
			for (int i = 0; i < CDConfigManager.Instance.audioConfig.Count; i++)
			{
				if (CDConfigManager.Instance.audioConfig[i].Effect != null && CDConfigManager.Instance.audioConfig[i].Effect != "")
				{
					Dropdown.OptionData optionData = new Dropdown.OptionData();
					optionData.text = CDConfigManager.Instance.audioConfig[i].Effect;
					selectEffect.options.Add(optionData);
				}
			}
			if (selectEffect.options.Count > 0)
			{
				selectEffect.value = 0;
				selectEffect.captionText.text = selectEffect.options[0].text;
			}
		}

		public void OnMusicValueChanged()
		{
			CDActionManager.Instance.currentAction.audioConfig.isMusicSet = musicToggle.isOn;
		}

		public void OnEffectValueChanged()
		{
			CDActionManager.Instance.currentAction.audioConfig.isEffectSet = effectToggle.isOn;
		}

		public void OnMusicLoopValueChanged()
		{
			CDActionManager.Instance.currentAction.audioConfig.isMusicLoop = musicLoopToggle.isOn;
		}

		public void OnEffectLoopValueChanged()
		{
			CDActionManager.Instance.currentAction.audioConfig.isEffectLoop = effectLoopToggle.isOn;
		}

		public void OnMusicStopValueChanged()
		{
			CDActionManager.Instance.currentAction.audioConfig.isMusicStop = musicStopToggle.isOn;
		}

		public void OnEffectStopValueChanged()
		{
			CDActionManager.Instance.currentAction.audioConfig.isEffectStop = effectStopToggle.isOn;
		}

		public void OnMusicMinTimeValueChanged()
		{
			if (musicTimeMinInput.text == "" || musicTimeMinInput.text.Substring(0, 1) == "-")
			{
				CDActionManager.Instance.currentAction.audioConfig.musicMinTime = -1f;
			}
			else
			{
				CDActionManager.Instance.currentAction.audioConfig.musicMinTime = float.Parse(musicTimeMinInput.text);
			}
		}

		public void OnMusicMaxTimeValueChanged()
		{
			if (musicTimeMaxInput.text == "" || musicTimeMaxInput.text.Substring(0, 1) == "-")
			{
				CDActionManager.Instance.currentAction.audioConfig.musicMaxTime = -1f;
			}
			else
			{
				CDActionManager.Instance.currentAction.audioConfig.musicMaxTime = float.Parse(musicTimeMaxInput.text);
			}
		}

		public void OnEffectMinTimeValueChanged()
		{
			if (effectTimeMinInput.text == "" || effectTimeMinInput.text.Substring(0, 1) == "-")
			{
				CDActionManager.Instance.currentAction.audioConfig.effectMinTime = -1f;
			}
			else
			{
				CDActionManager.Instance.currentAction.audioConfig.effectMinTime = float.Parse(effectTimeMinInput.text);
			}
		}

		public void OnEffectMaxTimeValueChanged()
		{
			if (effectTimeMaxInput.text == "" || effectTimeMaxInput.text.Substring(0, 1) == "-")
			{
				CDActionManager.Instance.currentAction.audioConfig.effectMaxTime = -1f;
			}
			else
			{
				CDActionManager.Instance.currentAction.audioConfig.effectMaxTime = float.Parse(effectTimeMaxInput.text);
			}
		}

		public void SelectMusicDropdownValueChanged(Dropdown change)
		{
			DebugUtils.Log(DebugType.Other, "New Value : " + change.value);
			CDActionManager.Instance.currentAction.audioConfig.musicName = (MusicClip)change.value;
		}

		public void SelectEffectDropdownValueChanged(Dropdown change)
		{
			DebugUtils.Log(DebugType.Other, "New Value : " + change.value);
			CDActionManager.Instance.currentAction.audioConfig.effectName = (EffectClip)change.value;
		}
	}
}
