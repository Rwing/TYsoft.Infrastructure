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
			container.Register<IFoo, Foo2>(new PerContainerLifetime());
			//container.Register(typeof(IFoo<>), typeof(Foo<>));
			var a = container.GetInstance<IFoo>();
			Console.WriteLine(a.GetHashCode());
			var b = container.GetInstance<IFoo>();
			Console.WriteLine(b.GetHashCode());
			Console.WriteLine("done");
			Console.Read();
		}
	}

	public interface IFoo<T> { };
	public class Foo<T> : IFoo<T> { };

	public interface IFoo
	{
	}

	public class Foo : IFoo
	{
	}

	public class Foo2 : IFoo
	{
	}
}
