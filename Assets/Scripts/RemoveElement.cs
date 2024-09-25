using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class RemoveElement
{
	public Element element;

	public Vector3 position;

	public RemoveElement(Element element, Vector3 position)
	{
		this.element = element;
		this.position = position;
	}
}
