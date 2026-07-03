using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RemoteBackups.Blazor.Models.Contracts.Auth.Register;
using RemoteBackups.Blazor.Models.Forms.Auth.Register;
using RemoteBackups.Blazor.Providers;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Pages.Auth
{
    public partial class Register
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }

        private RegisterForm registerModel = new RegisterForm();
        private string? errorMessage;
        private string? successMessage;
        private bool isSubmitting;

        protected override async Task OnInitializedAsync()
        {
            var authState = await ((CustomAuthStateProvider)AuthStateProvider).GetAuthenticationStateAsync();

            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                Navigation.NavigateTo("/");
            }
        }

        private async Task HandleRegister()
        {
            errorMessage = null;
            successMessage = null;
            isSubmitting = true;

            try
            {
                var command = new RegisterUserCommand(registerModel.Login, registerModel.Password);

                var response = await UserService.RegisterAsync(command);

                if (response != null)
                {
                    successMessage = "Konto zostało utworzone! Przekierowanie do logowania...";
                    StateHasChanged();

                    await Task.Delay(2000);
                    Navigation.NavigateTo("/login");
                }
                else
                {
                    errorMessage = "Rejestracja nie powiodła się. Ten login może być już zajęty.";
                }
            }
            catch (Exception)
            {
                errorMessage = "Wystąpił nieoczekiwany błąd serwera. Spróbuj ponownie później.";
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
