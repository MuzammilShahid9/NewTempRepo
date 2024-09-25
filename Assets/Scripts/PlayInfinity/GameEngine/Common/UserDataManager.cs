using System;
using System.Collections.Generic;
using System.IO;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Libs.DataStore;
using PlayInfinity.GameEngine.Libs.Log;
using PlayInfinity.Leah.Core;
using Umeng;
using UnityEngine;

namespace PlayInfinity.GameEngine.Common
{
	public class UserDataManager
	{
		private static UserDataManager instance;

		public const string DesKey = "PlayInfinity.Game.Pandora@2018";

		public const string UserDataFile = "/GameData.dat";

		private string dataFilePath = Application.persistentDataPath + "/GameData.dat";

		private string versionDataFilePath = Application.persistentDataPath + "/VersionData.dat";

		private UserData _userData;

		public VersionInfo versionData;

		public UserData userData
		{
			get
			{
				if (_userData == null)
				{
					Load();
				}
				return _userData;
			}
			set
			{
				_userData = value;
			}
		}

		public static UserDataManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UserDataManager();
				}
				return instance;
			}
		}

		public UserData GetService()
		{
			return userData;
		}

		public void UpdateUserData(UserData data)
		{
			userData = data;
			UpdateAskLifeData();
		}

		public void InitDesEncrypt(string key)
		{
			FileIOHelper.Instance.InitDesEnc("PlayInfinity.Game.Pandora@2018");
		}

		public void Load()
		{
			InitDesEncrypt(GeneralConfig.DesKey);
			LogManager.Debug(LogTags.IO, "User data file path: " + dataFilePath);
			LogManager.Debug(LogTags.IO, "User version data file path: " + versionDataFilePath);
			if (File.Exists(dataFilePath))
			{
				LogManager.Debug("DataStorage", "User Data Exists");
				userData = (UserData)FileIOHelper.Instance.ReadFile(dataFilePath);
				UpdateLoginData();
				UpdateAskLifeData();
				NewAddUserData();
				NewDayUpdateUserData();
				OldVersionDataAdaption();
				userData.sessionCount++;
			}
			else
			{
				LogManager.Debug("DataStorage", "User Data Not Exists");
				userData = new UserData();
				Init();
				InitGame.Instance.EnterGameFirstGame = true;
			}
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			userData.LastLoginTime = Convert.ToInt64(timeSpan.TotalSeconds);
			if (File.Exists(versionDataFilePath))
			{
				LogManager.Debug("DataStorage", "User Version Data Exists");
				versionData = (VersionInfo)FileIOHelper.Instance.ReadFile(versionDataFilePath);
			}
			else
			{
				versionData = new VersionInfo();
				versionData.Version = 0;
				LogManager.Debug("VersionDataStorage", "User Version Data Not Exists");
			}
			AdjustTutorialData();
			Save();
		}

		public void UpdateLoginData()
		{
			userData.LastLoginTime = DateTime.Now.Ticks / 10000000;
		}

		public void UpdateAskLifeData()
		{
			DebugUtils.Log(DebugType.NetWork, "UpdateAskLifeData!");
			string[] array = DateTime.Now.ToString("yyyy:MM:dd").Split(':');
			int num = int.Parse(array[2]);
			int num2 = int.Parse(array[1]);
			int num3 = int.Parse(array[0]);
			if (Instance.GetService().SaveDate != num || Instance.GetService().SaveMonth != num2 || Instance.GetService().SaveYear != num3)
			{
				DebugUtils.Log(DebugType.NetWork, "ask life overTime!");
				Instance.GetService().sentGiftFriendIds.Clear();
				Singleton<MessageDispatcher>.Instance().SendMessage(35u, null);
			}
		}

		public void Init()
		{
			userData.coin = GeneralConfig.InitialCoinNum;
			userData.level = 1;
			userData.life = 5;
			userData.preLevel = 0;
			userData.lifeConsumeTime = -1L;
			userData.exp = 0;
			userData.medal = 0;
			userData.soundEnabled = true;
			userData.musicEnabled = true;
			userData.isDownloadAB = false;
			userData.notificationEnabled = true;
			userData.sentGiftFriendIds = new List<string>();
			userData.unlockItemInfo = new List<ItemUnlockInfo>();
			userData.unlockRoomIds = 1;
			userData.sessionCount = 0;
			userData.scrollNum = 0;
			userData.stage = 1;
			userData.finishTaskString = "";
			userData.currDoTaskString = "";
			userData.unlimitedLife = false;
			userData.lifeRecoverTime = -1L;
			userData.unlimitedLifeTM = -1L;
			userData.unlimitedLifeStartTM = -1L;
			userData.language = Application.systemLanguage;
			userData.castleName = "";
			userData.catName = "";
			userData.firstEnterGame = true;
			userData.tutorialProgress = 1;
			userData.isCatUnlock = false;
			userData.bombNumber = 0;
			userData.rainBowBallNumber = 0;
			userData.doubleBeesNumber = 0;
			userData.malletNumber = 0;
			userData.magicMalletNumber = 0;
			userData.gloveNumber = 0;
			userData.facebookId = "";
			userData.watchVideoTime = 0;
			userData.SaveDate = int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[2]);
			userData.SaveMonth = int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[1]);
			userData.SaveYear = int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[0]);
			userData.DailyBonuseLevel = 0;
			userData.LastFinishTaskID = 0;
			userData.LastFinishTaskStage = 1;
			userData.DailyBonuseArray = new List<DropType>();
			userData.boosterTutorialShow = new List<bool>();
			userData.HasBeenAskForLiftFriendList = new List<string>();
			userData.UnlockRoleIDList = new List<int>();
			userData.UnlockRoomIDList = new List<int>();
			userData.boosterPurchaseData = new int[12];
			userData.UnlockRoleIDList.Add(1);
			userData.cameraSavePositioin = new PositionInfo();
			userData.roleSavePosition = new List<PositionInfo>();
			userData.roleSaveRotation = new List<RotationInfo>();
			userData.cameraSavePositioin.x = 78.1f;
			userData.cameraSavePositioin.y = 73.3f;
			userData.cameraSavePositioin.z = -102.5f;
			userData.UnlockAreaBomb = false;
			userData.UnlockDoubleBee = false;
			userData.UnlockColorBomb = false;
			userData.UnlockSpoon = false;
			userData.UnlockHammar = false;
			userData.UnlockGlove = false;
			userData.ShowMagicBook = false;
			userData.rateVersion = "";
			userData.currentVersion = GetVersionInt();
			userData.DayPassLevelSend = new List<bool>();
			userData.FirstDownloadTime = DateTime.Now.Ticks / 10000000;
			userData.LastLoginTime = DateTime.Now.Ticks / 10000000;
			userData.FirstDownloadVersion = "";
			userData.lastVersion = "";
			userData.nowVersion = Application.version;
			userData.PreGetDailyBonusTime2 = "";
			userData.SalePackID = 1;
			userData.BankNum = 0;
			for (int i = 0; i < 5; i++)
			{
				PositionInfo positionInfo = new PositionInfo();
				positionInfo.x = 0f;
				positionInfo.y = 0f;
				positionInfo.z = 0f;
				userData.roleSavePosition.Add(positionInfo);
				RotationInfo rotationInfo = new RotationInfo();
				rotationInfo.x = 0f;
				rotationInfo.y = 0f;
				rotationInfo.z = 0f;
				rotationInfo.w = 0f;
				userData.roleSaveRotation.Add(rotationInfo);
			}
			for (int j = 0; j < 3; j++)
			{
				userData.boosterTutorialShow.Add(false);
			}
			for (int k = 0; k < 9; k++)
			{
				userData.DayPassLevelSend.Add(false);
			}
			GA.SetUserLevel(1);
		}

		private int GetVersionInt()
		{
			string[] array = Application.version.Split('.');
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i];
			}
			return Convert.ToInt32(text);
		}

		private void NewDayUpdateUserData()
		{
			if (userData.SaveDate != int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[2]) || userData.SaveMonth != int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[1]) || userData.SaveYear != int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[0]))
			{
				userData.watchVideoTime = 0;
				userData.watchVideoTimeTodayInGameWin = 0;
			}
		}

		private void NewAddUserData()
		{
			if (userData.roleSaveRotation == null)
			{
				userData.roleSaveRotation = new List<RotationInfo>();
				for (int i = 0; i < 5; i++)
				{
					RotationInfo rotationInfo = new RotationInfo();
					rotationInfo.x = 0f;
					rotationInfo.y = 0f;
					rotationInfo.z = 0f;
					rotationInfo.w = 0f;
					userData.roleSaveRotation.Add(rotationInfo);
				}
			}
			if (userData.roleSavePosition == null)
			{
				userData.roleSavePosition = new List<PositionInfo>();
				for (int j = 0; j < 5; j++)
				{
					PositionInfo positionInfo = new PositionInfo();
					positionInfo.x = 0f;
					positionInfo.y = 0f;
					positionInfo.z = 0f;
					userData.roleSavePosition.Add(positionInfo);
				}
			}
			if (userData.LastFinishTaskStage == 0)
			{
				userData.LastFinishTaskStage = 1;
			}
			if (userData.boosterPurchaseData == null)
			{
				userData.boosterPurchaseData = new int[12];
			}
			if (userData.DailyBonuseArray == null)
			{
				userData.DailyBonuseArray = new List<DropType>();
			}
			if (userData.rateVersion == null)
			{
				userData.rateVersion = "";
			}
			if (userData.DayPassLevelSend == null)
			{
				userData.DayPassLevelSend = new List<bool>();
				for (int k = 0; k < 9; k++)
				{
					userData.DayPassLevelSend.Add(false);
				}
			}
			if (userData.FirstDownloadTime == 0L)
			{
				userData.FirstDownloadTime = -1L;
			}
			if (userData.FirstDownloadVersion == null)
			{
				userData.FirstDownloadVersion = "";
			}
			if (userData.lastVersion == null)
			{
				userData.lastVersion = "";
			}
			if (userData.nowVersion == null)
			{
				userData.nowVersion = Application.version;
			}
			if (userData.UnlockRoomIDList == null)
			{
				userData.UnlockRoomIDList = new List<int>();
			}
		}

		private void OldVersionDataAdaption()
		{
			if (userData.nowVersion != Application.version)
			{
				userData.lastVersion = userData.nowVersion;
				userData.nowVersion = Application.version;
			}
		}

		private void AdjustTutorialData()
		{
			if (userData.tutorialProgress < 6 && userData.tutorialProgress >= 3)
			{
				userData.tutorialProgress = 3;
			}
		}

		public void Save()
		{
			DebugUtils.Log(DebugType.NetWork, "Saving Data...");
			userData.SaveDate = int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[2]);
			userData.SaveMonth = int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[1]);
			userData.SaveYear = int.Parse(DateTime.Now.ToString("yyyy:MM:dd").Split(':')[0]);
			FileIOHelper.Instance.SaveFile(dataFilePath, userData);
			if (NetworkConfig.isSuccessfulGetDataFromService && userData.facebookId != "" && FacebookUtilities.Instance != null)
			{
				DebugUtils.Log(DebugType.NetWork, "Save UserData");
				FacebookUtilities.Instance.SaveAllData(userData);
				FacebookUtilities.Instance.SubmitCurrentLevel();
			}
		}

		public void SaveVersion()
		{
			DebugUtils.Log(DebugType.NetWork, "Saving Version Data...");
			FileIOHelper.Instance.SaveFile(versionDataFilePath, versionData);
		}

		public int GetLevel()
		{
			return userData.level;
		}

		public int GetCoin()
		{
			return userData.coin;
		}

		public void SetIsBanking(bool isBanking)
		{
			if (!isBanking && userData.isBanking)
			{
				if (Instance.GetBankNum() >= 7000 && CastleSceneUIManager.Instance != null && !DialogManagerTemp.Instance.IsDialogShowing() && PlotManager.Instance.isStepFinished && !GlobalVariables.IsMaskCastleUI && !GlobalVariables.IsMaskDialog && !SceneTransManager.Instance.isReadySwitch)
				{
					DialogManagerTemp.Instance.OpenBankDlg(0f, false);
				}
			}
			else if (isBanking && !userData.isBanking)
			{
				userData.isShowBankLastTime = false;
			}
			userData.isBanking = isBanking;
		}

		public bool GetIsBanking()
		{
			return userData.isBanking;
		}

		public void SetIsComboing(bool isComboing)
		{
			userData.isComboing = isComboing;
		}

		public bool GetIsComboing()
		{
			return userData.isComboing;
		}

		public void SetBankNum(int value)
		{
			userData.BankNum = Mathf.Clamp(value, 0, 12000);
		}

		public int GetBankNum()
		{
			return userData.BankNum;
		}

		public void IncreaseLevel()
		{
			if (TestConfig.active && !TestConfig.levelPlus)
			{
				Save();
				return;
			}
			userData.level++;
			Save();
		}

		public void DecreaseScrollNum(int number)
		{
			if (userData.scrollNum - number >= 0)
			{
				userData.scrollNum -= number;
			}
			else
			{
				UserData obj = userData;
				obj.scrollNum = obj.scrollNum;
			}
			Save();
		}

		public void IncressScrollNum(int num)
		{
			userData.scrollNum += num;
			Save();
		}

		public int GetScrollNum()
		{
			return userData.scrollNum;
		}

		public int GetStage()
		{
			return userData.stage;
		}

		public void IncressStage()
		{
			userData.stage++;
			Save();
		}

		public void SetStage(int num)
		{
			userData.stage = num;
			Save();
		}

		public string GetFinishTaskString()
		{
			return userData.finishTaskString;
		}

		public void SetFinishTaskString(string finishTask)
		{
			userData.finishTaskString = finishTask;
			Save();
		}

		public string GetCurrDoTaskString()
		{
			return userData.currDoTaskString;
		}

		public void SetCurrDoTaskString(string currDoTask)
		{
			userData.currDoTaskString = currDoTask;
			Save();
		}

		public void SetSoundEnabled(bool isEnabled)
		{
			userData.soundEnabled = isEnabled;
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.SetAudioEffectMute(!isEnabled);
			}
		}

		public void SetMusicEnabled(bool isEnabled)
		{
			userData.musicEnabled = isEnabled;
			if (AudioManager.Instance != null)
			{
				AudioManager.Instance.SetAudioMusicMute(!isEnabled);
			}
		}

		public void AddUnlockItemInfo(int roomId, int itemID, int selectIndex)
		{
			bool flag = false;
			for (int i = 0; i < userData.unlockItemInfo.Count; i++)
			{
				if (roomId == userData.unlockItemInfo[i].roomID && itemID == userData.unlockItemInfo[i].itemID)
				{
					flag = true;
					userData.unlockItemInfo[i].selectIndex = selectIndex;
				}
			}
			if (!flag)
			{
				ItemUnlockInfo itemUnlockInfo = new ItemUnlockInfo();
				itemUnlockInfo.roomID = roomId;
				itemUnlockInfo.itemID = itemID;
				itemUnlockInfo.selectIndex = selectIndex;
				userData.unlockItemInfo.Add(itemUnlockInfo);
			}
		}

		public void AddUnlockRoomInfo(int roomId)
		{
			bool flag = false;
			for (int i = 0; i < userData.UnlockRoomIDList.Count; i++)
			{
				if (roomId == userData.UnlockRoomIDList[i])
				{
					flag = true;
				}
			}
			if (!flag)
			{
				userData.UnlockRoomIDList.Add(roomId);
			}
		}

		public void CLearUnlockItemInfo()
		{
			userData.unlockItemInfo.Clear();
		}

		public int GetItemUnlockInfo(int roomId, int itemID)
		{
			if (userData == null || userData.unlockItemInfo == null)
			{
				return -1;
			}
			for (int i = 0; i < userData.unlockItemInfo.Count; i++)
			{
				if (userData.unlockItemInfo[i] != null && roomId == userData.unlockItemInfo[i].roomID && itemID == userData.unlockItemInfo[i].itemID)
				{
					return userData.unlockItemInfo[i].selectIndex;
				}
			}
			return -1;
		}

		public int GetProgress()
		{
			if (TestConfig.active)
			{
				return TestConfig.startLevel;
			}
			return userData.level;
		}

		public bool UnlockDrop(DropType type, bool unlock)
		{
			if (unlock)
			{
				bool flag = false;
				switch (type)
				{
				case DropType.AreaBomb:
					flag = userData.UnlockAreaBomb;
					userData.UnlockAreaBomb = true;
					break;
				case DropType.ColorBomb:
					flag = userData.UnlockColorBomb;
					userData.UnlockColorBomb = true;
					break;
				case DropType.DoubleBee:
					flag = userData.UnlockDoubleBee;
					userData.UnlockDoubleBee = true;
					break;
				case DropType.Spoon:
					flag = userData.UnlockSpoon;
					userData.UnlockSpoon = true;
					break;
				case DropType.Hammer:
					flag = userData.UnlockHammar;
					userData.UnlockHammar = true;
					break;
				case DropType.Glove:
					flag = userData.UnlockGlove;
					userData.UnlockGlove = true;
					break;
				}
				if (!flag)
				{
					DialogManagerTemp.Instance.OpenGetRewardDlg(1f, type);
					return true;
				}
				return false;
			}
			switch (type)
			{
			case DropType.AreaBomb:
				userData.UnlockAreaBomb = false;
				break;
			case DropType.ColorBomb:
				userData.UnlockColorBomb = false;
				break;
			case DropType.DoubleBee:
				userData.UnlockDoubleBee = false;
				break;
			case DropType.Spoon:
				userData.UnlockSpoon = false;
				break;
			case DropType.Hammer:
				userData.UnlockHammar = false;
				break;
			case DropType.Glove:
				userData.UnlockGlove = false;
				break;
			}
			return false;
		}
	}
}
