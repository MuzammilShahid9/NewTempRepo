using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class ProcessTreasure
{
	public Element element;

	public Vector3 position;

	public ElementType effectType;

	public ProcessTreasure(Element element, Vector3 position, ElementType effectType)
	{
		this.element = element;
		this.position = position;
		this.effectType = effectType;
	}
}
