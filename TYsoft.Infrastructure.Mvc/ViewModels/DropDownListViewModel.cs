using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TYsoft.Infrastructure.Mvc
{
	public class DropDownListViewModel<T> where T : class
	{
		public DropDownListViewModel(List<T> items, int? selectedId, string controlName, Converter<T, SelectListItem> converter)
			: this(items, selectedId, controlName, converter, string.Empty)
		{ }

		public DropDownListViewModel(List<T> items, int? selectedId, string controlName, Converter<T, SelectListItem> converter, string controlNamePrefix)
			: this(items, selectedId, controlName, converter, controlNamePrefix, false)
		{ }

		public DropDownListViewModel(List<T> items, int? selectedId, string controlName, Converter<T, SelectListItem> converter, string controlNamePrefix, bool allowEmpty)
		{
			Items = items;
			SelectedId = selectedId;
			ControlNamePrefix = controlNamePrefix;
			AllowEmpty = allowEmpty;
			ControlName = controlName;
			Converter = converter;
		}

		public List<T> Items { get; set; }
		public int? SelectedId { get; set; }
		public string ControlNamePrefix { get; set; }
		public bool AllowEmpty { get; set; }

		public Converter<T, SelectListItem> Converter { get; set; }

		public IEnumerable<SelectListItem> SelectList
		{
			get
			{
				var result = Items.ConvertAll<SelectListItem>(Converter);
				if (AllowEmpty)
					result.Insert(0, new SelectListItem() { Text = "", Value = "" });
				return result;
			}
		}

		private string _controlName;

		public string ControlName
		{
			get { return string.IsNullOrWhiteSpace(ControlNamePrefix) ? _controlName : (ControlNamePrefix + "." + _controlName); }
			set { _controlName = value; }
		}

	}
}
