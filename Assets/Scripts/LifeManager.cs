using System;
using PlayInfinity.GameEngine.Common;
using PlayInfinity.Laveda.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
	private static LifeManager instance;

	public static LifeManager Instance
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
		UpdateLifeData();
	}

	private void UpdateLifeData()
	{
		if (UserDataManager.Instance.GetService().lifeConsumeTime == -1 && UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal)
		{
			UserDataManager.Instance.GetService().life = GeneralConfig.LifeTotal;
			UserDataManager.Instance.Save();
		}
	}

	private void Update()
	{
		UpdateLife();
	}

	private void UpdateLife()
	{
		if (UserDataManager.Instance.GetService().unlimitedLife)
		{
			TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			if (UserDataManager.Instance.GetService().unlimitedLifeTM - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().unlimitedLifeStartTM) <= 0)
			{
				UserDataManager.Instance.GetService().unlimitedLife = false;
				UserDataManager.Instance.GetService().unlimitedLifeTM = -1L;
				UserDataManager.Instance.GetService().unlimitedLifeStartTM = -1L;
				UserDataManager.Instance.Save();
				if (CastleSceneUIManager.Instance != null)
				{
					CastleSceneUIManager.Instance.UpdateLifeNumberText();
				}
				if (GetLifeDlg.Instance != null && GetLifeDlg.Instance.gameObject.activeInHierarchy)
				{
					GetLifeDlg.Instance.ShowDetail();
				}
			}
		}
		else
		{
			if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal || UserDataManager.Instance.GetService().lifeConsumeTime == -1)
			{
				return;
			}
			long num = GeneralConfig.LifeRecoverTime - (DateTime.Now.Ticks / 10000000 - UserDataManager.Instance.GetService().lifeConsumeTime);
			if (num <= 0)
			{
				int num2 = (int)Math.Abs(num / GeneralConfig.LifeRecoverTime) + 1;
				DebugUtils.Log(DebugType.Other, "recover num" + num2);
				if (!(SceneManager.GetActiveScene().name == "GameScene") || UserDataManager.Instance.GetService().life < GeneralConfig.LifeTotal - 1)
				{
					AddUserLife(num2, true);
				}
			}
		}
	}

	public void AddUserLife(int lifeNumber, bool isRecoverConsumeTime = false)
	{
		UserDataManager.Instance.GetService().life += lifeNumber;
		if (UserDataManager.Instance.GetService().life >= GeneralConfig.LifeTotal)
		{
			UserDataManager.Instance.GetService().life = GeneralConfig.LifeTotal;
			UserDataManager.Instance.GetService().lifeConsumeTime = -1L;
			Singleton<MessageDispatcher>.Instance().SendMessage(36u, null);
		}
		else if (UserDataManager.Instance.GetService().life <= 0)
		{
			UserDataManager.Instance.GetService().life = 0;
			if (isRecoverConsumeTime)
			{
				UserDataManager.Instance.GetService().lifeConsumeTime = DateTime.Now.Ticks / 10000000;
			}
		}
		else if (isRecoverConsumeTime)
		{
			UserDataManager.Instance.GetService().lifeConsumeTime = DateTime.Now.Ticks / 10000000;
		}
		UserDataManager.Instance.Save();
		if (CastleSceneUIManager.Instance != null)
		{
			CastleSceneUIManager.Instance.UpdateLifeNumberText();
		}
		if (GetLifeDlg.Instance != null && GetLifeDlg.Instance.gameObject.activeInHierarchy)
		{
			GetLifeDlg.Instance.ShowDetail();
		}
	}
}
