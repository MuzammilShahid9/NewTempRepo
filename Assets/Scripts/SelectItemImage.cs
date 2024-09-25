using UnityEngine;
using UnityEngine.UI;

public class SelectItemImage : MonoBehaviour
{
	public Image[] showImageArray;

	private void Start()
	{
		for (int i = 0; i < showImageArray.Length; i++)
		{
			showImageArray[i].gameObject.SetActive(false);
		}
	}

	public void Enter(float percent)
	{
		base.gameObject.SetActive(true);
		int num = (int)(percent * 8f);
		for (int i = 0; i < showImageArray.Length; i++)
		{
			if (i <= num)
			{
				showImageArray[i].gameObject.SetActive(true);
			}
			else
			{
				showImageArray[i].gameObject.SetActive(false);
			}
		}
	}
}
