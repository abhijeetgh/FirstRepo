using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EZRAC.Risk.UI.Web.ViewModels.Admin
{
    public class UserViewModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Please enter First Name")]
        [Display(Name = "First Name")]
        [StringLength(100, ErrorMessage = "First Name should not exceed 100 characters.")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Please enter Last Name")]
        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "Last Name should not exceed 100 characters.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Please enter Email")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(100, ErrorMessage = "Email should not exceed 100 characters.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Please enter User Name")]
        [Display(Name = "User Name")]
        [StringLength(100, ErrorMessage = "User Name should not exceed 100 characters.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name="User Role")]
        [Required(ErrorMessage = "Please select user role")]
        public long UserRoleId { get; set; }

        public string UserRole { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<AssignedLocationViewModel> AssignedLocations { get; set; }

        public long[] AssignedLocationsId { get; set; }
    }
}