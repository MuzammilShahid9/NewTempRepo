using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class ShowStandbyEffect
{
	public Element elem;

	public ElementType type;

	public Transform parent;

	public bool isShowAnim;

	public ShowStandbyEffect(Element elem, ElementType type, Transform parent, bool isShowAnim)
	{
		this.elem = elem;
		this.type = type;
		this.parent = parent;
		this.isShowAnim = isShowAnim;
	}
}
