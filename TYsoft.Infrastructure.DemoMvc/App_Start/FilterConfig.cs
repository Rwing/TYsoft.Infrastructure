﻿using System.Web;
using System.Web.Mvc;

namespace TYsoft.Infrastructure.DemoMvc
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
