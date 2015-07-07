using Bootstrap.Extensions.StartupTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TYsoft.Infrastructure.IoC;


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
			
			var container = IoCContainerManager.CurrentIoCContainer;
			container.Initialize();
		}
	}
}
