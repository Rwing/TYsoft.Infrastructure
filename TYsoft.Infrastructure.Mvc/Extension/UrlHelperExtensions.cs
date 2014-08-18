using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace TYsoft.Infrastructure.Mvc
{
	public static class UrlHelperExtensions
	{
		public static string Action<TController>(this UrlHelper urlHelper, Expression<Action<TController>> action) where TController : Controller
		{
			var routeValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
			return UrlHelper.GenerateUrl(null, null, null, routeValues, urlHelper.RouteCollection, urlHelper.RequestContext, true);
		}
	}
}
