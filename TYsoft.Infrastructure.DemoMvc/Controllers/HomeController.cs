using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TYsoft.Infrastructure.Bussiness;
using TYsoft.Infrastructure.Domain;

namespace TYsoft.Infrastructure.DemoMvc.Controllers
{
	public class HomeController : Controller
	{

		public StudentService StudentService { get; set; }

		public ActionResult Index()
		{
			var student = StudentService.GetFirstStudentsWithClass();
			return Content(string.Format("name:{0}, class:{1}", student.Name, student.Class.Title));
		}

		public ActionResult Add()
		{
			var student = new Student()
			{
				Name = "lisi",
				Class = new Class()
				{
					Title = "3nian2ban"
				}
			};
			StudentService.Add(student);
			return Content("done");
		}

		public ActionResult Modify()
		{
			var result = StudentService.Test();
			int a = 1 + result[0].classid;
			var name = result[0].name + "wwo";
			return View();
		}
	}
}