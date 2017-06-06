using System;
using Nop.Core;

namespace Nop.Plugin.JobScheduler.Domain
{
    public class Scheduler : BaseEntity
    {
        public string Name { get; set; }
        /// <summary>
        /// 作业系统名
        /// </summary>
        public string SystemName { get; set; }

        public int TimeIntervalId { get; set; }

        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)TimeIntervalId; }
            set { TimeIntervalId = (int)value; }
        }

        /// <summary>
        /// 作业调度值
        /// </summary>
        public int IntervalValue { get; set; }

        public bool Enabled { get; set; }

        public bool Deleted { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        public DateTime? RunJobTime { get; set; }
    }
}
