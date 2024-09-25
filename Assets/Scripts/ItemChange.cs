using UnityEngine;

public class ItemChange : MonoBehaviour
{
	public ItemAnim parentItem;

	private void OnMouseUp()
	{
		parentItem.MouseUp();
	}

	private void OnMouseDown()
	{
		parentItem.MouseDownEvent();
	}
}
