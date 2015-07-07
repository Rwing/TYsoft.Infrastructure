using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.IoC
{
	public class IoCValidatorFactory : ValidatorFactoryBase
	{
		public IIoCContainer DIContainer { get; set; }

		public IoCValidatorFactory(IIoCContainer container)
		{
			this.DIContainer = container;
		}

		public override IValidator CreateInstance(Type validatorType)
		{
			return DIContainer.TryResolve(validatorType) as IValidator;
		}
	}
}
