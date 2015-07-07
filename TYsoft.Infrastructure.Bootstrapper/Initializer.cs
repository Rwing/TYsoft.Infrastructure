using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bootstrap.Extensions.StartupTasks;

[assembly: PreApplicationStartMethod(typeof(TYsoft.Infrastructure.Bootstrapper.Initializer), "Initialize")]
namespace TYsoft.Infrastructure.Bootstrapper
{
	public static class Initializer
	{
		static Initializer()
		{

		}

		public static void Initialize()
		{
			Bootstrap.Bootstrapper.With.StartupTasks()
			   .UsingThisExecutionOrder(s => s.First<RegisterIoC>())
			   .Start();
			//Bootstrap.Bootstrapper.With.SimpleInjector().UsingAutoRegistration().LookForTypesIn.ReferencedAssemblies().Start();
		}
	}
}
