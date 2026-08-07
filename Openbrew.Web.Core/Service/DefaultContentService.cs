using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Caching;
using Openbrew.Web.Core.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Data;

namespace Openbrew.Web.Core.Service
{
	public class DefaultContentService : IContentService
	{
		readonly IBrewgrRepository Repository;
		readonly ICachingService CachingService;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultContentService(IBrewgrRepository repository, ICachingService cachingService)
		{
			this.Repository = repository;
			this.CachingService = cachingService;
		}

		/// <summary>
		/// Gets a content entry by Id
		/// </summary>
		public Content GetContentById(int contentId, bool forceActive = false, bool forcePublic = false, bool cacheResult = false)
		{
			var func = new Func<Content>(() =>
			{
				var query = this.Repository.GetSet<Content>()
					.Where(x => x.ContentId == contentId);

				this.ApplyContentFilter(query, forceActive, forcePublic);

				return query.FirstOrDefault();
			});

			return cacheResult ? this.CachingService.Get("CONTENT_" + contentId,
				SlidingCacheExpirationSettings.Make(15, CacheItemPriority.Normal), func)
				: func();
		}

		/// <summary>
		/// Gets a Content entry by short name
		/// </summary>
		public Content GetContentByShortName(string shortName, bool forceActive = false, bool forcePublic = false, bool cacheResult = false)
		{
			var func = new Func<Content>(() =>
			{
				var query = this.Repository.GetSet<Content>()
					.Where(x => x.ShortName == shortName);

				this.ApplyContentFilter(query, forceActive, forcePublic);

				return query.FirstOrDefault();
			});

			return cacheResult ? this.CachingService.Get("CONTENT_" + shortName,
				SlidingCacheExpirationSettings.Make(15, CacheItemPriority.Normal), func)
				: func();
		}

		/// <summary>
		/// Gets content text by id
		/// </summary>
		public string GetContentTextById(int contentId, bool forceActive = false, bool forcePublic = false, bool cacheResult = false)
		{
			var func = new Func<string>(() =>
			{
				var query = this.Repository.GetSet<Content>()
					.Where(x => x.ContentId == contentId);

				this.ApplyContentFilter(query, forceActive, forcePublic);

				return query.Select(x => x.Text).FirstOrDefault();
			});

			return cacheResult ? this.CachingService.Get("CONTENTTEXT_" + contentId,
				SlidingCacheExpirationSettings.Make(15, CacheItemPriority.Normal), func)
				: func();
		}

		/// <summary>
		/// Gets content text by short name
		/// </summary>
		public string GetContentTextByShortName(string shortName, bool forceActive = false, bool forcePublic = false, bool cacheResult = false)
		{
			var func = new Func<string>(() =>
			{
				var query = this.Repository.GetSet<Content>()
					.Where(x => x.ShortName == shortName);

				this.ApplyContentFilter(query, forceActive, forcePublic);

				return query.Select(x => x.Text).FirstOrDefault();
			});

			return cacheResult ? this.CachingService.Get("CONTENTTEXT_" + shortName,
				SlidingCacheExpirationSettings.Make(15, CacheItemPriority.Normal), func)
				: func();
		}

		/// <summary>
		/// Applies active and public filter constraints
		/// </summary>
		void ApplyContentFilter(IQueryable<Content> query, bool forceActive, bool forcePublic, bool allowCache = false)
		{
			if(forceActive)
			{
				query = query.Where(x => x.IsActive);
			}

			if(forcePublic)
			{
				query = query.Where(x => x.IsPublic);
			}
		}
	}
}