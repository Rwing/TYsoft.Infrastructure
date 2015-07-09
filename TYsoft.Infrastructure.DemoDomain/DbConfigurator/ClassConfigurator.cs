using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TYsoft.Infrastructure.Data.EF;
using TYsoft.Infrastructure.Domain;

namespace TYsoft.Infrastructure.DemoDomain
{
	public class ClassConfigurator : EntityTypeConfiguration<Class>, IDomainConfigurator
	{
		public ClassConfigurator()
		{
			this.HasKey(i => i.Id);
			this.HasMany(a => a.Students).WithRequired(b => b.Class).HasForeignKey(b => b.ClassId);
		}

		public void AddTo(ConfigurationRegistrar registrar)
		{
			registrar.Add(this);
		}
	}
}
