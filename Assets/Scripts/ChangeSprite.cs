using UnityEngine;

public class ChangeSprite
{
	public int sprite;

	public GameObject renderer;

	public ChangeSprite(GameObject renderer, int sprite)
	{
		this.renderer = renderer;
		this.sprite = sprite;
	}
}
