using Nop.Core.Configuration;
using System;

namespace Nop4you.Plugin.Widgets.ThemeGrandNode
{
	public class ThemeGrandNodeSettings : ISettings
	{
		public string WidgetZone
		{
			get;
			set;
		}

		public string Color
		{
			get;
			set;
		}

		public bool showSwitchStyle
		{
			get;
			set;
		}

		public bool setMenuBarColor
		{
			get;
			set;
		}

		public bool setCustomValues
		{
			get;
			set;
		}

		public string setCustomBackground
		{
			get;
			set;
		}

		public string setCustomTextColor
		{
			get;
			set;
		}

		public string setCustomDarkColor
		{
			get;
			set;
		}

		public bool showBackToTop
		{
			get;
			set;
		}

		public string setBackToTopIcon
		{
			get;
			set;
		}

		public bool showBackToTopFaIcon
		{
			get;
			set;
		}

		public string setBackToTopFaIcon
		{
			get;
			set;
		}

		public bool useStoreClosed
		{
			get;
			set;
		}

		public string storeClosedTemplate
		{
			get;
			set;
		}

		public bool useCustomOrdersTemplate
		{
			get;
			set;
		}

		public string storeCustomOrdersTemplate
		{
			get;
			set;
		}

		public string licenseKey
		{
			get;
			set;
		}

		public bool fixedNav
		{
			get;
			set;
		}
	}
}
