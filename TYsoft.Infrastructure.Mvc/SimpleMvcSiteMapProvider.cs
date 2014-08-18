using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using TYsoft.Infrastructure.Utility;

namespace TYsoft.Infrastructure.Mvc
{
	/// <summary>
	/// 简单的mvc sitemap provider，实现了如下URL也可以对应到sitemap的node中的URL
	/// http//www.xx.com/home/index/
	/// http//www.xx.com/home/index/11
	/// 对应到
	/// http//www.xx.com/home/index
	/// </summary>
	public class SimpleMvcSiteMapProvider : XmlSiteMapProvider
	{
		public override SiteMapNode FindSiteMapNode(string rawUrl)
		{
			//if (rawUrl.LastIndexOf("/") == 0)//把/Home对应到/Home/Index
			//	rawUrl += "/Index";
			//else
			//if (rawUrl.EndsWith("/")) //把/Home/Index/ 对应到/Home/Index
			//	rawUrl = rawUrl.Substring(0, rawUrl.Length - 1);
			//else
			 if (rawUrl.IsMatch("[\\s\\S]+/\\d+$"))//把/Home/Index/11 对应到/Home/Index
				rawUrl = rawUrl.ReplaceWithRegex("/\\d+$", string.Empty);
			return base.FindSiteMapNode(rawUrl);
		}
	}
}
