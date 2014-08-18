using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TYsoft.Infrastructure.Utility;

namespace TYsoft.Infrastructure.Mvc
{
	public static class PagingExtensions
	{
		#region HtmlHelper extensions

		public static IHtmlString Pager(this HtmlHelper htmlHelper, int pageIndex, int pageSize)
		{
			return htmlHelper.Pager(new Pager(pageIndex, pageSize));
		}

		public static IHtmlString Pager(this HtmlHelper helper, IPager pager, string language = "zh-cn")
		{
			if (pager.TotalPage <= 1) return null;
			string path = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
			path = path.ReplaceWithRegex(@"/\d+$", string.Empty);

			if (!path.EndsWith("/"))
				path += "/";
			int textIndex = language == "zh-cn" ? 0 : 1;
			StringBuilder sb = new StringBuilder();
			sb.Append("<div class=\"pagination\">");
			sb.Append(BuildPagerItem(TEXT_FIRST_PAGES[textIndex], GenerateUrl(helper, pager, 1), "first", pager.IsAjax, pager.AjaxContainerId));
			if (pager.HasPreviousPage)
				sb.Append(BuildPagerItem(TEXT_PREV_PAGES[textIndex], GenerateUrl(helper, pager, pager.PageIndex - 1), "prev", pager.IsAjax, pager.AjaxContainerId));
			//设置最多只显示x个页,第一个从当前页-x/2开始
			int halfShowPage = (int)Math.Floor((double)pager.ShowPage / 2);
			int minIndex = pager.PageIndex - halfShowPage;
			if (minIndex <= 0) minIndex = 1;
			if (pager.TotalPage <= pager.ShowPage) minIndex = 1;
			int maxIndex = pager.TotalPage <= pager.ShowPage ? pager.TotalPage : (pager.PageIndex + (halfShowPage));
			if (maxIndex > pager.TotalPage) maxIndex = pager.TotalPage;
			for (; minIndex <= maxIndex; minIndex++)
			{
				sb.Append(BuildPagerItem(minIndex.ToString(), GenerateUrl(helper, pager, minIndex), (minIndex == pager.PageIndex ? "current" : "number"), pager.IsAjax, pager.AjaxContainerId, minIndex == pager.PageIndex));
			}
			if (pager.HasNextPage)
				sb.AppendFormat(BuildPagerItem(TEXT_NEXT_PAGES[textIndex], GenerateUrl(helper, pager, pager.PageIndex + 1), "next", pager.IsAjax, pager.AjaxContainerId));
			sb.Append(BuildPagerItem(TEXT_LAST_PAGES[textIndex], GenerateUrl(helper, pager, pager.TotalPage), "last", pager.IsAjax, pager.AjaxContainerId));
			sb.Append(BuildPagerItem(string.Format(TEXT_PAGE_INFOS[textIndex], pager.TotalPage, pager.PageSize, pager.TotalCount), "", "total", pager.IsAjax, pager.AjaxContainerId, true));
			sb.Append("</div>");
			return MvcHtmlString.Create(sb.ToString());
		}

		#endregion
		public static readonly string[] TEXT_FIRST_PAGES = { "« 首页", "« First" };
		public static readonly string[] TEXT_LAST_PAGES = { "尾页 »", "Last »" };
		public static readonly string[] TEXT_PREV_PAGES = { "« 上一页", "« Prev" };
		public static readonly string[] TEXT_NEXT_PAGES = { "下一页 »", "Next »" };
		public static readonly string[] TEXT_PAGE_INFOS = { "共{0}页(每页{1}条共{2}条)", "Total {0} pages({1} records per page, total {2} records)" };

		public static readonly string PAGER_ITEM_TEMPLATE = "<span class=\"{1}\">{0}</span>\r\n";
		public static readonly string PAGER_ITEM_A_TEMPLATE = "<a href=\"{1}\">{0}</a>";
		public static readonly string PAGER_AJAX_ITEM_A_TEMPLATE = "<a href=\"{1}\" onclick='$.ajaxTurnPage($(this).attr(\"href\"), \"{2}\");return false;'>{0}</a>";

		private static string BuildPagerItem(string text, string targetUrl, string cssClass, bool isAjax, string ajaxContainerId, bool skipTagA = false)
		{
			string tagA = string.Format(PAGER_ITEM_A_TEMPLATE, text, targetUrl);
			if (isAjax)
				tagA = string.Format(PAGER_AJAX_ITEM_A_TEMPLATE, text, targetUrl, ajaxContainerId.IsNullOrWhiteSpace() ? "pagerList" : ajaxContainerId);
			return string.Format(PAGER_ITEM_TEMPLATE, skipTagA ? text : tagA, cssClass);
		}

		private static string GenerateUrl(HtmlHelper helper, IPager pager, int pageIndex)
		{
			string pageParamName = string.IsNullOrWhiteSpace(pager.UniquePrefix) ? "pageIndex" : ("pageIndex" + pager.UniquePrefix);
			//重新clone一份
			var routeValues = new RouteValueDictionary(helper.ViewContext.RouteData.Values);

			//string actionName = (string)helper.ViewContext.RouteData.Values["action"];
			//string controllerName = (string)helper.ViewContext.RouteData.Values["controller"];
			//var routeValues = new RouteValueDictionary();
			var queryStrings = helper.ViewContext.HttpContext.Request.QueryString;
			foreach (string key in queryStrings.Keys)
				if (!key.IsNullOrWhiteSpace())
					routeValues[key] = queryStrings[key];
			if (helper.ViewContext.IsChildAction)
			{
				//如果是child action，那么特殊处理下
				routeValues["action"] = helper.ViewContext.ParentActionViewContext.RouteData.Values["action"];
				routeValues["controller"] = helper.ViewContext.ParentActionViewContext.RouteData.Values["controller"];
			}
			routeValues[pageParamName] = pageIndex;
			//routeValues["action"] = actionName;
			//routeValues["controller"] = controllerName;
			var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
			return urlHelper.RouteUrl(routeValues);
		}

		#region IQueryable Extensions

		#region overload

		/// <summary>
		/// 本操作前要先OrderBy 转换为PagedList
		/// </summary>
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex)
		{
			return source.ToPagedList(pageIndex, 10, false, null);
		}

		/// <summary>
		/// 本操作前要先OrderBy 转换为PagedList
		/// </summary>
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize)
		{
			return source.ToPagedList(pageIndex, pageSize, false, null);
		}

		/// <summary>
		/// 本操作前要先OrderBy 转换为PagedList
		/// </summary>
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool isAjax)
		{
			return source.ToPagedList(pageIndex, pageSize, isAjax, null);
		}

		/// <summary>
		/// 本操作前要先OrderBy 转换为PagedList
		/// </summary>
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool isAjax, string ajaxContainerId)
		{
			return source.ToPagedList(pageIndex, pageSize, isAjax, ajaxContainerId, null);
		}

		/// <summary>
		/// 本操作前要先OrderBy 转换为PagedList
		/// </summary>
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, bool isAjax, string ajaxContainerId, string uniqueSuffix)
		{
			var pager = new Pager(source.Count(), pageIndex < 1 ? 1 : pageIndex, pageSize, isAjax, ajaxContainerId, uniqueSuffix);
			return source.ToPagedList(pager);
		}

		#endregion

		/// <summary>
		/// 本操作前要先OrderBy 转换为PagedList
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <param name="pager"></param>
		/// <returns></returns>
		public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, IPager pager)
		{
			return new PagedList<T>(source, pager);
		}

		#endregion
	}
}
