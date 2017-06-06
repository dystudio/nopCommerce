using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.JobScheduler.Models
{
    public class VisitCustomerModel : BaseNopEntityModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string RealName { get; set; }

        public bool IsRegisterCustomer { get; set; }

        public string VisitIpAddress { get; set; }

        public string VisitedTime { get; set; }
    }

    /// <summary>
    /// 统计表
    /// </summary>
    public class VisitCustomerExtensionModel : BaseNopModel
    {
        public string VisitDateTime { get; set; }
        public string VisitCustomerCount { get; set; }
    }
}
