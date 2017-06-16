using System.ComponentModel.DataAnnotations;

namespace RateShopper.Domain.Entities
{

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resource_Files.CustomMessages), ErrorMessageResourceName = "Login_LogIdRequired")]
        [Display(Name = "LoginIdLabelText", ResourceType = typeof(Resource_Files.CustomMessages))]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource_Files.CustomMessages), ErrorMessageResourceName = "Login_PasswordRequired")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordLabelText", ResourceType = typeof(Resource_Files.CustomMessages))]
        public string Password { get; set; }
    }

}
