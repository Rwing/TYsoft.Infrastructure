using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Bootstrap.Extensions.StartupTasks;
using System.IO;
using System.Reflection;
using System.Web.Compilation;

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

			var assembly = typeof(Initializer).Assembly;
			var references = assembly.GetReferencedAssemblies();
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
			var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

			var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory+"bin", "*.dll");
			var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
			toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));
			//Bootstrap.Bootstrapper.With.SimpleInjector().UsingAutoRegistration().LookForTypesIn.ReferencedAssemblies().Start();
		}

		
	}
}
