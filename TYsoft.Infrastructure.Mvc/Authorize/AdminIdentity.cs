using System.Web.Security;
using TYsoft.EnShine.Domain;
using TYsoft.Infrastructure.Utility;

namespace TYsoft.EnShine.Kernel.Mvc.Authorize
{
	public class AdminIdentity : PlatformIdentity
	{
		public AdminIdentity(FormsAuthenticationTicket ticket)
			: base(ticket)
		{ }

		public Administrator Admin
		{
			get { return DeserializeFromTicket(base.Ticket); }
		}

		private Administrator DeserializeFromTicket(FormsAuthenticationTicket ticket)
		{
			return JsonFormatter.Deserialize<Administrator>(ticket.UserData);
		}
	}
}
