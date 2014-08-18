using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Data.EF
{
	public interface IDomainConfigurator
	{
		void AddTo(ConfigurationRegistrar registrar);
	}
}
