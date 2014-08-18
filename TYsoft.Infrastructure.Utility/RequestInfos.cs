using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TYsoft.Infrastructure.Utility
{
	/// <summary>
	/// 获取HTTP请求的相关信息
	/// </summary>
	public static class RequestInfos
	{

		/// <summary>
		/// 获取当前物理目录路径
		/// </summary>
		public static string CurrentDirectoryPhysicalPath
		{
			get
			{
				return HttpContext.Current.Server.MapPath("./");
			}
		}

		/// <summary>
		/// 获取站点根目录物理路径
		/// </summary>
		public static string RootDirectoryPhysicalPath
		{
			get
			{
				return HttpContext.Current.Server.MapPath("~/");
			}
		}

		public static string CurrentFilePhysicalPath
		{
			get
			{
				return HttpContext.Current.Request.PhysicalPath;
			}
		}

		/// <summary>
		/// 当前相对路径
		/// </summary>
		public static string CurrentFileVisualPath
		{
			get
			{
				return HttpContext.Current.Request.Path;
			}
		}

		public static readonly Dictionary<string, string> OS_DICT = new Dictionary<string, string>(){
			{"Windows NT 5.1", "Windows XP"},
			{"Windows NT 6.0", "Windows Vista"},
			{"Windows NT 6.1", "Windows 7"},
			{"Windows NT 6.2", "Windows 8"},
			{"Windows NT 6.3", "Windows 8.1"},
			{"Linux", "Linux"},
			{"Mac OS X", "Mac OS X"}
		};

		public static string Os
		{
			get
			{
				var os = "other";
				foreach (var item in OS_DICT)
				{
					if (HttpContext.Current.Request.UserAgent.IndexOf(item.Key) >= 0)
					{
						os = item.Value;
						break;
					}
				}
				return os;
			}
		}

		public static string UserIP
		{
			get
			{
				return HttpContext.Current.Request.UserHostAddress;
			}
		}

		public static long UserIPLong
		{
			get
			{
				return System.Net.IPAddress.Parse(HttpContext.Current.Request.UserHostAddress).Address;
			}
		}

		public static string HttpMethod
		{
			get
			{
				return HttpContext.Current.Request.HttpMethod;
			}
		}

		public static string UserAgent
		{
			get
			{
				return HttpContext.Current.Request.UserAgent;
			}
		}

		public static string Referrer
		{
			get
			{
				Uri uri = HttpContext.Current.Request.UrlReferrer;
				return uri != null ? uri.ToString() : string.Empty;
			}
		}

		/// <summary>
		/// 是否HttpMethod == "POST"
		/// </summary>
		public static bool IsPost
		{
			get
			{
				return HttpContext.Current.Request.HttpMethod == "POST";
			}
		}

		public static string DomainName
		{
			get
			{
				string fullName = HttpContext.Current.Request.Url.Host.ToLower().Trim();
				return fullName.Replace("www.", ""); //
			}
		}

		/// <summary>
		/// 是否非法提交 即Referrer是否来源本站
		/// </summary>
		public static bool IsIllegalSubmit
		{
			get
			{
				if (HttpMethod == "POST")
					if (Referrer.IndexOf(DomainName) == -1)
						return true;
				return false;
			}
		}
	}
}
