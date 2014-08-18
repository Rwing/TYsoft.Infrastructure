using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using TYsoft.Infrastructure.Utility;
using Microsoft.Web.Mvc;
using System.Web;
using System.Text.RegularExpressions;

namespace TYsoft.Infrastructure.Mvc
{
	//public static class HtmlHelperDynamic
	//{
	//	public static MvcHtmlString Hidden(this HtmlHelper helper, string name, dynamic value)
	//	{
	//		return helper.Hidden(name, (object)value);
	//	}
	//}

	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString DisplayDivision(this HtmlHelper helper, decimal divisor, decimal dividend)
		{
			return helper.DisplayDivision(divisor, dividend, false);
		}

		public static MvcHtmlString DisplayDivision(this HtmlHelper helper, decimal divisor, decimal dividend, bool isPercent)
		{
			if (isPercent) divisor *= 100;
			var result = dividend == 0 ? 0 : divisor / dividend;
			var template = dividend == 0 ? "<del style=\"color:#999\">{0:F2}</del>" : "{0:F2}";
			return MvcHtmlString.Create(string.Format(template, result.ToOneDigit()));
		}

		#region Action strong-type extension

		public static MvcHtmlString Action<TController>(this HtmlHelper helper, Expression<Action<TController>> action) where TController : Controller
		{
			var routeValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
			if (routeValues["action"].IsNull() || routeValues["controller"].IsNull())
				throw new ArgumentException("action or controller is null");
			return helper.Action(routeValues["action"].ToString(), routeValues["controller"].ToString(), routeValues);
		}

		#endregion

		#region readonly & disable

		public static MvcHtmlString ReadOnlyFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
		{
			return htmlHelper.TextBoxFor(expression, new { @readonly = "readonly", @class = "ro" });
		}

		public static MvcHtmlString ReadOnly(this HtmlHelper htmlHelper, string name, object value)
		{
			return htmlHelper.TextBox(name, value, new { @readonly = "readonly", @class = "ro" });
		}

		public static MvcHtmlString DisabledFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
		{
			return htmlHelper.TextBoxFor(expression, new { disabled = "disabled" });
		}

		#endregion

		#region TextArea

