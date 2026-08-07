using System;
using System.Collections.Generic;
using System.Data;
using Openbrew.Web.Core.Model;
using ctorx.Core.Data;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Core.Data
{
	public class DefaultBrewgrBlogRepository : IBrewgrBlogRepository
	{
		readonly IBrewgrBlogConnection BrewgrBlogConnection;

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public DefaultBrewgrBlogRepository(IBrewgrBlogConnection brewgrBlogConnection)
		{
			this.BrewgrBlogConnection = brewgrBlogConnection;
		}

		/// <summary>
		/// Searches Blog Posts
		/// </summary>
		public IEnumerable<BlogPost> SearchBlogPosts(string searchTerm)
		{
			if(string.IsNullOrWhiteSpace(searchTerm))
			{
				throw new ArgumentNullException("searchTerm");
			}

			var query = 
				"SELECT " +
				"	Slug " +
				" , Title " +
				" ,	Author " +
				" ,	PostContent = LEFT(PostContent, 600) " +
				" ,	DateCreated " +
				"FROM  " +
				"	be_Posts with(nolock) " +
				"WHERE  " +
				"	IsDeleted = 0 " +
				"	 AND IsPublished = 1 " +
				"	 AND " +
				"	  ( " +
				"		Title LIKE '%' + @SearchTerm + '%'  " +
				"		OR Description LIKE '%' + @SearchTerm + '%'   " +
				"		OR PostContent LIKE '%' + @SearchTerm + '%'  " +
				"		OR Slug LIKE '%' + @SearchTerm + '%'  " +
				"	 ) " +
				"ORDER BY " +
				"	DateCreated DESC";

		    using(var command = SqlQueryCommand.Make(query)
		        .UsingConnection(this.BrewgrBlogConnection.ConnectionString)
		        .WithParam("@SearchTerm", searchTerm))
		    {
                var results = command.GetDataSet();

                foreach (DataRow dataRow in results.Tables[0].Rows)
                {
                    yield return new BlogPost
                    {
                        Slug = dataRow["Slug"].ToString(),
                        Title = dataRow["Title"].ToString(),
                        Author = dataRow["Author"].ToString(),
                        PostContent = StringCleaner.CleanForPreviewText(dataRow["PostContent"].ToString(), 400),
                        DateCreated = Convert.ToDateTime(dataRow["DateCreated"])
                    };
                }
            }
		}
	}
}