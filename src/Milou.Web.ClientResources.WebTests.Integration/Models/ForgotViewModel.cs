using System.ComponentModel.DataAnnotations;

namespace Milou.Web.ClientResources.WebTests.Integration.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}