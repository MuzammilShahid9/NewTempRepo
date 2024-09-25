using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class ProcessBox
{
	public Element element;

	public Vector3 position;

	public ElementType effectType;

	public ProcessBox(Element element, Vector3 position, ElementType effectType)
	{
		this.element = element;
		this.position = position;
		this.effectType = effectType;
	}
}
