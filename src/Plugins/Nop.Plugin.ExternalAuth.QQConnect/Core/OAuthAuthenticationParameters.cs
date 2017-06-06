using Nop.Services.Authentication.External;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.ExternalAuth.QQConnect.Core
{
	[Serializable]
	public class OAuthAuthenticationParameters : OpenAuthenticationParameters
	{
		private readonly string _providerSystemName;

		private IList<Nop.Services.Authentication.External.UserClaims> _claims;

		public override string ProviderSystemName
		{
			get
			{
				return this._providerSystemName;
			}
		}

		public override IList<Nop.Services.Authentication.External.UserClaims> UserClaims
		{
			get
			{
				return this._claims;
			}
		}

		public OAuthAuthenticationParameters(string providerSystemName)
		{
			this._providerSystemName = providerSystemName;
		}

		public void AddClaim(Nop.Services.Authentication.External.UserClaims claim)
		{
			if (this._claims == null)
			{
				this._claims = new List<Nop.Services.Authentication.External.UserClaims>();
			}
			this._claims.Add(claim);
		}
	}
}