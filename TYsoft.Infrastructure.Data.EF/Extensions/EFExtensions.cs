using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Reflection;
using System.Reflection.Emit;
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

		public static IEnumerable<dynamic> DynamicSqlQuery(this Database database, string sql, params object[] parameters)
		{
			TypeBuilder builder = createTypeBuilder(
					"MyDynamicAssembly", "MyDynamicModule", "MyDynamicType");

			using (System.Data.IDbCommand command = database.Connection.CreateCommand())
			{
				try
				{
					database.Connection.Open();
					command.CommandText = sql;
					command.CommandTimeout = command.Connection.ConnectionTimeout;
					foreach (var param in parameters)
					{
						command.Parameters.Add(param);
					}

					using (System.Data.IDataReader reader = command.ExecuteReader())
					{
						var schema = reader.GetSchemaTable();

						foreach (System.Data.DataRow row in schema.Rows)
						{
							string name = (string)row["ColumnName"];
							//var a=row.ItemArray.Select(d=>d.)
							Type type = (Type)row["DataType"];
							if (type != typeof(string) && (bool)row.ItemArray[schema.Columns.IndexOf("AllowDbNull")])
							{
								type = typeof(Nullable<>).MakeGenericType(type);
							}
							createAutoImplementedProperty(builder, name, type);
						}
					}
				}
				finally
				{
					database.Connection.Close();
					command.Parameters.Clear();
				}
			}

			Type resultType = builder.CreateType();
			var result = database.SqlQuery(resultType, sql, parameters);
			foreach (dynamic item in result)
			{
				yield return item;
			}
		}

		private static TypeBuilder createTypeBuilder(
			string assemblyName, string moduleName, string typeName)
		{
			TypeBuilder typeBuilder = AppDomain
				.CurrentDomain
				.DefineDynamicAssembly(new AssemblyName(assemblyName),
									   AssemblyBuilderAccess.Run)
				.DefineDynamicModule(moduleName)
				.DefineType(typeName, TypeAttributes.Public);
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			return typeBuilder;
		}

		private static void createAutoImplementedProperty(
			TypeBuilder builder, string propertyName, Type propertyType)
		{
			const string PrivateFieldPrefix = "m_";
			const string GetterPrefix = "get_";
			const string SetterPrefix = "set_";

			// Generate the field.
			FieldBuilder fieldBuilder = builder.DefineField(
				string.Concat(PrivateFieldPrefix, propertyName),
							  propertyType, FieldAttributes.Private);

			// Generate the property
			PropertyBuilder propertyBuilder = builder.DefineProperty(
				propertyName, System.Reflection.PropertyAttributes.HasDefault, propertyType, null);

			// Property getter and setter attributes.
			MethodAttributes propertyMethodAttributes =
				MethodAttributes.Public | MethodAttributes.SpecialName |
				MethodAttributes.HideBySig;

			// Define the getter method.
			MethodBuilder getterMethod = builder.DefineMethod(
				string.Concat(GetterPrefix, propertyName),
				propertyMethodAttributes, propertyType, Type.EmptyTypes);

			// Emit the IL code.
			// ldarg.0
			// ldfld,_field
			// ret
			ILGenerator getterILCode = getterMethod.GetILGenerator();
			getterILCode.Emit(OpCodes.Ldarg_0);
			getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
			getterILCode.Emit(OpCodes.Ret);

			// Define the setter method.
			MethodBuilder setterMethod = builder.DefineMethod(
				string.Concat(SetterPrefix, propertyName),
				propertyMethodAttributes, null, new Type[] { propertyType });

			// Emit the IL code.
			// ldarg.0
			// ldarg.1
			// stfld,_field
			// ret
			ILGenerator setterILCode = setterMethod.GetILGenerator();
			setterILCode.Emit(OpCodes.Ldarg_0);
			setterILCode.Emit(OpCodes.Ldarg_1);
			setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
			setterILCode.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getterMethod);
			propertyBuilder.SetSetMethod(setterMethod);
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
