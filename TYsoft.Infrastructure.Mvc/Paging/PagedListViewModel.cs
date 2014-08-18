using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Mvc.Paging
{
	/// <summary>
	/// 包含一个已分页列表数据的ViewModel
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PagedListViewModel<T>
	{
		public PagedListViewModel()
		{

		}

		public PagedListViewModel(PagedList<T> items)
		{
			Items = items;
		}

		public PagedListViewModel(PagedList<T> items, string keyword)
		{
			Items = items;
			Keyword = keyword;
		}


		public PagedList<T> Items { get; set; }

		private string _keyword;

		public string Keyword
		{
			get { return string.IsNullOrWhiteSpace(_keyword) ? string.Empty : _keyword; }
			set { _keyword = value; }
		}
	}
}
