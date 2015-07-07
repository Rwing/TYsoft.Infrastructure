using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TYsoft.Infrastructure.DemoMvc.Controllers
{
	public class HomeController : Controller
	{
		public IFoo A { get; set; }
		public IFoo B { get; set; }

		public ActionResult Index()
		{

			return Content("A:"+A.GetHashCode()+", B:"+ B.GetHashCode());
		}

	}
}