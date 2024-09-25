using UnityEngine;

public class ParitcleSystem : MonoBehaviour
{
	private float timer;

	public float lifetime = 5f;

	private bool isFinish;

	private ItemAnim currItemAnim;

	private void Start()
	{
		isFinish = false;
	}

	private void Update()
	{
		if (isFinish)
		{
			return;
		}
		timer += Time.deltaTime;
		if (timer >= lifetime)
		{
			timer = 0f;
			if (currItemAnim != null)
			{
				currItemAnim.FinishAnim();
			}
			isFinish = true;
		}
	}

	public void Enter(Transform tempTransform)
	{
		currItemAnim = tempTransform.GetComponent<ItemAnim>();
	}
}
