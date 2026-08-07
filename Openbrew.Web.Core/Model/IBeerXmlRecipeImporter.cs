using System;
using System.Collections.Generic;

namespace Openbrew.Web.Core.Model
{
	public interface IBeerXmlRecipeImporter
	{
		/// <summary>
		/// Imports a Recipe from Beer Xml
		/// </summary>
		Recipe Import(string beerXml);

		/// <summary>
		/// Imports every recipe record in a BeerXML document. BeerXML files may
		/// contain more than one RECIPE inside their RECIPES record set.
		/// </summary>
		IList<Recipe> ImportMany(string beerXml);
	}
}
