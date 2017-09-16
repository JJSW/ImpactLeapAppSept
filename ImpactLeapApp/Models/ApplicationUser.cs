using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ImpactLeapApp.Models.OrderModels;
using ImpactLeapApp.Models.BillingModels;

namespace ImpactLeapApp.Models
{
    public enum UserRoleList
    {
        Admin,
        Manager,
        Member,
        Unregistered,
    }

    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public override string Email { get; set; }

        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(20, ErrorMessage = "The {0} mucst be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required]
        [Display(Name = "User Role")]
        public UserRoleList UserRole { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        public List<Order> Orders { get; set; }

        public BillingAddress BillingAddress { get; set; }
    }
}
