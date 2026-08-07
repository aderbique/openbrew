using System;
using ctorx.Core.Data;

namespace Openbrew.Web.Core.Data
{
	public class BrewgrReadOnlyRepository : ReadOnlyContextRepository<BrewgrContext>, IBrewgrRepository
	{
		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public BrewgrReadOnlyRepository(IDataContextFactory<BrewgrContext> dataContextFactory) : base(dataContextFactory) { }
	}
}