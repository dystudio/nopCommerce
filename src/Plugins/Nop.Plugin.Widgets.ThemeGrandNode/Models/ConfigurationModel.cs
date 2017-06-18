using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop4you.Plugin.Widgets.ThemeGrandNode.Models
{
	public class ConfigurationModel : BaseNopModel
	{
		[NopResourceDisplayName("Nop4you.Theme.GrandNode.Color")]
		public string Color
		{
			get;
			set;
		}

		public IList<SelectListItem> Colors
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.showSwitchStyle")]
		public bool showSwitchStyle
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.showWidgetZone")]
		public string showWidgetZone
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setMenuBarColor")]
		public bool setMenuBarColor
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setCustomValues")]
		public bool setCustomValues
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setCustomBackground")]
		public string setCustomBackground
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setCustomTextColor")]
		public string setCustomTextColor
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setCustomDarkColor")]
		public string setCustomDarkColor
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.showBackToTop")]
		public bool showBackToTop
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setBackToTopIcon")]
		public string setBackToTopIcon
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.showBackToTopfa")]
		public bool showBackToTopFaIcon
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.setBackToTopFaIcon")]
		public string setBackToTopFaIcon
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.useStoreClosed")]
		public bool useStoreClosed
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.storeClosedTemplate")]
		public string storeClosedTemplate
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.useCustomOrdersTemplate")]
		public bool useCustomOrdersTemplate
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.storeCustomOrdersTemplate")]
		public string storeCustomOrdersTemplate
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.licenseKey")]
		public string licenseKey
		{
			get;
			set;
		}

		[NopResourceDisplayName("Nop4you.Theme.GrandNode.fixedNav")]
		public bool fixedNav
		{
			get;
			set;
		}

		public ConfigurationModel()
		{
			this.Colors = new List<SelectListItem>();
			ICollection<SelectListItem> colors = this.Colors;
			SelectListItem blackwhite = new SelectListItem();
            blackwhite.Text = "blackwhite";
            blackwhite.Value = "blackwhite";
            colors.Add(blackwhite);

			SelectListItem blue = new SelectListItem();
            blue.Text = "blue";
            blue.Value = "blue";
            colors.Add(blue);

			SelectListItem bluegray = new SelectListItem();
            bluegray.Text= "bluegray";
            bluegray.Value = "bluegray";
			colors.Add(bluegray);

			SelectListItem gray = new SelectListItem();
            gray.Text = "gray";
            gray.Value = "gray";
			colors.Add(gray);

            SelectListItem green = new SelectListItem();
            gray.Text = "green";
            gray.Value = "green";
            colors.Add(green);

            SelectListItem lightblue = new SelectListItem();
            gray.Text = "lightblue";
            gray.Value = "lightblue";
            colors.Add(lightblue);

            SelectListItem orange = new SelectListItem();
            gray.Text = "orange";
            gray.Value = "orange";
            colors.Add(orange);

            SelectListItem purple = new SelectListItem();
            gray.Text = "purple";
            gray.Value = "purple";
            colors.Add(purple);


            SelectListItem red = new SelectListItem();
            gray.Text = "red";
            gray.Value = "red";
            colors.Add(red);


            SelectListItem yellow = new SelectListItem();
            gray.Text = "yellow";
            gray.Value = "yellow";
            colors.Add(yellow);
		}
	}
}
