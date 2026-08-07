namespace Openbrew.Web.Core.Model
{
	public interface IPartnerIdResolver
	{
		/// <summary>
		/// Resolves the partner Id
		/// </summary>
		int? Resolve();

		/// <summary>
		/// Persists the partner Id
		/// </summary>
		void Persist(int partnerId);
	}
}