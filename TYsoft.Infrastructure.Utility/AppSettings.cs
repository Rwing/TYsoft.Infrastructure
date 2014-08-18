using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TYsoft.Infrastructure.Utility
{
	/// <summary>
	/// 获取web.config中的AppSettings节点
	/// </summary>
	public static class AppSettings
	{
		#region properties

		public static string PageTitleSeparator { get { return Get("PageTitleSeparator"); } }
		public static int UploadMaxLength { get { return Get("UploadMaxLength").IsNullOrWhiteSpace() ? 2 * 1024 * 1024 : Convert.ToInt32(Get("UploadMaxLength")); } }
		public static string UploadPhysicalPath { get { return Get("UploadPhysicalPath"); } }
		public static string UploadVirtualPath { get { return Get("UploadVirtualPath"); } }
		public static string GlobalDateTimeFormat { get { return Get("GlobalDateTimeFormat"); } }
		public static string GlobalCurrencyFormat { get { return Get("GlobalCurrencyFormat"); } }
		public static string AnyMarketSetCookieUrl { get { return Get("AnyMarketSetCookieUrl"); } }
		public static string MD5Salt { get { return Get("MD5Salt"); } }

		#endregion


		public static string Get(string key)
		{
			return ConfigurationManager.AppSettings.Get(key);
		}
	}
}
