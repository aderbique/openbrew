using System;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public interface INotification
	{
		/// <summary>
		/// Performs the notification
		/// </summary>
		void Notify(object data);
	}
}