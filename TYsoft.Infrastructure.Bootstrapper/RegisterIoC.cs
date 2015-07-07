using Bootstrap.Extensions.StartupTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace TYsoft.Infrastructure.Bootstrapper
{
	public class RegisterIoC : IStartupTask
	{
		public void Reset()
		{
			//throw new NotImplementedException();
		}

		public void Run()
		{

			var container = (ServiceContainer)IoCContainerManager.CurrentIoCContainer;
			//container.RegisterControllers();
			//container.RegisterAssembly("WA*.dll");
			foreach (Assembly assembly in container.AssemblyLoader.Load("WA*.dll"))
			{
				container.RegisterAssembly(assembly, () => new PerRequestLifeTime());
			}
			container.Register<IDbContext, EFDbContext>(new PerScopeLifetime());
			container.EnableMvc();
		}
	}
}
