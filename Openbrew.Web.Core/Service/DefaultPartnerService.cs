using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Management;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Linq;

namespace Openbrew.Web.Core.Service
{
	public class DefaultPartnerService : IPartnerService
	{
		readonly IBrewgrRepository Repository;
		readonly IUserResolver UserResolver;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultPartnerService(IBrewgrRepository repository, IUserResolver userResolver)
		{
			this.Repository = repository;
			this.UserResolver = userResolver;
		}

		/// <summary>
		/// Gets a list of parter summaries linked to a specific user id
		/// </summary>
		public IList<PartnerSummary> GetUserPartnerSummaries(int userId)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<Partner>()
					.Where(x => x.IsActive)
					.Where(x => x.UserPartnerAdmins.Any(y => y.IsActive && y.UserId == userId))
					.Select(x => x.PartnerSummary)
					.ToList();
		}

		/// <summary>
		/// Determines if a user is an admin for a partner
		/// </summary>
		public bool UserIsPartnerAdmin(int userId, int partnerId)
		{
			if (userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			if (partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<UserPartnerAdmin>()
				.Any(x => x.UserId == userId && x.PartnerId == partnerId && x.IsActive);
		}

		/// <summary>
		/// Gets a partner Summary by Id
		/// </summary>
		public PartnerSummary GetPartnerSummaryById(int partnerId)
		{
			if (partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<PartnerSummary>()
				.FirstOrDefault(x => x.PartnerId == partnerId);
		}

		/// <summary>
		/// Gets a partner by Id
		/// </summary>
		public Partner GetPartnerById(int partnerId)
		{
			if(partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<Partner>()
				.FirstOrDefault(x => x.PartnerId == partnerId);
		}

		/// <summary>
		/// Gets a list of active partner services
		/// </summary>
		public IList<PartnerService> GetPartnerActiveServices(int partnerId)
		{
			if (partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<PartnerService>()
				.Where(x => x.PartnerId == partnerId)
				.ToList();
		}

		/// <summary>
		/// Gets a partner service by service type id
		/// </summary>
		public PartnerService GetPartnerServiceByType(int partnerId, PartnerServiceType partnerServiceType)
		{
			if(partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<PartnerService>()
				.Where(x => x.IsActive)
				.Where(x => x.PartnerId == partnerId)
				.Where(x => x.PartnerServiceTypeId == (int)partnerServiceType)
				.FirstOrDefault();
		}

		/// <summary>
		/// Gets Partner Send To shop Settings for a given partner
		/// </summary>
		public PartnerSendToShopSettings GetPartnerSendToShopSettings(int partnerId)
		{
			if(partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<PartnerSendToShopSettings>()
				.FirstOrDefault(x => x.PartnerId == partnerId);
		}

		/// <summary>
		/// Gets a list of send to shop ingredients
		/// </summary>
		public IList<PartnerSendToShopIngredient> GetSendToShopIngredients(int partnerId)
		{
			if(partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			return this.Repository.GetSet<PartnerSendToShopIngredient>()
				.Where(x => x.PartnerId == partnerId)
				.ToList();
		}

		/// <summary>
		/// Adds an entity
		/// </summary>
		public void Add<TEntity>(TEntity entity) where TEntity : class
		{
			if(entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			this.Repository.Add(entity);
		}

		/// <summary>
		/// Deletes an Entity
		/// </summary>
		public void Delete<TEntity>(TEntity entity) where TEntity : class
		{
			if (entity == null)
			{
				throw new ArgumentNullException("entity");
			}

			this.Repository.Delete(entity);
		}

		/// <summary>
		/// Gets a partner if from a partner token
		/// </summary>
		public int? GetPartnerIdFromToken(string token)
		{
			if(string.IsNullOrWhiteSpace(token))
			{
				throw new ArgumentNullException("token");
			}

			return this.Repository.GetSet<Partner>()
				.Where(x => x.IsActive)
				.Where(x => x.IsPublic)
				.Where(x => x.Token == token)
				.Select(x => x.PartnerId)
				.FirstOrDefault();
		}

		/// <summary>
		/// Determines if a partner offers a service
		/// </summary>
		public bool PartnerOffersService(int partnerId, PartnerServiceType partnerServiceType)
		{
			if(partnerId <= 0)
			{
				throw new ArgumentOutOfRangeException("partnerId");
			}

			int? userId = null;
			var user = this.UserResolver.Resolve();
			if(user != null)
			{
				userId = user.UserId;
			}

			return this.Repository.GetSet<Partner>()
				.Where(x => x.IsActive && x.IsPublic)
				.Any(x => x.PartnerServices.Any(y => y.IsActive && (y.IsPublic || (userId != null && x.UserPartnerAdmins.Any(z => z.UserId == userId && z.IsActive)))));
		}

		/// <summary>
		/// Gets a users last send to shop order
		/// </summary>
		public SendToShopOrder GetUserLastSendToShopOrder(int userId)
		{
			if(userId <= 0)
			{
				throw new ArgumentOutOfRangeException("userId");
			}

			return this.Repository.GetSet<SendToShopOrder>()
				.Where(x => x.UserId == userId)
				.OrderByDescending(x => x.DateCreated)
				.FirstOrDefault();
		}
	}
}