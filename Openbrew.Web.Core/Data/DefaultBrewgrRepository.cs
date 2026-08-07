using System;
using ctorx.Core.Data;

namespace Openbrew.Web.Core.Data
{
	public class DefaultBrewgrRepository : WritableContextRepository<BrewgrContext>, IBrewgrRepository
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultBrewgrRepository(IDataContextResolver<BrewgrContext> dataContextResolver) 
			: base(dataContextResolver) { }
	}
}