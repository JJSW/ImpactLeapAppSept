using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Models.OrderModels
{
    public enum SavingDiscountMethodList
    {
        Fixed,
        Percentage,
    }

    public class Saving : BaseEntity
    {
        [Key]
        [Display(Name = "Saving ID")]
        public Int32 SavingId { get; set; }
       
        [Display(Name = "Saving Name")]
        public string SavingName { get; set; }

        [Required]
        [Display(Name = "Discount Method")]
        public SavingDiscountMethodList DiscountMethod { get; set; }

        [Display(Name = "Saving Rate")]
        public int SavingRate { get; set; }

        [Display(Name = "Select From")]
        public int SelectFrom { get; set; }

        [Display(Name = "Select To")]
        public int SelectTo { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
