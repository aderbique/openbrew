using System;

namespace Openbrew.Web.Core.Model
{
	public class Content
	{
		/// <summary>
		/// Gets or sets the ContentId
		/// </summary>
		public int ContentId { get; set; }

		/// <summary>
		/// Gets or sets the ContentTypeId
		/// </summary>
		public int ContentTypeId { get; set; }

		/// <summary>
		/// Gets or sets the Name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the ShortName
		/// </summary>
		public string ShortName { get; set; }

		/// <summary>
		/// Gets or sets the Text
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Gets or sets the IsActive
		/// </summary>
		public bool IsActive { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the CreatedBy
		/// </summary>
		public int CreatedBy { get; set; }

		/// <summary>
		/// Gets or sets the ModifiedBy
		/// </summary>
		public int? ModifiedBy { get; set; }

		/// <summary>
		/// Gets or sets the DateCreated
		/// </summary>
		public DateTime DateCreated { get; set; }

		/// <summary>
		/// Gets or sets the DateModified
		/// </summary>
		public DateTime? DateModified { get; set; }
	}
}