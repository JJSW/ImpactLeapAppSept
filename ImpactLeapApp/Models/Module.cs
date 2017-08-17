using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
