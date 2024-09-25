using PlayInfinity.AliceMatch3.Core;

public struct Combo
{
	public ElementType type;

	public int num;

	public Combo(ElementType type, int num)
	{
		this.type = type;
		this.num = num;
	}
}
