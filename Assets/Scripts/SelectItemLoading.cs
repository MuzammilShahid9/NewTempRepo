using UnityEngine;
using UnityEngine.UI;

public class SelectItemLoading : MonoBehaviour
{
	public Image progressImage;

	private void Start()
	{
		progressImage.fillAmount = 0f;
	}

	public void Enter(float percent)
	{
		base.gameObject.SetActive(true);
		progressImage.fillAmount = percent;
	}
}
