using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TYsoft.Infrastructure.Mvc
{
	public interface IPager
	{
		/// <summary>
		/// 每页显示条数
		/// </summary>
		int PageSize { get; set; }
		
		/// <summary>
		/// 当前页码
		/// </summary>
		int PageIndex { get; set; }
		
		/// <summary>
		/// 总条数
		/// </summary>
		int TotalCount { get; set; }

		/// <summary>
		/// 总页数
		/// </summary>
		int TotalPage { get; }

		/// <summary>
		/// 显示页数 其余用...代替
		/// </summary>
		int ShowPage { get; set; }
		bool HasPreviousPage { get; }
		bool HasNextPage { get; }

		/// <summary>
		/// 当一个页面有多个分页的时候 提供url参数的标识前缀
		/// </summary>
		string UniquePrefix {get;set;}

		bool IsAjax { get; set; }
		string AjaxContainerId { get; set; }
	}
}
