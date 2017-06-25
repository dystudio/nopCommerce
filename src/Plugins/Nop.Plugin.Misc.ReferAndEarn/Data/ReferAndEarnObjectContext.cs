using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Misc.ReferAndEarn.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Nop.Plugin.Misc.ReferAndEarn.Data
{
	public class ReferAndEarnObjectContext : DbContext, IDbContext
	{
		private readonly IWebHelper _webHelper = EngineContext.Current.Resolve<IWebHelper>();

		public virtual bool ProxyCreationEnabled
		{
			get
			{
				return base.Configuration.ProxyCreationEnabled;
			}
			set
			{
				base.Configuration.ProxyCreationEnabled=value;
			}
		}

		public virtual bool AutoDetectChangesEnabled
		{
			get
			{
				return base.Configuration.AutoDetectChangesEnabled;
			}
			set
			{
				base.Configuration.AutoDetectChangesEnabled = (value);
			}
		}

		public ReferAndEarnObjectContext(string nameOrConnectionString) : base(nameOrConnectionString)
		{
			//this.ObjectContext.ContextOptions.LazyLoadingEnabled = (true);
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add<CustomerReferrerCode>(new CustomerReferrerCodeMap());
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			base.OnModelCreating(modelBuilder);
		}

		public string CreateDatabaseScript()
		{
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public void Install()
		{
			string createDatabaseScript = this.CreateDatabaseScript();
			Database.ExecuteSqlCommand(createDatabaseScript);
			string customerAttribute = "INSERT INTO [dbo].[CustomerAttribute] ([Name],[IsRequired],[AttributeControlTypeId],[DisplayOrder]) VALUES('Referrer Code', 0, 4, 0)";
			string invitedSignup  = "INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores],[DelayBeforeSend],[DelayPeriodId]) VALUES (N'ReferAndEarn.ReferrerNotification', NULL, N'%Store.Name%. You have been referred by %ReferrerCustomer%', N'<p>Your friend %ReferrerCustomer% has invited you to signup with %Store.Name%.</p><p>You will get %NewCustomerRewardPoint% reward points after successful signup and your friend will also get %ReferrerRewardPoint% reward points for reffering you.</p><p><a href=\"%ReferrerUrl%\" target=\"_blank\">Click</a>&nbsp;here to signup or you can simply copy and paste below url into your web browser for signup.</p><p>%ReferrerUrl%</p><p>Thanks you!!</p><p>%Store.Name%</p>', 1, 0, 1, 0,NULL,0)";
			string customerRegistered = "INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores],[DelayBeforeSend],[DelayPeriodId]) VALUES (N'ReferAndEarn.ReferrerNotificationToReferrer', NULL, N'%Store.Name%. Your referrered customer is registered !', N'<p>Hi %Customer.FullName%,</p><p>We are pleased to inform you that your referred customer %NewCustomer% is registered with %Store.Name%.</p><p>Your account has credited with %ReferrerRewardPoint% reward points.</p><p>Thanks you!!</p><p>%Store.Name%</p>', 1, 0, 1, 0,NULL,0)";
			string rewardPointsNotification = "INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores],[DelayBeforeSend],[DelayPeriodId]) VALUES (N'ReferAndEarn.ReferrerNotificationToNewCustomer', NULL, N'%Store.Name%. Your reward point', N'<p>Hi %Customer.FirstName%,</p><p>Welcome to %Store.Name% !!</p><p>We have credited %NewCustomerRewardPoint% reward points in your account for registration.</p><p>You can use them whille shopping with our store.</p><p>Thank you.</p><p>Regards,</p><p>%Store.Name%</p><p>&nbsp;</p>', 1, 0, 1, 0,NULL,0)";
			string refereeRewardsForFirstPurchase = "INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores],[DelayBeforeSend],[DelayPeriodId]) VALUES (N'ReferAndEarn.RefereeNotificationForFirstOrder', NULL, N'%Store.Name%. Your reward point', N'<p>Hi %Customer.FirstName%,</p><p>Welcome to %Store.Name% !!</p><p>We have credited %RefereeRewardsForFirstPurchase% reward points in your account for your first order.</p><p>You can use them whille another shopping with our store.</p><p>Thank you.</p><p>Regards,</p><p>%Store.Name%</p><p>&nbsp;</p>', 1, 0, 1, 0,NULL,0)";
			string referrerNotificationForFirstOrder = "INSERT [dbo].[MessageTemplate] ([Name], [BccEmailAddresses], [Subject], [Body], [IsActive], [AttachedDownloadId], [EmailAccountId], [LimitedToStores],[DelayBeforeSend],[DelayPeriodId]) VALUES (N'ReferAndEarn.ReferrerNotificationForFirstOrder', NULL, N'%Store.Name%. Your reward point for first order of your invited customer', N'<p>Hi %Customer.FirstName%,</p><p>Welcome to %Store.Name% !!</p><p>We have credited %ReferrelRewardsForFirstPurchase% reward points in your account for first order of your invited customer.</p><p>You can use them whille shopping with our store.</p><p>Thank you.</p><p>Regards,</p><p>%Store.Name%</p><p>&nbsp;</p>', 1, 0, 1, 0,NULL,0)";
			base.Database.ExecuteSqlCommand(customerAttribute);
			base.Database.ExecuteSqlCommand(invitedSignup);
			base.Database.ExecuteSqlCommand(customerRegistered);
			base.Database.ExecuteSqlCommand(rewardPointsNotification);
			base.Database.ExecuteSqlCommand(refereeRewardsForFirstPurchase);
			base.Database.ExecuteSqlCommand(referrerNotificationForFirstOrder);
			this.SaveChanges();
		}

		public void Uninstall()
		{
			string customerAttribute = "DELETE FROM [dbo].[CustomerAttribute] WHERE [Name] LIKE 'Referrer Code' AND AttributeControlTypeId = 4";
			string invitedSignup = "DELETE FROM [dbo].[MessageTemplate] WHERE [Name] like 'ReferAndEarn.ReferrerNotification'";
			string customerRegistered = "DELETE FROM [dbo].[MessageTemplate] WHERE [Name] like 'ReferAndEarn.ReferrerNotificationToReferrer'";
			string rewardPointsNotification = "DELETE FROM [dbo].[MessageTemplate] WHERE [Name] like 'ReferAndEarn.ReferrerNotificationToNewCustomer'";
			string refereeRewardsForFirstPurchase = "DELETE FROM [dbo].[MessageTemplate] WHERE [Name] like 'ReferAndEarn.RefereeNotificationForFirstOrder'";
			string referrerNotificationForFirstOrder = "DELETE FROM [dbo].[MessageTemplate] WHERE [Name] like 'ReferAndEarn.ReferrerNotificationForFirstOrder'";
			base.Database.ExecuteSqlCommand(customerAttribute);
			base.Database.ExecuteSqlCommand(invitedSignup);
			base.Database.ExecuteSqlCommand(customerRegistered);
			base.Database.ExecuteSqlCommand(rewardPointsNotification);
			base.Database.ExecuteSqlCommand(refereeRewardsForFirstPurchase);
			base.Database.ExecuteSqlCommand(referrerNotificationForFirstOrder);
			DbContextExtensions.DropPluginTable(this, "SNCCustomerReferrerCode");
			this.SaveChanges();
		}

		public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public void Detach(object entity)
		{
			throw new NotImplementedException();
		}
	}
}
