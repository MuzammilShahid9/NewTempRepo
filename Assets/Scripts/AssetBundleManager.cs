using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AssetBundleManager : MonoBehaviour
{
	private SpriteAtlas imageAtlas;

	private Dictionary<string, SpriteAtlas> atlasDic = new Dictionary<string, SpriteAtlas>();

	private static AssetBundleManager instance;

	public static AssetBundleManager Instance
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

	private void LoadAssetBundles()
	{
		AssetBundle assetBundle = AssetBundle.LoadFromFile("AssetBundles/atlas.ab");
		string[] allAssetNames = assetBundle.GetAllAssetNames();
		for (int i = 0; i < allAssetNames.Length; i++)
		{
			atlasDic.Add(allAssetNames[i], assetBundle.LoadAsset<SpriteAtlas>(allAssetNames[i]));
		}
	}

	public Sprite Load(string imageName)
	{
		string[] array = imageName.Split('/');
		string text = array[array.Length - 1];
		string key = array[array.Length - 2];
		return atlasDic[key].GetSprite(text);
	}
}
