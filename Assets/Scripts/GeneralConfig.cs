using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public static class GeneralConfig
{
	public static string gameName = "CastleStory";

	public static string PackageName = "com.playinfinity.game.castlestory";

	public static bool InternalBuild = false;

	public static string ContactUsEmail = "support@playinfinity.cn";

	public static string DesKey = "PlayInfinity.Game.Alice@2019";

	public static string AndroidStoreLink = "https://play.google.com/store/apps/details?id=com.playinfinity.game.castlestory";

	public static string IOSStoreLink = "";

	public static int InterstitalStartLevel = 5;

	public static int RewardVideoStartLevel = 10;

	public static int MaxStage = 8;

	public static int VideoLimits = 5;

	public static int RoleNumber = 5;

	public static int VideoRewardMin = 15;

	public static int VideoRewardMax = 25;

	public static int SendCoinsNum = 10;

	public static int LevelClearReward = 25;

	public static int InitialCoinNum = 1000;

	public static int FbConnectReward = 1000;

	public static int HintPrice = 25;

	public static int LifeTotal = 5;

	public static int LifeRecoverTime = 1800;

	public static int BuyFullLifeCost = 900;

	public static int SendNoLifeLevelDataCoinNumber = 900;

	public static float ItemPressShowChangeTime = 0.7f;

	public static float ItemPressShowArrowTime = 0.3f;

	public static float BubbleShowTime = 4f;

	public static float RoomUnlockFadeInTime = 2f;

	public static float MaxCameraSize = 5.4f;

	public static float MinCameraSize = 2f;

	public static float StartCameraSize = 3.2f;

	public static float CameraSlowDownSpeed = 0.06f;

	public static float DragSpeed = 0.5f;

	public static float ShowMagicLevel = 3f;

	public static int ChangeItemTutorialStartPlot = 5;

	public static int[] ItemUnlockLevel = new int[6] { 14, 25, 34, 9, 19, 28 };

	public static int[] ItemUnlockSendNumber = new int[6] { 3, 3, 3, 3, 3, 3 };

	public static int[] ItemBuyPrice = new int[6] { 1800, 1800, 1800, 1800, 3600, 2700 };

	public static int[] ShowRateLevel = new int[7] { 11, 41, 71, 101, 131, 161, 191 };

	public static int[] WatchVideoRewardCoinNumber = new int[3] { 40, 50, 60 };

	public static int PerCoinImageContainCoinNumber = 10;

	public static int WatchVideoTimeLimit = 5;

	public static float BubbleMaxLength = 320f;

	public static float FixedMaxMoveDistanceX = 125f;

	public static float FixedMinMoveDistanceX = -125f;

	public static float FixedMaxMoveDistanceY = 125f;

	public static float FixedMinMoveDistanceY = -125f;

	public static float MaxMoveDistanceX = 5f;

	public static float MinMoveDistanceX = -25f;

	public static float MaxMoveDistanceY = 25f;

	public static float MinMoveDistanceY = -10f;

	public static int AddStep = 5;

	public static int IncreaseCoinToContinue = 2800;

	public static int RemindPlayNotificationID = 10000;

	public static int RemindDailyBonusNotificationID = 20000;

	public static int RemindHintNotificationID = 30000;

	public static int RemindRewardNotificationID = 40000;

	public static int RemindOneLifeRecoverNotificationID = 90000;

	public static int RemindLifeRecoverNotificationID = 90001;

	public static int[] SendDayPassLevelDay = new int[9] { 1, 2, 3, 4, 5, 6, 7, 14, 30 };

	public static int LifeRecoverTM = 1800;

	public static Dictionary<int, Sprite> ElementPictures;

	public static Dictionary<int, Sprite> SkillPictures;

	public static Dictionary<int, Sprite> CreaterPictures;

	public static Dictionary<UIType, Sprite> UIPictures;

	public static Dictionary<int, Texture> CreaterTexturePictures;

	public static Dictionary<int, GameObject> ItemCollect;

	public static void Load()
	{
		ItemCollect = new Dictionary<int, GameObject>();
		ItemCollect.Add(2, Resources.Load("Prefabs/GameScene/FriendItem", typeof(GameObject)) as GameObject);
		ItemCollect.Add(3, Resources.Load("Prefabs/GameScene/InBoxItem", typeof(GameObject)) as GameObject);
		ItemCollect.Add(4, Resources.Load("Prefabs/GameScene/FriendListItem", typeof(GameObject)) as GameObject);
		ItemCollect.Add(5, Resources.Load("Prefabs/GameScene/CreaterTip", typeof(GameObject)) as GameObject);
		ItemCollect.Add(6, Resources.Load("Prefabs/GameScene/JewelTip", typeof(GameObject)) as GameObject);
		ElementPictures = new Dictionary<int, Sprite>();
		ElementPictures.Add(-3, Resources.Load("Textures/Editor/col", typeof(Sprite)) as Sprite);
		ElementPictures.Add(-2, Resources.Load("Textures/Editor/row", typeof(Sprite)) as Sprite);
		ElementPictures.Add(-1, Resources.Load<GameObject>("Textures/GameElements/none").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1, Resources.Load<GameObject>("Textures/GameElements/green").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(2, Resources.Load<GameObject>("Textures/GameElements/blue").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(3, Resources.Load<GameObject>("Textures/GameElements/cyan").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(4, Resources.Load<GameObject>("Textures/GameElements/orange").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(5, Resources.Load<GameObject>("Textures/GameElements/red").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(6, Resources.Load<GameObject>("Textures/GameElements/yellow").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(7, Resources.Load<GameObject>("Textures/GameElements/purple").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(10, Resources.Load<GameObject>("Textures/GameElements/fly_bomb").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(11, Resources.Load<GameObject>("Textures/GameElements/hengshuxiaochu2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(12, Resources.Load<GameObject>("Textures/GameElements/hengshuxiaochu").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(13, Resources.Load<GameObject>("Textures/GameElements/area_bomb").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(14, Resources.Load<GameObject>("Textures/GameElements/color_bomb").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(23, Resources.Load<GameObject>("Textures/GameElements/butterfly").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(22, Resources.Load<GameObject>("Textures/GameElements/jewel").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(21, Resources.Load<GameObject>("Textures/GameElements/shell").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(31, Resources.Load<GameObject>("Textures/GameElements/box_0").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(32, Resources.Load<GameObject>("Textures/GameElements/box_1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(33, Resources.Load<GameObject>("Textures/GameElements/box_2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(34, Resources.Load<GameObject>("Textures/GameElements/box_3").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(35, Resources.Load<GameObject>("Textures/GameElements/box_4").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(71, Resources.Load<GameObject>("Textures/GameElements/kouzi1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(72, Resources.Load<GameObject>("Textures/GameElements/kouzi2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(73, Resources.Load<GameObject>("Textures/GameElements/kouzi3").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(74, Resources.Load<GameObject>("Textures/GameElements/kouzi4").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(51, Resources.Load<GameObject>("Textures/GameElements/baoxiang").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(52, Resources.Load<GameObject>("Textures/GameElements/cbaoxiang").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(61, Resources.Load<GameObject>("Textures/GameElements/baoxiangnull").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(62, Resources.Load<GameObject>("Textures/GameElements/cbaoxiangnull").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(41, Resources.Load<GameObject>("Textures/GameElements/cloud_0").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(42, Resources.Load<GameObject>("Textures/GameElements/cloud_1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(43, Resources.Load<GameObject>("Textures/GameElements/cloud_2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(99, Resources.Load<GameObject>("Textures/GameElements/no_init").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(100, Resources.Load<GameObject>("Textures/GameElements/ice_0").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(200, Resources.Load<GameObject>("Textures/GameElements/ice_1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(300, Resources.Load<GameObject>("Textures/GameElements/ice_2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(600, Resources.Load<GameObject>("Textures/GameElements/grass").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(302, Resources.Load<GameObject>("Textures/GameElements/trans2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(301, Resources.Load<GameObject>("Textures/GameElements/trans1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(24, Resources.Load<GameObject>("Textures/GameElements/cat").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(304, Resources.Load<GameObject>("Textures/GameElements/trans2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(303, Resources.Load<GameObject>("Textures/GameElements/trans1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(81, Resources.Load<GameObject>("Textures/GameElements/pearl").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(10000, Resources.Load<GameObject>("Textures/GameElements/green_bramble_0").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(11000, Resources.Load<GameObject>("Textures/GameElements/green_bramble_1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(20000, Resources.Load<GameObject>("Textures/GameElements/honey_0").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(21000, Resources.Load<GameObject>("Textures/GameElements/honey_1").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(22000, Resources.Load<GameObject>("Textures/GameElements/honey_2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(23000, Resources.Load<GameObject>("Textures/GameElements/honey_3").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(24000, Resources.Load<GameObject>("Textures/GameElements/honey_4").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1000000, Resources.Load<GameObject>("Textures/GameElements/collect_3_6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1100000, Resources.Load<GameObject>("Textures/GameElements/collect_3_6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1200000, Resources.Load<GameObject>("Textures/GameElements/collect_3_6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1300000, Resources.Load<GameObject>("Textures/GameElements/collect_3_6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1400000, Resources.Load<GameObject>("Textures/GameElements/collect_3_6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(1500000, Resources.Load<GameObject>("Textures/GameElements/collect_3_6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(40000006, Resources.Load<GameObject>("Textures/GameElements/shnegchengqi4").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(40000007, Resources.Load<GameObject>("Textures/GameElements/shnegchengqi5").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(40000005, Resources.Load<GameObject>("Textures/GameElements/shnegchengqi2").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(40000009, Resources.Load<GameObject>("Textures/GameElements/shnegchengqi6").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(40000008, Resources.Load<GameObject>("Textures/GameElements/shnegchengqi3").GetComponent<SpriteRenderer>().sprite);
		ElementPictures.Add(40000004, Resources.Load<GameObject>("Textures/GameElements/shnegchengqi1").GetComponent<SpriteRenderer>().sprite);
		CreaterPictures = new Dictionary<int, Sprite>();
		CreaterPictures.Add(1, Resources.Load("Editor/build_jewel", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(2, Resources.Load("Editor/build_fly", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(3, Resources.Load("Editor/build_rc", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(4, Resources.Load("Editor/build_area", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(5, Resources.Load("Editor/build_color", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(6, Resources.Load("Editor/build_shell", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(7, Resources.Load("Editor/build_2fs", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(8, Resources.Load("Editor/build_2frc", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(9, Resources.Load("Editor/build_2fa", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(10, Resources.Load("Editor/build_2cs", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(11, Resources.Load("Editor/build_3frca", typeof(Sprite)) as Sprite);
		CreaterPictures.Add(12, Resources.Load("Editor/build_3rcas", typeof(Sprite)) as Sprite);
		CreaterTexturePictures = new Dictionary<int, Texture>();
		CreaterTexturePictures.Add(51, Resources.Load("Textures/GameElements/awesom", typeof(Texture)) as Texture);
		CreaterTexturePictures.Add(52, Resources.Load("Textures/GameElements/Amazing", typeof(Texture)) as Texture);
		CreaterTexturePictures.Add(53, Resources.Load("Textures/GameElements/tennif", typeof(Texture)) as Texture);
		CreaterTexturePictures.Add(54, Resources.Load("Textures/GameElements/magnif", typeof(Texture)) as Texture);
		UIPictures = new Dictionary<UIType, Sprite>();
		UIPictures.Add(UIType.blueBtn, Resources.Load<GameObject>("Textures/HardChangeUI/button_tongyong2").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.DefultHeadPic, Resources.Load<GameObject>("Textures/HardChangeUI/touxiang").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_buttonRuZhan, Resources.Load<GameObject>("Textures/HardChangeUI/button_ruzhan").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_buttonRuZhan, Resources.Load<GameObject>("Textures/HardChangeUI/button_ruzhan_kn").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_buttonRuZhan, Resources.Load<GameObject>("Textures/HardChangeUI/button_ruzhan_cjkn").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_green, Resources.Load<GameObject>("Textures/HardChangeUI/button_tongyong").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_green, Resources.Load<GameObject>("Textures/HardChangeUI/button_tongyong").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_green, Resources.Load<GameObject>("Textures/HardChangeUI/button_tongyong").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_red, Resources.Load<GameObject>("Textures/HardChangeUI/red").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_red, Resources.Load<GameObject>("Textures/HardChangeUI/red").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_red, Resources.Load<GameObject>("Textures/HardChangeUI/red").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_title, Resources.Load<GameObject>("Textures/HardChangeUI/taitou").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_title, Resources.Load<GameObject>("Textures/HardChangeUI/taitou_hardlevel").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_title, Resources.Load<GameObject>("Textures/HardChangeUI/taitou_superhardlevel").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_tishiban, Resources.Load<GameObject>("Textures/HardChangeUI/tishidi").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_tishiban, Resources.Load<GameObject>("Textures/HardChangeUI/tishidi").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_tishiban, Resources.Load<GameObject>("Textures/HardChangeUI/tishidi").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_DlgBG, Resources.Load<GameObject>("Textures/HardChangeUI/tongyongmianban").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_DlgBG, Resources.Load<GameObject>("Textures/HardChangeUI/mianban_hardlevel").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_DlgBG, Resources.Load<GameObject>("Textures/HardChangeUI/mianban_superhardlevel").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_DlgBG2, Resources.Load<GameObject>("Textures/HardChangeUI/tongyongxiaoban2").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_DlgBG2, Resources.Load<GameObject>("Textures/HardChangeUI/xiaoban_hardlevel_1").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_DlgBG2, Resources.Load<GameObject>("Textures/HardChangeUI/xiaoban_superhardlevel_1").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Easy_DlgBG3, Resources.Load<GameObject>("Textures/HardChangeUI/tongyongxiaoban3").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Normal_DlgBG3, Resources.Load<GameObject>("Textures/HardChangeUI/xiaoban_hardlevel_2").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Hard_DlgBG3, Resources.Load<GameObject>("Textures/HardChangeUI/xiaoban_superhardlevel_2").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.Vase, Resources.Load<GameObject>("Textures/GameElements/icon_collect_huaping").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.ComboLevel1, Resources.Load<GameObject>("Textures/Combo/1").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.ComboLevel2, Resources.Load<GameObject>("Textures/Combo/2").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.ComboLevel3, Resources.Load<GameObject>("Textures/Combo/3").GetComponent<SpriteRenderer>().sprite);
		UIPictures.Add(UIType.ComboLevel4, Resources.Load<GameObject>("Textures/Combo/4").GetComponent<SpriteRenderer>().sprite);
		PoolManager.Ins.Init();
		PoolManager.Ins.AddObjPool(1, Resources.Load("Effect/Eff/DX/DX_Green", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(4, Resources.Load("Effect/Eff/DX/DX_Orange", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(2, Resources.Load("Effect/Eff/DX/DX_Blue", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(3, Resources.Load("Effect/Eff/DX/Dx_Cyan", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(6, Resources.Load("Effect/Eff/DX/DX_Yellow", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(7, Resources.Load("Effect/Eff/DX/DX_Purple", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(5, Resources.Load("Effect/Eff/DX/DX_Red", typeof(GameObject)) as GameObject, 5);
		PoolManager.Ins.AddObjPool(23, Resources.Load("Effect/Eff/DX/DX_Butterfly", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(20000, Resources.Load("Effect/Eff/DX/DX_Honey", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(21, Resources.Load("Effect/Eff/DX/DX_jianguo", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(100, Resources.Load("Effect/Eff/DX/DX_Ice", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(200, Resources.Load("Effect/Eff/DX/DX_Ice2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(300, Resources.Load("Effect/Eff/DX/DX_Ice3", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(41, Resources.Load("Effect/Eff/DX/DX_CloudX", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(40000001, Resources.Load("Effect/Eff/DX/DX_DCloudM", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(31, Resources.Load("Effect/Eff/DX/DX_Board1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(32, Resources.Load("Effect/Eff/DX/DX_Board2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(33, Resources.Load("Effect/Eff/DX/DX_Board3", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(34, Resources.Load("Effect/Eff/DX/DX_Board4", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(35, Resources.Load("Effect/Eff/DX/DX_Board5", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(71, Resources.Load("Effect/Eff/DX/DX_niukou", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(72, Resources.Load("Effect/Eff/DX/DX_niukou1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(73, Resources.Load("Effect/Eff/DX/DX_niukou2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(74, Resources.Load("Effect/Eff/DX/DX_niukou3", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(40000010, Resources.Load("Effect/Eff/DX/DX_niukouf", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(51, Resources.Load("Effect/Eff/DX/DX_shoushihe", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(52, Resources.Load("Effect/Eff/DX/DX_Board1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(61, Resources.Load("Effect/Eff/DX/DX_shoushihe", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(62, Resources.Load("Effect/Eff/DX/DX_Board1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(600, Resources.Load("Effect/Eff/DX/DX_Grass", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(10000, Resources.Load("Effect/Eff/DX/DX_Thorns1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(11000, Resources.Load("Effect/Eff/DX/DX_Thorns2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(302, Resources.Load("Effect/Eff/DX/Trans2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(301, Resources.Load("Effect/Eff/DX/Trans1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(304, Resources.Load("Effect/Eff/DX/Trans2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(303, Resources.Load("Effect/Eff/DX/Trans1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(42, Resources.Load("Effect/Eff/DX/DX_DCloudX2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(40000002, Resources.Load("Effect/Eff/DX/DX_DCloudM2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(40000003, Resources.Load("Effect/Eff/DX/DX_DCloudS", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000000, Resources.Load("Effect/Eff/DX/DX_Tap", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000001, Resources.Load("Effect/Eff/DX/DX_MoveE", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000002, Resources.Load("Effect/Eff/DX/DX_StandbyBee_F", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000003, Resources.Load("Effect/Eff/DX/StandbyBomb", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000005, Resources.Load("Effect/Eff/DX/StandbyHBomb", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000006, Resources.Load("Effect/Eff/DX/StandbyVBomb", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000034, Resources.Load("Effect/Eff/DX/StandbyCBomb", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000035, Resources.Load("Effect/Eff/DX/StandbyShell", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000007, Resources.Load("Effect/Eff/DX/StandbyTreasure", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000080, Resources.Load("Effect/Eff/DX/StandbyCat", typeof(GameObject)) as GameObject, 1);
		PoolManager.Ins.AddObjPool(50000081, Resources.Load("Effect/Eff/DX/StandbyFish", typeof(GameObject)) as GameObject, 1);
		PoolManager.Ins.AddObjPool(50000027, Resources.Load("Effect/Eff/DX/StandbyRing", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000028, Resources.Load("Effect/Eff/DX/DX_StandbyBee", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000004, Resources.Load("Effect/Eff/DX/DX_Mix", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000009, Resources.Load("Effect/Eff/UI/eff_ui_jinbi", typeof(GameObject)) as GameObject, 10);
		PoolManager.Ins.AddObjPool(50000010, Resources.Load("Effect/Eff/UI/eff_ui_jinbi2", typeof(GameObject)) as GameObject, 30);
		PoolManager.Ins.AddObjPool(50000011, Resources.Load("Effect/Eff/UI/eff_ui_juanzoutuowei", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000012, Resources.Load("Effect/Eff/DX/DX_TriggerBomb5", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000015, Resources.Load("Effect/Eff/DX/DX_TriggerRocket1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000014, Resources.Load("Effect/Eff/DX/DX_TriggerRocket2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000016, Resources.Load("Effect/Eff/DX/DX_TriggerBomb7", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000017, Resources.Load("Effect/Eff/DX/DX_DoubleCrown", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000018, Resources.Load("Effect/Eff/DX/DX_ComboBees", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000019, Resources.Load("Effect/Eff/DX/DX_zhadan_hengshuxiao", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000020, Resources.Load("Effect/Eff/DX/DX_point_shang", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000021, Resources.Load("Effect/Eff/DX/DX_point_xia", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000023, Resources.Load("Effect/Eff/DX/DX_point_you", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000022, Resources.Load("Effect/Eff/DX/DX_point_zuo", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000032, Resources.Load("Effect/Eff/DX/DX_BeeFly", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000033, Resources.Load("Effect/Eff/DX/DX_BeeHit", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000041, Resources.Load("Effect/Eff/DX/DX_mifeng_TNT_2fei", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000039, Resources.Load("Effect/Eff/DX/DX_mifeng_TNT_3jizhong", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000040, Resources.Load("Effect/Eff/DX/DX_mifeng_TNT_1fashe", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000038, Resources.Load("Effect/Eff/DX/DX_mifeng_hsx_2fei", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000036, Resources.Load("Effect/Eff/DX/DX_mifeng_hsx_3jizhong", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000037, Resources.Load("Effect/Eff/DX/DX_mifeng_hsx_1fashe", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000026, Resources.Load("Effect/Eff/DX/DX_TriggerRainbow_bao", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000024, Resources.Load("Effect/Eff/DX/DX_TriggerRainbow2f3", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000025, Resources.Load("Effect/Eff/DX/DX_TriggerRainbow3", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000013, Resources.Load("Effect/Eff/DX/DX_TriggerRainbow1", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000029, Resources.Load("Effect/Eff/DX/DX_MToB", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000030, Resources.Load("Effect/Eff/DX/DX_MToB2", typeof(GameObject)) as GameObject);
		PoolManager.Ins.AddObjPool(50000047, Resources.Load("Effect/Eff/UI/eff_ui_jinengshouji_bai", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000048, Resources.Load("Prefabs/GameScene/U_Line", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000049, Resources.Load("Prefabs/GameScene/U_Line_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000050, Resources.Load("Prefabs/GameScene/L_Line", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000051, Resources.Load("Prefabs/GameScene/L_Line_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000052, Resources.Load("Prefabs/GameScene/R_Line", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000053, Resources.Load("Prefabs/GameScene/R_Line_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000054, Resources.Load("Prefabs/GameScene/D_Line", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000055, Resources.Load("Prefabs/GameScene/D_Line_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000056, Resources.Load("Prefabs/GameScene/LU", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000057, Resources.Load("Prefabs/GameScene/LU_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000058, Resources.Load("Prefabs/GameScene/RU", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000059, Resources.Load("Prefabs/GameScene/RU_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000060, Resources.Load("Prefabs/GameScene/LD", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000061, Resources.Load("Prefabs/GameScene/LD_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000062, Resources.Load("Prefabs/GameScene/RD", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000063, Resources.Load("Prefabs/GameScene/RD_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000064, Resources.Load("Prefabs/GameScene/U_SLine1", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000065, Resources.Load("Prefabs/GameScene/U_SLine2", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000066, Resources.Load("Prefabs/GameScene/L_SLine1", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000067, Resources.Load("Prefabs/GameScene/L_SLine2", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000068, Resources.Load("Prefabs/GameScene/R_SLine1", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000069, Resources.Load("Prefabs/GameScene/R_SLine2", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000070, Resources.Load("Prefabs/GameScene/D_SLine1", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000071, Resources.Load("Prefabs/GameScene/D_SLine2", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000072, Resources.Load("Prefabs/GameScene/U_SLine1_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000073, Resources.Load("Prefabs/GameScene/U_SLine2_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000074, Resources.Load("Prefabs/GameScene/L_SLine1_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000075, Resources.Load("Prefabs/GameScene/L_SLine2_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000076, Resources.Load("Prefabs/GameScene/R_SLine1_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000077, Resources.Load("Prefabs/GameScene/R_SLine2_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000078, Resources.Load("Prefabs/GameScene/D_SLine1_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000079, Resources.Load("Prefabs/GameScene/D_SLine2_N", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000031, Resources.Load("Prefabs/GameScene/reshuffle", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000042, Resources.Load("Effect/Eff/DX/DX_ruzhanqian", typeof(GameObject)) as GameObject, 3);
		PoolManager.Ins.AddObjPool(50000043, Resources.Load("Effect/Eff/UI/eff_ui_s_cuizi", typeof(GameObject)) as GameObject, 1);
		PoolManager.Ins.AddObjPool(50000044, Resources.Load("Effect/Eff/UI/eff_ui_saozi", typeof(GameObject)) as GameObject, 1);
		PoolManager.Ins.AddObjPool(50000045, Resources.Load("Effect/Eff/DX/DX_Portal", typeof(GameObject)) as GameObject, 4);
		PoolManager.Ins.AddObjPool(50000046, Resources.Load("Effect/Eff/UI/Glove", typeof(GameObject)) as GameObject, 1);
		PoolManager.Ins.AddObjPool(50000008, Resources.Load("Effect/Eff/DX/StandByPearl", typeof(GameObject)) as GameObject, 4);
		SkillPictures = new Dictionary<int, Sprite>();
		SkillPictures.Add(1, Resources.Load<GameObject>("Textures/SkillElements/1").GetComponent<SpriteRenderer>().sprite);
		SkillPictures.Add(2, Resources.Load<GameObject>("Textures/SkillElements/2").GetComponent<SpriteRenderer>().sprite);
		SkillPictures.Add(3, Resources.Load<GameObject>("Textures/SkillElements/3").GetComponent<SpriteRenderer>().sprite);
		SkillPictures.Add(4, Resources.Load<GameObject>("Textures/SkillElements/4").GetComponent<SpriteRenderer>().sprite);
		SkillPictures.Add(5, Resources.Load<GameObject>("Textures/SkillElements/5").GetComponent<SpriteRenderer>().sprite);
		SkillPictures.Add(6, Resources.Load<GameObject>("Textures/SkillElements/6").GetComponent<SpriteRenderer>().sprite);
		SkillPictures.Add(7, Resources.Load<GameObject>("Textures/SkillElements/7").GetComponent<SpriteRenderer>().sprite);
	}
}
