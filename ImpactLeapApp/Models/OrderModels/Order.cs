using System;
using System.ComponentModel.DataAnnotations;

namespace ImpactLeapApp.Models.OrderModels
{
    public enum OrderStatusList
    {
        [Display(Name = "Awaiting Payment")]
        AwaitingPayment = 0,

        Processing = 1,
        Completed = 2,
        Cancelled = 3,       
    }

    public class Order : BaseEntity
    {
        [Key]
        [Display(Name = "Order ID")]
        public Int32 OrderId { get; set; }

        [Display(Name = "Order Number")]
        public string OrderNum { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "User ID")]
        public string UserId { get; set; }


        [Display(Name = "Sales Rep")]
        public string SalesRep { get; set; }

        [Display(Name = "Ordered Date")]
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime OrderedDate { get; set; }

        [Display(Name = "Delivered Date")]
        [DisplayFormat(DataFormatString = "{0: yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DeliveredDate { get; set; }

        [Display(Name = "Order Status")]
        public OrderStatusList OrderStatus { get; set; }


        [Display(Name = "Notes")]
        public string NoteFromUser { get; set; }

        [Display(Name = "Note from admin")]
        public string NoteFromAdmin { get; set; }


        [Display(Name = "Modules")]
        public string ModuleIds { get; set; }

        [Display(Name = "Total Price")]
        public int TotalPrice { get; set; }

        [Display(Name = "Saving Discount")]
        public int SavingDiscount { get; set; }

        [Display(Name = "Saving Discount Method")]
        public SavingDiscountMethodList SavingDiscountMethod { get; set; }

        [Display(Name = "Total To Pay")]
        public int TotalToPay { get; set; }


        [Display(Name = "Uploaded File Name")]
        public string UploadedFileName { get; set; }

        [Display(Name = "Uploaded File Path")]
        public string UploadedFilePath { get; set; }


        [Display(Name = "Promotion ID")]
        public Int32 PromotionId { get; set; }

        [Display(Name = "Promotion Code")]
        public bool IsPromotionCodeApplied { get; set; }


        [Display(Name = "Portfolio ID")]
        public Int32 PortfolioId { get; set; }
    }
}
