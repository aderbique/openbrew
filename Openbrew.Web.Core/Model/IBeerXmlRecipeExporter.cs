using System;
using System.Linq;
using System.Xml.Linq;
using ctorx.Core.Formatting;

namespace Openbrew.Web.Core.Model
{
	public interface IBeerXmlRecipeExporter
	{
		/// <summary>
		/// Exports a Recipe to Beer Xml
		/// </summary>
		string Export(Recipe recipe);
	}
}