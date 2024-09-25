using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PlayInfinity.GameEngine.Libs.DataStore
{
	public class DesEncryption
	{
		private const int KeyLength = 24;

		private const int IVLength = 8;

		private readonly TripleDES _algorithm;

		public DesEncryption(string key)
		{
			_algorithm = TripleDES.Create();
			_algorithm.Key = MakeKey(key);
			_algorithm.IV = MakeIV(key);
		}

		public byte[] Encrypt(byte[] bytes)
		{
			using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ICryptoTransform transform = tripleDESCryptoServiceProvider.CreateEncryptor(_algorithm.Key, _algorithm.IV))
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(bytes, 0, bytes.Length);
							cryptoStream.FlushFinalBlock();
							return memoryStream.ToArray();
						}
					}
				}
			}
		}

		public byte[] Decrypt(byte[] bytes)
		{
			using (TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider())
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ICryptoTransform transform = tripleDESCryptoServiceProvider.CreateDecryptor(_algorithm.Key, _algorithm.IV))
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
						{
							cryptoStream.Write(bytes, 0, bytes.Length);
							cryptoStream.FlushFinalBlock();
							return memoryStream.ToArray();
						}
					}
				}
			}
		}

		private byte[] MakeKey(string input)
		{
			string s = input.PadRight(24, '0').Substring(0, 24);
			return Encoding.ASCII.GetBytes(s);
		}

		private byte[] MakeIV(string input)
		{
			string s = input.PadRight(8, '0').Substring(0, 8);
			return Encoding.ASCII.GetBytes(s);
		}
	}
}
