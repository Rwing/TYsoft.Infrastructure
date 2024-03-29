﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Collections;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Reflection;

namespace TYsoft.Infrastructure.Utility
{
	///LinqToSql LinqTOEntity  构建查询语句类文件
	///用于其动态构建查询语句
	///但在查询句话前对像必须 使用asExpandable 如：(ObjectSet as IQueryable<TEntity>).AsExpandable();
	/// <summary>Refer to http://www.albahari.com/nutshell/linqkit.html and
	/// http://tomasp.net/blog/linq-expand.aspx for more information.</summary>

	public static class ExpressionTreeExtensions
	{
		public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query)
		{
			if (query is ExpandableQuery<T>) return (ExpandableQuery<T>)query;
			return new ExpandableQuery<T>(query);
		}

		public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr)
		{
			return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
		}

		public static Expression Expand(this Expression expr)
		{
			return new ExpressionExpander().Visit(expr);
		}

		public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr)
		{
			return expr.Compile().Invoke();
		}

		public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1)
		{
			return expr.Compile().Invoke(arg1);
		}

		public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2)
		{
			return expr.Compile().Invoke(arg1, arg2);
		}

		public static TResult Invoke<T1, T2, T3, TResult>(
			this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3)
		{
			return expr.Compile().Invoke(arg1, arg2, arg3);
		}

		public static TResult Invoke<T1, T2, T3, T4, TResult>(
			this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
		}
	}


	#region ExpandableQuery

	/// <summary>
	/// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
	/// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
	/// </summary>
	public class ExpandableQuery<T> : IQueryable<T>, IOrderedQueryable<T>, IOrderedQueryable
	{
		ExpandableQueryProvider<T> _provider;
		IQueryable<T> _inner;

		internal IQueryable<T> InnerQuery { get { return _inner; } }			// Original query, that we're wrapping

		internal ExpandableQuery(IQueryable<T> inner)
		{
			_inner = inner;
			_provider = new ExpandableQueryProvider<T>(this);
		}

		Expression IQueryable.Expression { get { return _inner.Expression; } }
		Type IQueryable.ElementType { get { return typeof(T); } }
		IQueryProvider IQueryable.Provider { get { return _provider; } }
		public IEnumerator<T> GetEnumerator() { return _inner.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return _inner.GetEnumerator(); }
		public override string ToString() { return _inner.ToString(); }
	}

	class ExpandableQueryProvider<T> : IQueryProvider
	{
		ExpandableQuery<T> _query;

		internal ExpandableQueryProvider(ExpandableQuery<T> query)
		{
			_query = query;
		}

		// The following four methods first call ExpressionExpander to visit the expression tree, then call
		// upon the inner query to do the remaining work.

		IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
		{
			return new ExpandableQuery<TElement>(_query.InnerQuery.Provider.CreateQuery<TElement>(expression.Expand()));
		}

		IQueryable IQueryProvider.CreateQuery(Expression expression)
		{
			return _query.InnerQuery.Provider.CreateQuery(expression.Expand());
		}

		TResult IQueryProvider.Execute<TResult>(Expression expression)
		{
			return _query.InnerQuery.Provider.Execute<TResult>(expression.Expand());
		}

		object IQueryProvider.Execute(Expression expression)
		{
			return _query.InnerQuery.Provider.Execute(expression.Expand());
		}
	}

	#endregion

	#region ExpressionExpander

	/// <summary>
	/// Custom expresssion visitor for ExpandableQuery. This expands calls to Expression.Compile() and
	/// collapses captured lambda references in subqueries which LINQ to SQL can't otherwise handle.
	/// </summary>
	class ExpressionExpander : ExpressionVisitor
	{
		// Replacement parameters - for when invoking a lambda expression.
		Dictionary<ParameterExpression, Expression> _replaceVars = null;

		internal ExpressionExpander() { }

		private ExpressionExpander(Dictionary<ParameterExpression, Expression> replaceVars)
		{
			_replaceVars = replaceVars;
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			if ((_replaceVars != null) && (_replaceVars.ContainsKey(p)))
				return _replaceVars[p];
			else
				return base.VisitParameter(p);
		}

		/// <summary>
		/// Flatten calls to Invoke so that Entity Framework can understand it. Calls to Invoke are generated
		/// by PredicateBuilder.
		/// </summary>
		protected override Expression VisitInvocation(InvocationExpression iv)
		{
			Expression target = iv.Expression;
			if (target is MemberExpression) target = TransformExpr((MemberExpression)target);
			if (target is ConstantExpression) target = ((ConstantExpression)target).Value as Expression;

			LambdaExpression lambda = (LambdaExpression)target;

			Dictionary<ParameterExpression, Expression> replaceVars;
			if (_replaceVars == null)
				replaceVars = new Dictionary<ParameterExpression, Expression>();
			else
				replaceVars = new Dictionary<ParameterExpression, Expression>(_replaceVars);

			try
			{
				for (int i = 0; i < lambda.Parameters.Count; i++)
					replaceVars.Add(lambda.Parameters[i], iv.Arguments[i]);
			}
			catch (ArgumentException ex)
			{
				throw new InvalidOperationException("Invoke cannot be called recursively - try using a temporary variable.", ex);
			}

			return new ExpressionExpander(replaceVars).Visit(lambda.Body);
		}

		protected override Expression VisitMethodCall(MethodCallExpression m)
		{
			if (m.Method.Name == "Invoke" && m.Method.DeclaringType == typeof(Extensions))
			{
				Expression target = m.Arguments[0];
				if (target is MemberExpression) target = TransformExpr((MemberExpression)target);
				if (target is ConstantExpression) target = ((ConstantExpression)target).Value as Expression;

				LambdaExpression lambda = (LambdaExpression)target;

				Dictionary<ParameterExpression, Expression> replaceVars;
				if (_replaceVars == null)
					replaceVars = new Dictionary<ParameterExpression, Expression>();
				else
					replaceVars = new Dictionary<ParameterExpression, Expression>(_replaceVars);

				try
				{
					for (int i = 0; i < lambda.Parameters.Count; i++)
						replaceVars.Add(lambda.Parameters[i], m.Arguments[i + 1]);
				}
				catch (ArgumentException ex)
				{
					throw new InvalidOperationException("Invoke cannot be called recursively - try using a temporary variable.", ex);
				}

				return new ExpressionExpander(replaceVars).Visit(lambda.Body);
			}

			// Expand calls to an expression's Compile() method:
			if (m.Method.Name == "Compile" && m.Object is MemberExpression)
			{
				var me = (MemberExpression)m.Object;
				Expression newExpr = TransformExpr(me);
				if (newExpr != me) return newExpr;
			}

			// Strip out any nested calls to AsExpandable():
			if (m.Method.Name == "AsExpandable" && m.Method.DeclaringType == typeof(Extensions))
				return m.Arguments[0];

			return base.VisitMethodCall(m);
		}

		protected override Expression VisitMember(MemberExpression m)
		{
			// Strip out any references to expressions captured by outer variables - LINQ to SQL can't handle these:
			if (m.Member.DeclaringType.Name.StartsWith("<>"))
				return TransformExpr(m);

			return base.VisitMember(m);
		}

		Expression TransformExpr(MemberExpression input)
		{
			// Collapse captured outer variables
			if (input == null
				|| !(input.Member is FieldInfo)
				|| !input.Member.ReflectedType.IsNestedPrivate
				|| !input.Member.ReflectedType.Name.StartsWith("<>"))	// captured outer variable
				return input;

			if (input.Expression is ConstantExpression)
			{
				object obj = ((ConstantExpression)input.Expression).Value;
				if (obj == null) return input;
				Type t = obj.GetType();
				if (!t.IsNestedPrivate || !t.Name.StartsWith("<>")) return input;
				FieldInfo fi = (FieldInfo)input.Member;
				object result = fi.GetValue(obj);
				if (result is Expression) return Visit((Expression)result);
			}
			return input;
		}
	}

	#endregion

	#region ExpressionVisitor

	/// <summary>
	/// This comes from Matt Warren's sample:
	/// http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
	/// </summary>
	//public abstract class ExpressionVisitor
	//{
	//    public virtual Expression Visit(Expression exp)
	//    {
	//        if (exp == null)
	//            return exp;

	//        switch (exp.NodeType)
	//        {
	//            case ExpressionType.Negate:
	//            case ExpressionType.NegateChecked:
	//            case ExpressionType.Not:
	//            case ExpressionType.Convert:
	//            case ExpressionType.ConvertChecked:
	//            case ExpressionType.ArrayLength:
	//            case ExpressionType.Quote:
	//            case ExpressionType.TypeAs:
	//                return this.VisitUnary((UnaryExpression)exp);
	//            case ExpressionType.Add:
	//            case ExpressionType.AddChecked:
	//            case ExpressionType.Subtract:
	//            case ExpressionType.SubtractChecked:
	//            case ExpressionType.Multiply:
	//            case ExpressionType.MultiplyChecked:
	//            case ExpressionType.Divide:
	//            case ExpressionType.Modulo:
	//            case ExpressionType.And:
	//            case ExpressionType.AndAlso:
	//            case ExpressionType.Or:
	//            case ExpressionType.OrElse:
	//            case ExpressionType.LessThan:
	//            case ExpressionType.LessThanOrEqual:
	//            case ExpressionType.GreaterThan:
	//            case ExpressionType.GreaterThanOrEqual:
	//            case ExpressionType.Equal:
	//            case ExpressionType.NotEqual:
	//            case ExpressionType.Coalesce:
	//            case ExpressionType.ArrayIndex:
	//            case ExpressionType.RightShift:
	//            case ExpressionType.LeftShift:
	//            case ExpressionType.ExclusiveOr:
	//                return this.VisitBinary((BinaryExpression)exp);
	//            case ExpressionType.TypeIs:
	//                return this.VisitTypeIs((TypeBinaryExpression)exp);
	//            case ExpressionType.Conditional:
	//                return this.VisitConditional((ConditionalExpression)exp);
	//            case ExpressionType.Constant:
	//                return this.VisitConstant((ConstantExpression)exp);
	//            case ExpressionType.Parameter:
	//                return this.VisitParameter((ParameterExpression)exp);
	//            case ExpressionType.MemberAccess:
	//                return this.VisitMemberAccess((MemberExpression)exp);
	//            case ExpressionType.Call:
	//                return this.VisitMethodCall((MethodCallExpression)exp);
	//            case ExpressionType.Lambda:
	//                return this.VisitLambda((LambdaExpression)exp);
	//            case ExpressionType.New:
	//                return this.VisitNew((NewExpression)exp);
	//            case ExpressionType.NewArrayInit:
	//            case ExpressionType.NewArrayBounds:
	//                return this.VisitNewArray((NewArrayExpression)exp);
	//            case ExpressionType.Invoke:
	//                return this.VisitInvocation((InvocationExpression)exp);
	//            case ExpressionType.MemberInit:
	//                return this.VisitMemberInit((MemberInitExpression)exp);
	//            case ExpressionType.ListInit:
	//                return this.VisitListInit((ListInitExpression)exp);
	//            default:
	//                throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
	//        }
	//    }

	//    protected virtual MemberBinding VisitBinding(MemberBinding binding)
	//    {
	//        switch (binding.BindingType)
	//        {
	//            case MemberBindingType.Assignment:
	//                return this.VisitMemberAssignment((MemberAssignment)binding);
	//            case MemberBindingType.MemberBinding:
	//                return this.VisitMemberMemberBinding((MemberMemberBinding)binding);
	//            case MemberBindingType.ListBinding:
	//                return this.VisitMemberListBinding((MemberListBinding)binding);
	//            default:
	//                throw new Exception(string.Format("Unhandled binding type '{0}'", binding.BindingType));
	//        }
	//    }

	//    protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
	//    {
	//        ReadOnlyCollection<Expression> arguments = this.VisitExpressionList(initializer.Arguments);
	//        if (arguments != initializer.Arguments)
	//        {
	//            return Expression.ElementInit(initializer.AddMethod, arguments);
	//        }
	//        return initializer;
	//    }

	//    protected virtual Expression VisitUnary(UnaryExpression u)
	//    {
	//        Expression operand = this.Visit(u.Operand);
	//        if (operand != u.Operand)
	//        {
	//            return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method);
	//        }
	//        return u;
	//    }

	//    protected virtual Expression VisitBinary(BinaryExpression b)
	//    {
	//        Expression left = this.Visit(b.Left);
	//        Expression right = this.Visit(b.Right);
	//        Expression conversion = this.Visit(b.Conversion);
	//        if (left != b.Left || right != b.Right || conversion != b.Conversion)
	//        {
	//            if (b.NodeType == ExpressionType.Coalesce && b.Conversion != null)
	//                return Expression.Coalesce(left, right, conversion as LambdaExpression);
	//            else
	//                return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method);
	//        }
	//        return b;
	//    }

	//    protected virtual Expression VisitTypeIs(TypeBinaryExpression b)
	//    {
	//        Expression expr = this.Visit(b.Expression);
	//        if (expr != b.Expression)
	//        {
	//            return Expression.TypeIs(expr, b.TypeOperand);
	//        }
	//        return b;
	//    }

	//    protected virtual Expression VisitConstant(ConstantExpression c)
	//    {
	//        return c;
	//    }

	//    protected virtual Expression VisitConditional(ConditionalExpression c)
	//    {
	//        Expression test = this.Visit(c.Test);
	//        Expression ifTrue = this.Visit(c.IfTrue);
	//        Expression ifFalse = this.Visit(c.IfFalse);
	//        if (test != c.Test || ifTrue != c.IfTrue || ifFalse != c.IfFalse)
	//        {
	//            return Expression.Condition(test, ifTrue, ifFalse);
	//        }
	//        return c;
	//    }

	//    protected virtual Expression VisitParameter(ParameterExpression p)
	//    {
	//        return p;
	//    }

	//    protected virtual Expression VisitMemberAccess(MemberExpression m)
	//    {
	//        Expression exp = this.Visit(m.Expression);
	//        if (exp != m.Expression)
	//        {
	//            return Expression.MakeMemberAccess(exp, m.Member);
	//        }
	//        return m;
	//    }

	//    protected virtual Expression VisitMethodCall(MethodCallExpression m)
	//    {
	//        Expression obj = this.Visit(m.Object);
	//        IEnumerable<Expression> args = this.VisitExpressionList(m.Arguments);
	//        if (obj != m.Object || args != m.Arguments)
	//        {
	//            return Expression.Call(obj, m.Method, args);
	//        }
	//        return m;
	//    }

	//    protected virtual ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
	//    {
	//        List<Expression> list = null;
	//        for (int i = 0, n = original.Count; i < n; i++)
	//        {
	//            Expression p = this.Visit(original[i]);
	//            if (list != null)
	//            {
	//                list.Add(p);
	//            }
	//            else if (p != original[i])
	//            {
	//                list = new List<Expression>(n);
	//                for (int j = 0; j < i; j++)
	//                {
	//                    list.Add(original[j]);
	//                }
	//                list.Add(p);
	//            }
	//        }
	//        if (list != null)
	//        {
	//            return list.AsReadOnly();
	//        }
	//        return original;
	//    }

	//    protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
	//    {
	//        Expression e = this.Visit(assignment.Expression);
	//        if (e != assignment.Expression)
	//        {
	//            return Expression.Bind(assignment.Member, e);
	//        }
	//        return assignment;
	//    }

	//    protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
	//    {
	//        IEnumerable<MemberBinding> bindings = this.VisitBindingList(binding.Bindings);
	//        if (bindings != binding.Bindings)
	//        {
	//            return Expression.MemberBind(binding.Member, bindings);
	//        }
	//        return binding;
	//    }

	//    protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
	//    {
	//        IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(binding.Initializers);
	//        if (initializers != binding.Initializers)
	//        {
	//            return Expression.ListBind(binding.Member, initializers);
	//        }
	//        return binding;
	//    }

	//    protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
	//    {
	//        List<MemberBinding> list = null;
	//        for (int i = 0, n = original.Count; i < n; i++)
	//        {
	//            MemberBinding b = this.VisitBinding(original[i]);
	//            if (list != null)
	//            {
	//                list.Add(b);
	//            }
	//            else if (b != original[i])
	//            {
	//                list = new List<MemberBinding>(n);
	//                for (int j = 0; j < i; j++)
	//                {
	//                    list.Add(original[j]);
	//                }
	//                list.Add(b);
	//            }
	//        }
	//        if (list != null)
	//            return list;
	//        return original;
	//    }

	//    protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
	//    {
	//        List<ElementInit> list = null;
	//        for (int i = 0, n = original.Count; i < n; i++)
	//        {
	//            ElementInit init = this.VisitElementInitializer(original[i]);
	//            if (list != null)
	//            {
	//                list.Add(init);
	//            }
	//            else if (init != original[i])
	//            {
	//                list = new List<ElementInit>(n);
	//                for (int j = 0; j < i; j++)
	//                {
	//                    list.Add(original[j]);
	//                }
	//                list.Add(init);
	//            }
	//        }
	//        if (list != null)
	//            return list;
	//        return original;
	//    }

	//    protected virtual Expression VisitLambda(LambdaExpression lambda)
	//    {
	//        Expression body = this.Visit(lambda.Body);
	//        if (body != lambda.Body)
	//        {
	//            return Expression.Lambda(lambda.Type, body, lambda.Parameters);
	//        }
	//        return lambda;
	//    }

	//    protected virtual NewExpression VisitNew(NewExpression nex)
	//    {
	//        IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);
	//        if (args != nex.Arguments)
	//        {
	//            if (nex.Members != null)
	//                return Expression.New(nex.Constructor, args, nex.Members);
	//            else
	//                return Expression.New(nex.Constructor, args);
	//        }
	//        return nex;
	//    }

	//    protected virtual Expression VisitMemberInit(MemberInitExpression init)
	//    {
	//        NewExpression n = this.VisitNew(init.NewExpression);
	//        IEnumerable<MemberBinding> bindings = this.VisitBindingList(init.Bindings);
	//        if (n != init.NewExpression || bindings != init.Bindings)
	//        {
	//            return Expression.MemberInit(n, bindings);
	//        }
	//        return init;
	//    }

	//    protected virtual Expression VisitListInit(ListInitExpression init)
	//    {
	//        NewExpression n = this.VisitNew(init.NewExpression);
	//        IEnumerable<ElementInit> initializers = this.VisitElementInitializerList(init.Initializers);
	//        if (n != init.NewExpression || initializers != init.Initializers)
	//        {
	//            return Expression.ListInit(n, initializers);
	//        }
	//        return init;
	//    }

	//    protected virtual Expression VisitNewArray(NewArrayExpression na)
	//    {
	//        IEnumerable<Expression> exprs = this.VisitExpressionList(na.Expressions);
	//        if (exprs != na.Expressions)
	//        {
	//            if (na.NodeType == ExpressionType.NewArrayInit)
	//            {
	//                return Expression.NewArrayInit(na.Type.GetElementType(), exprs);
	//            }
	//            else
	//            {
	//                return Expression.NewArrayBounds(na.Type.GetElementType(), exprs);
	//            }
	//        }
	//        return na;
	//    }

	//    protected virtual Expression VisitInvocation(InvocationExpression iv)
	//    {
	//        IEnumerable<Expression> args = this.VisitExpressionList(iv.Arguments);
	//        Expression expr = this.Visit(iv.Expression);
	//        if (args != iv.Arguments || expr != iv.Expression)
	//        {
	//            return Expression.Invoke(expr, args);
	//        }
	//        return iv;
	//    }
	//}

	#endregion

	#region ParameterRebinder

	/// <summary>
	/// reference http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx
	/// </summary>
	public class ParameterRebinder : ExpressionVisitor
	{
		private readonly Dictionary<ParameterExpression, ParameterExpression> map;

		public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
		{
			this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
		}

		public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
		{
			return new ParameterRebinder(map).Visit(exp);
		}

		protected override Expression VisitParameter(ParameterExpression p)
		{
			ParameterExpression replacement;
			if (map.TryGetValue(p, out replacement))
			{
				p = replacement;
			}
			return base.VisitParameter(p);
		}
	}

	#endregion

	#region PredicateBuilder

	/// <summary>
	/// 构建LinqToSQL查询表达式
	/// See http://www.albahari.com/expressions for information and examples.
	/// </summary>
	public static class PredicateBuilder
	{
		public static Expression<Func<T, bool>> True<T>() { return f => true; }
		public static Expression<Func<T, bool>> False<T>() { return f => false; }

		//public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
		//                                          Expression<Func<T, bool>> expr2)
		//{
		//    var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
		//    return Expression.Lambda<Func<T, bool>>
		//         (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
		//}

		//public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
		//                                           Expression<Func<T, bool>> expr2)
		//{
		//    var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
		//    return Expression.Lambda<Func<T, bool>>
		//         (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
		//}

		public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
		{
			// build parameter map (from parameters of second to parameters of first)
			var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
			// replace parameters in the second lambda expression with parameters from the first
			var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
			// apply composition of lambda expression bodies to parameters from the first expression 
			return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
		}

		public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.AndAlso);
		}

		public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
		{
			return first.Compose(second, Expression.OrElse);
		}
	}


	#endregion

	#region Linq

	/// <summary>
	/// Another good idea by Tomas Petricek.
	/// See http://tomasp.net/blog/dynamic-linq-queries.aspx for information on how it's used.
	/// </summary>
	public static class Linq
	{
		// Returns the given anonymous method as a lambda expression
		public static Expression<Func<T, TResult>> Expr<T, TResult>(Expression<Func<T, TResult>> expr)
		{
			return expr;
		}

		// Returns the given anonymous function as a Func delegate
		public static Func<T, TResult> Func<T, TResult>(Func<T, TResult> expr)
		{
			return expr;
		}
	}

	#endregion
}
