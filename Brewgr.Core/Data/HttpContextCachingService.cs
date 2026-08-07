using System;
using System.Collections.Concurrent;

namespace ctorx.Core.Data
{
	public class HttpContextCachingService : ICachingService
	{
		static readonly ConcurrentDictionary<string, CacheEntry> Cache = new ConcurrentDictionary<string, CacheEntry>();

		sealed class CacheEntry
		{
			public object Value { get; set; }
			public DateTime AbsoluteExpirationUtc { get; set; }
			public TimeSpan SlidingExpiration { get; set; }
			public DateTime LastAccessUtc { get; set; }
		}

		static CacheEntry BuildEntry(object value, ICacheExpirationSettings cacheExpirationSettings)
		{
			if (cacheExpirationSettings == null)
			{
				cacheExpirationSettings = AbsoluteCacheExpirationSettings.Make(5);
			}

			return new CacheEntry
			{
				Value = value,
				AbsoluteExpirationUtc = cacheExpirationSettings.AbsoluteExpiration == DateTime.MaxValue
					? DateTime.MaxValue
					: cacheExpirationSettings.AbsoluteExpiration.ToUniversalTime(),
				SlidingExpiration = cacheExpirationSettings.SlidingExpiration
					,
				LastAccessUtc = DateTime.UtcNow
			};
		}

		static bool IsExpired(CacheEntry entry)
		{
			if (entry == null)
			{
				return true;
			}

			if (entry.AbsoluteExpirationUtc != DateTime.MaxValue && DateTime.UtcNow >= entry.AbsoluteExpirationUtc)
			{
				return true;
			}

			if (entry.SlidingExpiration > TimeSpan.Zero && (DateTime.UtcNow - entry.LastAccessUtc) >= entry.SlidingExpiration)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines if data is cached under a given key
		/// </summary>
		public bool IsInCache(string key)
		{
			CacheEntry entry;
			if (!Cache.TryGetValue(key, out entry))
			{
				return false;
			}

			if (IsExpired(entry))
			{
				this.Remove(key);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Gets a value from cache if it exists, otherwise invokes the func setting its result in Cache
		/// </summary>
		public TValue Get<TValue>(string key, ICacheExpirationSettings cacheExpirationSettings = null, Func<TValue> retrieveFunc = null) where TValue : class
		{
			if (this.IsInCache(key))
			{
				CacheEntry entry;
				if (Cache.TryGetValue(key, out entry))
				{
					entry.LastAccessUtc = DateTime.UtcNow;
					var cached = entry.Value as TValue;
					if (cached != null)
					{
						return cached;
					}
				}

				this.Remove(key);
			}

			if(retrieveFunc == null)
			{
				return null;
			}

			// Invoke Func to Get Data
			var value = retrieveFunc.Invoke();

			if (value != null)
			{
				this.Set(key, value, cacheExpirationSettings);
			}

			return value;
		}

		/// <summary>
		/// Sets a value in cache using the provided settings
		/// </summary>
		public void Set<TValue>(string key, TValue value, ICacheExpirationSettings cacheExpirationSettings = null) where TValue : class
		{
			if(cacheExpirationSettings == null)
			{
				cacheExpirationSettings = AbsoluteCacheExpirationSettings.Make(5);
			}

			if (value != null)
			{
				Cache[key] = BuildEntry(value, cacheExpirationSettings);
			}
		}

		/// <summary>
		/// Updates a value in the Cache
		/// </summary>
		public void Update<TValue>(string key, TValue value) where TValue : class
		{
			this.Set(key, value);
		}

		/// <summary>
		/// Removes a value from the Cache
		/// </summary>
		public void Remove(string key)
		{
			CacheEntry ignored;
			Cache.TryRemove(key, out ignored);
		}
	}
}
