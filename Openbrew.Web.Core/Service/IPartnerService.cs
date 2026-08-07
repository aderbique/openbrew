using System.Collections;
using System.Collections.Generic;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Core.Service
{
	public interface IPartnerService
	{
		/// <summary>
		/// Gets a list of parter summaries linked to a specific user id
		/// </summary>
		IList<PartnerSummary> GetUserPartnerSummaries(int userId);

		/// <summary>
		/// Determines if a user is an admin for a partner
		/// </summary>
		bool UserIsPartnerAdmin(int userId, int partnerId);

		/// <summary>
		/// Gets a partner Summary by Id
		/// </summary>
		PartnerSummary GetPartnerSummaryById(int partnerId);

		/// <summary>
		/// Gets a partner by Id
		/// </summary>
		Partner GetPartnerById(int partnerId);

		/// <summary>
		/// Gets a list of active partner services
		/// </summary>
		IList<PartnerService> GetPartnerActiveServices(int partnerId);

		/// <summary>
		/// Gets a partner service by service type id
		/// </summary>
		PartnerService GetPartnerServiceByType(int partnerId, PartnerServiceType partnerServiceType);

		/// <summary>
		/// Gets Partner Send To shop Settings for a given partner
		/// </summary>
		PartnerSendToShopSettings GetPartnerSendToShopSettings(int partnerId);

		/// <summary>
		/// Gets a list of send to shop ingredients
		/// </summary>
		IList<PartnerSendToShopIngredient> GetSendToShopIngredients(int partnerId);

		/// <summary>
		/// Adds an entity
		/// </summary>
		void Add<TEntity>(TEntity entity) where TEntity : class;

		/// <summary>
		/// Deletes an Entity
		/// </summary>
		void Delete<TEntity>(TEntity entity) where TEntity : class;

		/// <summary>
		/// Gets a partner if from a partner token
		/// </summary>
		int? GetPartnerIdFromToken(string token);

		/// <summary>
		/// Determines if a partner offers a service
		/// </summary>
		bool PartnerOffersService(int partnerId, PartnerServiceType partnerServiceType);

		/// <summary>
		/// Gets a users last send to shop order
		/// </summary>
		SendToShopOrder GetUserLastSendToShopOrder(int userId);
	}
}