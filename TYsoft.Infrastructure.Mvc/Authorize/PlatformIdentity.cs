using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Web.Security;
using System.Runtime.Serialization;

namespace TYsoft.EnShine.Kernel.Mvc.Authorize
{
	public class PlatformIdentity : FormsIdentity, ISerializable
	{
		public PlatformIdentity(FormsAuthenticationTicket ticket)
			: base(ticket)
		{ }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (context.State == StreamingContextStates.CrossAppDomain)
			{
				GenericIdentity gIdent = new GenericIdentity(this.Name, this.AuthenticationType);
				info.SetType(gIdent.GetType());

				System.Reflection.MemberInfo[] serializableMembers;
				object[] serializableValues;

				serializableMembers = FormatterServices.GetSerializableMembers(gIdent.GetType());
				serializableValues = FormatterServices.GetObjectData(gIdent, serializableMembers);

				for (int i = 0; i < serializableMembers.Length; i++)
				{
					info.AddValue(serializableMembers[i].Name, serializableValues[i]);
				}
			}
			else
			{
				throw new InvalidOperationException("Serialization not supported");
			}
		}
	}
}
