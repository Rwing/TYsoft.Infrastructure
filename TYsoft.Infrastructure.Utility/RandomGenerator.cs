using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TYsoft.Infrastructure.Utility
{
	public static class RandomGenerator
	{
		private static Random random = new Random();


		private const string BASE_CHAR_36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string BASE_CHAR_52 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		/// <summary>
		/// 转换成36进制
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static string ConvertToChar36(UInt64 num)
		{
			string str = "";
			while (num > 0)
			{
				int cur = (int)(num % 36);
				str = BASE_CHAR_36[cur] + str;
				num = num / 36;
			}
			return str;
		}

		/// <summary>
		/// 与Random.Next同样用法
		/// </summary>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Random(int max)
		{
			return random.Next(max);
		}

		/// <summary>
		/// 与Random.Next同样用法
		/// </summary>
		/// <param name="max"></param>
		/// <returns></returns>
		public static int Random(int min, int max)
		{
			return random.Next(min, max);
		}

		/// <summary>
		/// 纯数字
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Number(int length)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				sb.Append(random.Next(0, 9));
			}
			return sb.ToString();
		}

		/// <summary>
		/// 字母数字混合
		/// </summary>
		/// <returns></returns>
		public static string LetterNumber(int length)
		{
			string s = "";
			for (int j = 0; j < length; j++)
			{
				int i = random.Next(3);
				int ch;
				switch (i)
				{
					case 1:
						ch = random.Next(0, 9);
						s = s + ch.ToString();
						break;
					case 2:
						ch = random.Next(65, 90);
						s = s + Convert.ToChar(ch).ToString();
						break;
					case 3:
						ch = random.Next(97, 122);
						s = s + Convert.ToChar(ch).ToString();
						break;
					default:
						ch = random.Next(97, 122);
						s = s + Convert.ToChar(ch).ToString();
						break;
				}
				random.NextDouble();
				random.Next(100, 1999);
			}
			return s;
		}
	}
}
