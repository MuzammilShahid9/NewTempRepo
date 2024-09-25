using System;
using System.Collections.Generic;
using UnityEngine;

public class CDConfigManager : MonoBehaviour
{
	[Serializable]
	public class AudioConfigDataList
	{
		public List<AudioConfigData> data = new List<AudioConfigData>();
	}

	[Serializable]
	public class AudioConfigData
	{
		public int ID;

		public string Music;

		public string Effect;
	}

	[Serializable]
	public class RoleTypeConfigDataList
	{
		public List<RoleTypeConfigData> data = new List<RoleTypeConfigData>();
	}

	[Serializable]
	public class RoleTypeConfigData
	{
		public int ID;

		public string RoleType;
	}

	[Serializable]
	public class RoleAnimConfigDataList
	{
		public List<RoleAnimConfigData> data = new List<RoleAnimConfigData>();
	}

	[Serializable]
	public class RoleAnimConfigData
	{
		public int ID;

		public string Alice;

		public string John;

		public string Tina;

		public string Cat;

		public string Arthur;
	}

	[Serializable]
	public class RoleImageConfigDataList
	{
		public List<RoleImageConfigData> data = new List<RoleImageConfigData>();
	}

	[Serializable]
	public class RoleImageConfigData
	{
		public string ID;

		public string Alice;

		public string John;

		public string Tina;

		public string Cat;

		public string Arthur;
	}

	public List<AudioConfigData> audioConfig;

	public List<RoleTypeConfigData> roleTypeConfig;

	public List<RoleAnimConfigData> roleAnimConfig;

	public List<RoleImageConfigData> roleImageConfig;

	public static CDConfigManager instance;

	public static CDConfigManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		AudioConfigDataList audioConfigDataList = JsonUtility.FromJson<AudioConfigDataList>((Resources.Load("Config/Editor/AudioConfig") as TextAsset).text);
		audioConfig = audioConfigDataList.data;
		RoleTypeConfigDataList roleTypeConfigDataList = JsonUtility.FromJson<RoleTypeConfigDataList>((Resources.Load("Config/Editor/RoleTypeConfig") as TextAsset).text);
		roleTypeConfig = roleTypeConfigDataList.data;
		RoleAnimConfigDataList roleAnimConfigDataList = JsonUtility.FromJson<RoleAnimConfigDataList>((Resources.Load("Config/Editor/RoleAnimConfig") as TextAsset).text);
		roleAnimConfig = roleAnimConfigDataList.data;
		RoleImageConfigDataList roleImageConfigDataList = JsonUtility.FromJson<RoleImageConfigDataList>((Resources.Load("Config/Editor/RoleImageConfig") as TextAsset).text);
		roleImageConfig = roleImageConfigDataList.data;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
