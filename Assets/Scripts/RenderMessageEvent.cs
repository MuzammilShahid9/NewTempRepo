public class RenderMessageEvent
{
	public RenderMessageType type;

	public object message;

	public RenderMessageEvent(RenderMessageType type, object message = null)
	{
		this.type = type;
		this.message = message;
	}
}
