using System;
using ctorx.Core.Crypto;

namespace Openbrew.Web
{
	public class BrewgrCryptoKeyProvider : IStringCryptoKeyProvider
	{

		/// <summary>
		/// Gets the key to be used for string encryption 
		/// </summary>
		public string Key
		{
			get { return Environment.GetEnvironmentVariable("BrewgrCryptoKeyProvider_Key") ?? "ThisIsTheDevKeyAndItShouldNotGetChanged"; }
		}

		/// <summary>
		/// Gets the Salt used for the string encryption
		/// </summary>
		public string Salt
		{
			get { return Environment.GetEnvironmentVariable("BrewgrCryptoKeyProvider_Salt") ?? "ThisIsTheDevSaltAndItShouldNotGetChanged"; }
		}
	}
}