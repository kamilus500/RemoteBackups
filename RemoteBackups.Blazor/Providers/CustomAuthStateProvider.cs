using Microsoft.AspNetCore.Components.Authorization;
using RemoteBackups.Blazor.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace RemoteBackups.Blazor.Providers
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationState _anonymous;
        private const string TokenKey = "authToken";

        public CustomAuthStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            _anonymous = new AuthenticationState(anonymousUser);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _localStorageService.GetItemAsync(TokenKey);

                if (string.IsNullOrWhiteSpace(token))
                {
                    return _anonymous;
                }

                var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            catch (Exception)
            {
                return _anonymous;
            }
        }

        public async Task NotifyUserLogin(string token)
        {
            await _localStorageService.SetItemAsync(TokenKey, token);

            var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            var authState = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task NotifyUserLogout()
        {
            await _localStorageService.RemoveItemAsync(TokenKey);

            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                if (keyValuePairs.TryGetValue(ClaimTypes.Name, out var name) ||
                    keyValuePairs.TryGetValue("unique_name", out name) ||
                    keyValuePairs.TryGetValue("sub", out name))
                {
                    claims.Add(new Claim(ClaimTypes.Name, name.ToString() ?? string.Empty));
                }
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}