using System.Collections.Generic;
using ctorx.Core.Messaging;

namespace Openbrew.Web.Models
{
	public abstract class AbstractAsyncViewModel : IAsyncViewModel
	{
		/// <summary>
		/// Gets or sets the Messages
		/// </summary>
		public IList<IMessage> Messages { get; set; }

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		protected AbstractAsyncViewModel()
		{
			this.Messages = new List<IMessage>();
		}
	}
}