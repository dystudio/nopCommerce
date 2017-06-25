using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using System;
using System.IO;
using System.Web.Hosting;

namespace Nop.Plugin.Misc.ReferAndEarn.PluginExpiry
{
	public class PluginTrial
	{
		private string _pluginSystemName = "Nop.Plugin.Misc.ReferAndEarn";

		private string _configureControllerName = "ReferAndEarn";

		private int _trialDays = 15;

		public const string EXPIRY_SETTING_KEY = "nop.supernop.referandearn.settingkey";

		private string _filePath = "~/Plugins/Nop.Plugin.Misc.ReferAndEarn/";

		private string _fileName = "SuperNopReferAndEarn.dat";

		private ISettingService _settingService = EngineContext.Current.Resolve<ISettingService>();

		public string ConfigureControllerName
		{
			get
			{
				return this._configureControllerName;
			}
			set
			{
				this._configureControllerName = value;
			}
		}

		public string PluginSystemName
		{
			get
			{
				return this._pluginSystemName;
			}
			set
			{
				this._pluginSystemName = value;
			}
		}

		public string MapPath(string path)
		{
			bool isHosted = HostingEnvironment.IsHosted;
			string result;
			if (isHosted)
			{
				result = HostingEnvironment.MapPath(path);
			}
			else
			{
				string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				path = path.Replace("~/", "").TrimStart(new char[]
				{
					'/'
				}).Replace('/', '\\');
				result = Path.Combine(baseDirectory, path);
			}
			return result;
		}

		public void WriteLicenseFile(string expiryDate, bool isEncryptedForm)
		{
			string path = Path.Combine(this.MapPath(this._filePath), this._fileName);
			bool flag = string.IsNullOrEmpty(expiryDate) || string.IsNullOrWhiteSpace(expiryDate);
			if (flag)
			{
				isEncryptedForm = false;
				expiryDate = DateTime.UtcNow.AddDays((double)this._trialDays).ToString("yyyy-MM-dd");
			}
			string contents = string.Empty;
			bool flag2 = isEncryptedForm;
			if (flag2)
			{
				contents = expiryDate;
				bool flag3 = File.Exists(path);
				if (flag3)
				{
					File.Delete(path);
					using (File.Create(path))
					{
					}
					File.WriteAllText(path, contents);
				}
			}
			else
			{
				bool flag4 = !File.Exists(path);
				if (flag4)
				{
					contents = SNCEncryption.Encrypt(expiryDate, true);
					using (File.Create(path))
					{
					}
					File.WriteAllText(path, contents);
				}
			}
		}

		public void StoreExpiryDateInSetting(string expiryDate, bool isEncryptedForm)
		{
			bool flag = string.IsNullOrEmpty(expiryDate) || string.IsNullOrWhiteSpace(expiryDate);
			if (flag)
			{
				isEncryptedForm = false;
				expiryDate = DateTime.UtcNow.AddDays((double)this._trialDays).ToString("yyyy-MM-dd");
			}
			string text = string.Empty;
			bool flag2 = isEncryptedForm;
			if (flag2)
			{
				text = expiryDate;
				string settingByKey = this._settingService.GetSettingByKey<string>("nop.supernop.referandearn.settingkey", null, 0, false);
				bool flag3 = !string.IsNullOrWhiteSpace(settingByKey) && !string.IsNullOrWhiteSpace(settingByKey) && settingByKey.Length > 0;
				if (flag3)
				{
					this._settingService.DeleteSetting(this._settingService.GetSetting("nop.supernop.referandearn.settingkey", 0, false));
					this._settingService.SetSetting<string>("nop.supernop.referandearn.settingkey", text, 0, true);
				}
			}
			else
			{
				string settingByKey2 = this._settingService.GetSettingByKey<string>("nop.supernop.referandearn.settingkey", null, 0, false);
				bool flag4 = !string.IsNullOrWhiteSpace(settingByKey2) && !string.IsNullOrWhiteSpace(settingByKey2) && settingByKey2.Length > 0;
				if (!flag4)
				{
					text = SNCEncryption.Encrypt(expiryDate, true);
					this._settingService.SetSetting<string>("nop.supernop.referandearn.settingkey", text, 0, true);
				}
			}
		}

		public string GetExpiryDate()
		{
			string result = string.Empty;
			bool flag = true;
			string path = Path.Combine(this.MapPath(this._filePath), this._fileName);
			bool flag2 = File.Exists(path);
			if (flag2)
			{
				string text = File.ReadAllText(path);
				bool flag3 = !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
				if (flag3)
				{
					result = SNCEncryption.Decrypt(text, true);
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			bool flag4 = !flag;
			if (flag4)
			{
				string settingByKey = this._settingService.GetSettingByKey<string>("nop.supernop.referandearn.settingkey", null, 0, false);
				bool flag5 = !string.IsNullOrWhiteSpace(settingByKey) && !string.IsNullOrWhiteSpace(settingByKey) && settingByKey.Length > 0;
				if (flag5)
				{
					result = SNCEncryption.Decrypt(settingByKey, true);
				}
			}
			return result;
		}

		public string GetEncryptedKey()
		{
			string result = string.Empty;
			bool flag = true;
			string path = Path.Combine(this.MapPath(this._filePath), this._fileName);
			bool flag2 = File.Exists(path);
			if (flag2)
			{
				string text = File.ReadAllText(path);
				bool flag3 = !string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text);
				if (flag3)
				{
					result = text;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			bool flag4 = !flag;
			if (flag4)
			{
				result = this._settingService.GetSettingByKey<string>("nop.supernop.referandearn.settingkey", null, 0, false);
			}
			return result;
		}
	}
}
