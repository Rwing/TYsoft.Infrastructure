using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TYsoft.Infrastructure.Mvc
{
	/// <summary>
	/// 已分页数据
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PagedList<T> : List<T>
	{
		public IPager Pager { get; private set; }

		public PagedList()
			: this(new List<T>().AsQueryable(), 0, 0)
		{ }

		public PagedList(IQueryable<T> items, int pageIndex, int pageSize)
			: this(items, items.Count(), pageIndex, pageSize)
		{ }

		public PagedList(IQueryable<T> items, int pageIndex, int pageSize, bool isAjax)
			: this(items, items.Count(), pageIndex, pageSize, isAjax)
		{ }

		public PagedList(IQueryable<T> items, int pageIndex, int pageSize, bool isAjax, string ajaxContainerId)
			: this(items, items.Count(), pageIndex, pageSize, isAjax, ajaxContainerId)
		{ }

		public PagedList(IQueryable<T> items, int count, int pageIndex, int pageSize)
			: this(items, count, pageIndex, pageSize, false, null)
		{ }

		public PagedList(IQueryable<T> items, int count, int pageIndex, int pageSize, bool isAjax)
			: this(items, count, pageIndex, pageSize, isAjax, null)
		{ }

		public PagedList(IQueryable<T> items, int count, int pageIndex, int pageSize, bool isAjax, string ajaxContainerId)
			: this(items, new Pager(count, pageIndex, pageSize, isAjax, ajaxContainerId))
		{ }

		public PagedList(IQueryable<T> items, IPager pager)
		{
			if (pager.TotalCount == 0)
				pager.TotalCount = items.Count();
			if (pager.PageIndex == 0)
				pager.PageIndex = 1;
			if (pager.PageSize == 0)
				pager.PageSize = 10;
			this.Pager = pager;
			var filtered = items.Skip((Pager.PageIndex - 1) * Pager.PageSize).Take(Pager.PageSize);
			AddRange(filtered);
		}
	}
}
