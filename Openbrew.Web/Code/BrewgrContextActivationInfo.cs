using System;
using System.Configuration;
using ctorx.Core.Data;
using ctorx.Core.Configuration;
using Openbrew.Web.Core.Data;

namespace Openbrew.Web
{
	public class BrewgrContextActivationInfo : IDataContextActivationInfo<BrewgrContext>
	{
		public string ConnectionString
		{
		    get
		    {
		        return ConfigReader.EnvironmentVariables.Read("OPENBREW_CONNECTION_STRING", "Brewgr_ConnectionString");
		    }
		}
	}
}
