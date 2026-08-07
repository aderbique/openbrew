using System;
using System.Linq;

namespace Openbrew.Web.Core.Model
{
	public class BlogPost
	{
		/// <summary>
		/// Gets or sets the Slug
		/// </summary>
		public string Slug { get; set; }

		/// <summary>
		/// Gets or sets the Title
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the Author
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// Gets or sets the PostContent
		/// </summary>
		public string PostContent { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the Url
		/// </summary>
		public string Url
		{
			get
			{
				return string.Format("http://brewgr.com/blog/post/{0}/{1}/{2}/{3}.aspx",
					this.DateCreated.Year, this.DateCreated.Month.ToString("00"), this.DateCreated.Day.ToString("00"), this.Slug);
			}
		}

		/// <summary>
		/// Gets or sets the AuthorUrl
		/// </summary>
		public string AuthorUrl
		{
			get
			{
				return string.Format("http://brewgr.com/blog/author/{0}.aspx", this.Author);
			}
		}
	}
}