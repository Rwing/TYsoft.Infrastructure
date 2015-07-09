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
	public class StudentConfigurator : EntityTypeConfiguration<Student>, IDomainConfigurator
	{
		public StudentConfigurator()
		{
			this.HasKey(i => i.Id);
			this.Property(i => i.Name).HasMaxLength(50);
		}

		public void AddTo(ConfigurationRegistrar registrar)
		{
			registrar.Add(this);
		}
	}
}
