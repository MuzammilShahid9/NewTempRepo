using System;
using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

namespace PlayInfinity.GameEngine.Common
{
	[Serializable]
	public class UserData
	{
		public int coin;

		public int level;

		public int preLevel;

		public int life;

		public long lifeConsumeTime;

		public int exp;

		public int medal;

		public int moneySpend;

		public long LastBuyTime;

		public bool soundEnabled;

		public bool firstEnterGame;

		public bool musicEnabled;

		public bool notificationEnabled;

		public bool removeAdPurchased;

		public bool isDownloadAB;

		public bool isUnloadAB;

		public int popRateLevel;

		public int sessionCount;

		public long lastQuitTime;

		public long lastLoginDay;

		public string facebookId = "";

		public List<string> sentGiftFriendIds;

		public List<ItemUnlockInfo> unlockItemInfo;

		public int unlockRoomIds;

		public int videoLeft;

		public int scrollNum;

		public int stage = 1;

		public string finishTaskString = "";

		public string currDoTaskString = "";

		public int plotFinishNum;

		public bool unlimitedLife;

		public long lifeRecoverTime;

		public long unlimitedLifeTM;

		public long unlimitedLifeStartTM;

		public long SaleStartTM;

		public long SaleTM;

		public bool isSaling;

		public long BankSaleStartTM;

		public long BankSaleTM;

		public bool isBanking;

		public bool isShowBankLastTime;

		public SystemLanguage language;

		public string castleName;

		public string catName;

		public int tutorialProgress;

		public bool isCatUnlock;

		public int bombNumber;

		public int rainBowBallNumber;

		public int doubleBeesNumber;

		public int malletNumber;

		public int magicMalletNumber;

		public int gloveNumber;

		public long LastQuitTime;

		public DateTime PreGetDailyBonusTime;

		public string PreGetDailyBonusTime2 = "";

		public int SaveDate;

		public int SaveMonth;

		public int SaveYear;

		public int DailyBonuseLevel;

		public int LastFinishTaskID;

		public int LastFinishTaskStage;

		public List<DropType> DailyBonuseArray = new List<DropType>();

		public int watchVideoTime;

		public int watchVideoTimeTodayInGameWin;

		public List<bool> boosterTutorialShow;

		public List<string> HasBeenAskForLiftFriendList;

		public List<int> UnlockRoleIDList;

		public List<int> UnlockRoomIDList;

		public PositionInfo cameraSavePositioin;

		public List<PositionInfo> roleSavePosition;

		public List<RotationInfo> roleSaveRotation;

		public int[] boosterPurchaseData = new int[12];

		public int currentVersion;

		public bool UnlockAreaBomb;

		public bool UnlockDoubleBee;

		public bool UnlockColorBomb;

		public bool UnlockSpoon;

		public bool UnlockHammar;

		public bool UnlockGlove;

		public bool NoGoldLevelSend;

		public bool NoLifeLevelSend;

		public List<bool> DayPassLevelSend;

		public long FirstDownloadTime = -1L;

		public string FirstDownloadVersion = "";

		public string lastVersion = "";

		public string nowVersion = "";

		public bool isFreshMan = true;

		public int SaleShowNum;

		public int SaleShowTime;

		public long SaleShowTime_L;

		public int BankSaleShowNum;

		public int BankSaleShowTime;

		public long BankSaleShowTime_L;

		public bool ShowMagicBook;

		public string rateVersion;

		public int SalePackID = 1;

		public int BankNum;

		public ShopConfigData productData;

		public long ComboStartTM;

		public long ComboTM;

		public bool isComboing;

		public int ComboShowNum;

		public long LastLoginTime = -1L;

		public bool isNewData()
		{
			if (level == 1 && coin == 100)
			{
				return scrollNum == 0;
			}
			return false;
		}
	}
}