		public static MvcHtmlString XhEditor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression) where TModel : class
		{
			return htmlHelper.TextAreaFor(expression, new { @class = "xheditor {width:'800px',height:'300px',forcePtag:false, upLinkUrl:'!/AjaxHandle/EditorUpload', upLinkExt:'zip,rar,txt,doc,docx', upImgUrl:'!/AjaxHandle/EditorUpload', upImgExt:'jpg,jpeg,gif,png', html5Upload:false, localUrlTest:/^http/i, remoteImgSaveUrl:'/AjaxHandle/EditorRemoteUpload'}" });
		}

		#endregion

		#region RadioButtonWithLabel

		public static MvcHtmlString RadioButtonWithLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, string labelText)
		{
			return htmlHelper.RadioButtonWithLabelFor(expression, value, labelText, null);
		}

		public static MvcHtmlString RadioButtonWithLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, string labelText, object htmlAttributes)
		{
			var name = ExpressionHelper.GetExpressionText(expression);
			ModelMetadata modelMetadata = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
			var model = modelMetadata.Model;
			string b = Convert.ToString(value, CultureInfo.CurrentCulture);
			bool isChecked = model != null && string.Equals(model.ToString(), b, StringComparison.OrdinalIgnoreCase);
			if (model.IsNull() && string.Equals(b, "true", StringComparison.OrdinalIgnoreCase))
				isChecked = true;
			return htmlHelper.RadioButtonWithLabel(name, value, isChecked, labelText, htmlAttributes);
		}

		public static MvcHtmlString RadioButtonWithLabel(this HtmlHelper htmlHelper, string name, object value, bool isChecked, string labelText)
		{
			return htmlHelper.RadioButtonWithLabel(name, value, isChecked, labelText, null);
		}

		public static MvcHtmlString RadioButtonWithLabel(this HtmlHelper htmlHelper, string name, object value, bool isChecked, string labelText, object htmlAttributes)
		{
			return htmlHelper.RadioButtonWithLabel(name, value, isChecked, labelText, new RouteValueDictionary(htmlAttributes));
		}

		public static MvcHtmlString RadioButtonWithLabel(this HtmlHelper htmlHelper, string name, object value, bool isChecked, string labelText, IDictionary<string, object> htmlAttributes)
		{
			var fullId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(value.ToString());
			var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
			StringBuilder inputItemBuilder = new StringBuilder();
			TagBuilder builder = new TagBuilder("input");
			builder.MergeAttributes(htmlAttributes);
			builder.MergeAttribute("type", "radio");
			builder.MergeAttribute("value", value.ToString());
			builder.MergeAttribute("name", fullName, true);
			builder.MergeAttribute("id", fullId, true);
			if (isChecked)
				builder.MergeAttribute("checked", "checked");
			builder.GenerateId(name);
			inputItemBuilder.Append(builder.ToString(TagRenderMode.SelfClosing));
			builder = new TagBuilder("label");
			builder.MergeAttribute("for", fullId);
			builder.SetInnerText(labelText);
			inputItemBuilder.Append(builder.ToString(TagRenderMode.Normal));
			return MvcHtmlString.Create(inputItemBuilder.ToString());
		}

		#endregion

		#region CheckBoxWithLabel

		public static MvcHtmlString CheckBoxWithLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string labelText) where TModel : class
		{
			return htmlHelper.CheckBoxWithLabelFor(expression, false, labelText);
		}

		public static MvcHtmlString CheckBoxWithLabelFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isChecked, string labelText) where TModel : class
		{
			return htmlHelper.CheckBoxWithLabel(ExpressionHelper.GetExpressionText(expression), isChecked, labelText);
		}

		public static MvcHtmlString CheckBoxWithLabel(this HtmlHelper htmlHelper, string name, string labelText)
		{
			return htmlHelper.CheckBoxWithLabel(name, false, labelText);
		}

		public static MvcHtmlString CheckBoxWithLabel(this HtmlHelper htmlHelper, string name, bool isChecked, string labelText)
		{
			return htmlHelper.CheckBoxWithLabel(name, isChecked, labelText, (object)null);
		}

		/// <summary>
		/// 生成一个type为check的input标签以及label标签 注:htmlAttributes为check的属性
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="name"></param>
		/// <param name="isChecked"></param>
		/// <param name="labelText"></param>
		/// <param name="htmlAttributes">Checkbox的html属性</param>
		/// <returns></returns>
		public static MvcHtmlString CheckBoxWithLabel(this HtmlHelper htmlHelper, string name, bool isChecked, string labelText, object htmlAttributes)
		{
			return htmlHelper.CheckBoxWithLabel(name, isChecked, labelText, new RouteValueDictionary(htmlAttributes));
		}

		public static MvcHtmlString CheckBoxWithLabel(this HtmlHelper htmlHelper, string name, bool isChecked, string labelText, IDictionary<string, object> htmlAttributes)
		{
			StringBuilder inputItemBuilder = new StringBuilder();
			TagBuilder builder = new TagBuilder("input");
			builder.MergeAttributes(htmlAttributes);
			builder.MergeAttribute("type", "checkbox");
			builder.MergeAttribute("name", name, true);
			builder.MergeAttribute("id", name, true);
			if (isChecked)
				builder.MergeAttribute("checked", "checked");
			builder.GenerateId(name);
			inputItemBuilder.Append(builder.ToString(TagRenderMode.SelfClosing));
			builder = new TagBuilder("label");
			builder.MergeAttribute("for", name.Replace('.', '_'));
			builder.SetInnerText(labelText);
			inputItemBuilder.Append(builder.ToString(TagRenderMode.Normal));
			return MvcHtmlString.Create(inputItemBuilder.ToString());
		}

		#endregion

		#region ValidationMessage



		#endregion

		public static MvcForm BeginForm(this HtmlHelper htmlHelper, string className)
		{
			string rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
			TagBuilder tagBuilder = new TagBuilder("form");
			tagBuilder.MergeAttribute("class", className);
			tagBuilder.MergeAttribute("action", rawUrl);
			tagBuilder.MergeAttribute("method", HtmlHelper.GetFormMethodString(FormMethod.Post), true);
			bool flag = htmlHelper.ViewContext.ClientValidationEnabled && !htmlHelper.ViewContext.UnobtrusiveJavaScriptEnabled;
			if (flag)
			{
				//tagBuilder.GenerateId(htmlHelper.ViewContext.FormIdGenerator());
			}
			htmlHelper.ViewContext.Writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));
			MvcForm result = new MvcForm(htmlHelper.ViewContext);
			if (flag)
			{
				htmlHelper.ViewContext.FormContext.FormId = tagBuilder.Attributes["id"];
			}
			return result;
		}

		//public static readonly string TEMPLATE_NEWS = "<a title=\"{3}\" href=\"{0}\"{2}>{1}</a>";

		//public static MvcHtmlString FormatNews(this HtmlHelper htmlHelper, Niu99.Platform.Domain.News news)
		//{
		//	return htmlHelper.FormatNews(news, 200);
		//}

		//public static MvcHtmlString FormatNews(this HtmlHelper htmlHelper, Niu99.Platform.Domain.News news, int truncateLength)
		//{
		//	//var url = news.ExternalLink.IsNullOrWhiteSpace() ? ("/News/" + news.NewsId) : news.ExternalLink;
		//	//var titleColor = news.Color.IsNullOrWhiteSpace() ? (news.ShowDate.AddHours(36) > DateTime.Now ? " style=\"color:red\"" : "") : (" style=\"color:#" + news.Color + "\"");
		//	//return new MvcHtmlString(string.Format(TEMPLATE_NEWS, url, news.Title.Truncate(truncateLength), titleColor));
		//	return htmlHelper.FormatNews(news, new Domain.NewsFormatSetting() { TruncateLength = truncateLength });
		//}

		//public static MvcHtmlString FormatNews(this HtmlHelper htmlHelper, Niu99.Platform.Domain.News news, Niu99.Platform.Domain.NewsFormatSetting setting)
		//{
		//	var baseUrl = setting.BaseUrl ?? "/News/";
		//	var url = news.ExternalLink.IsNullOrWhiteSpace() ? (baseUrl + news.NewsId) : news.ExternalLink;
		//	if (setting.TargetBlank || !news.ExternalLink.IsNullOrWhiteSpace())
		//		url += "\" target=\"_blank";
		//	if (setting.NoFollow)
		//		url += "\" rel=\"nofollow";
		//	var titleColor = news.Color.IsNullOrWhiteSpace() ? (setting.AutoRed && news.ShowDate.AddHours(36) > DateTime.Now ? " style=\"color:red\"" : "") : (" style=\"color:#" + news.Color + "\"");
		//	return new MvcHtmlString(string.Format(TEMPLATE_NEWS, url, news.Title.Truncate(setting.TruncateLength), titleColor, news.Title));
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="htmlHelper"></param>
		/// <param name="linkText"></param>
		/// <param name="actionName"></param>
		/// <param name="controllerName"></param>
		/// <param name="ignoreAction">忽略action,只比较controller</param>
		/// <returns></returns>
		public static MvcHtmlString MenuLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object htmlAttributes, bool ignoreAction = false)
		{
			var routeData = htmlHelper.ViewContext.IsChildAction ? htmlHelper.ViewContext.ParentActionViewContext.RouteData : htmlHelper.ViewContext.RouteData;
			string currentAction = routeData.GetRequiredString("action");
			string currentController = routeData.GetRequiredString("controller");
			var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
			if ((ignoreAction || actionName == currentAction) && controllerName == currentController)
			{
				attrs.Add("class", "current");
			}
			if (!attrs.ContainsKey("title"))
				attrs.Add("title", linkText);
			return htmlHelper.ActionLink(linkText, actionName, controllerName, null, attrs);
		}

		public static MvcHtmlString MenuLink<TController>(this HtmlHelper htmlHelper, string linkText, Expression<Action<TController>> action) where TController : Controller
		{
			//增加新方法来判断是否导航在当前页
			//使用url来判断
			var targetUrl = htmlHelper.BuildUrlFromExpression(action);
			var currentUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
			bool isCurrent = currentUrl.Equals(targetUrl, StringComparison.OrdinalIgnoreCase);
			return new MvcHtmlString("<a href=\"" + targetUrl + "\"" + (isCurrent ? " class=\"current\"" : "") + ">" + linkText + "</a>");
		}

		public static MvcHtmlString MenuLink<TController>(this HtmlHelper htmlHelper, string linkText, Expression<Action<TController>> action, bool ignoreAction, object htmlAttributes = null) where TController : Controller
		{
			var routeValues = Microsoft.Web.Mvc.Internal.ExpressionHelper.GetRouteValuesFromExpression(action);
			if (routeValues["action"].IsNull() || routeValues["controller"].IsNull())
				throw new ArgumentException("action or controller is null");
			return htmlHelper.MenuLink(linkText, routeValues["action"].ToString(), routeValues["controller"].ToString(), htmlAttributes, ignoreAction);
		}

		public static MvcHtmlString ActionLinkWithImage(this HtmlHelper html, string imgSrc, string actionName)
		{
			var urlHelper = new UrlHelper(html.ViewContext.RequestContext);

			string imgUrl = urlHelper.Content(imgSrc);
			TagBuilder imgTagBuilder = new TagBuilder("img");
			imgTagBuilder.MergeAttribute("src", imgUrl);
			imgTagBuilder.MergeAttribute("border", "0");
			string img = imgTagBuilder.ToString(TagRenderMode.SelfClosing);

			string url = urlHelper.Action(actionName);

			TagBuilder tagBuilder = new TagBuilder("a")
			{
				InnerHtml = img
			};
			tagBuilder.MergeAttribute("href", url);

			return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
		}

		public static MvcHtmlString ActionLinkWithImage(this HtmlHelper html, string imgSrc, string actionName, string controllerName, object routeValue = null)
		{
			var urlHelper = new UrlHelper(html.ViewContext.RequestContext);

			string imgUrl = urlHelper.Content(imgSrc);
			TagBuilder imgTagBuilder = new TagBuilder("img");
			imgTagBuilder.MergeAttribute("src", imgUrl);
			imgTagBuilder.MergeAttribute("border", "0");
			string img = imgTagBuilder.ToString(TagRenderMode.SelfClosing);

			string url = urlHelper.Action(actionName, controllerName, routeValue);

			TagBuilder tagBuilder = new TagBuilder("a")
			{
				InnerHtml = img
			};
			tagBuilder.MergeAttribute("href", url);

			return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
		}

		public static string SanitizeHtml(this HtmlHelper html, string source)
		{
			//string acceptable = "script|link|title";
			//string stringPattern = @"</?(?(?=" + acceptable + @")notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
			//return Regex.Replace(source, stringPattern, "");
			if(string.IsNullOrWhiteSpace(source)) return string.Empty;
			return Regex.Replace(source, @"<[^>]+>", "");
		}

		/// <summary>
		/// 把文本中的\r\n替换成br
		/// </summary>
		/// <param name="html"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IHtmlString ReplaceRN(this HtmlHelper html, string source)
		{
			return html.Raw(source.IsNullOrWhiteSpace() ? string.Empty : source.Replace("\r\n", "<br/>"));
		}
	}
}