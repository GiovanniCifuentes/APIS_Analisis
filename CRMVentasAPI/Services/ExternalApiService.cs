// Services/ExternalApiService.cs
using CRMVentasAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CRMVentasAPI.Services
{
    public interface IExternalApiService
    {
        Task<string> GetTokenAsync();
        Task<List<ExternalCliente>> GetClientesAsync();
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _token = string.Empty;
        private DateTime _tokenExpiry = DateTime.MinValue;

        public ExternalApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri("https://gestioncontactosapi-production.up.railway.app");
        }

        public async Task<string> GetTokenAsync()
        {
            // Si el token es válido, lo retornamos
            if (!string.IsNullOrEmpty(_token) && DateTime.UtcNow < _tokenExpiry)
                return _token;

            var loginRequest = new LoginRequest
            {
                Email = "InventarioUMG@miumg.edu.gt",
                Password = "UMGinv2025$"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/Auth/Login", loginRequest);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al autenticar con la API externa");

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            _token = authResponse?.Token ?? throw new Exception("Token no recibido");
            _tokenExpiry = DateTime.UtcNow.AddHours(6); // El token dura 6 horas

            return _token;
        }

        public async Task<List<ExternalCliente>> GetClientesAsync()
        {
            var token = await GetTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Cliente");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error al obtener clientes de la API externa");

            var clientes = await response.Content.ReadFromJsonAsync<List<ExternalCliente>>();
            return clientes ?? new List<ExternalCliente>();
        }
    }
}