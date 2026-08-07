using System.Collections.Generic;
using System.Text;
using Openbrew.Web.Core.Model;

namespace Openbrew.Web.Models
{
    public class UserProfileViewModel
    {
        /// <summary>
        /// Gets or sets user summary
        /// </summary>
        public UserSummary UserSummary { get; set; }

        /// <summary>
        /// Gets or set users recipes
        /// </summary>
        public IList<RecipeSummary> Recipes { get; set; }

    	/// <summary>
    	/// Gets or sets the BrewSessionSummaries
    	/// </summary>
    	public IList<BrewSessionSummary> BrewSessionSummaries { get; set; }

	    /// <summary>
	    /// Gets or sets the Followers
	    /// </summary>
	    public IList<MiniUserSummary> Followers { get; set; }

	    /// <summary>
	    /// Gets or sets the Follows
	    /// </summary>
	    public IList<MiniUserSummary> Follows { get; set; }

		/// <summary>
		/// Gets the Description
		/// </summary>
    	public string GetDescription()
    	{
			return string.Format("{0} has been using the free homebrew recipe calculator since {1}.  Create your free account today to start building and sharing your homebrew recipes.",
				this.UserSummary.Username, this.UserSummary.DateCreated.ToShortDateString());	
    	}
    }
}