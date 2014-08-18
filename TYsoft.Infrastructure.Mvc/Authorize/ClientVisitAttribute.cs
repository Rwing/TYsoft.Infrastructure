using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TYsoft.EnShine.Domain;

namespace TYsoft.EnShine.Kernel.Mvc.Authorize
{
	/// <summary>
	/// 客户端访问验证
	/// </summary>
	public class ClientVisitAttribute : ActionFilterAttribute
	{
		public ClientVisitAttribute()
		{
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			if (filterContext == null)
				throw new ArgumentNullException("filterContext");
			var userAgent = filterContext.HttpContext.Request.UserAgent;
			if (!userAgent.StartsWith("browser=95niu;code=xzxabdeafces;version=") && !filterContext.HttpContext.Request.IsLocal)
			{
				filterContext.Result = new ContentResult() { Content = "错误验证，请联系相关人员" };
			}
		}
	}
}
