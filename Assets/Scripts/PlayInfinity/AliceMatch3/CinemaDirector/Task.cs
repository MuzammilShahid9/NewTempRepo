using System.Collections.Generic;
using System.Xml;

namespace PlayInfinity.AliceMatch3.CinemaDirector
{
	public class Task
	{
		public List<CDAction> actions;

		public XmlElement taskElem;

		public List<Task> subTaskList;
	}
}
