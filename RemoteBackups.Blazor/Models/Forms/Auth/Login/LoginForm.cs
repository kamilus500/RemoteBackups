using System.ComponentModel.DataAnnotations;

namespace RemoteBackups.Blazor.Models.Forms.Auth.Login
{
    public class LoginForm
    {
        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "Field_login_required")]
        public string Login { get; set; } = string.Empty;

        [Required(
            ErrorMessageResourceType = typeof(Resources.Resources),
            ErrorMessageResourceName = "Field_password_required")]
        public string Password { get; set; } = string.Empty;
    }
}
