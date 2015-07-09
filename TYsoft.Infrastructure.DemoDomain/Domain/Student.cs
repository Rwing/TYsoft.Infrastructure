using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Domain
{
	public class Student
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public int ClassId { get; set; }
		public Class Class { get; set; }
	}
}
