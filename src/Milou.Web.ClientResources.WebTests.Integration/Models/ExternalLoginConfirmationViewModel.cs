﻿using System.ComponentModel.DataAnnotations;

namespace Milou.Web.ClientResources.WebTests.Integration.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}