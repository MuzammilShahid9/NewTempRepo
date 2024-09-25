using PlayInfinity.AliceMatch3.Core;
using UnityEngine;

public class ProcessIce
{
	public Element element;

	public Vector3 position;

	public int type;

	public ProcessIce(Element element, Vector3 position, int type)
	{
		this.element = element;
		this.position = position;
		this.type = type;
	}
}
