using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.IoC
{
	public static class IoCContainerManager
	{
		private static IIoCContainer _container;

		static IoCContainerManager()
		{
			if(_container == null)
				InitContainer();
		}

		private static void InitContainer()
		{
			_container = new LightInjectContainer();
		}

		public static IIoCContainer CurrentIoCContainer
		{
			get
			{
				if(_container == null)
					InitContainer();
				return _container;
			}
		}
	}
}
