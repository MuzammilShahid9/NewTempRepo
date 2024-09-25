using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PlayInfinity.AliceMatch3.Core;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Leah.Core;
using Umeng;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class InitGame : MonoBehaviour
{
	public AsyncOperation asyncMainScene;

	public AsyncOperation asyncGameScene;

	public GameObject mainSceneUI;

	public Transform CastleUI;

	public Transform LogoImage;

	public LocalizationText loadingText;

	public LocalizationText slideText;

	public Slider loadingBar;

	public Text progressText;

	public GameObject JiaZai;

	public GameObject Loading;

	public Transform CastleAnim;

	public bool EnterGameFirstGame;

	private int _resTotal = 36;

	private int _resCount;

	private SpriteAtlas antechAtlas;

	private SpriteAtlas bedRoomAtlas;

	private SpriteAtlas wallAtlas;

	private AssetBundle imageBundle;

	private AssetBundle atlasBundle;

	private bool isCanConnectVersionService;

	private bool isForceUpdate;

	private string serverPath = "http://update.playinfinity.cn:8000/update";

	private int hotFixResTotal = 7;

	private float hotFixtimer;

	private float hotFixStoptimer;

	private int hotFixResCount;

	private bool finishDownload = true;

	private bool getHotfixNumberFinished;

	private string[] downloadLuaName = new string[3] { "util", "level", "levelDispose" };

	private List<string> downloadABName = new List<string>();

	private List<string> deleteABName = new List<string>();

	private bool isHotFix;

	private bool isHotFixConnected;

	private Coroutine hotFixDownLoadCoroutine;

	public bool isStartLoad;

	public bool isStarting;

	private static InitGame instance;

	public bool CanContinue;

	public static InitGame Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		base.gameObject.AddComponent<UpdateManager>();
		base.gameObject.AddComponent<FacebookUtilities>();
		DebugUtils.Init(base.gameObject);
		Application.backgroundLoadingPriority = ThreadPriority.High;
		AdjustScreen();
		SdkManager.InitSDK();
		UserDataManager.Instance.Load();
		Input.multiTouchEnabled = false;
		Application.targetFrameRate = 60;
		mainSceneUI.SetActive(false);
		loadingBar.value = 0.1f;
		LanguageConfig.Load();
		DebugUtils.Log(DebugType.Other, "DeviceInfo: " + SystemInfo.deviceUniqueIdentifier);
		GetComponent<FPS>().enabled = TestConfig.isEnableFrameAndMemoryInfo;
		isStarting = true;
		serverPath = "http://update.playinfinity.cn:8000/update/" + Application.version + "/Android";
		float time = 0f;
		UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate(float duration)
		{
			if (time > 10f)
			{
				if (Application.internetReachability != 0 && !Purchaser.Instance.IsInitialized())
				{
					Purchaser.Instance.InitializePurchasing();
				}
				else if (Purchaser.Instance.IsInitialized())
				{
					return true;
				}
				time = 0f;
			}
			time += duration;
			return false;
		}));
	}

	private void Start()
	{
		Singleton<MessageDispatcher>.Instance().RegisterMessageHandler(29u, StartLoadData);
		Transform parent = GameObject.Find("GameManager").transform;
		slideText.SetKeyString("Main_Updating");
		loadingText.SetKeyString("MainLoadingText" + UnityEngine.Random.Range(1, 6));
		UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Common/AudioManager")).transform.SetParent(parent);
		AudioManager.Instance.PlayAudioMusic("music_main");
		Loading.SetActive(true);
		CheckFirebase();
		//if (Application.internetReachability == NetworkReachability.NotReachable)
		//{
			isCanConnectVersionService = false;
			StartLoadingData();
			return;
		//}
		//CanContinue = false;
		//isHotFixConnected = false;
		//isCanConnectVersionService = false;
		//isHotFix = false;
		//isStartLoad = false;
		//Coroutine co = StartCoroutine(ConfirmVersion());
		//Coroutine co2 = StartCoroutine(HotFixLoad());
		//Timer.Schedule(this, 1.2f, delegate
		//{
		//	if (!isCanConnectVersionService)
		//	{
		//		StopCoroutine(co);
		//		if (!isHotFixConnected)
		//		{
		//			StopCoroutine(co2);
		//			StartLoadingData();
		//			if (hotFixDownLoadCoroutine != null)
		//			{
		//				StopCoroutine(hotFixDownLoadCoroutine);
		//			}
		//		}
		//		else
		//		{
		//			CanContinue = true;
		//		}
		//	}
		//	else if (!isHotFixConnected && CanContinue)
		//	{
		//		StopCoroutine(co2);
		//		StartLoadingData();
		//		if (hotFixDownLoadCoroutine != null)
		//		{
		//			StopCoroutine(hotFixDownLoadCoroutine);
		//		}
		//	}
		//});
	}

	private void CheckFirebase()
	{
		
	}

	private void Update()
	{
		StartAsyncLoad();
		UpdateSlider();
		if (Input.GetKeyDown(KeyCode.Escape) && !GlobalVariables.ShowingTutorial)
		{
			Singleton<MessageDispatcher>.Instance().SendMessage(21u, null);
		}
		UpdateBankTimeText();
		UpdateComboTimeText();
	}

	private void UpdateBankTimeText()
	{
		long num = -1L;
		if (UserDataManager.Instance.GetIsBanking())
		{
			if (CastleSceneUIManager.Instance != null)
			{
				CastleSceneUIManager.Instance.BankBtn.gameObject.SetActive(true);
			}
			num = UserDataManager.Instance.GetService().BankSaleTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().BankSaleStartTM);
			if (CastleSceneUIManager.Instance != null)
			{
				int num2 = (int)num / 3600;
				int num3 = (int)(num - num2 * 60 * 60) / 60;
				int num4 = Mathf.Clamp((int)num - num2 * 60 * 60 - num3 * 60, 0, 60);
				if (num2 / 24 > 0)
				{
					CastleSceneUIManager.Instance.BankTime.text = num2 / 24 + " Days";
				}
				else
				{
					CastleSceneUIManager.Instance.BankTime.text = num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0') + ":" + num4.ToString().PadLeft(2, '0');
				}
			}
			if (num > 0)
			{
				return;
			}
			UserDataManager.Instance.SetIsBanking(false);
			UserDataManager.Instance.GetService().BankSaleTM = -1L;
			if (!(CastleSceneUIManager.Instance != null))
			{
				return;
			}
			UpdateManager.Instance.AddNormalUpdateToManager(new ActionUpdate(delegate
			{
				if (CastleSceneUIManager.Instance.BankCollectDelay <= 0f)
				{
					CastleSceneUIManager.Instance.BankBtn.gameObject.SetActive(false);
					return true;
				}
				return false;
			}));
		}
		else if (CastleSceneUIManager.Instance != null)
		{
			CastleSceneUIManager.Instance.BankBtn.gameObject.SetActive(false);
		}
	}

	private void UpdateComboTimeText()
	{
		long num = -1L;
		if (UserDataManager.Instance.GetIsComboing())
		{
			if (CastleSceneUIManager.Instance != null)
			{
				CastleSceneUIManager.Instance.ComboBtn.gameObject.SetActive(true);
			}
			num = UserDataManager.Instance.GetService().ComboTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().ComboStartTM);
			if (CastleSceneUIManager.Instance != null)
			{
				int num2 = (int)num / 3600;
				int num3 = (int)(num - num2 * 60 * 60) / 60;
				int num4 = Mathf.Clamp((int)num - num2 * 60 * 60 - num3 * 60, 0, 60);
				if (num2 / 24 > 0)
				{
					CastleSceneUIManager.Instance.ComboTime.text = num2 / 24 + " Days";
				}
				else
				{
					CastleSceneUIManager.Instance.ComboTime.text = num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0') + ":" + num4.ToString().PadLeft(2, '0');
				}
			}
			if (num <= 0)
			{
				UserDataManager.Instance.SetIsComboing(false);
				UserDataManager.Instance.GetService().ComboTM = -1L;
				GlobalVariables.ComboNum = 0;
				if (CastleSceneUIManager.Instance != null)
				{
					CastleSceneUIManager.Instance.ComboBtn.gameObject.SetActive(false);
				}
			}
		}
		else if (CastleSceneUIManager.Instance != null)
		{
			CastleSceneUIManager.Instance.ComboBtn.gameObject.SetActive(false);
		}
	}

	public void UpdateSlider()
	{
		if (!isHotFix || isStartLoad || hotFixResCount >= hotFixResTotal || !getHotfixNumberFinished)
		{
			return;
		}
		hotFixStoptimer += Time.deltaTime;
		slideText.SetKeyString("");
		slideText.text = LanguageConfig.GetString("Main_Updating") + "(" + hotFixResCount + "/" + hotFixResTotal + ")";
		if (loadingBar != null)
		{
			hotFixtimer += Time.deltaTime;
			if (loadingBar.value >= (float)hotFixResCount * 1f / (float)hotFixResTotal)
			{
				hotFixtimer = 0f;
			}
			loadingBar.value = Mathf.Lerp(loadingBar.value, (float)hotFixResCount * 1f / (float)hotFixResTotal + 0.1f, hotFixtimer / 5f);
		}
	}

	public void StartAsyncLoad()
	{
		if (!isStartLoad && hotFixResCount >= hotFixResTotal)
		{
			isStartLoad = true;
			loadingBar.value = 0.1f;
			StartLoadingData();
		}
	}

	private IEnumerator HotFixLoad()
	{
		if (!UserDataManager.Instance.GetService().isDownloadAB)
		{
			string path = Application.persistentDataPath + "/Lua";
			if (!File.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			path = Application.persistentDataPath + "/Level";
			if (!File.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			path = Application.persistentDataPath + "/AB";
			if (!File.Exists(path))
			{
				Directory.CreateDirectory(path);
				hotFixDownLoadCoroutine = StartCoroutine(DownLoadAssetBundle());
			}
		}
		else if (!File.Exists(Application.persistentDataPath + "/AB/AssetBundles"))
		{
			hotFixDownLoadCoroutine = StartCoroutine(DownLoadAssetBundle());
		}
		else
		{
			hotFixDownLoadCoroutine = StartCoroutine(LoadServerManifest());
		}
		yield return null;
	}

	private IEnumerator DownLoadAssetBundle()
	{
		string uri = serverPath + "/AB/version.ab";
		UnityWebRequest versionRequest = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0u);
		yield return versionRequest.SendWebRequest();
		if (!string.IsNullOrEmpty(versionRequest.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + versionRequest.error + "    HotFixGetData|    ");
			yield break;
		}
		isHotFixConnected = true;
		yield return new WaitUntil(() => CanContinue);
		isHotFix = true;
		AssetBundle content = DownloadHandlerAssetBundle.GetContent(versionRequest);
		if (content == null)
		{
			StartLoadingData();
			yield break;
		}
		if (content.LoadAsset<TextAsset>("version").text == Application.version)
		{
			StartLoadingData();
			yield break;
		}
		string uri2 = serverPath + "/AB/AssetBundles";
		UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri2, 0u);
		yield return request.SendWebRequest();
		isHotFixConnected = true;
		yield return new WaitUntil(() => CanContinue);
		isHotFix = true;
		if (!string.IsNullOrEmpty(request.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + request.error + "    HotFixGetData|    ");
			StartLoadingData();
			yield break;
		}
		AssetBundle content2 = DownloadHandlerAssetBundle.GetContent(request);
		if (content2 == null)
		{
			StartLoadingData();
			yield break;
		}
		UserDataManager.Instance.GetService().isUnloadAB = false;
		UserDataManager.Instance.Save();
		AssetBundleManifest assetBundleManifest = content2.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] dependencies = assetBundleManifest.GetAllAssetBundles();
		content2.Unload(true);
		hotFixResTotal = dependencies.Length * 2 + 2;
		getHotfixNumberFinished = true;
		for (int i = 0; i < dependencies.Length; i++)
		{
			finishDownload = false;
			StartCoroutine(DownloadResourceCortine(serverPath, dependencies[i], Application.persistentDataPath + "/AB"));
			yield return new WaitUntil(() => finishDownload);
			finishDownload = false;
			StartCoroutine(DownloadResourceCortine(serverPath, dependencies[i].Split('.')[0] + ".ab.manifest", Application.persistentDataPath + "/AB"));
			yield return new WaitUntil(() => finishDownload);
		}
		finishDownload = false;
		StartCoroutine(DownloadResourceCortine(serverPath, "AssetBundles", Application.persistentDataPath + "/AB"));
		yield return new WaitUntil(() => finishDownload);
		finishDownload = false;
		StartCoroutine(DownloadResourceCortine(serverPath, "AssetBundles.manifest", Application.persistentDataPath + "/AB"));
		yield return null;
		WriteLuaFile();
		StartCoroutine(UnloadLevelInfo());
		DebugUtils.Log(DebugType.Other, "HotFixMark");
		if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("IsHotFix", true.ToString());
			Analytics.Event("HotFixFinish", dictionary);
		}
		if (!UserDataManager.Instance.GetService().isDownloadAB)
		{
			UserDataManager.Instance.GetService().isDownloadAB = true;
			UserDataManager.Instance.Save();
		}
	}

	private IEnumerator DownloadResourceCortine(string AssetsHost, string AssetName, string saveLocalPath)
	{
		WWW wwwAsset = new WWW(AssetsHost + "/AB/" + AssetName);
		yield return wwwAsset;
		string[] array = AssetName.Split('/');
		if (array.Length > 1 && !File.Exists(saveLocalPath + "/" + array[0]))
		{
			Directory.CreateDirectory(saveLocalPath + "/" + array[0]);
		}
		FileInfo fileInfo = new FileInfo(saveLocalPath + "/" + AssetName);
		if (fileInfo.Exists)
		{
			fileInfo.Delete();
		}
		FileStream fileStream = fileInfo.Create();
		fileStream.Write(wwwAsset.bytes, 0, wwwAsset.bytes.Length);
		fileStream.Flush();
		fileStream.Close();
		fileStream.Dispose();
		hotFixResCount++;
		finishDownload = true;
	}

	public void WriteLuaFile()
	{
		AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/lua/lua.ab");
		File.WriteAllText(contents: assetBundle.LoadAsset<TextAsset>("level.lua").text, path: Application.persistentDataPath + "/Lua/level.lua.txt");
		File.WriteAllText(contents: assetBundle.LoadAsset<TextAsset>("levelDispose.lua").text, path: Application.persistentDataPath + "/Lua/levelDispose.lua.txt");
		File.WriteAllText(contents: assetBundle.LoadAsset<TextAsset>("util.lua").text, path: Application.persistentDataPath + "/Lua/util.lua.txt");
		assetBundle.Unload(true);
	}

	private IEnumerator UnloadLevelInfo()
	{
		if (!File.Exists(Application.persistentDataPath + "/AB/levelinfo/levelinfo.ab"))
		{
			yield break;
		}
		AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/levelinfo/levelinfo.ab");
		if (assetBundle != null)
		{
			string path = Application.persistentDataPath + "/Level";
			if (!File.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			FileInfo[] files = new DirectoryInfo(Application.persistentDataPath + "/Level").GetFiles("*", SearchOption.AllDirectories);
			for (int i = 0; i < files.Length; i++)
			{
				if (!files[i].Name.EndsWith(".meta"))
				{
					File.Delete(Application.persistentDataPath + "/Level/" + files[i].Name);
				}
			}
			string[] allAssetNames = assetBundle.GetAllAssetNames();
			for (int j = 0; j < allAssetNames.Length; j++)
			{
				string text = assetBundle.LoadAsset<TextAsset>(allAssetNames[j]).text;
				string[] array = allAssetNames[j].Split('/');
				File.WriteAllText(Application.persistentDataPath + "/Level/" + array[array.Length - 1], text);
			}
			UserDataManager.Instance.GetService().isUnloadAB = true;
			UserDataManager.Instance.Save();
		}
		assetBundle.Unload(true);
		yield return null;
	}

	private IEnumerator LoadServerManifest()
	{
		string uri = serverPath + "/AB/version.ab";
		UnityWebRequest versionRequest = UnityWebRequestAssetBundle.GetAssetBundle(uri, 0u);
		yield return versionRequest.SendWebRequest();
		if (!string.IsNullOrEmpty(versionRequest.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + versionRequest.error + "    HotFixGetData|    ");
			yield break;
		}
		isHotFixConnected = true;
		yield return new WaitUntil(() => CanContinue);
		isHotFix = true;
		AssetBundle content = DownloadHandlerAssetBundle.GetContent(versionRequest);
		if (content == null)
		{
			StartLoadingData();
			yield break;
		}
		if (content.LoadAsset<TextAsset>("version").text == Application.version)
		{
			StartLoadingData();
			yield break;
		}
		yield return null;
		string uri2 = serverPath + "/AB/AssetBundles";
		UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri2, 0u);
		yield return request.SendWebRequest();
		isHotFixConnected = true;
		yield return new WaitUntil(() => CanContinue);
		isHotFix = true;
		if (!string.IsNullOrEmpty(request.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + request.error + "    HotFixGetData|    ");
			StartLoadingData();
			yield break;
		}
		AssetBundle content2 = DownloadHandlerAssetBundle.GetContent(request);
		if (content2 == null)
		{
			StartLoadingData();
			yield break;
		}
		AssetBundleManifest assetBundleManifest = content2.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] allAssetBundles = assetBundleManifest.GetAllAssetBundles();
		Dictionary<Hash128, string> dictionary = new Dictionary<Hash128, string>();
		string[] array = allAssetBundles;
		foreach (string text in array)
		{
			Hash128 assetBundleHash = assetBundleManifest.GetAssetBundleHash(text);
			dictionary.Add(assetBundleHash, text);
		}
		content2.Unload(true);
		AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/AssetBundles");
		AssetBundleManifest assetBundleManifest2 = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		string[] allAssetBundles2 = assetBundleManifest2.GetAllAssetBundles();
		Dictionary<Hash128, string> dictionary2 = new Dictionary<Hash128, string>();
		array = allAssetBundles2;
		foreach (string text2 in array)
		{
			Hash128 assetBundleHash2 = assetBundleManifest2.GetAssetBundleHash(text2);
			dictionary2.Add(assetBundleHash2, text2);
		}
		assetBundle.Unload(true);
		foreach (Hash128 key in dictionary2.Keys)
		{
			if (!dictionary.ContainsKey(key))
			{
				DebugUtils.Log(DebugType.Other, "~~~~~~~~10 " + dictionary2[key]);
				deleteABName.Add(dictionary2[key]);
			}
		}
		foreach (Hash128 key2 in dictionary.Keys)
		{
			if (!dictionary2.ContainsKey(key2))
			{
				DebugUtils.Log(DebugType.Other, "~~~~~~~~11 " + dictionary[key2]);
				downloadABName.Add(dictionary[key2]);
			}
		}
		if (deleteABName.Count > 0 || downloadABName.Count > 0)
		{
			hotFixResTotal = downloadABName.Count * 2 + deleteABName.Count * 2 + 2;
			getHotfixNumberFinished = true;
		}
		else
		{
			StartCoroutine(JudgeLackOfFile());
		}
		for (int k = 0; k < deleteABName.Count; k++)
		{
			FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/AB/" + deleteABName[k]);
			FileInfo fileInfo2 = new FileInfo(Application.persistentDataPath + "/AB/" + deleteABName[k] + ".manifest");
			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}
			hotFixResCount++;
			if (fileInfo.Exists)
			{
				fileInfo2.Delete();
				hotFixResCount++;
			}
			hotFixResCount++;
		}
		for (int i = 0; i < downloadABName.Count; i++)
		{
			finishDownload = false;
			StartCoroutine(DownloadResourceCortine(serverPath, downloadABName[i], Application.persistentDataPath + "/AB"));
			yield return new WaitUntil(() => finishDownload);
			finishDownload = false;
			StartCoroutine(DownloadResourceCortine(serverPath, downloadABName[i].Split('.')[0] + ".ab.manifest", Application.persistentDataPath + "/AB"));
			yield return new WaitUntil(() => finishDownload);
			if (downloadABName[i] == "luainfo")
			{
				WriteLuaFile();
			}
		}
		if (deleteABName.Count > 0 || downloadABName.Count > 0)
		{
			UserDataManager.Instance.GetService().isUnloadAB = false;
			UserDataManager.Instance.Save();
			finishDownload = false;
			StartCoroutine(DownloadResourceCortine(serverPath, "AssetBundles", Application.persistentDataPath + "/AB"));
			yield return new WaitUntil(() => finishDownload);
			finishDownload = false;
			StartCoroutine(DownloadResourceCortine(serverPath, "AssetBundles.manifest", Application.persistentDataPath + "/AB"));
			yield return new WaitUntil(() => finishDownload);
			DebugUtils.Log(DebugType.Other, "HotFixMark");
			if (UserDataManager.Instance.GetCoin() < 5000000 && UserDataManager.Instance.GetScrollNum() < 1000)
			{
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				dictionary3.Add("IsHotFix", true.ToString());
				Analytics.Event("HotFixFinish", dictionary3);
			}
			StartCoroutine(UnloadLevelInfo());
		}
		else if (!UserDataManager.Instance.GetService().isUnloadAB)
		{
			StartCoroutine(UnloadLevelInfo());
		}
	}

	private IEnumerator JudgeLackOfFile()
	{
		yield return null;
		AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/AssetBundles");
		string[] allAssetBundles = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest").GetAllAssetBundles();
		assetBundle.Unload(true);
		hotFixResTotal = 0;
		string[] array = allAssetBundles;
		foreach (string text in array)
		{
			if (!File.Exists(Application.persistentDataPath + "/AB/" + text))
			{
				hotFixResTotal += 2;
			}
		}
		getHotfixNumberFinished = true;
		string[] array2 = allAssetBundles;
		foreach (string dependency in array2)
		{
			if (!File.Exists(Application.persistentDataPath + "/AB/" + dependency))
			{
				finishDownload = false;
				StartCoroutine(DownloadResourceCortine(serverPath, dependency, Application.persistentDataPath + "/AB"));
				yield return new WaitUntil(() => finishDownload);
				finishDownload = false;
				StartCoroutine(DownloadResourceCortine(serverPath, dependency.Split('.')[0] + ".ab.manifest", Application.persistentDataPath + "/AB"));
				yield return new WaitUntil(() => finishDownload);
			}
		}
	}

	private void StartLoadData(uint iMessageType, object arg)
	{
		if (isHotFixConnected)
		{
			CanContinue = true;
			return;
		}
		if (hotFixDownLoadCoroutine != null)
		{
			StopCoroutine(hotFixDownLoadCoroutine);
		}
		StartLoadingData();
	}

	private IEnumerator ConfirmVersion()
	{
		string getVersionURL = NetworkConfig.GetVersionURL;
		int nativeVersion = Convert.ToInt32(Application.version.Replace(".", ""));
		DebugUtils.Log(DebugType.NetWork, "native Version is " + nativeVersion);
		DebugUtils.Log(DebugType.NetWork, "get New Version url is " + getVersionURL);
		UnityWebRequest www = UnityWebRequest.Get(getVersionURL);
		yield return www.Send();
		if (!string.IsNullOrEmpty(www.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    ConfirmVersion|    ");
			CanContinue = true;
			yield break;
		}
		string text = www.downloadHandler.text;
		if (text != null && text != "" && text != "NoData")
		{
			isCanConnectVersionService = true;
			DebugUtils.Log(DebugType.NetWork, "New Version rawData is " + text + " skip version: " + UserDataManager.Instance.versionData.Version);
			try
			{
				string[] array = text.Split('#');
				if ((Convert.ToInt32(array[0]) > nativeVersion && array[1] == "0") || nativeVersion <= Convert.ToInt32(array[3]))
				{
					isForceUpdate = true;
					DialogManagerTemp.Instance.CreateDialog(DialogType.ForceUpdateDlg);
					DialogManagerTemp.Instance.ShowDialog(DialogType.ForceUpdateDlg);
					yield break;
				}
				if (UserDataManager.Instance.versionData.Version < nativeVersion)
				{
					UserDataManager.Instance.versionData.Version = nativeVersion;
					UserDataManager.Instance.SaveVersion();
				}
				if (UserDataManager.Instance.versionData.Version != Convert.ToInt32(array[0]))
				{
					UserDataManager.Instance.versionData.Version = Convert.ToInt32(array[0]);
					UserDataManager.Instance.SaveVersion();
					if (Convert.ToInt32(array[0]) > nativeVersion && array[1] != "0")
					{
						isForceUpdate = false;
						DialogManagerTemp.Instance.CreateDialog(DialogType.UpdateDlg);
						DialogManagerTemp.Instance.ShowDialog(DialogType.UpdateDlg);
					}
					else
					{
						CanContinue = true;
					}
				}
				else
				{
					CanContinue = true;
				}
			}
			catch (Exception)
			{
				CanContinue = true;
				DebugUtils.Log(DebugType.Other, "Retrieve version data failed.");
			}
		}
		else
		{
			CanContinue = true;
		}
	}

	private void StartLoadingData()
	{
		Singleton<PlayGameData>.Instance();
		Loading.SetActive(true);
		isHotFix = false;
		Timer.Schedule(this, 0.1f, delegate
		{
			StartCoroutine(AsyncLoad());
		});
	}

	private IEnumerator AsyncLoad()
	{
		slideText.SetKeyString("LoadingDlg_Loading");
		Transform parent = GameObject.Find("GameManager").transform;
		UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Common/SceneTransition")).transform.SetParent(parent);
		loadingBar.value += 1f / (float)_resTotal;
		_resCount++;
		yield return null;
		(UnityEngine.Object.Instantiate(Resources.Load("Prefabs/UI/Tutorials/TutorialManager")) as GameObject).transform.SetParent(GameObject.Find("UI").transform);
		loadingBar.value += 1f / (float)_resTotal;
		_resCount++;
		yield return null;
		SendDayPassLevelUmengAnalytic();
		if (UserDataManager.Instance.GetService().tutorialProgress == 2)
		{
			StartCoroutine(LoadGameScene());
		}
		else
		{
			StartCoroutine(LoadCastleScene());
		}
		foreach (DialogType value in Enum.GetValues(typeof(DialogType)))
		{
			if (value != DialogType.UpdateDlg && value != DialogType.ForceUpdateDlg)
			{
				DialogManagerTemp.Instance.CreateDialog(value);
				loadingBar.value += 1f / (float)_resTotal;
				_resCount++;
				yield return null;
			}
		}
		DebugUtils.Log(DebugType.Other, "loadingBar.value " + loadingBar.value);
		DebugUtils.Log(DebugType.Other, "_resCount " + _resCount);
		if (TestConfig.active && TestConfig.noCoin)
		{
			UserDataManager.Instance.GetService().coin = 0;
		}
		if (TestConfig.active && TestConfig.noLife)
		{
			UserDataManager.Instance.GetService().life = 0;
		}
		NotificationManager.RegisterForNotification();
		NotificationManager.UnregisterAllNotifications();
		StartCoroutine(SendServerNoSaveData());
		LogoImage.SetParent(CastleUI);
		loadingBar.gameObject.SetActive(false);
		loadingText.gameObject.SetActive(false);
		progressText.gameObject.SetActive(false);
		Animation[] componentsInChildren = CastleAnim.GetComponentsInChildren<Animation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		
		Timer.Schedule(this, 2.3f, delegate
		{
			isStarting = false;
			mainSceneUI.SetActive(true);
		});
		yield return null;
	}

	public byte[] Serialize(object data)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		binaryFormatter.Serialize(memoryStream, data);
		return memoryStream.GetBuffer();
	}

	private IEnumerator SendServerNoSaveData()
	{
		string analyticsDataAddress = NetworkConfig.AnalyticsDataAddress;
		if (!File.Exists(Application.persistentDataPath + "/levelData.txt"))
		{
			yield break;
		}
		string text = File.ReadAllText(Application.persistentDataPath + "/levelData.txt");
		switch (text)
		{
		case "":
			yield break;
		case " ":
			yield break;
		}
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("data", text);
		UnityWebRequest www = UnityWebRequest.Post(analyticsDataAddress, wWWForm);
		yield return www.Send();
		if (!string.IsNullOrEmpty(www.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + www.error + "    Analytics|    ");
		}
		else
		{
			DebugUtils.Log(DebugType.Other, www.downloadHandler.text);
			if (www.downloadHandler.text == "success")
			{
				File.WriteAllText(Application.persistentDataPath + "/levelData.txt", "");
			}
		}
		if (www.GetResponseHeader("response") != null)
		{
			DebugUtils.Log(DebugType.Other, www.GetResponseHeader("response"));
		}
		string analyticsMoneySpendAddress = NetworkConfig.AnalyticsMoneySpendAddress;
		if (!File.Exists(Application.persistentDataPath + "/MoneySpend.txt"))
		{
			yield break;
		}
		string text2 = File.ReadAllText(Application.persistentDataPath + "/MoneySpend.txt");
		switch (text2)
		{
		case "":
			yield break;
		case " ":
			yield break;
		}
		WWWForm wWWForm2 = new WWWForm();
		wWWForm2.AddField("data", text2);
		UnityWebRequest www2 = UnityWebRequest.Post(analyticsMoneySpendAddress, wWWForm2);
		yield return www2.Send();
		if (!string.IsNullOrEmpty(www2.error))
		{
			DebugUtils.LogError(DebugType.NetWork, "   |      " + www2.error + "    Analytics|    ");
		}
		else
		{
			DebugUtils.Log(DebugType.Other, www2.downloadHandler.text);
			if (www2.downloadHandler.text == "success")
			{
				File.WriteAllText(Application.persistentDataPath + "/MoneySpend.txt", "");
			}
		}
		if (www2.GetResponseHeader("response") != null)
		{
			DebugUtils.Log(DebugType.Other, www2.GetResponseHeader("response"));
		}
		DebugUtils.Log(DebugType.Other, "SendServerNoSaveData success.");
	}

	private void SendDayPassLevelUmengAnalytic()
	{
		if (UserDataManager.Instance.GetService().FirstDownloadTime <= 0)
		{
			return;
		}
		for (int i = 0; i < GeneralConfig.SendDayPassLevelDay.Length && DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().FirstDownloadTime > 86400 * GeneralConfig.SendDayPassLevelDay[i]; i++)
		{
			if (!UserDataManager.Instance.GetService().DayPassLevelSend[i])
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				int num = UserDataManager.Instance.GetService().level - 1;
				if (num <= 0)
				{
					num = 1;
				}
				dictionary.Add("Day" + GeneralConfig.SendDayPassLevelDay[i] + "PassLevel", num.ToString());
				Analytics.Event("DayPassLevel", dictionary);
				dictionary.Clear();
				dictionary.Add("PassLevel", num.ToString());
				Analytics.Event("Day" + GeneralConfig.SendDayPassLevelDay[i] + "PassLevel", dictionary, num);
				UserDataManager.Instance.GetService().DayPassLevelSend[i] = true;
				UserDataManager.Instance.Save();
			}
		}
	}

	private void CreateModelFile(string path, string name, byte[] info)
	{
		FileInfo fileInfo = new FileInfo(path + "/" + name);
		if (!fileInfo.Exists)
		{
			Stream stream = fileInfo.Create();
			stream.Write(info, 0, info.Length);
			stream.Close();
			stream.Dispose();
		}
	}

	private IEnumerator LoadCastleScene()
	{
		asyncMainScene = SceneManager.LoadSceneAsync("_Scenes/CastleScene");
		asyncMainScene.allowSceneActivation = false;
		while (!asyncMainScene.isDone)
		{
			float progress = asyncMainScene.progress;
			float num = 0.9f;
			yield return null;
		}
		yield return asyncMainScene;
	}

	private IEnumerator LoadGameScene()
	{
		asyncGameScene = SceneManager.LoadSceneAsync("_Scenes/GameScene");
		asyncGameScene.allowSceneActivation = false;
		while (!asyncGameScene.isDone)
		{
			float progress = asyncGameScene.progress;
			float num = 0.9f;
			yield return null;
		}
		yield return asyncGameScene;
	}

	public void AdjustScreen()
	{
		int height = Screen.height;
		int width = Screen.width;
		float orthographicSize2 = Camera.main.orthographicSize;
		float orthographicSize = 3.8f * ((float)Screen.height * 1f / (float)Screen.width);
		Camera.main.orthographicSize = orthographicSize;
	}

	public Sprite GetAtlasImage(string spriteName)
	{
		Sprite sprite = bedRoomAtlas.GetSprite(spriteName);
		if (sprite == null)
		{
			sprite = wallAtlas.GetSprite(spriteName);
		}
		if (sprite == null)
		{
			sprite = antechAtlas.GetSprite(spriteName);
		}
		return sprite;
	}

	public Texture2D GetLocalTexture(string spriteName)
	{
		return imageBundle.LoadAsset<Sprite>(spriteName).texture;
	}

	public Sprite GetLocalSprite(string spriteName)
	{
		return imageBundle.LoadAsset<Sprite>(spriteName);
	}

	private void OnDestroy()
	{
		if (imageBundle != null)
		{
			imageBundle.Unload(true);
		}
		if (atlasBundle != null)
		{
			atlasBundle.Unload(true);
		}
	}
}
