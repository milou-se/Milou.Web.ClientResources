using System.ComponentModel.DataAnnotations;

namespace Milou.Web.ClientResources.WebTests.Integration.Models
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}