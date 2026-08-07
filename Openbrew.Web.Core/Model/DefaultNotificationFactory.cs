using System;
using ctorx.Core.Ninject;

namespace Openbrew.Web.Core.Model
{
	public class DefaultNotificationFactory : INotificationFactory
	{
		/// <summary>
		/// Makes a Notification
		/// </summary>
		public INotification Make(NotificationType notificationType)
		{
			var kernel = KernelPersister.Get();

			switch (notificationType)
			{
				case NotificationType.RecipeComment:
					return kernel.GetService(typeof(RecipeCommentNotification)) as RecipeCommentNotification;
                case NotificationType.BrewerFollowed:
                    return kernel.GetService(typeof(BrewerFollowNotification)) as BrewerFollowNotification;
                case NotificationType.BrewSessionComment:
                    return kernel.GetService(typeof(BrewSessionCommentNotification)) as BrewSessionCommentNotification;
                default:
					throw new NotImplementedException();
			}
		}
	}
}