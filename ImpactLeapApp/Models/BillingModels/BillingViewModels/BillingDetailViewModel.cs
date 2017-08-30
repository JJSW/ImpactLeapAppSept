using ImpactLeapApp.Models.OrderModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models.BillingModels
{
    public class BillingDetailViewModel
    {
        // Order
        public string UserId { get; set; }

        [Display(Name = "Order Id")]
        public Int32 OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNum { get; set; }

        [Display(Name = "Ordered Date")]
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderedDate { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatusList OrderStatus { get; set; }

        [Display(Name = "Total To Pay")]
        public int TotalToPay { get; set; }

        // Upload
        [Display(Name = "Notes")]
        public string NoteFromUser { get; set; }

        [Display(Name = "Uploaded File Name")]
        public string UploadedFileName { get; set; }


        // Module
        [Display(Name = "Module Id")]
        public Int32 ModuleId { get; set; }

        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Display(Name = "Module Price")]
        public int UnitPrice { get; set; }

        // Portfolio 
        [Display(Name = "Portfolio ID")]
        public Int32 PortfolioId { get; set; }

        [Display(Name = "Billing Address Number")]
        public Int32 BillingAddressId { get; set; }
    }
}
