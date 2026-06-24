using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace TaskMaster.MVC.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;
        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private async Task<HttpClient> CreateClientAsync()
        {
            var client = _httpClientFactory.CreateClient("ApiClient");

            var context = _httpContextAccessor.HttpContext;

            if (context != null)
            {
                var token = await context.GetTokenAsync("TaskMasterCookie", "jwt");
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }

            return client;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var client = await CreateClientAsync();
            var response = await client.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, _jsonOptions);
            }

            return default;
        }

        public async Task<TResponse> PostAsync<TResponse, TRequest>(string endpoint, TRequest data)
        {
            var client = await CreateClientAsync();
            var jsonContent = JsonSerializer.Serialize(data);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions);
            }

            return default;
        }
        public async Task<bool> PutAsync<TRequest>(string endpoint, TRequest data)
        {
            var client = await CreateClientAsync();
            var jsonContent = JsonSerializer.Serialize(data);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, httpContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var client = await CreateClientAsync();
            var response = await client.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }

    }
}
