using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Data.EF
{
	public class EFDbContext : DbContext, IDbContext
	{
		public EFDbContext()
			: base("EFDbContext") { }

		public EFDbContext(string nameOrConnectionString)
			: base(nameOrConnectionString) { }

		public EFDbContext(DbConnection existingConnection)
			: base(existingConnection, true) { }

		public IEnumerable<IDomainConfigurator> DomainConfigurators { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			if (DomainConfigurators != null)
			{
				foreach (var configurator in DomainConfigurators)
				{
					configurator.AddTo(modelBuilder.Configurations);
				}
			}
		}
	}
}
