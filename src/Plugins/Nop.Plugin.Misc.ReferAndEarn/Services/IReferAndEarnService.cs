using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using System;

namespace Nop.Plugin.Misc.ReferAndEarn.Services
{
	public interface IReferAndEarnService
	{
		CustomerReferrerCode GetCustomerReferrerCodeByCustomerId(int customerId);

		CustomerReferrerCode GetCustomerReferrerCodeByReferrerCodeId(string referrerCode);

		void InsertCustomerReferrerCode(CustomerReferrerCode customerReferrerCode);

		void UpdateCustomerReferrerCode(CustomerReferrerCode customerReferrerCode);

		int SendNotificationToNewCustomerAfterRegistration(Customer newCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId);

		int SendReferACustomerNotification(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId);

		int SendReferACustomerNotificationToReferrer(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId);

		int SendRefereeNotificationForFirstOrder(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId);

		int SendReferrerNotificationForFirstOrder(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId);

		int GetReferrerCodeAttributeId();

		void AssignCodeAndRewardPoint(string referrerCode, string newCustomerEmail);
	}
}
