using System;
using System.Collections.Generic;
using System.Linq;
using Openbrew.Web.Core.Model;
using ctorx.Core.Linq;

namespace Openbrew.Web.Models
{
	public class IngredientListViewModel<TIngredientType> where TIngredientType : IIngredient
	{
		/// <summary>
		/// Gets or sets the Ingredients
		/// </summary>
		public IList<IngredientGroup<TIngredientType>> IngredientGroups { get; set; }

		/// <summary>
		/// Gets or sets the top ten Ingredients
		/// </summary>
		public IList<TIngredientType> TopTen { get; set; }

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public IngredientListViewModel(IList<IngredientGroup<TIngredientType>> IngredientGroups, IList<int> topTenIds)
		{
			this.IngredientGroups = IngredientGroups;

			var allIngredients = this.IngredientGroups.SelectMany(x => x.Ingredients).ToList();

			this.TopTen = new List<TIngredientType>();
			foreach (var id in topTenIds)
			{
				TopTen.Add(allIngredients.FirstOrDefault(x => x.IngredientId == id));
			}
		}
	}
}