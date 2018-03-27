using System.ComponentModel.DataAnnotations;

namespace Milou.Web.ClientResources.WebTests.Integration.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}