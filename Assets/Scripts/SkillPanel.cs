using UnityEngine;

public class SkillPanel : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			base.gameObject.SetActive(false);
		}
	}
}
