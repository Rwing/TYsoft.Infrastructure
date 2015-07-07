using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TYsoft.Infrastructure.Data.EF;

namespace TYsoft.Infrastructure.DemoMvc.Controllers
{
	public class HomeController : Controller
	{
		public IDbContext DbContext { get; set; }
		public IRepository<Item> Repository { get; set; }

		public ActionResult Index()
		{
			var count = Repository.GetItems().Count();
			return Content("sasdf" + count);
		}

	}


	public class Item
	{
		public int Id { get; set; }
		public string Name { get; set; }

	}
}