using System;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;

namespace Nop.Plugin.DiscountRules.ShippingCountry
{
    public partial class ShippingCountryDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ShippingCountryDiscountRequirementRule(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>Result</returns>
        public DiscountRequirementValidationResult CheckRequirement(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            if (request.Customer == null)
                return result;

            if (request.Customer.ShippingAddress == null)
                return result;

            var shippingCountryId = _settingService.GetSettingByKey<int>(string.Format("DiscountRequirement.ShippingCountry-{0}", request.DiscountRequirementId));

            if (shippingCountryId == 0)
                return result;

            result.IsValid = request.Customer.ShippingAddress.CountryId == shippingCountryId;

            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            //configured in RouteProvider.cs
            string result = "Plugins/DiscountRulesShippingCountry/Configure/?discountId=" + discountId;

            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);

            return result;
        }

        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.SelectCountry", "Select country");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country", "Shipping country");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country.Hint", "Select required shipping country.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.SelectCountry");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.ShippingCountry.Fields.Country.Hint");
            base.Uninstall();
        }

        #endregion
    }
}