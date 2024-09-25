using System.IO;

namespace Umeng
{
	public class JSONBool : JSONNode
	{
		private bool m_Data;

		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		public override string Value
		{
			get
			{
				return m_Data.ToString();
			}
			set
			{
				bool result;
				if (bool.TryParse(value, out result))
				{
					m_Data = result;
				}
			}
		}

		public override bool AsBool
		{
			get
			{
				return m_Data;
			}
			set
			{
				m_Data = value;
			}
		}

		public JSONBool(bool aData)
		{
			m_Data = aData;
		}

		public JSONBool(string aData)
		{
			Value = aData;
		}

		public override string ToString()
		{
			if (!m_Data)
			{
				return "false";
			}
			return "true";
		}

		internal override string ToString(string aIndent, string aPrefix)
		{
			if (!m_Data)
			{
				return "false";
			}
			return "true";
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write((byte)6);
			aWriter.Write(m_Data);
		}
	}
}
