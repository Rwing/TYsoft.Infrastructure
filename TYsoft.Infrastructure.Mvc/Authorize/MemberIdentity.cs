using System.Web.Security;
using TYsoft.EnShine.Domain;
using System;
using TYsoft.Infrastructure.Utility;

namespace TYsoft.EnShine.Kernel.Mvc.Authorize
{
	[Serializable]
	public class MemberIdentity : PlatformIdentity
	{
		public MemberIdentity(FormsAuthenticationTicket ticket)
			: base(ticket)
		{ }

		public Member Member
		{
			get { return DeserializeFromTicket(base.Ticket); }
		}

		private Member DeserializeFromTicket(FormsAuthenticationTicket ticket)
		{
			return JsonFormatter.Deserialize<Member>(ticket.UserData);
		}
	}
}
