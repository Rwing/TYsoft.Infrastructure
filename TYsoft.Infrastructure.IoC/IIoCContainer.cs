using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.IoC
{
	public interface IIoCContainer
	{
		T Resolve<T>();
		T Resolve<T>(string name);
		object Resolve(Type type);
		object Resolve(Type type, string name);

		object TryResolve(Type type);
		object TryResolve(Type type, string name);
		T TryResolve<T>();
		T TryResolve<T>(string name);

		void Initialize();
	}
}
