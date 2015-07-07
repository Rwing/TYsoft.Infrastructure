using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using LightInject;

namespace TYsoft.Infrastructure.DemoMvc
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			var container = new ServiceContainer();
			container.RegisterControllers();
			//register other services
			container.Register<IFoo, Foo>(new PerScopeLifetime());

			container.EnableMvc();
		}
	}

	public interface IFoo
	{
		DateTime GetDateTime();
	}

	public class Foo : IFoo
	{
		public DateTime Date { get; set; }

		public Foo()
		{
			Date = DateTime.Now;
		}

		public DateTime GetDateTime()
		{
			return Date;
		}
	}
}
