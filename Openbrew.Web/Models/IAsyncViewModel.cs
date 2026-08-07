using System.Collections;
using System.Collections.Generic;
using ctorx.Core.Messaging;

namespace Openbrew.Web.Models
{
	public interface IAsyncViewModel
	{
		/// <summary>
		/// Gets or sets the Messages
		/// </summary>
		IList<IMessage> Messages { get; set; }
	}
}