using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TYsoft.Infrastructure.Mvc.Paging
{
	/// <summary>
	/// 包含一个已分页列表数据的ViewModel
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PagedListViewModel<T, U>
	{
		public PagedList<T> Items { get; set; }
		public U SearchCriteria { get; set; }
	}
}
