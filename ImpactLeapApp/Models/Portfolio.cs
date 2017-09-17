using System;
using System.ComponentModel.DataAnnotations;

namespace ImpactLeapApp.Models
{
    public class Portfolio : BaseEntity
    {
        [Key]
        [Display(Name = "Portfolio ID")]
        public Int32 PortfolioId { get; set; }

        [Display(Name = "Fund Name")]
        public string FundName { get; set; }

        [Display(Name = "Fund Manager")]
        public string FundManager { get; set; }

        [Display(Name = "Strategy")]
        public string Strategy { get; set; }

        [Display(Name = "Geography")]
        public string Geography { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
