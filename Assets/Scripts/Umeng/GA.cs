using System;

namespace Umeng
{
	public class GA : Analytics
	{
		public enum Gender
		{
			Unknown,
			Male,
			Female
		}

		public enum PaySource
		{
			AppStore = 1,
			支付宝,
			网银,
			财付通,
			移动,
			联通,
			电信,
			Paypal,
			Source9,
			Source10,
			Source11,
			Source12,
			Source13,
			Source14,
			Source15,
			Source16,
			Source17,
			Source18,
			Source19,
			Source20,
			Source21,
			Source22,
			Source23,
			Source24,
			Source25,
			Source26,
			Source27,
			Source28,
			Source29,
			Source30
		}

		public enum BonusSource
		{
			玩家赠送 = 1,
			Source2,
			Source3,
			Source4,
			Source5,
			Source6,
			Source7,
			Source8,
			Source9,
			Source10
		}

		public static void SetUserLevel(int level)
		{
			
		}

		[Obsolete("SetUserInfo已弃用, 请使用ProfileSignIn")]
		public static void SetUserInfo(string userId, Gender gender, int age, string platform)
		{
			
		}

		public static void StartLevel(string level)
		{
			
		}

		public static void FinishLevel(string level)
		{
			
		}

		public static void FailLevel(string level)
		{
			
		}

		public static void Pay(double cash, PaySource source, double coin)
		{
			
		}

		public static void Pay(double cash, int source, double coin)
		{
			if (source < 1 || source > 100)
			{
				throw new ArgumentException();
			}
			
		}

		public static void Pay(double cash, PaySource source, string item, int amount, double price)
		{
			
		}

		public static void Buy(string item, int amount, double price)
		{
			
		}

		public static void Use(string item, int amount, double price)
		{
			
		}

		public static void Bonus(double coin, BonusSource source)
		{
			
		}

		public static void Bonus(string item, int amount, double price, BonusSource source)
		{
			
		}

		public static void ProfileSignIn(string userId)
		{
			
		}

		public static void ProfileSignIn(string userId, string provider)
		{
			
		}

		public static void ProfileSignOff()
		{
			
		}
	}
}
