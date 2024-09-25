using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RolePanelManager : MonoBehaviour
{
	private static RolePanelManager instance;

	public GameObject roleSonPanel;

	public GridLayoutGroup grid;

	public float gridMinHeight = 515f;

	public List<RoleConfigData> roleConfig;

	private List<RoleSonPanel> roleSonPanelList = new List<RoleSonPanel>();

	public static RolePanelManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		instance = this;
		roleConfig = RoleManager.Instance.roleConfig;
		for (int i = 0; i < roleConfig.Count; i++)
		{
			RoleSonPanel component = Object.Instantiate(roleSonPanel, grid.transform).GetComponent<RoleSonPanel>();
			component.Enter(roleConfig[i]);
			roleSonPanelList.Add(component);
		}
		float preferredHeight = grid.preferredHeight;
		if (preferredHeight < gridMinHeight)
		{
			preferredHeight = gridMinHeight;
		}
		grid.GetComponent<RectTransform>().sizeDelta = new Vector2(grid.GetComponent<RectTransform>().sizeDelta.x, preferredHeight);
	}

	private void OnEnable()
	{
		for (int i = 0; i < roleConfig.Count; i++)
		{
			roleSonPanelList[i].Enter(roleConfig[i]);
		}
	}

	public int GerRoleIndex(RoleConfigData tempRoleData)
	{
		for (int i = 0; i < roleConfig.Count; i++)
		{
			if (tempRoleData == roleConfig[i])
			{
				return i;
			}
		}
		return -1;
	}

	public RoleConfigData GerNextRoleInfo(RoleConfigData tempRoleData)
	{
		for (int i = 0; i < roleConfig.Count; i++)
		{
			if (tempRoleData == roleConfig[i] && i < roleConfig.Count - 1)
			{
				return roleConfig[i + 1];
			}
		}
		return null;
	}

	public RoleConfigData GerLastRoleInfo(RoleConfigData tempRoleData)
	{
		for (int i = 0; i < roleConfig.Count; i++)
		{
			if (tempRoleData == roleConfig[i] && i > 0)
			{
				return roleConfig[i - 1];
			}
		}
		return null;
	}

	private void OnDisable()
	{
		for (int i = 0; i < roleSonPanelList.Count; i++)
		{
			roleSonPanelList[i].Exist();
		}
	}
}
