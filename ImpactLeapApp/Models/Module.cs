using System;
using System.ComponentModel.DataAnnotations;

namespace ImpactLeapApp.Models.OrderModels
{
    public class Module : BaseEntity
    {
        [Key]
        [Display(Name = "Module ID")]
        public Int32 ModuleId { get; set; }

        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Display(Name = "Module Description")]
        public string Description { get; set; }

        [Display(Name = "Unit Price")]
        public int UnitPrice { get; set; }

        [Display(Name = "View Sample")]
        public string ModuleSampleName { get; set; }

        [Display(Name = "View Sample Path")]
        public string ModuleSamplePath { get; set; }
    }
}
