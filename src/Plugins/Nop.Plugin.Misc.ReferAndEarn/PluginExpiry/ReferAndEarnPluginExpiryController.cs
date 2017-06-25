using Nop.Web.Framework.Controllers;
using System;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.ReferAndEarn.PluginExpiry
{
	public class ReferAndEarnPluginExpiryController : BaseController
	{
		private readonly PluginTrial _pluginTrial;

		private string _errorMessage = string.Empty;

		public ReferAndEarnPluginExpiryController(PluginTrial pluginTrial)
		{
			this._pluginTrial = pluginTrial;
		}

		public ActionResult PluginExpiry()
		{
			return base.View("~/Plugins/" + this._pluginTrial.PluginSystemName + "/Views/ReferAndEarnPluginExpiry/PluginExpiry.cshtml");
		}

		[FormValueRequired(new string[]
		{
			"license-key"
		}), HttpPost]
		public ActionResult LicenseKey(LicenseKeyModel model)
		{
			bool flag = !string.IsNullOrWhiteSpace(model.LicenseKey) && !string.IsNullOrWhiteSpace(model.LicenseKey) && model.LicenseKey.Length > 0 && model.LicenseKey.IsBase64();
			if (flag)
			{
				this._pluginTrial.WriteLicenseFile(model.LicenseKey, true);
				this._pluginTrial.StoreExpiryDateInSetting(model.LicenseKey, true);
				this.SuccessNotification("License Key has updated.", true);
			}
			else
			{
				this.ErrorNotification("Invalid License Key...", true);
			}
			return this.Redirect("~/Admin/Widget/ConfigureWidget?systemName=Nop.Plugin.Misc.ReferAndEarn");
		}

		public ActionResult LicenseKey()
		{
			LicenseKeyModel licenseKeyModel = new LicenseKeyModel();
			licenseKeyModel.LicenseKey = this._pluginTrial.GetEncryptedKey();
			licenseKeyModel.ExpiryDate = "Warning: You are running on trial version. Your Plugin will be expire on " + this._pluginTrial.GetExpiryDate() + ".";
			return base.View("~/Plugins/" + this._pluginTrial.PluginSystemName + "/Views/ReferAndEarnPluginExpiry/LicenseKey.cshtml", licenseKeyModel);
		}
	}
}
