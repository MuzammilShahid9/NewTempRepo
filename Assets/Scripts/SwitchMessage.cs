using PlayInfinity.AliceMatch3.Core;

public struct SwitchMessage
{
	public Element element1;

	public Element element2;

	public bool checkMatch;

	public Board board;

	public Direction dir;

	public SwitchMessage(Board board, Element element1, Element element2, bool checkMatch, Direction dir)
	{
		this.board = board;
		this.element1 = element1;
		this.element2 = element2;
		this.checkMatch = checkMatch;
		this.dir = dir;
	}
}
