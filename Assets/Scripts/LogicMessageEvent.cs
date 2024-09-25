public class LogicMessageEvent
{
	public LogicMessageType type;

	public object message;

	public LogicMessageEvent(LogicMessageType type, object message = null)
	{
		this.type = type;
		this.message = message;
	}
}
