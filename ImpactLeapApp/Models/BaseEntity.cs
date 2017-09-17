using System;
using System.ComponentModel.DataAnnotations;

namespace ImpactLeapApp.Models
{
    public class BaseEntity
    {
        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }
    }
}
