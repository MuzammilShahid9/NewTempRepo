using UnityEngine;

public class ShowOrHideSprite
{
	public bool flag;

	public SpriteRenderer renderer;

	public ShowOrHideSprite(SpriteRenderer renderer, bool flag)
	{
		this.renderer = renderer;
		this.flag = flag;
	}
}
