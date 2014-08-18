using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TYsoft.Infrastructure.Mvc
{
	/// <summary>
	/// 分页器 标识分页属性
	/// </summary>
	public class Pager : IPager
	{
		#region IPager Members

		public int PageSize { get; set; }

		public int PageIndex { get; set; }

		public int TotalCount { get; set; }

		public int TotalPage { get { return (int)Math.Ceiling(this.TotalCount / (double)this.PageSize); } }

		public bool HasPreviousPage { get { return PageIndex > 1; } }

		public bool HasNextPage { get { return PageIndex < TotalPage; } }

		public bool IsAjax { get; set; }

		public string AjaxContainerId { get; set; }

		public string UniquePrefix { get; set; }

		private int _showPage;

		public int ShowPage
		{
			get { return _showPage <= 0 ? 7 : _showPage; }
			set { _showPage = value; }
		}

		#endregion

		public Pager()
		{ }

		public Pager(int pageIndex, int pageSize)
			: this(0, pageIndex, pageSize, false, null)
		{ }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="index"></param>
		/// <param name="pageSize"></param>
		public Pager(int count, int pageIndex, int pageSize)
			: this(count, pageIndex, pageSize, false, null)
		{ }

		public Pager(int count, int pageIndex, int pageSize, bool isAjax)
			: this(count, pageIndex, pageSize, isAjax, null)
		{ }

		public Pager(int count, int pageIndex, int pageSize, bool isAjax, string ajaxContainerId)
			: this(count, pageIndex, pageSize, isAjax, ajaxContainerId, null)
		{
		}

		public Pager(int count, int pageIndex, int pageSize, bool isAjax, string ajaxContainerId, string uniquePrefix)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
			this.TotalCount = count;
			this.IsAjax = isAjax;
			this.AjaxContainerId = ajaxContainerId;
			this.UniquePrefix = uniquePrefix;
		}
	}
}
