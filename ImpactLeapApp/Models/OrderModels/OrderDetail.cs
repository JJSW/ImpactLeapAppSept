using ImpactLeapApp.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models.OrderModels
{
    public class OrderDetail : BaseEntity
    {
        [Key]
        [Display(Name = "Order Detail Number")]
        public Int32 OrderDetailId { get; set; }

        [Display(Name = "Order ID")]
        public Int32 OrderId {get;set;}

        [Display(Name = "Module Number")]
        public Int32 ModuleId { get; set; }

        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }
 
        public Module Module { get; set; }

        [Display(Name = "Fund Number")]
        public Fund FundId { get; set; }
    }
}
