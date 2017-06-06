using Nop.Plugin.JobScheduler.Common;
using Nop.Plugin.JobScheduler.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.JobScheduler.Data
{
    public class SchedulerMap : EntityTypeConfiguration<Scheduler>
    {
        public SchedulerMap()
        {
            ToTable(SchedulerConstant.SchedulerTableName);

            HasKey(x => x.Id);

            Property(x => x.Name).IsRequired().HasMaxLength(100);
            Property(x => x.SystemName).IsRequired().HasMaxLength(255);

            Ignore(x => x.TimeInterval);
        }
    }
}
