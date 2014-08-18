using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace TYsoft.Infrastructure.Mvc
{
	public static class MasterPageExtensions
	{
		public static readonly string SiteMapNodeTemplate = @"<li{2}><a href=""{1}"">{0}</a></li>";
		public static readonly string CurrentMenuTemplate = @"<a href=""{1}""{2}>{0}</a>";
		//public static readonly string SplitTemplate = @"<li class=""split"">&nbsp;</li>";

		public static IHtmlString GetLeftMenu(this HtmlHelper htmlHelper)
		{
			StringBuilder sb = new StringBuilder();
			var nodes = SiteMap.RootNode.ChildNodes;
			foreach (SiteMapNode node in nodes)
			{
				bool isCurrent = SiteMap.CurrentNode != null && SiteMap.CurrentNode.Url.IndexOf(node.Url) >= 0;
				sb.AppendLine(string.Format(SiteMapNodeTemplate, node.Title, node.Url, isCurrent ? " class=\"current\"" : string.Empty));
				//sb.AppendLine(SplitTemplate);
			}
			//sb.Remove(sb.Length - SplitTemplate.Length - 2, SplitTemplate.Length);
			return MvcHtmlString.Create(sb.ToString());
		}

		public static IHtmlString GetTopMenu(this HtmlHelper htmlHelper)
		{
			StringBuilder sb = new StringBuilder();
			if (SiteMap.CurrentNode != null && SiteMap.CurrentNode.ParentNode != null)
			{
				var nodes = SiteMap.CurrentNode.ChildNodes;
				var currentNode = SiteMap.CurrentNode;
				//判断是第3级还是第4级
				if (SiteMap.CurrentNode.ParentNode != null && SiteMap.CurrentNode.ParentNode.ParentNode != null && SiteMap.CurrentNode.ParentNode.ParentNode.ParentNode != null)
				{
					nodes = SiteMap.CurrentNode.ParentNode.ParentNode.ChildNodes;
					currentNode = SiteMap.CurrentNode.ParentNode;
				}
				else if (SiteMap.CurrentNode.ParentNode != null && SiteMap.CurrentNode.ParentNode.ParentNode != null)
					nodes = SiteMap.CurrentNode.ParentNode.ChildNodes;
				foreach (SiteMapNode node in nodes)
				{
					if (node.Description.IndexOf("hidden;") >= 0 && !SiteMap.CurrentNode.Equals(node))
						continue;
					sb.AppendLine(string.Format(SiteMapNodeTemplate, node.Title, node.Url, currentNode == node ? " class=\"current\"" : string.Empty));
				}
			}
			return MvcHtmlString.Create(sb.ToString());
		}

		public static string GetCurrentNode(this HtmlHelper htmlHelper)
		{
			var node = SiteMap.CurrentNode;
			if (node != null && node.ParentNode != null && node.ParentNode != SiteMap.RootNode)
				return node.ParentNode.Title;
			return string.Empty;
		}

		public static IHtmlString GetCurrentMenu(this HtmlHelper htmlHelper)
		{
			string result = string.Empty;
			var node = SiteMap.CurrentNode;
			if (node != null && node.ParentNode != null)
			{
				result = string.Format(CurrentMenuTemplate, node.Title, node.Url, " class=\"current\"");
				node = node.ParentNode;
				while (node.ParentNode != null)
				{
					result = string.Format(CurrentMenuTemplate, node.Title, node.Url, "") + " &raquo; " + result;
					node = node.ParentNode;
				}
			}
			return MvcHtmlString.Create(result);
		}
	}
}