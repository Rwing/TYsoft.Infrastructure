using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;

namespace TYsoft.EnShine.Kernel.Mvc.Authorize
{
	/// <summary>
	/// 该attribute表示需要用户登录
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class MemberAuthorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (!(httpContext.User.Identity is MemberIdentity))
				return false;
			return base.AuthorizeCore(httpContext);
		}
	}
}
