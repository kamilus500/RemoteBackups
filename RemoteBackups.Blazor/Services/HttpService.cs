using RemoteBackups.Blazor.Services.Interfaces;
using System.Net.Http.Json;

namespace RemoteBackups.Blazor.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<T>(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wykonywania GET dla adresu: {Url}", url);
                return default;
            }
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, data);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TResponse>();
                }

                _logger.LogWarning("Zapytanie POST do {Url} zakończyło się statusem: {StatusCode}", url, response.StatusCode);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wyjątek podczas wykonywania POST (z odpowiedzią) dla adresu: {Url}", url);
                return default;
            }
        }

        public async Task<bool> PostAsync<TRequest>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wyjątek podczas wykonywania POST (bez odpowiedzi) dla adresu: {Url}", url);
                return false;
            }
        }

        public async Task<bool> PutAsync<TRequest>(string url, TRequest data)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(url, data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wykonywania PUT dla adresu: {Url}", url);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wykonywania DELETE dla adresu: {Url}", url);
                return false;
            }
        }
    }
}
