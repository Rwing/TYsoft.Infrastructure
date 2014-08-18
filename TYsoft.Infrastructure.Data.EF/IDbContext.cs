using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Data.EF
{
	public interface IDbContext
	{
		Database Database { get; }
		DbEntityEntry Entry(object entity);
		DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
		int SaveChanges();
		DbSet<TEntity> Set<TEntity>() where TEntity : class;
		DbSet Set(Type entityType);
	}
}
