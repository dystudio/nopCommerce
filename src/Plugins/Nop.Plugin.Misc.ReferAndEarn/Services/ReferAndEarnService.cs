using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Stores;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Nop.Plugin.Misc.ReferAndEarn.Services
{
	public class ReferAndEarnService : IReferAndEarnService
	{

		private readonly IRepository<CustomerReferrerCode> _customerReferrerCodeRepository;

		private readonly ICustomerService _customerService;

		private readonly IWorkContext _workContext;

		private readonly IStoreContext _storeContext;

		private readonly IEmailAccountService _emailAccountService;

		private readonly EmailAccountSettings _emailAccountSettings;

		private readonly IMessageTemplateService _messageTemplateService;

		private readonly ILanguageService _languageService;

		private readonly ITokenizer _tokenizer;

		private readonly IQueuedEmailService _queuedEmailService;

		private readonly IMessageTokenProvider _messageTokenProvider;

		private readonly IEventPublisher _eventPublisher;

		private readonly ISettingService _settingService;

		private readonly ReferAndEarnSetting _referAndEarnSetting;

		private readonly ICustomerAttributeService _customerAttributeService;

		private readonly ICustomerAttributeParser _customerAttributeParser;

		private readonly IGenericAttributeService _genericAttributeService;

		private readonly ILocalizationService _localizationService;

		private readonly IRewardPointService _rewardPointService;

		private readonly HttpContextBase _httpContext;

		public ReferAndEarnService(IRepository<CustomerReferrerCode> customerReferrerCodeRepository, ICustomerService customerService, IWorkContext workContext, IStoreContext storeContext, IEmailAccountService emailAccountService, EmailAccountSettings emailAccountSettings, IMessageTemplateService messageTemplateService, ILanguageService languageService, ITokenizer tokenizer, IQueuedEmailService queuedEmailService, IMessageTokenProvider messageTokenProvider, IEventPublisher eventPublisher, ISettingService settingService, ReferAndEarnSetting referAndEarnSetting, ICustomerAttributeService customerAttributeService, ICustomerAttributeParser customerAttributeParser, IGenericAttributeService genericAttributeService, ILocalizationService localizationService, IRewardPointService rewardPointService, HttpContextBase httpContext)
		{
			this._customerReferrerCodeRepository = customerReferrerCodeRepository;
			this._customerService = customerService;
			this._workContext = workContext;
			this._storeContext = storeContext;
			this._emailAccountService = emailAccountService;
			this._emailAccountSettings = emailAccountSettings;
			this._messageTemplateService = messageTemplateService;
			this._languageService = languageService;
			this._tokenizer = tokenizer;
			this._queuedEmailService = queuedEmailService;
			this._messageTokenProvider = messageTokenProvider;
			this._eventPublisher = eventPublisher;
			this._settingService = settingService;
			this._referAndEarnSetting = referAndEarnSetting;
			this._customerAttributeService = customerAttributeService;
			this._customerAttributeParser = customerAttributeParser;
			this._genericAttributeService = genericAttributeService;
			this._localizationService = localizationService;
			this._rewardPointService = rewardPointService;
			this._httpContext = httpContext;
		}

		private string RandomString(int length)
		{
			Random random = new Random();
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789", length)
			select s[random.Next(s.Length)]).ToArray<char>());
		}

		public virtual void AddReferAndEarnTokens(IList<Token> tokens, Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward)
		{
			CustomerReferrerCode customerReferrerCodeByCustomerId = this.GetCustomerReferrerCodeByCustomerId(referrerCustomer.Id);

			if (customerReferrerCodeByCustomerId != null)
			{
				ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(this._storeContext.CurrentStore.Id);
				tokens.Add(new Token("ReferrelRewardsForFirstPurchase", Convert.ToString(referAndEarnSetting.ReferrelRewardsForFirstPurchase)));
				tokens.Add(new Token("RefereeRewardsForFirstPurchase", Convert.ToString(referAndEarnSetting.RefereeRewardsForFirstPurchase)));
				tokens.Add(new Token("ReferrerCustomer", Nop.Services.Customers.CustomerExtensions.GetFullName(referrerCustomer)));
				tokens.Add(new Token("NewCustomer", refereeCustomerEmail));
				tokens.Add(new Token("NewCustomerRewardPoint", Convert.ToString(referAndEarnSetting.RefereeRewardPoints)));
				tokens.Add(new Token("ReferrerRewardPoint", Convert.ToString(referAndEarnSetting.ReferrerRewardPoints)));
				tokens.Add(new Token("ReferrerUrl", string.Format("{0}RegisterReferral?referrerCode={1}", this._storeContext.CurrentStore.Url, customerReferrerCodeByCustomerId.ReferrerCode)));
			}
		}

		protected virtual EmailAccount GetEmailAccountOfMessageTemplate(MessageTemplate messageTemplate, int languageId)
		{
			int localized = LocalizationExtensions.GetLocalized<MessageTemplate, int>(messageTemplate, (MessageTemplate mt) => mt.EmailAccountId, languageId, true, true);
			EmailAccount emailAccount = this._emailAccountService.GetEmailAccountById(localized);

			if (emailAccount == null)
			{
				emailAccount = this._emailAccountService.GetEmailAccountById(this._emailAccountSettings.DefaultEmailAccountId);
			}
            else
            {
                emailAccount = this._emailAccountService.GetAllEmailAccounts().FirstOrDefault<EmailAccount>();
            }
			return emailAccount;
		}

		protected virtual MessageTemplate GetActiveMessageTemplate(string messageTemplateName, int storeId)
		{
			MessageTemplate messageTemplateByName = this._messageTemplateService.GetMessageTemplateByName(messageTemplateName, storeId);

			MessageTemplate result;
			if (messageTemplateByName == null)
			{
				result = null;
			}
			else
			{
				bool isActive = messageTemplateByName.IsActive;
				if (!isActive)
				{
					result = null;
				}
				else
				{
					result = messageTemplateByName;
				}
			}
			return result;
		}

		protected virtual int EnsureLanguageIsActive(int languageId, int storeId)
		{
			Language language = this._languageService.GetLanguageById(languageId);

			if (language == null || !language.Published)
			{
				language = this._languageService.GetAllLanguages(false, storeId).FirstOrDefault<Language>();
			}

			if (language == null || !language.Published)
			{
				language = this._languageService.GetAllLanguages(false, 0).FirstOrDefault<Language>();
			}
			
			if (language == null)
			{
				throw new Exception("No active language could be loaded");
			}
			return language.Id;
		}

		protected virtual int SendNotification(MessageTemplate messageTemplate, EmailAccount emailAccount, int languageId, IEnumerable<Token> tokens, string toEmailAddress, string toName, string attachmentFilePath = null, string attachmentFileName = null, string replyToEmailAddress = null, string replyToName = null)
		{
			string bccTemplate = LocalizationExtensions.GetLocalized<MessageTemplate>(messageTemplate, (MessageTemplate mt) => mt.BccEmailAddresses, languageId, true, true);
			string subjectTemplate = LocalizationExtensions.GetLocalized<MessageTemplate>(messageTemplate, (MessageTemplate mt) => mt.Subject, languageId, true, true);
			string bodyTemplate = LocalizationExtensions.GetLocalized<MessageTemplate>(messageTemplate, (MessageTemplate mt) => mt.Body, languageId, true, true);
			string subject = this._tokenizer.Replace(subjectTemplate, tokens, false);
			string body = this._tokenizer.Replace(bodyTemplate, tokens, true);
			QueuedEmail email = new QueuedEmail();
            email.Priority = QueuedEmailPriority.High;
            email.From = emailAccount.Email;
            email.FromName = emailAccount.DisplayName;
            email.To = toEmailAddress;
            email.ToName = toName;
            email.ReplyTo = replyToEmailAddress;
            email.ReplyToName = replyToName;
            email.CC = string.Empty;
            email.Bcc = bccTemplate;
            email.Subject = subject;
            email.Body = body;
            email.AttachmentFilePath = attachmentFilePath;
            email.AttachmentFileName = attachmentFileName;
            email.AttachedDownloadId = messageTemplate.AttachedDownloadId;
            email.CreatedOnUtc = DateTime.UtcNow;
            email.EmailAccountId = emailAccount.Id;
            QueuedEmail queuedEmail = email;
			this._queuedEmailService.InsertQueuedEmail(queuedEmail);
			return queuedEmail.Id;
		}

		public virtual CustomerReferrerCode GetCustomerReferrerCodeByCustomerId(int customerId)
		{

			CustomerReferrerCode result;
			if (customerId == 0)
			{
				result = null;
			}
			else
			{
				IQueryable<CustomerReferrerCode> source = from a in this._customerReferrerCodeRepository.Table
				where a.CustomerId == customerId
				select a;
				CustomerReferrerCode customerReferrerCode = source.ToList<CustomerReferrerCode>().FirstOrDefault<CustomerReferrerCode>();
		
				if (customerReferrerCode == null)
				{
					ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(this._storeContext.CurrentStore.Id);
					string referrerCode = this.RandomString(referAndEarnSetting.ReferrerCodeLenght).ToUpperInvariant();
					customerReferrerCode = new CustomerReferrerCode
					{
						CustomerId = customerId,
						NoOfTimesUsed = 0,
						ReferrerCode = referrerCode,
						CreatedDate = DateTime.UtcNow
					};
					this.InsertCustomerReferrerCode(customerReferrerCode);
				}
				result = customerReferrerCode;
			}
			return result;
		}

		public virtual CustomerReferrerCode GetCustomerReferrerCodeByReferrerCodeId(string referrerCode)
		{
			
			CustomerReferrerCode result;
			if (string.IsNullOrEmpty(referrerCode))
			{
				result = null;
			}
			else
			{
				IQueryable<CustomerReferrerCode> source = from a in this._customerReferrerCodeRepository.Table
				where a.ReferrerCode == referrerCode
				select a;
				result = source.ToList<CustomerReferrerCode>().FirstOrDefault<CustomerReferrerCode>();
			}
			return result;
		}

		public virtual void InsertCustomerReferrerCode(CustomerReferrerCode customerReferrerCode)
		{

			if (customerReferrerCode == null)
			{
				throw new ArgumentNullException("customerReferrerCode");
			}
			this._customerReferrerCodeRepository.Insert(customerReferrerCode);
		}

		public virtual void UpdateCustomerReferrerCode(CustomerReferrerCode customerReferrerCode)
		{
			if (customerReferrerCode == null)
			{
				throw new ArgumentNullException("customerReferrerCode");
			}
			this._customerReferrerCodeRepository.Update(customerReferrerCode);
		}

		public virtual int SendNotificationToNewCustomerAfterRegistration(Customer newCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId)
		{
			Store currentStore = this._storeContext.CurrentStore;
			MessageTemplate activeMessageTemplate = this.GetActiveMessageTemplate("ReferAndEarn.ReferrerNotificationToNewCustomer", currentStore.Id);

			int result;
			if (activeMessageTemplate == null)
			{
				result = 0;
			}
			else
			{
				EmailAccount emailAccountOfMessageTemplate = this.GetEmailAccountOfMessageTemplate(activeMessageTemplate, languageId);
				List<Token> list = new List<Token>();
				this._messageTokenProvider.AddStoreTokens(list, currentStore, emailAccountOfMessageTemplate);
				this._messageTokenProvider.AddCustomerTokens(list, newCustomer);
				this.AddReferAndEarnTokens(list, newCustomer, referrerCustomerReward, refereeCustomerEmail, refereeCustomerReward);
                Nop.Services.Messages.EventPublisherExtensions.MessageTokensAdded<Token>(this._eventPublisher, activeMessageTemplate, list);
				string toEmailAddress = (newCustomer.Email != null) ? newCustomer.Email : refereeCustomerEmail;
				string displayName = emailAccountOfMessageTemplate.DisplayName;
				result = this.SendNotification(activeMessageTemplate, emailAccountOfMessageTemplate, languageId, list, toEmailAddress, displayName, null, null, null, null);
			}
			return result;
		}

		public virtual int SendReferACustomerNotification(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId)
		{
			Store currentStore = this._storeContext.CurrentStore;
			MessageTemplate activeMessageTemplate = this.GetActiveMessageTemplate("ReferAndEarn.ReferrerNotification", currentStore.Id);
		
			int result;
			if (activeMessageTemplate == null)
			{
				result = 0;
			}
			else
			{
				EmailAccount emailAccountOfMessageTemplate = this.GetEmailAccountOfMessageTemplate(activeMessageTemplate, languageId);
				List<Token> list = new List<Token>();
				this._messageTokenProvider.AddStoreTokens(list, currentStore, emailAccountOfMessageTemplate);
				this._messageTokenProvider.AddCustomerTokens(list, referrerCustomer);
				this.AddReferAndEarnTokens(list, referrerCustomer, referrerCustomerReward, refereeCustomerEmail, refereeCustomerReward);
                Nop.Services.Messages.EventPublisherExtensions.MessageTokensAdded<Token>(this._eventPublisher, activeMessageTemplate, list);
				string toEmailAddress = refereeCustomerEmail.Trim();
				string displayName = emailAccountOfMessageTemplate.DisplayName;
				result = this.SendNotification(activeMessageTemplate, emailAccountOfMessageTemplate, languageId, list, toEmailAddress, displayName, null, null, null, null);
			}
			return result;
		}

		public virtual int SendReferACustomerNotificationToReferrer(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId)
		{
			Store currentStore = this._storeContext.CurrentStore;
			MessageTemplate activeMessageTemplate = this.GetActiveMessageTemplate("ReferAndEarn.ReferrerNotificationToReferrer", currentStore.Id);
		
			int result;
			if (activeMessageTemplate == null)
			{
				result = 0;
			}
			else
			{
				EmailAccount emailAccountOfMessageTemplate = this.GetEmailAccountOfMessageTemplate(activeMessageTemplate, languageId);
				List<Token> list = new List<Token>();
				this._messageTokenProvider.AddStoreTokens(list, currentStore, emailAccountOfMessageTemplate);
				this._messageTokenProvider.AddCustomerTokens(list, referrerCustomer);
				this.AddReferAndEarnTokens(list, referrerCustomer, referrerCustomerReward, refereeCustomerEmail, refereeCustomerReward);
                Nop.Services.Messages.EventPublisherExtensions.MessageTokensAdded<Token>(this._eventPublisher, activeMessageTemplate, list);
				string toEmailAddress = referrerCustomer.Email.Trim();
				string displayName = emailAccountOfMessageTemplate.DisplayName;
				result = this.SendNotification(activeMessageTemplate, emailAccountOfMessageTemplate, languageId, list, toEmailAddress, displayName, null, null, null, null);
			}
			return result;
		}

		public virtual int SendRefereeNotificationForFirstOrder(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId)
		{
			Store currentStore = this._storeContext.CurrentStore;
			MessageTemplate activeMessageTemplate = this.GetActiveMessageTemplate("ReferAndEarn.RefereeNotificationForFirstOrder", currentStore.Id);

			int result;
			if (activeMessageTemplate == null)
			{
				result = 0;
			}
			else
			{
				EmailAccount emailAccountOfMessageTemplate = this.GetEmailAccountOfMessageTemplate(activeMessageTemplate, languageId);
				List<Token> list = new List<Token>();
				this._messageTokenProvider.AddStoreTokens(list, currentStore, emailAccountOfMessageTemplate);
				this._messageTokenProvider.AddCustomerTokens(list, referrerCustomer);
				this.AddReferAndEarnTokens(list, referrerCustomer, referrerCustomerReward, refereeCustomerEmail, refereeCustomerReward);
                Nop.Services.Messages.EventPublisherExtensions.MessageTokensAdded<Token>(this._eventPublisher, activeMessageTemplate, list);
				string toEmailAddress = referrerCustomer.Email.Trim();
				string displayName = emailAccountOfMessageTemplate.DisplayName;
				result = this.SendNotification(activeMessageTemplate, emailAccountOfMessageTemplate, languageId, list, toEmailAddress, displayName, null, null, null, null);
			}
			return result;
		}

		public virtual int SendReferrerNotificationForFirstOrder(Customer referrerCustomer, int referrerCustomerReward, string refereeCustomerEmail, int refereeCustomerReward, int languageId)
		{
			Store currentStore = this._storeContext.CurrentStore;
			MessageTemplate activeMessageTemplate = this.GetActiveMessageTemplate("ReferAndEarn.ReferrerNotificationForFirstOrder", currentStore.Id);

			int result;
			if (activeMessageTemplate == null)
			{
				result = 0;
			}
			else
			{
				EmailAccount emailAccountOfMessageTemplate = this.GetEmailAccountOfMessageTemplate(activeMessageTemplate, languageId);
				List<Token> list = new List<Token>();
				this._messageTokenProvider.AddStoreTokens(list, currentStore, emailAccountOfMessageTemplate);
				this._messageTokenProvider.AddCustomerTokens(list, referrerCustomer);
				this.AddReferAndEarnTokens(list, referrerCustomer, referrerCustomerReward, refereeCustomerEmail, refereeCustomerReward);
                Nop.Services.Messages.EventPublisherExtensions.MessageTokensAdded<Token>(this._eventPublisher, activeMessageTemplate, list);
				string toEmailAddress = referrerCustomer.Email.Trim();
				string displayName = emailAccountOfMessageTemplate.DisplayName;
				result = this.SendNotification(activeMessageTemplate, emailAccountOfMessageTemplate, languageId, list, toEmailAddress, displayName, null, null, null, null);
			}
			return result;
		}

		public int GetReferrerCodeAttributeId()
		{
			IList<CustomerAttribute> allCustomerAttributes = this._customerAttributeService.GetAllCustomerAttributes();
            //IEnumerable<CustomerAttribute> arg_2D_0 = allCustomerAttributes;
            //Func<CustomerAttribute, bool> arg_2D_1;
            //if ((arg_2D_1 = ReferAndEarnService.<>c.<>9__36_0) == null)
            //{
            //	arg_2D_1 = (ReferAndEarnService.<>c.<>9__36_0 = new Func<CustomerAttribute, bool>(ReferAndEarnService.<>c.<>9.<GetReferrerCodeAttributeId>b__36_0));
            //}
            CustomerAttribute customerAttribute = new CustomerAttribute();
			return (customerAttribute != null) ? customerAttribute.Id: 0;
		}

		public void AssignCodeAndRewardPoint(string referrerCode, string newCustomerEmail)
		{
			Customer customerByEmail = this._customerService.GetCustomerByEmail(newCustomerEmail);
			bool flag = GenericAttributeExtensions.GetAttribute<string>(customerByEmail, "ReferrerCode", 0) != null;
			if (!flag)
			{
				CustomerReferrerCode customerReferrerCodeByReferrerCodeId = this.GetCustomerReferrerCodeByReferrerCodeId(referrerCode);
				string text = string.Format(this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Register.ReffererMsg"), (customerByEmail.Email != null) ? customerByEmail.Email : newCustomerEmail);
				string text2 = string.Format(this._localizationService.GetResource("SuperNop.Plugin.Misc.ReferAndEarn.Register.ReffereeMsg"), new object[0]);
				ReferAndEarnSetting referAndEarnSetting = this._settingService.LoadSetting<ReferAndEarnSetting>(this._storeContext.CurrentStore.Id);

				if (customerReferrerCodeByReferrerCodeId != null)
				{
					Customer customerById = this._customerService.GetCustomerById(customerReferrerCodeByReferrerCodeId.CustomerId);
			
					if (referAndEarnSetting.ReferrerRewardPoints > 0)
					{
						this._rewardPointService.AddRewardPointsHistoryEntry(customerById, referAndEarnSetting.ReferrerRewardPoints, this._storeContext.CurrentStore.Id, text, null, decimal.Zero, null);
						this.SendReferACustomerNotificationToReferrer(customerById, referAndEarnSetting.ReferrerRewardPoints, (customerByEmail.Email != null) ? customerByEmail.Email : newCustomerEmail, referAndEarnSetting.RefereeRewardPoints, this._workContext.WorkingLanguage.Id);
					}
					CustomerReferrerCode expr_135 = customerReferrerCodeByReferrerCodeId;
					int noOfTimesUsed = expr_135.NoOfTimesUsed;
					expr_135.NoOfTimesUsed = noOfTimesUsed + 1;
					this.UpdateCustomerReferrerCode(customerReferrerCodeByReferrerCodeId);

					if (referAndEarnSetting.RefereeRewardPoints > 0)
					{
						this._rewardPointService.AddRewardPointsHistoryEntry(customerByEmail, referAndEarnSetting.RefereeRewardPoints, this._storeContext.CurrentStore.Id, text2, null, decimal.Zero, null);
						this.SendNotificationToNewCustomerAfterRegistration(customerByEmail, referAndEarnSetting.ReferrerRewardPoints, (customerByEmail.Email != null) ? customerByEmail.Email : newCustomerEmail, referAndEarnSetting.RefereeRewardPoints, this._workContext.WorkingLanguage.Id);
					}
					this._genericAttributeService.SaveAttribute<string>(customerByEmail, "ReferrerCode", customerReferrerCodeByReferrerCodeId.ReferrerCode, 0);
					this._httpContext.Request.Cookies.Remove("referrercode");
				}
			}
		}
	}
}
