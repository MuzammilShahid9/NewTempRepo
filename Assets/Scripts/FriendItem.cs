using PlayInfinity.Laveda.Core.UI;
using PlayInfinity.Leah.Core;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
	public Image Head;

	public new Text name;

	public Toggle toggle;

	private string url = "";

	public string id = "";

	private void Start()
	{
		toggle.onValueChanged.AddListener(ActiveSelf);
	}

	private void ActiveSelf(bool isOn)
	{
		AskForLifeDlg.Instance.ActiveToggle(isOn, this);
	}

	public void SetUrl(string url)
	{
		this.url = url;
		FacebookUtilities.Instance.GetPictureFromUrl(Head, url);
	}

	public string GetUrl()
	{
		return url;
	}

	public void SetName(string name)
	{
		this.name.text = name;
	}

	public string GetName()
	{
		return name.text;
	}

	public void SetId(string id)
	{
		this.id = id;
	}

	public string GetId()
	{
		return id;
	}
}
