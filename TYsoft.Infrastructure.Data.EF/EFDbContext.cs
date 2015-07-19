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
			: this("EFDbContext") { }

		public EFDbContext(string nameOrConnectionString)
			: base(nameOrConnectionString)
		{
			//这句是为了引用一下Entityframework.SqlServer.dll，不然不会copy到bin目录，貌似是EF的bug
			bool instanceExists = System.Data.Entity.SqlServer.SqlProviderServices.Instance != null;
		}

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
