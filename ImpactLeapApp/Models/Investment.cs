using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models
{
    public class Investment : BaseEntity
    {
        [Key]
        [Display(Name = "Investment ID")]
        public Int32 InvestmentId { get; set; }

        [Required]
        [StringLength(160, MinimumLength = 2)]
        [Display(Name = "Investment Name")]
        public string InvestmentName { get; set; }

        [StringLength(160, MinimumLength = 1)]
        [Display(Name = "ISIN")]
        public string ISIN { get; set; }

        [Display(Name = "Share Number")]
        public int ShareNumber { get; set; }

        [Required]
        [Range(0.00, double.MaxValue)]
        [DataType(DataType.Currency)]
        [Display(Name = "Estimate Value")]
        public decimal EstimateValue { get; set; }                
    }
}
