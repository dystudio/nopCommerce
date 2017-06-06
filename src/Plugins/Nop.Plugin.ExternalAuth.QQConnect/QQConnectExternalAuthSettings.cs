using Nop.Core.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace Nop.Plugin.ExternalAuth.QQConnect
{
	public class QQConnectExternalAuthSettings : ISettings
	{
		public string ClientKeyIdentifier
		{
			get;
			set;
		}

		public string ClientSecret
		{
			get;
			set;
		}

		public QQConnectExternalAuthSettings()
		{
		}
	}
}