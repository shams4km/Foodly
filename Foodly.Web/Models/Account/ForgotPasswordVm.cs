using System.ComponentModel.DataAnnotations;

namespace Foodly.Web.Models.Account
{
    public class ForgotPasswordVm
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        public bool Sent { get; set; } // для показа модалки "Reset email sent"
    }
}
