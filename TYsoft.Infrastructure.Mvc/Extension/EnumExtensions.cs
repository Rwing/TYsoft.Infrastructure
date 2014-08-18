using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace TYsoft.Infrastructure.Mvc
{
	public static class SelectListItemsExtensions
	{
		//cache
		public static readonly ConcurrentDictionary<string, List<SelectListItem>> _cache = new ConcurrentDictionary<string, List<SelectListItem>>();

		#region enum Extensions

		public static List<SelectListItem> ToSelectListItems<TEnum>(this TEnum source) where TEnum : struct
		{
			return source.ToSelectListItems(false);
		}

		public static List<SelectListItem> ToSelectListItems<TEnum>(this TEnum source, bool allowEmpty) where TEnum : struct
		{
			Type enumType = typeof(TEnum);
			List<SelectListItem> result;
			if (_cache.TryGetValue(enumType.Name + allowEmpty, out result))
				return result;
			result = (from TEnum e in Enum.GetValues(typeof(TEnum))
					  let atts = enumType.GetField(e.ToString()).GetCustomAttributes(false)
					  select new SelectListItem()
					  {
						  Text = atts.Count() > 0 && atts[0] is DescriptionAttribute ? ((DescriptionAttribute)atts[0]).Description : e.ToString(),
						  Value = Convert.ToInt32(e).ToString(),
						  Selected = source.Equals(e)
					  }).ToList();
			if (allowEmpty)
				result.Insert(0, new SelectListItem() { Value = "", Text = "" });
			_cache.AddOrUpdate(enumType.Name + allowEmpty, result, (k, v) => v);
			return result.ToList();
		}

		#endregion

		public static List<SelectListItem> ToSelectListItems(this bool? source)
		{
			return source.ToSelectListItems("是", "否");
		}

		public static List<SelectListItem> ToSelectListItems(this bool? source, string trueText, string falseText)
		{
			List<SelectListItem> result = new List<SelectListItem>();
			result.Add(new SelectListItem() { Selected = !source.HasValue, Text = "", Value = "" });
			result.Add(new SelectListItem() { Selected = source.HasValue && source.Value, Text = trueText, Value = "true" });
			result.Add(new SelectListItem() { Selected = source.HasValue && !source.Value, Text = falseText, Value = "false" });
			return result;
		}
	}
}
