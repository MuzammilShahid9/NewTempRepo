using System.Collections.Generic;
using PlayInfinity.GameEngine.Common;
using UnityEngine;

public static class GlobalVariables
{
	public static bool ShowingTutorial = false;

	public static int ShowChapterList = 0;

	public static bool ShopRewardVideoDisplayed = false;

	public static Queue<string> DialogOpenList;

	public static bool Purchasing = false;

	public static bool isSale = false;

	public static bool isBank = false;

	public static bool ResumeFromDesktop = true;

	public static int PurchasingID = -1;

	public static bool PurchaseSuccess = false;

	public static bool SwitchOutRate = false;

	public static bool SwitchDisplayAd = false;

	public static SceneType fromScene = SceneType.LoadingScene;

	public static SceneType targetScene = SceneType.GameScene;

	public static bool LevelFirstPass = false;

	public static bool UnderChangeItemControl = false;

	public static int ChallenageNum = -1;

	public static int[] probabilityList = null;

	public static int FlyingGoldCoinCount = 0;

	public static int LevelStartTime = 0;

	public static bool UseAddStep = false;

	public static bool IsMaskCastleUI = false;

	public static bool IsMaskDialog = false;

	private static int _ComboNum = 0;

	public static int ComboNum
	{
		get
		{
			return _ComboNum;
		}
		set
		{
			_ComboNum = Mathf.Clamp(value, 0, 4);
		}
	}
}
