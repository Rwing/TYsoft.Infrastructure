using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Data.EF
{
	public class EFRepository<TEntity> : IRepository<TEntity> where TEntity : class, new()
	{
		//#region member

		//private DbContext _dataContext;
		//private IDbSet<TEntity> _dbSet;

		//#endregion

		//#region Constructor

		//public EFRepository(DbContext dataContext)
		//{

		//	_dataContext = dataContext;
		//	_dbSet = dataContext.Set<TEntity>();
		//}

		//#endregion

		public IDbContext DbContext { get; set; }


		#region IRepository<TEntity> Members

		public void Insert(TEntity item)
		{
			DbContext.Set<TEntity>().Add(item);
		}

		public void Delete(TEntity item)
		{
			DbContext.Set<TEntity>().Remove(item);
		}

		public void Delete(Expression<Func<TEntity, bool>> predicate)
		{

		}

		public void Update(TEntity item)
		{
			if (DbContext.Entry(item).State != EntityState.Modified)
			{
				Attach(item);
				DbContext.Entry(item).State = EntityState.Modified;
			}
		}

		public void Attach(TEntity item)
		{
			DbContext.Set<TEntity>().Attach(item);
		}

		public IQueryable<TEntity> GetItems(params string[] paths)
		{
			var dbSet = DbContext.Set<TEntity>() as IQueryable<TEntity>;
			if (paths != null)
				foreach (var path in paths)
					dbSet = dbSet.Include(path);
			return dbSet;
		}

		public IQueryable<TEntity> GetItems(params Expression<Func<TEntity, object>>[] paths)
		{
			var dbSet = DbContext.Set<TEntity>() as IQueryable<TEntity>;
			if (paths != null)
				foreach (var path in paths)
					dbSet = dbSet.Include(path);
			return dbSet;
		}

		public IQueryable<TEntity> GetItemsByPredicate(Expression<Func<TEntity, bool>> predicate, params string[] paths)
		{
			return GetItems(paths).Where(predicate);
		}

		public IQueryable<TEntity> GetItemsByPredicate(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] paths)
		{
			return GetItems(paths).Where(predicate);
		}

		public TEntity GetItemByPredicate(Expression<Func<TEntity, bool>> predicate, params string[] paths)
		{
			return this.GetItemsByPredicate(predicate, paths).FirstOrDefault();
		}

		public TEntity GetItemByPredicate(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] paths)
		{
			return this.GetItemsByPredicate(predicate, paths).FirstOrDefault();
		}

		public IEnumerable<TEntity> ExecuteSqlQuery(string sql, params object[] parameters)
		{
			return DbContext.Database.SqlQuery<TEntity>(sql, parameters);
		}

		public IEnumerable<T> ExecuteSqlQuery<T>(string sql, params object[] parameters)
		{
			return DbContext.Database.SqlQuery<T>(sql, parameters);
		}

		public int Save()
		{
			try
			{
				int result = DbContext.SaveChanges();
				return result;
			}
			catch (DbUpdateException e)
			{
				if (e.InnerException != null && e.InnerException.InnerException is SqlException)
				{
					SqlException sqlEx = e.InnerException.InnerException as SqlException;
					throw sqlEx;
				}
				throw e;
			}
		}

		#endregion
	}
}
