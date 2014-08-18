using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TYsoft.Infrastructure.Data.EF
{
	public static class EFExtensions
	{
		public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, object>> selector) where TEntity : class
		{
			string path = new PropertyPathVisitor().GetPropertyPath(selector);
			return source.Include(path);
			//StringBuilder pathBuilder = new StringBuilder();

			//MemberExpression pro = path.Body as MemberExpression;
			//while (pro != null)
			//{
			//	//如: x=> x.Customer.CustomerAddress
			//	//path.Body是CustomerAddress
			//	//CustomerAddress的Expression是Customer
			//	//Customer的Expression是x
			//	pathBuilder.Insert(0, "." + pro.Member.Name);
			//	pro = pro.Expression as MemberExpression;
			//}
			//return source.Include(pathBuilder.ToString(1, pathBuilder.Length - 1));
		}

		class PropertyPathVisitor : ExpressionVisitor
		{
			private Stack<string> _stack;

			public string GetPropertyPath(Expression expression)
			{
				_stack = new Stack<string>();
				Visit(expression);
				return _stack
					.Aggregate(
						new StringBuilder(),
						(sb, name) =>
							(sb.Length > 0 ? sb.Append(".") : sb).Append(name))
					.ToString();
			}

			protected override Expression VisitMember(MemberExpression expression)
			{
				if (_stack != null)
					_stack.Push(expression.Member.Name);
				return base.VisitMember(expression);
			}

			protected override Expression VisitMethodCall(MethodCallExpression expression)
			{
				if (IsLinqOperator(expression.Method))
				{
					for (int i = 1; i < expression.Arguments.Count; i++)
					{
						Visit(expression.Arguments[i]);
					}
					Visit(expression.Arguments[0]);
					return expression;
				}
				return base.VisitMethodCall(expression);
			}

			private static bool IsLinqOperator(MethodInfo method)
			{
				if (method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable))
					return false;
				return Attribute.GetCustomAttribute(method, typeof(ExtensionAttribute)) != null;
			}
		}
	}
}
