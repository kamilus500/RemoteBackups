using System.ComponentModel.DataAnnotations;

namespace RemoteBackups.Blazor.Models.Forms.Auth.Login
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Pole Login jest wymagane.")]
        [MinLength(3, ErrorMessage = "Login musi mieć minimum 3 znaki.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        public string Password { get; set; } = string.Empty;
    }
}
