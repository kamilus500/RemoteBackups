using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using RemoteBackups.Blazor.Models.Contracts.Auth.LoginUser;
using RemoteBackups.Blazor.Models.Forms.Auth.Login;
using RemoteBackups.Blazor.Providers;
using RemoteBackups.Blazor.Services.Interfaces;

namespace RemoteBackups.Blazor.Pages.Auth
{
    public partial class Login
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IUserService UserService { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }

        private LoginForm loginModel = new LoginForm();
        private string? errorMessage;
        private bool isSubmitting;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();

            if (authState.User.Identity?.IsAuthenticated ?? false)
            {
                Navigation.NavigateTo("/");
            }
        }

        private async Task HandleLogin()
        {
            errorMessage = null;
            isSubmitting = true;

            try
            {
                var command = new LoginUserCommand(loginModel.Login, loginModel.Password);

                var response = await UserService.LoginAsync(command);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    if (AuthStateProvider is CustomAuthStateProvider customProvider)
                    {
                        await customProvider.NotifyUserLogin(response.Token);
                    }

                    Navigation.NavigateTo("/");
                }
                else
                {
                    errorMessage = "Nieprawidłowy login lub hasło.";
                }
            }
            catch (Exception)
            {
                errorMessage = "Wystąpił problem z połączeniem. Spróbuj ponownie później.";
            }
            finally
            {
                isSubmitting = false;
            }
        }
    }
}
