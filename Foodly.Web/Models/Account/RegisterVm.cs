using System.ComponentModel.DataAnnotations;

namespace Foodly.Web.Models.Account
{
    public class RegisterVm
    {
        [Required, Display(Name="Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
