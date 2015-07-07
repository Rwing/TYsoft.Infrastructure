using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.DemoConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			var container = new LightInject.ServiceContainer();
			container.Register<IFoo, Foo>(new PerContainerLifetime());

			var a = container.GetInstance<IFoo>();
			Console.WriteLine(a.GetHashCode());
			var b = container.GetInstance<IFoo>();
			Console.WriteLine(b.GetHashCode());
			Console.WriteLine("done");
			Console.Read();
		}
	}

	public interface IFoo
	{
		DateTime GetDateTime();
	}

	public class Foo : IFoo
	{
		public DateTime Date{ get; set; }

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
