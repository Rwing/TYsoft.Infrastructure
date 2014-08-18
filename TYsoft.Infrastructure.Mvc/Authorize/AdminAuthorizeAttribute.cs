using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using TYsoft.Infrastructure.Utility;

namespace TYsoft.EnShine.Kernel.Mvc.Authorize
{
	/// <summary>
	/// 该attribute表示需要管理员登录
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class AdminAuthorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (!(httpContext.User.Identity is AdminIdentity))
				return false;
			if (!base.Users.IsNullOrWhiteSpace())
				if (!base.Users.Contains((httpContext.User.Identity as AdminIdentity).Admin.UserName))
				{
					httpContext.Response.StatusCode = 999;
					return false;
				}
				else
					return true;//这里强制返回true，让他再进行base的验证，因为base验证还会返回false
			return base.AuthorizeCore(httpContext);
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			if (filterContext.HttpContext.Response.StatusCode == 999)
			{
				filterContext.HttpContext.Response.StatusCode = 200;
				filterContext.Result = new ContentResult() { Content = "对不起，您没有权限，请返回" };
			}
			else
				filterContext.Result = new RedirectResult("/Admin/LogOn");
			//base.HandleUnauthorizedRequest(filterContext);
		}
	}
}
