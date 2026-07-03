using RemoteBackups.Blazor.Services.Interfaces;
using System.Net.Http.Headers;

namespace RemoteBackups.Blazor.Services
{
    public class JwtInterceptor : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public JwtInterceptor(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync("authToken");

            if (!string.IsNullOrWhiteSpace(token) && request.Headers.Authorization is null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
