using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BaseUI
{
	private static SettingPanel instance;

	private Sprite soundOpen;

	private Sprite soundClose;

	private Sprite musicOpen;

	private Sprite musicClose;

	public Button musicBtn;

	public Button soundBtn;

	private bool isClick = true;

	public static SettingPanel Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		soundOpen = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_sound_1").GetComponent<SpriteRenderer>().sprite;
		soundClose = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_sound_close_1").GetComponent<SpriteRenderer>().sprite;
		musicOpen = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_music_1").GetComponent<SpriteRenderer>().sprite;
		musicClose = Resources.Load<GameObject>("Textures/ButtonIcon/shezhi_music_close_1").GetComponent<SpriteRenderer>().sprite;
	}

	private void ClickMouse(uint iMessageType, object arg)
	{
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void SettingBtnClick()
	{
		base.gameObject.SetActive(false);
	}

	public void MusicBtnClick()
	{
		if (UserDataManager.Instance.GetService().musicEnabled)
		{
			UserDataManager.Instance.SetMusicEnabled(false);
			musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicClose;
		}
		else
		{
			musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicOpen;
			UserDataManager.Instance.SetMusicEnabled(true);
		}
		AudioManager.Instance.PlayAudioEffect("button");
		UserDataManager.Instance.Save();
	}

	public void SoundBtnClick()
	{
		if (UserDataManager.Instance.GetService().soundEnabled)
		{
			UserDataManager.Instance.SetSoundEnabled(false);
			soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundClose;
		}
		else
		{
			soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundOpen;
			UserDataManager.Instance.SetSoundEnabled(true);
		}
		AudioManager.Instance.PlayAudioEffect("button");
		UserDataManager.Instance.Save();
	}

	public void ExitBtnClick()
	{
		base.gameObject.SetActive(false);
		if (GameLogic.Instance.TotleMoveCount == 0 && GameLogic.Instance.DropUseNum == 0)
		{
			GlobalVariables.ChallenageNum--;
			if (!UserDataManager.Instance.GetService().unlimitedLife)
			{
				LifeManager.Instance.AddUserLife(1);
			}
			GameConfig.CreateOneAreaBoom = false;
			GameConfig.CreateOneColorBoom = false;
			GameConfig.BeeLevel = 1;
			Singleton<MessageDispatcher>.Instance().SendMessage(6u, false);
			SceneTransManager.Instance.ChangeSceneWithEffect(delegate
			{
				DialogManagerTemp.Instance.CloseAllDialogs();
				SceneTransManager.Instance.TransToSwitch(SceneType.CastleScene);
			});
		}
		else
		{
			DialogManagerTemp.Instance.ShowDialog(DialogType.QuitLevelDlg);
		}
	}

	private new void Start()
	{
		ShowSoundInfo();
		ShowMusicInfo();
	}

	private void ShowSoundInfo()
	{
		if (UserDataManager.Instance.GetService().soundEnabled)
		{
			soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundOpen;
		}
		else
		{
			soundBtn.GetComponent<Button>().GetComponent<Image>().sprite = soundClose;
		}
	}

	private void ShowMusicInfo()
	{
		if (UserDataManager.Instance.GetService().musicEnabled)
		{
			musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicOpen;
		}
		else
		{
			musicBtn.GetComponent<Button>().GetComponent<Image>().sprite = musicClose;
		}
	}

	private void Update()
	{
		if (isClick && Input.GetMouseButtonDown(0))
		{
			Collider2D collider2D = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			if (collider2D == null || collider2D.transform.name != "SettingPanel")
			{
				ClickMouse(0u, null);
			}
		}
	}
}
