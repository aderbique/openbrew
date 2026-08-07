using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public class RecipeCreationOptions
	{
		/// <summary>
		/// Gets or sets the Styles
		/// </summary>
		public IList<BjcpStyleGroup> StyleGroups { get; set; }

		/// <summary>
		/// Gets or sets the Fermentables
		/// </summary>
		public IList<Fermentable> Fermentables { get; set; }

		/// <summary>
		/// Gets or sets the Hops
		/// </summary>
		public IList<Hop> Hops { get; set; }

		/// <summary>
		/// Gets or sets the Yeasts
		/// </summary>
		public IList<IngredientGroup<Yeast>> YeastGroups { get; set; }

		/// <summary>
		/// Gets or sets the Adjuncts
		/// </summary>
		public IList<Adjunct> Adjuncts { get; set; }

		/// <summary>
		/// Gets or sets the MashSteps
		/// </summary>
		public IList<MashStep> MashSteps { get; set; }

		/// <summary>
		/// Gets or sets the SendToShopSettings
		/// </summary>
		public RecipeCreationSendToShopSettings SendToShopSettings { get; set; }

		/// <summary>
		/// ctor the Mighty
		/// </summary>
		public RecipeCreationOptions()
		{
			this.StyleGroups = new List<BjcpStyleGroup>();
			this.Fermentables = new List<Fermentable>();
			this.Hops = new List<Hop>();
			this.YeastGroups = new List<IngredientGroup<Yeast>>();
			this.Adjuncts = new List<Adjunct>();
			this.MashSteps = new List<MashStep>();
			this.SendToShopSettings = new RecipeCreationSendToShopSettings();
		}
	}
}