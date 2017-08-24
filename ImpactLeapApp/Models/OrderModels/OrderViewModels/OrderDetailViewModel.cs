using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models.OrderModels
{
    public class OrderDetailViewModel
    {
        // Order
        [Display(Name = "Order ID")]
        public Int32 OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNum { get; set; }

        [Display(Name = "Order Detail Number")]
        public Int32 OrderDetailId { get; set; }

        [Display(Name = "Ordered Date")]
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderedDate { get; set; }

        [Display(Name = "Delivered Date")]
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveredDate { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatusList OrderStatus { get; set; }

        [Display(Name = "Total To Pay")]
        public int TotalToPay { get; set; }



        [Display(Name = "Notes")]
        public string NoteFromUser { get; set; }

        [Display(Name = "Note From Admin")]
        public string NoteFromAdmin { get; set; }

        [Display(Name = "Uploaded File Path")]
        public string UploadedFilePath { get; set; }

        [Display(Name = "Uploaded File Name")]
        public string UploadedFileName { get; set; }

        [Display(Name = "Sales Rep")]
        public string SalesRep { get; set; }



        // Module
        public Module Module { get; set; }

        [Display(Name = "Module ID")]
        public Int32 ModuleId { get; set; }

        [Display(Name = "Module IDs")]
        public string ModuleIds { get; set; }

        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }



        // User
        public string UserId { get; set; }

        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Module Price")]
        public int UnitPrice { get; set; }


        // Portfolio
        public Int32 PortfolioId { get; set; }

        public Portfolio Portfolio { get; set; }


        [Display(Name = "Billing Address Number")]
        public Int32 BillingAddressId { get; set; }

        [Display(Name = "Promotion")]
        public Promotion Promotion { get; set; }
    }
}
