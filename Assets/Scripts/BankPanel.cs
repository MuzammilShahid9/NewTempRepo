using UnityEngine;

public class BankPanel : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			base.gameObject.SetActive(false);
		}
	}
}
