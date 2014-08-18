using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Utility
{
	public static class DynamicExpressionExtensions
	{
		public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool desc) where TEntity : class
		{
			string command = desc ? "OrderByDescending" : "OrderBy";
			var type = typeof(TEntity);
			var property = type.GetProperty(orderByProperty);
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			var orderByExpression = Expression.Lambda(propertyAccess, parameter);
			var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
										  source.Expression, Expression.Quote(orderByExpression));
			return source.Provider.CreateQuery<TEntity>(resultExpression);
		}

		public static IQueryable<TResult> Select<TEntity, TResult>(this IQueryable<TEntity> source, string selectField) where TEntity : class
		{
			var type = typeof(TEntity);
			var property = type.GetProperty(selectField);
			//var fieldType = property.DeclaringType;
			var parameter = Expression.Parameter(type, "p");
			var propertyAccess = Expression.MakeMemberAccess(parameter, property);
			var orderByExpression = Expression.Lambda(propertyAccess, parameter);
			var resultExpression = Expression.Call(typeof(Queryable), "select", new Type[] { type, property.PropertyType },
										  source.Expression, Expression.Quote(orderByExpression));
			return source.Provider.CreateQuery<TResult>(resultExpression);
		}
	}
}
