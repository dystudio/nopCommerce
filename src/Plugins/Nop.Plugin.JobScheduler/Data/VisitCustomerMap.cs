using Nop.Plugin.JobScheduler.Common;
using Nop.Plugin.JobScheduler.Domain;
using System.Data.Entity.ModelConfiguration;

namespace Nop.Plugin.JobScheduler.Data
{
    public class VisitCustomerMap : EntityTypeConfiguration<VisitCustomer>
    {
        public VisitCustomerMap()
        {
            ToTable(SchedulerConstant.VisitedCustomerTableName);

            HasKey(x => x.Id);

            Property(x => x.Username).HasMaxLength(255);
            Property(x => x.Email).HasMaxLength(1000);
        }
    }
}
