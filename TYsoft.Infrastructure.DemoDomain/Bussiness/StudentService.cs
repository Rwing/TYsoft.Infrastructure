﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TYsoft.Infrastructure.Bussiness;
using TYsoft.Infrastructure.Domain;

namespace TYsoft.Infrastructure.Bussiness
{
	public class StudentService : BaseService<Student>
	{
		public Student GetFirstStudentsWithClass()
		{
			return Repository.GetItemsByPredicate(i => i.Id == 1, i => i.Class).FirstOrDefault();
		}

		public List<dynamic> Test()
		{
			var result = Repository.SqlQueryDynamic("select name, classid from students").ToList();
			return result;
		}
	}
}
