using System;
using System.Collections.Generic;
using System.Configuration;
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
		public LightInjectContainer()
			: base(new ContainerOptions { EnableVariance = false })
		{

		}

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
			var assembieInConfig = ConfigurationManager.AppSettings["Infrastructure.RegisterAssemblies"];
			if (!string.IsNullOrWhiteSpace(assembieInConfig))
			{
				foreach (var assembly in assembieInConfig.Split(','))
				{
					RegisterAssembly(assembly);
				}
			}
			Register<IDbContext, EFDbContext>(new PerScopeLifetime());
			Register(typeof(IRepository<>), typeof(EFRepository<>), new PerScopeLifetime());
			this.EnableMvc();
			//var currentMvcAssembly = Assembly.Load(System.Web.Hosting.HostingEnvironment.SiteName);
			//this.RegisterControllers(currentMvcAssembly);
			var currentMvcAssembly = GetCurrentMvcAssembly();
			if (currentMvcAssembly!=null)
			{
				this.RegisterControllers(currentMvcAssembly);
			}
			
		}

		private Assembly GetCurrentMvcAssembly()
		{
			var serviceType = typeof(System.Web.HttpApplication);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var mvc = assemblies
				.SelectMany(s => s.GetTypes())
				.Where(t => serviceType.IsAssignableFrom(t) && !t.AssemblyQualifiedName.StartsWith("System"))
				.Select(t => t.Assembly);
			return mvc.FirstOrDefault();

		}

		private static IEnumerable<Assembly> GetAssemblies()
		{
			var referencedPaths = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "bin", "*.dll");
			foreach (var path in referencedPaths)
			{
				yield return AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path));
			}
			//GetAssemblies获取到的程序集有时候少...不知道为什么....
			//return AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.FullName.StartsWith("System") && !i.FullName.StartsWith("Microsoft"));
		}
	}
}
