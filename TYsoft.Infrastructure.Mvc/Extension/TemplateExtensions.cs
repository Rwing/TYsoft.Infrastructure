using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Collections;
//using Microsoft.Web.Mvc;

namespace TYsoft.Infrastructure.Mvc
{
	public static class TemplateExtensions
	{
		public static string DisplayForModel<TModel>(this HtmlHelper<TModel> html, string headerTemplateName, string rowTemplateName)
		{
			return html.DisplayForModel(headerTemplateName, rowTemplateName, string.Empty);
		}

		public static string DisplayForModel<TModel>(this HtmlHelper<TModel> html, string headerTemplateName, string rowTemplateName, string rowActionHtml)
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			var itemsProperty = html.ViewData.ModelMetadata.Properties.SingleOrDefault(p => p.PropertyName == "Items");
			IEnumerable listSource = null;
			if (itemsProperty != null)
				listSource = itemsProperty.Model as IEnumerable;
			if (listSource == null)
				return "ViewModel下不存在名称为Items并实现IEnumerable的属性";
			foreach (var item in listSource)
			{
				if (i++ == 0)
					sb.Append(html.DisplayFor(e => item, headerTemplateName));
				sb.Append(html.DisplayFor(e => item, rowTemplateName, new { RowActionHtml = rowActionHtml }));
			}
			return sb.ToString();
		}

	}
}
