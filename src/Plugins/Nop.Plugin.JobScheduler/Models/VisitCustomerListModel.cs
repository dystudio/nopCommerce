using System;
using Nop.Web.Framework;

namespace Nop.Plugin.JobScheduler.Models
{
    public class VisitCustomerListModel
    {
        [NopResourceDisplayName("Plugins.JobScheduler.VisitCustomer.Fields.SearchDateTime")]
        public DateTime? SearchDateTime { get; set; }

        public bool UsernamesEnabled { get; set; }
    }
}
