using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TYsoft.Infrastructure.Utility
{
	public static class SerialNumberGenerator
	{
		/// <summary>
		/// 按时间生成流水号(timestamp并4位随机数)
		/// </summary>
		/// <returns></returns>
		public static string Generate()
		{
			return Generate(string.Empty);
		}

		/// <summary>
		/// 按时间生成流水号(timestamp并4位随机数),标识符为可选 
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public static string Generate(string identifier)
		{
			return string.Format("{0}{1}{2}", identifier.ToUpper(), (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds, RandomGenerator.Number(4));
		}
	}
}
