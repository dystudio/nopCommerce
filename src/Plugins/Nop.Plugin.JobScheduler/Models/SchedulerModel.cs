using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.JobScheduler.Models
{
    public class SchedulerModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SystemName { get; set; }

        public string TimeIntervalId { get; set; }

        public string TimeIntervalDesc { get; set; }

        public int IntervalValue { get; set; }

        public bool Enabled { get; set; }

        public string LastRunTime { get; set; }

        public string RunJobTime { get; set; }
    }
}
