using System;
using System.Security;
using System.Web;
using ctorx.Core.Data;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Service;

namespace Openbrew.Web.Core.Model
{
	public class DefaultUserResolver : IUserResolver
	{
		readonly IUserLoginService UserLoginService;
		readonly IUnitOfWorkFactory<BrewgrContext> UnitOfWorkFactory;

		const string UserSessionKey = "UserSummaryKey";

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultUserResolver(IUserService userService, IUserLoginService userLoginService, IUnitOfWorkFactory<BrewgrContext> unitOfWorkFactory)
		{
			this.UserLoginService = userLoginService;
			this.UnitOfWorkFactory = unitOfWorkFactory;
		}

		/// <summary>
		/// Resolves the current User
		/// </summary>
		public UserSummary Resolve()
		{
			var httpContext = HttpContext.Current;

			if(httpContext.Session[UserSessionKey] != null)
			{
				return httpContext.Session[UserSessionKey] as UserSummary;
			}

			// Get User Id
			var identity = HttpContext.Current.User.Identity;
			if(string.IsNullOrWhiteSpace(identity.Name))
			{
				return null;
			}

			var userId = Int32.Parse(identity.Name);

			UserSummary userSummary = null;

			using (var unitOfWork = this.UnitOfWorkFactory.NewUnitOfWork())
			{
				if (!this.UserLoginService.ReturnLogin(userId, out userSummary))
				{
					throw new SecurityException("Could not resolve the user");
				}

				unitOfWork.Commit();
			}

			this.Persist(userSummary);

			return userSummary;
		}

		/// <summary>
		/// Updates the persisted user
		/// </summary>
		public void Update(UserSummary userSummary)
		{
			var httpContext = HttpContext.Current;
			httpContext.Session.Remove(UserSessionKey);

			this.Persist(userSummary);
		}

		/// <summary>
		/// Persists the user for resolution
		/// </summary>
		public void Persist(UserSummary userSummary)
		{
			if(userSummary == null)
			{
				throw new ArgumentNullException("userSummary");
			}

			var httpContext = HttpContext.Current;
			httpContext.Session.Add(UserSessionKey, userSummary);
		}
	}
}