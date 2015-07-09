using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TYsoft.Infrastructure.Domain
{
	public class Class
	{
		public int Id { get; set; }
		public string Title { get; set; }

		public IList<Student> Students { get; set; }
	}
}
