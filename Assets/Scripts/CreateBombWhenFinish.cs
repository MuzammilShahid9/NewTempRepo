using System.Collections.Generic;
using PlayInfinity.AliceMatch3.Core;

public class CreateBombWhenFinish
{
	public List<Cell> bombList;

	public List<ElementType> bombType;

	public CreateBombWhenFinish(List<Cell> bombList, List<ElementType> bombType)
	{
		this.bombList = bombList;
		this.bombType = bombType;
	}
}
