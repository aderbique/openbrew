using System.Web;
using System.Web.Script.Serialization;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
	public class PostedSessionViewModel
	{
		/// <summary>
		/// Gets or sets the SessionJson
		/// </summary>
		public string SessionJson { get; set; }

		/// <summary>
		/// Hydrates the Session Json
		/// </summary>
		public BrewSession HydrateBrewSessionJson()
		{
			if (string.IsNullOrWhiteSpace(this.SessionJson))
			{
				return null;
			}

			var serializer = new JavaScriptSerializer();
			var session = serializer.Deserialize<BrewSession>(HttpUtility.UrlDecode(this.SessionJson));

			if (string.IsNullOrWhiteSpace(session.Notes))
			{
				session.Notes = null;
			}

			if(string.IsNullOrWhiteSpace(session.PrimingSugarType))
			{
				session.PrimingSugarType = null;
			}

			return session;
		}
	}
}