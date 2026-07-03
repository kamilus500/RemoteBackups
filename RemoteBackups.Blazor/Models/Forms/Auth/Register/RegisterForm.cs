using System.ComponentModel.DataAnnotations;

namespace RemoteBackups.Blazor.Models.Forms.Auth.Register
{
    public class RegisterForm
    {
        [Required(ErrorMessage = "Pole Login jest wymagane.")]
        [MinLength(3, ErrorMessage = "Login musi mieć minimum 3 znaki.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pole Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć minimum 6 znaków.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
        [Compare(nameof(Password), ErrorMessage = "Podane hasła nie są identyczne.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
