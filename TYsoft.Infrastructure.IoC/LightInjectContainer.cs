using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TYsoft.Infrastructure.Data.EF;
using TYsoft.Infrastructure.IoC.LightInject;

namespace TYsoft.Infrastructure.IoC
{
	internal class LightInjectContainer : ServiceContainer, IIoCContainer
	{

		public T Resolve<T>()
		{
			return base.GetInstance<T>();
		}

		public T Resolve<T>(string name)
		{
			return base.GetInstance<T>(name);
		}

		public object Resolve(Type type)
		{
			return base.GetInstance(type);
		}

		public object Resolve(Type type, string name)
		{
			return base.GetInstance(type, name);
		}

		public T TryResolve<T>()
		{
			return base.TryGetInstance<T>();
		}

		public T TryResolve<T>(string name)
		{
			return base.TryGetInstance<T>(name);
		}

		public object TryResolve(Type type)
		{
			return base.TryGetInstance(type);
		}

		public object TryResolve(Type type, string name)
		{
			return base.TryGetInstance(type, name);
		}


		public void Initialize()
		{
			Register<IDbContext, EFDbContext>(new PerScopeLifetime());
			Register(typeof(IRepository<>), typeof(EFRepository<>), new PerScopeLifetime());
			this.EnableMvc();
			var currentMvcAssembly = Assembly.Load(System.Web.Hosting.HostingEnvironment.SiteName);
			this.RegisterControllers(currentMvcAssembly);
		}
	}
}
