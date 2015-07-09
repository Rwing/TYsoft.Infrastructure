using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TYsoft.Infrastructure.Data.EF;

namespace TYsoft.Infrastructure.Bussiness
{
	public class BaseService<T> where T : class, new()
	{
		public IRepository<T> Repository { get; set; }


		public void Add(T item)
		{
			this.Repository.Insert(item);
			this.Repository.Save();
		}

		public void Modify(T item)
		{
			this.Repository.Update(item);
			this.Repository.Save();
		}

		public void Modify()
		{
			this.Repository.Save();
		}

		public void Remove(object[] keyValues)
		{
			
		}

		public T GetItem(Expression<Func<T, bool>> predicate)
		{
			return Repository.GetItemByPredicate(predicate);
		}
	}
}
