using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models
{
    public class Fund : BaseEntity
    {
        [Key]
        [Display(Name = "Fund ID")]
        public Int32 FundId { get; set; }

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
