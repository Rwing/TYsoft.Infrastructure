using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Data.EF
{
	public interface IRepository<TEntity> where TEntity : class, new()
	{
		/// <summary>
		/// 向数据库中插入数据
		/// </summary>
		/// <param name="item"></param>
		void Insert(TEntity item);

		/// <summary>
		/// 从数据库中删除数据
		/// </summary>
		/// <param name="item"></param>
		void Delete(TEntity item);


		/// <summary>
		/// Deletes the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		void Delete(Expression<Func<TEntity, bool>> predicate);

		/// <summary>
		/// 更新数据库中数据
		/// </summary>
		/// <param name="item"></param>
		void Update(TEntity item);

		void Attach(TEntity item);

		/// <summary>
		/// 返回所有数据
		/// </summary>
		/// <returns></returns>
		IQueryable<TEntity> GetItems(params string[] paths);

		/// <summary>
		/// path为强类型的GetItems
		/// </summary>
		/// <param name="paths"></param>
		/// <returns></returns>
		IQueryable<TEntity> GetItems(params Expression<Func<TEntity, object>>[] paths);

		/// <summary>
		/// 根据where条件返回数据
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		IQueryable<TEntity> GetItemsByPredicate(Expression<Func<TEntity, bool>> predicate, params string[] paths);

		/// <summary>
		/// path为强类型的GetItemsByPredicate
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="paths"></param>
		/// <returns></returns>
		IQueryable<TEntity> GetItemsByPredicate(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] paths);

		/// <summary>
		/// 同GetItemsByPredicate(predicate).SingleOrDefault();
		/// </summary>
		/// <param name="predicate"></param>
		/// <returns></returns>
		TEntity GetItemByPredicate(Expression<Func<TEntity, bool>> predicate, params string[] paths);

		/// <summary>
		/// path为强类型的GetItemByPredicate
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="paths"></param>
		/// <returns></returns>
		TEntity GetItemByPredicate(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] paths);

		/// <summary>
		/// 执行sql语句
		/// </summary>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IEnumerable<TEntity> ExecuteSqlQuery(string sql, params object[] parameters);

		/// <summary>
		/// 执行sql语句
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sql"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		IEnumerable<T> ExecuteSqlQuery<T>(string sql, params object[] parameters);

		/// <summary>
		/// 保存至数据库 返回影响行数
		/// </summary>
		int Save();
	}
}
