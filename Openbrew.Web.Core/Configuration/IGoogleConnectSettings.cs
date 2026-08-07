namespace Openbrew.Web.Core.Configuration
{
	public interface IGoogleConnectSettings
	{
		/// <summary>
		/// Gets or sets the ApplicationKey
		/// </summary>
		string ApplicationKey { get; }

		/// <summary>
		/// Gets or sets the ApplicationSecret
		/// </summary>
		string ApplicationSecret { get; }
	}
}
