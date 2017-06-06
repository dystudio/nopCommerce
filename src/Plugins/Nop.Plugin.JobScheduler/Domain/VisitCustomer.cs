using System;
using Nop.Core;

namespace Nop.Plugin.JobScheduler.Domain
{
    public class VisitCustomer : BaseEntity
    {
        /// <summary>
        /// (匿名/注册)用户Id
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 是否为注册用户（用于区分匿名用户）
        /// </summary>
        public bool IsRegisterCustomer { get; set; }

        /// <summary>
        /// 访问IP
        /// </summary>
        public string VisitIpAddress { get; set; }

        /// <summary>
        /// 访问时间
        /// </summary>
        public DateTime VisitedTime { get; set; }
    }
}
