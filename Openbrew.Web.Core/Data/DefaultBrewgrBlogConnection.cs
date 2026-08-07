using System;
using ctorx.Core.Configuration;

namespace Openbrew.Web.Core.Data
{
    public class DefaultBrewgrBlogConnection : IBrewgrBlogConnection
    {
        /// <summary>
        /// Gets the connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return ConfigReader.EnvironmentVariables.Read("OPENBREW_BLOG_CONNECTION_STRING", "BrewgrBlog_ConnectionString");
            }
        }
    }
}
