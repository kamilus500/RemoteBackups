using System.ComponentModel.DataAnnotations;

namespace RemoteBackups.Blazor.Models.Forms.Auth.Register
{
    public class RegisterForm
    {
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "Field_login_required")]
        public string Login { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "Field_password_required")]
        public string Password { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "Confirmation_password")]
        [Compare(nameof(Password),
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "Passwords_not_identical")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
