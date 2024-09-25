using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PlayInfinity.GameEngine.Libs.DataStore
{
	public class FileIOHelper
	{
		private DesEncryption desEncrypt;

		private static FileIOHelper instance;

		private IFormatter form = new BinaryFormatter();

		public static FileIOHelper Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new FileIOHelper();
				}
				return instance;
			}
		}

		public void InitDesEnc(string key)
		{
			desEncrypt = new DesEncryption(key);
		}

		public void SaveFile(string filePath, object userData)
		{
			DebugUtils.Log(DebugType.IO, "Save encrypted user data to file: " + filePath);
			MemoryStream memoryStream = new MemoryStream();
			form.Serialize(memoryStream, userData);
			byte[] array = desEncrypt.Encrypt(memoryStream.ToArray());
			File.WriteAllBytes(filePath, array);
			DebugUtils.Log(DebugType.IO, "Save complete size: " + array.Length);
		}

		public object ReadFile(string filePath)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			byte[] bytes = File.ReadAllBytes(filePath);
			MemoryStream serializationStream = new MemoryStream(desEncrypt.Decrypt(bytes));
			return ((IFormatter)binaryFormatter).Deserialize((Stream)serializationStream);
		}
	}
}
