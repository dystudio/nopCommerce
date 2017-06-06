using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.ExternalAuth.QQConnect;
using Nop.Services.Authentication.External;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.ExternalAuth.QQConnect.Core
{
	public class QQConnectProviderAuthorizer : IOAuthProviderQQConnectAuthorizer, IExternalProviderAuthorizer
	{
		private readonly IExternalAuthorizer _authorizer;

		private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;

		private readonly QQConnectExternalAuthSettings _qqConnectExternalAuthSettings;

		private readonly HttpContextBase _httpContext;

		private readonly IWebHelper _webHelper;

		private QQConnectClient _qqConnectApplication;

		private QQConnectClient QQConnectApplication
		{
			get
			{
				QQConnectClient qQConnectClient = this._qqConnectApplication;
				if (qQConnectClient == null)
				{
					QQConnectClient qQConnectClient1 = new QQConnectClient(this._qqConnectExternalAuthSettings.ClientKeyIdentifier, this._qqConnectExternalAuthSettings.ClientSecret);
					QQConnectClient qQConnectClient2 = qQConnectClient1;
					this._qqConnectApplication = qQConnectClient1;
					qQConnectClient = qQConnectClient2;
				}
				return qQConnectClient;
			}
		}

		public QQConnectProviderAuthorizer(IExternalAuthorizer authorizer, ExternalAuthenticationSettings externalAuthenticationSettings, QQConnectExternalAuthSettings qqConnectExternalAuthSettings, HttpContextBase httpContext, IWebHelper webHelper)
		{
			this._authorizer = authorizer;
			this._externalAuthenticationSettings = externalAuthenticationSettings;
			this._qqConnectExternalAuthSettings = qqConnectExternalAuthSettings;
			this._httpContext = httpContext;
			this._webHelper = webHelper;
		}

		public AuthorizeState Authorize(string returnUrl, bool? verifyResponse = null)
		{
			AuthorizeState authorizeState;
			if (!verifyResponse.HasValue)
			{
				throw new ArgumentException("QQConnect plugin cannot automatically determine verifyResponse property");
			}
			authorizeState = (!verifyResponse.Value ? this.RequestAuthentication() : this.VerifyAuthentication(returnUrl));
			return authorizeState;
		}

		public Uri GenerateLocalCallbackUri()
		{
			string url = string.Format("{0}plugins/externalauthQQConnect/logincallback/", this._webHelper.GetStoreLocation());
			return new Uri(url);
		}

		private void ParseClaims(AuthenticationResult authenticationResult, OAuthAuthenticationParameters parameters)
		{
			UserClaims userClaim = new UserClaims();
			userClaim.Name = new NameClaims();
			ContactClaims contactClaim = new ContactClaims();
			contactClaim.Email = string.Format("{0}@qq.com", authenticationResult.ProviderUserId);
			userClaim.Contact = contactClaim;
			UserClaims claims = userClaim;
			if (authenticationResult.ExtraData.ContainsKey("name"))
			{
				string name = authenticationResult.ExtraData["name"];
				if (!string.IsNullOrEmpty(name))
				{
					claims.Name.Nickname = name;
				}
			}
			parameters.AddClaim(claims);
		}

		private AuthorizeState RequestAuthentication()
		{
			string authUrl = this.QQConnectApplication.GetServiceLoginUrl(this.GenerateLocalCallbackUri()).AbsoluteUri;
			AuthorizeState authorizeState = new AuthorizeState("",OpenAuthenticationStatus.RequiresRedirect);
			authorizeState.Result = new RedirectResult(authUrl);
			return authorizeState;
		}

		private AuthorizeState VerifyAuthentication(string returnUrl)
		{
			AuthorizeState authorizeState;
			object message;
			AuthenticationResult authResult = this.QQConnectApplication.VerifyAuthentication(this._httpContext, this.GenerateLocalCallbackUri());
			if (!authResult.IsSuccessful)
			{
				AuthorizeState state = new AuthorizeState(returnUrl,OpenAuthenticationStatus.Error);
				Exception error = authResult.Error;
				if (error != null)
				{
					message = error.Message;
				}
				else
				{
					message = null;
				}
				if (message == null)
				{
					message = "Unknown error";
				}
				state.AddError((string)message);
				authorizeState = state;
			}
			else
			{
				if (!authResult.ExtraData.ContainsKey("id"))
				{
					throw new Exception("Authentication result does not contain id data");
				}
				if (!authResult.ExtraData.ContainsKey("accesstoken"))
				{
					throw new Exception("Authentication result does not contain accesstoken data");
				}
				OAuthAuthenticationParameters oAuthAuthenticationParameter = new OAuthAuthenticationParameters(Provider.SystemName);
				((OpenAuthenticationParameters)oAuthAuthenticationParameter).ExternalDisplayIdentifier = (authResult.ProviderUserId);
				((OpenAuthenticationParameters)oAuthAuthenticationParameter).OAuthToken = (authResult.ExtraData["accesstoken"]);
				((OpenAuthenticationParameters)oAuthAuthenticationParameter).OAuthAccessToken = (authResult.ProviderUserId);
				OAuthAuthenticationParameters parameters = oAuthAuthenticationParameter;
				if (this._externalAuthenticationSettings.AutoRegisterEnabled)
				{
					this.ParseClaims(authResult, parameters);
				}
				authorizeState = new AuthorizeState(returnUrl, this._authorizer.Authorize(parameters));
			}
			return authorizeState;
		}
	}
}