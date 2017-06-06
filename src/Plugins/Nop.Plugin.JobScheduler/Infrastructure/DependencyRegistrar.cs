using Autofac;
using Autofac.Core;
using Autofac.Extras.Quartz;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.JobScheduler.Data;
using Nop.Plugin.JobScheduler.Domain;
using Nop.Plugin.JobScheduler.SchedulerJobs;
using Nop.Plugin.JobScheduler.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.JobScheduler.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string JobObjectContextName = "nop_object_context_job_Scheduler";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            // dbcontext
            this.RegisterPluginDataContext<JobSchedulerObjectContext>(builder, JobObjectContextName);

            // repositories
            RegisterRepository<Scheduler>(builder);
            RegisterRepository<VisitCustomer>(builder);

            // services
            RegisterService<SchedulerService, ISchedulerService>(builder);
            RegisterService<VisitCustomerService, IVisitCustomerService>(builder);

            // quartz
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(BackupDatabaseJob).Assembly));
        }

        private void RegisterRepository<T>(ContainerBuilder builder) where T : BaseEntity
        {
            builder.RegisterType<EfRepository<T>>()
                .As<IRepository<T>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(JobObjectContextName))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注册服务类
        /// </summary>
        /// <typeparam name="T">实现类</typeparam>
        /// <typeparam name="TK">接口</typeparam>
        /// <param name="builder"></param>
        /// <param name="useStaticCache">是否使用缓存</param>
        private void RegisterService<T, TK>(ContainerBuilder builder, bool useStaticCache = false)
        {
            if (useStaticCache)
                builder.RegisterType<T>().As<TK>()
                   .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"))
                   .InstancePerLifetimeScope();
            else
                builder.RegisterType<T>().As<TK>().InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
