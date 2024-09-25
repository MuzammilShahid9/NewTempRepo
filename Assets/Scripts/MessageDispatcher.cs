using System;
using System.Collections.Generic;

public class MessageDispatcher : Singleton<MessageDispatcher>
{
	private Dictionary<uint, MessageHandler> m_HandlerMap;

	public MessageDispatcher()
	{
		m_HandlerMap = new Dictionary<uint, MessageHandler>();
	}

	public MessageHandler RegisterMessageHandler(uint iMessageType, MessageHandler handler)
	{
		if (handler == null)
		{
			return null;
		}
		if (!m_HandlerMap.ContainsKey(iMessageType))
		{
			m_HandlerMap.Add(iMessageType, handler);
		}
		else
		{
			Dictionary<uint, MessageHandler> handlerMap = m_HandlerMap;
			handlerMap[iMessageType] = (MessageHandler)Delegate.Combine(handlerMap[iMessageType], handler);
		}
		return handler;
	}

	public void UnRegisterMessageHandler(uint iMessageType, MessageHandler handler)
	{
		if (handler != null && m_HandlerMap.ContainsKey(iMessageType))
		{
			Dictionary<uint, MessageHandler> handlerMap = m_HandlerMap;
			handlerMap[iMessageType] = (MessageHandler)Delegate.Remove(handlerMap[iMessageType], handler);
		}
	}

	public void SendMessage(uint iMessageType, object arg)
	{
		if (m_HandlerMap.ContainsKey(iMessageType) && m_HandlerMap[iMessageType] != null)
		{
			m_HandlerMap[iMessageType](iMessageType, arg);
		}
	}
}
