using Openbrew.Web.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Openbrew.Web.Models
{
    public class CommentWrapperViewModel
    {
        /// <summary>
        /// Gets or sets the RecipeID
        /// </summary>
        public int GenericId { get; set; }

        /// <summary>
        /// Gets or sets the CommentType
        /// </summary>
        public CommentType CommentType { get; set; }

        /// <summary>
        /// Gets or sets the CommentViewModels
        /// </summary>
        public IList<CommentViewModel> CommentViewModels { get; set; }
    }
}