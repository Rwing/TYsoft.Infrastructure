using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{
	public static class PrimitiveTypeExtensions
	{
		#region String Extensions

		public static string HtmlEncode(this string source)
		{
			return HttpUtility.HtmlEncode(source);
		}

		public static string HtmlDecode(this string source)
		{
			return HttpUtility.HtmlDecode(source);
		}

		public static string UrlEncode(this string source)
		{
			return HttpUtility.UrlEncode(source);
		}

		public static string UrlDecode(this string source)
		{
			return HttpUtility.UrlDecode(source);
		}

		public static string UrlEncode(this string source, bool isGb2312)
		{
			return HttpUtility.UrlEncode(source, Encoding.GetEncoding("gb2312"));
		}

		public static string UrlDecode(this string source, bool isGb2312)
		{
			return HttpUtility.UrlDecode(source, Encoding.GetEncoding("gb2312"));
		}

		public static string ToJavaScriptString(this string source)
		{
			return source.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'");
		}

		/// <summary>
		/// 扩展方法 同string.IsNullOrWhiteSpace()(.net fx4.0)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		/// <summary>
		/// 同string.Format()
		/// </summary>
		/// <param name="source"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static string Format(this string source, params object[] args)
		{
			return string.Format(source, args);
		}

		private static Regex _htmlTagRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
		/// <summary>
		/// 移除所有html标记
		/// </summary>
		public static string StripHtml(this string html)
		{
			if (html.IsNullOrWhiteSpace())
				return string.Empty;

			return _htmlTagRegex.Replace(html, string.Empty);
		}

		/// <summary>
		/// 限制字符串长度为length，超过length则截取并结尾加"..."(已对中文字符进行处理）
		/// </summary>
		/// <param name="source"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string Truncate(this string source, int length)
		{
			if (length == 0 || source.IsNullOrWhiteSpace()) return source;
			int j = 0;
			int k = 0;
			for (int i = 0; i < source.Length; i++)
			{
				if (Regex.IsMatch(source.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
				{
					j += 2;
				}
				else
				{
					j += 1;
				}
				if (j <= length)
				{
					k += 1;
				}
				if (j >= length)
				{
					return source.Substring(0, k) + "...";
				}
			}
			return source;
		}

		/// <summary>
		/// 同Regex.IsMatch
		/// </summary>
		/// <param name="source"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		public static bool IsMatch(this string source, string pattern)
		{
			return Regex.IsMatch(source, pattern);
		}

		/// <summary>
		/// 同Regex.Replace
		/// </summary>
		/// <param name="source"></param>
		/// <param name="pattern"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static string ReplaceWithRegex(this string source, string pattern, string newValue)
		{
			return Regex.Replace(source, pattern, newValue, RegexOptions.IgnoreCase);
		}

		#endregion

		#region Int Extensions

		public static string ToString(this int? source, int defaultValue)
		{
			return source.HasValue ? source.Value.ToString() : defaultValue.ToString();
		}

		/// <summary>
		/// 转换为数字格式 增加逗号 小数点后两位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToNumber(this int? source)
		{
			return source.HasValue ? source.Value.ToNumber() : string.Empty;
		}

		/// <summary>
		/// 转换为数字格式 增加逗号 小数点后两位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToNumber(this int source)
		{
			return source.ToString("N");
		}

		#endregion

		#region Decimal Extensions

		/// <summary>
		/// 转换为货币格式 +货币符号 小数点后两位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToCurrency(this decimal source)
		{
			//return source.ToString("C");//默认的半角符号看不清啊¥，换成全角吧￥- -
			return "￥" + source.ToNumber();
		}

		/// <summary>
		/// 转换为数字格式 增加逗号 小数点后两位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToNumber(this decimal source)
		{
			return source.ToString("N");
		}

		/// <summary>
		/// 转换为货币格式 +货币符号 小数点后两位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToCurrency(this decimal? source)
		{
			return source.HasValue ? source.Value.ToCurrency() : string.Empty;
		}

		/// <summary>
		/// 转换为数字格式 增加逗号 小数点后两位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToNumber(this decimal? source)
		{
			return source.HasValue ? source.Value.ToNumber() : string.Empty;
		}

		/// <summary>
		/// 输出整型显示
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToInt(this decimal source)
		{
			return Convert.ToInt32(source).ToString();
		}

		/// <summary>
		/// 输出整型显示
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToInt(this decimal? source)
		{
			return source.HasValue ? source.Value.ToInt() : string.Empty;
		}

		/// <summary>
		/// 输出小数点后只有1位
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToOneDigit(this decimal source)
		{
			return source.ToString("0.0");
		}

		/// <summary>
		/// 输出1位小数
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string ToOneDigit(this decimal? source)
		{
			return source.HasValue ? source.Value.ToOneDigit() : string.Empty;
		}

		#endregion

		#region Object Extensions

		public static bool IsNull(this object source)
		{
			return source == null;
		}

		public static bool IsNotNull(this object source)
		{
			return !source.IsNull();
		}

		#endregion

		#region DateTime Extensions

		public static string ToyyyyMMddHHmm(this DateTime source)
		{
			return source.ToString("yyyy-MM-dd HH:mm");
		}

		public static string ToyyyyMMdd(this DateTime source)
		{
			return source.ToString("yyyy-MM-dd");
		}

		public static string ToMMdd(this DateTime source)
		{
			return source.ToString("MM-dd");
		}

		public static int ToTimestamp(this DateTime source)
		{
			return (int)source.Subtract(new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
		}

		public static UInt64 ToTimestamp64(this DateTime source)
		{
			return (UInt64)source.Subtract(new DateTime(1970, 1, 1).ToLocalTime()).TotalMilliseconds;
		}

		/// <summary>
		/// 返回当前周的第一天0点和最后一天的23:59，中国周：周一到周日
		/// </summary>
		/// <param name="source"></param>
		public static Tuple<DateTime, DateTime> FirstLastDayOfWeek(this DateTime source)
		{
			DateTime firstDayOfWeek, lastDayOfWeek;
			var dayOfWeek = (int)source.DayOfWeek;
			if (dayOfWeek == 0)
			{
				firstDayOfWeek = source.Date.AddDays(-6);
				lastDayOfWeek = source.Date.AddDays(1).AddMilliseconds(-1);
			}
			else
			{
				firstDayOfWeek = source.Date.AddDays(-(dayOfWeek - 1));
				lastDayOfWeek = source.Date.AddDays(dayOfWeek + 6).AddMilliseconds(-1);
			}
			return new Tuple<DateTime, DateTime>(firstDayOfWeek, lastDayOfWeek);//就返回Tuple吧，不想引入新的类型了
		}

		#endregion

		#region IEnumerable Extensions
		/// <summary>
		/// 与List的ForEach相同
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="col"></param>
		/// <param name="handler"></param>
		public static void Each<T>(this IEnumerable<T> col, Action<T> handler)
		{
			foreach (var item in col)
				handler(item);
		}

		/// <summary>
		/// 带索引
		/// </summary>
		public static void Each<T>(this IEnumerable<T> col, Action<T, int> handler)
		{
			int index = 0;
			foreach (var item in col)
				handler(item, index++);
		}

		/// <summary>
		/// 可中断
		/// </summary>
		public static void Each<T>(this IEnumerable<T> col, Func<T, bool> handler)
		{
			foreach (var item in col)
				if (!handler(item)) break;
		}

		/// <summary>
		/// 可中断 带索引
		/// </summary>
		public static void Each<T>(this IEnumerable<T> col, Func<T, int, bool> handler)
		{
			int index = 0;
			foreach (var item in col)
				if (!handler(item, index++)) break;
		}
		#endregion

		#region General Extensions

		public static bool In<T>(this T source, params T[] array)
		{
			return array.Contains(source);
		}

		#endregion
	}
}
