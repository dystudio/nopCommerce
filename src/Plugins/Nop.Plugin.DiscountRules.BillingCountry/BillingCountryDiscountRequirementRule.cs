using System;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;

namespace Nop.Plugin.DiscountRules.BillingCountry
{
    public partial class BillingCountryDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public BillingCountryDiscountRequirementRule(ISettingService settingService)
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

            if (request.Customer.BillingAddress == null)
                return result;

            var billingCountryId = _settingService.GetSettingByKey<int>(string.Format("DiscountRequirement.BillingCountry-{0}", request.DiscountRequirementId));

            if (billingCountryId == 0)
                return result;

            result.IsValid = request.Customer.BillingAddress.CountryId == billingCountryId;

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
            string result = "Plugins/DiscountRulesBillingCountry/Configure/?discountId=" + discountId;
            if (discountRequirementId.HasValue)
                result += string.Format("&discountRequirementId={0}", discountRequirementId.Value);

            return result;
        }
        
        public override void Install()
        {
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.SelectCountry", "Select country");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country", "Billing country");
            this.AddOrUpdatePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country.Hint", "Select required billing country.");
            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.SelectCountry");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country");
            this.DeletePluginLocaleResource("Plugins.DiscountRules.BillingCountry.Fields.Country.Hint");
            base.Uninstall();
        }

        #endregion
    }
}