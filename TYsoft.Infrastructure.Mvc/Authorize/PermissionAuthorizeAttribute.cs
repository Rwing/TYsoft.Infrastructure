//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using TYsoft.EnShine.Domain;

//namespace TYsoft.EnShine.Kernel.Mvc.Authorize
//{
//	/// <summary>
//	/// 权限验证
//	/// </summary>
//	public class PermissionAuthorizeAttribute : ActionFilterAttribute
//	{
//		private List<AdminAction> _adminActions;

//		public List<AdminAction> AdminActions
//		{
//			get { return _adminActions; }
//			set { _adminActions = value; }
//		}

//		//TODO: 这个attribute设计的不太好，应该可以进一步抽象，现在太依赖具体项目了，以后重构吧
//		public PermissionAuthorizeAttribute(List<AdminAction> adminActions)
//		{
//			this.AdminActions = adminActions;
//		}

//		public override void OnActionExecuting(ActionExecutingContext filterContext)
//		{
//			if (filterContext == null)
//				throw new ArgumentNullException("filterContext");
//			var path = filterContext.HttpContext.Request.Path.ToLower();
//			if (path == "/LogOn".ToLower() || filterContext.IsChildAction)
//				return;//忽略对Login登录页以及ChildAction的权限判定
//			if (VerifyAdmin(filterContext))
//			{
//				//验证用户成功，继续验证角色
//				if (VerifyRole(filterContext))
//				{
//					//不做任何事
//				}
//				else
//				{
//					filterContext.Result = new ContentResult { Content = @"对不起,你不具有当前操作的权限!" };
//				}
//			}
//			else
//				filterContext.Result = new HttpUnauthorizedResult();
//		}

//		public bool VerifyAdmin(ActionExecutingContext filterContext)
//		{
//			return filterContext.HttpContext.User.Identity.IsAuthenticated;
//		}

//		public bool VerifyRole(ActionExecutingContext filterContext)
//		{
//			var currentAdmin = filterContext.HttpContext.User.Identity as AdminIdentity;
//			if (currentAdmin.IsNotNull())
//			{
//				var controllerName = filterContext.RouteData.Values["controller"].ToString();
//				var actionName = filterContext.RouteData.Values["action"].ToString();
//				var action = (from item in this.AdminActions
//							  where actionName.Equals(item.ActionName, StringComparison.OrdinalIgnoreCase)
//							  && controllerName.Equals(item.ControllerName, StringComparison.OrdinalIgnoreCase)
//							  select item).SingleOrDefault();
//				if (action.IsNotNull())
//				{
//					var actionRoles = action.Roles;
//					var result = actionRoles.Intersect(currentAdmin.Admin.Roles, new RoleEqualityComparer());
//					if (result.Count() > 0)
//						return true;
//				}
//			}
//			return false;
//		}
//	}
//}
