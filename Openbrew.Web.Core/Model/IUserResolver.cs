using System.Linq;
using System.Web.UI.WebControls.WebParts;

namespace Openbrew.Web.Core.Model
{
	public interface IUserResolver
	{
		/// <summary>
		/// Resolves the current User
		/// </summary>
		UserSummary Resolve();

		/// <summary>
		/// Updates the persisted user
		/// </summary>
		void Update(UserSummary userSummary);

		/// <summary>
		/// Persists the user for resolution
		/// </summary>
		void Persist(UserSummary userSummary);
	}
}