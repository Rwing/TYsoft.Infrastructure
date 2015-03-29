using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TYsoft.Infrastructure.Mvc.Authorize
{
	/// <summary>
	/// 简单的验证，只有一个密码，保存在session["SimplePasswordAuthorize"]
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class SimplePasswordAuthorizeAttribute : AuthorizeAttribute
	{
		private string _password;
		private string _logOnUrl = "/Admin/LogOn";

		public SimplePasswordAuthorizeAttribute(string password)
			: this(password, string.Empty)
		{ }

		public SimplePasswordAuthorizeAttribute(string password, string logOnUrl)
		{
			_password = password;
			if (!string.IsNullOrWhiteSpace(logOnUrl))
				_logOnUrl = logOnUrl;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			var result = new ContentResult();
			result.Content = "你的密码有误，无权查看此目录或页面。<a href='" + _logOnUrl + "?returnUrl=" + System.Web.HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl) + "'>登录</a>";
			filterContext.Result = result;
		}

		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			var sessionPassword = httpContext.Session["SimplePasswordAuthorize"];
			if (sessionPassword != null && (string)sessionPassword == _password)
				return true;
			//var cookie = httpContext.Request.Cookies["SimplePasswordAuthorize"];
			//if (cookie != null && cookie.Value == _password)
			//	return true;
			return false;
		}
	}
}
