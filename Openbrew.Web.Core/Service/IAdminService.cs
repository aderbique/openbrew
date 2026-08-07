using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IAdminService
	{
		/// <summary>
		/// Gets Site Stats
		/// </summary>
		SiteStats GetSiteStats();

		/// <summary>
		/// Resolves Feedback
		/// </summary>
		void ResolveFeedback(int userFeedbackId, int userId);
	}
}