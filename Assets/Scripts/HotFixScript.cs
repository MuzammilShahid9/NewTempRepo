using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.U2D;
using XLua;

public class HotFixScript : MonoBehaviour
{
	private LuaEnv luaEnv;

	private string versionString;

	private AssetBundle levelAb;

	private AssetBundle imageAb;

	private AssetBundle roomAb;

	private AssetBundle confAb;

	private AssetBundle gameElementAb;

	public static Dictionary<string, GameObject> prefabDict = new Dictionary<string, GameObject>();

	public static Dictionary<string, GameObject> roomPrefabDict = new Dictionary<string, GameObject>();

	private static HotFixScript instance;

	public static HotFixScript Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		luaEnv = new LuaEnv();
		luaEnv.AddLoader(MyLoader);
		luaEnv.DoString("print('asdfasdf1')");
	}

	private IEnumerator DoLuaDelay()
	{
		yield return new WaitUntil(() => InitGame.Instance.isStartLoad);
		luaEnv.DoString("require 'level'");
	}

	private byte[] MyLoader(ref string filePath)
	{
		string path = Application.persistentDataPath + "/Lua/" + filePath + ".lua.txt";
		return Encoding.UTF8.GetBytes(File.ReadAllText(path));
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		luaEnv.Dispose();
	}

	[LuaCallCSharp(GenFlag.No)]
	public static void LoadResource(string resName, string filePath)
	{
		GameObject value = AssetBundle.LoadFromFile("AssetBundles/" + filePath).LoadAsset<GameObject>(resName);
		prefabDict.Add(resName, value);
	}

	[LuaCallCSharp(GenFlag.No)]
	public void LoadResources(string resName, string filePath)
	{
		StartCoroutine(LoadResourceCortine(resName, filePath));
	}

	public JSONNode LoadLevelData(int level)
	{
		string path = Application.persistentDataPath + "/Level/level_" + level + ".txt";
		if (!File.Exists(path))
		{
			return null;
		}
		string text = File.ReadAllText(path);
		if (text == null)
		{
			return null;
		}
		JSONNode jSONNode = JSONNode.Parse(text);
		if (jSONNode == null)
		{
			return null;
		}
		return jSONNode;
	}

	public TextAsset LoadConfData(string dataName)
	{
		string[] array = dataName.Split('/');
		string text = array[array.Length - 1];
		if (confAb == null)
		{
			confAb = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/conf/conf.ab");
		}
		return confAb.LoadAsset<TextAsset>(text);
	}

	public Sprite LoadImageData(string dataName)
	{
		string[] array = dataName.Split('/');
		string text = array[array.Length - 1];
		if (gameElementAb == null)
		{
			gameElementAb = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/gameelement/gameelement.ab");
		}
		return gameElementAb.LoadAsset<SpriteAtlas>("Elements2").GetSprite(text);
	}

	public void LoadRoomResouce()
	{
		DebugUtils.Log(DebugType.Other, "~~~~~~~~~~~~~~~~~~3");
		StartCoroutine(LoadRoomResourceCortine());
	}

	public void LoadConfResouce()
	{
		StartCoroutine(LoadConfResourceCortine());
	}

	public void LoadAtlasResouce()
	{
		StartCoroutine(LoadGameElementResourceCortine());
	}

	private IEnumerator LoadResourceCortine(string resName, string filePath)
	{
		yield return null;
		if (prefabDict.ContainsKey(resName))
		{
			yield break;
		}
		if (filePath == "seastar.ab")
		{
			if (imageAb == null)
			{
				imageAb = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/" + filePath);
			}
			GameObject value = imageAb.LoadAsset<GameObject>(resName);
			prefabDict.Add(resName, value);
		}
		else
		{
			bool flag = filePath == "room/room.ab";
		}
	}

	private IEnumerator LoadRoomResourceCortine()
	{
		yield return null;
		DebugUtils.Log(DebugType.Other, "~~~~~~~~~~~~~~~~~~4");
		if (roomAb == null)
		{
			roomAb = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/room/room.ab");
		}
		DebugUtils.Log(DebugType.Other, "~~~~~~~~~~~~~~~~~~5");
		GameObject[] array = roomAb.LoadAllAssets<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			roomPrefabDict.Add(array[i].name, array[i]);
		}
	}

	private IEnumerator LoadConfResourceCortine()
	{
		yield return null;
		if (confAb == null)
		{
			confAb = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/conf/conf.ab");
		}
	}

	private IEnumerator LoadGameElementResourceCortine()
	{
		yield return null;
		if (gameElementAb == null)
		{
			gameElementAb = AssetBundle.LoadFromFile(Application.persistentDataPath + "/AB/gameelement/gameelement.ab");
		}
	}

	[LuaCallCSharp(GenFlag.No)]
	public static GameObject GetGameObject(string goName)
	{
		return prefabDict[goName];
	}
}
