using Backend.Interfaces;
using System.Text.Json;
using System.Text;
using Backend.Infrastructure;
using Microsoft.Extensions.Options;
using Backend.Models.DTOs.PaymentDTOs;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Http.Headers;

namespace Backend.Services
{
    public class YooKassaService : IYookassaService
    {
        private readonly HttpClient _httpClient;
        private readonly YookassaSettings _settings;

        public YooKassaService(
            IOptions<YookassaSettings> settings)
        {
            _settings = settings.Value;
            var isTest = _settings.TestMode;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(isTest
                    ? "https://api.yookassa.ru/v3/"
                    : "https://api.yookassa.ru/v3/")
            };

            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ShopId}:{_settings.SecretKey}"));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authString}");
        }

        public async Task<(bool success, string? paymentId, string? paymentUrl, string? error)> CreatePaymentAsync(
            decimal amount, string description, int bookingId, int userId)
        {
            try
            {
                var request = new YooKassaPaymentRequest
                {
                    Amount = new Amount
                    {
                        Value = amount.ToString("F2", CultureInfo.InvariantCulture),
                        Currency = "RUB"
                    },
                    Description = description,
                    Confirmation = new Confirmation
                    {
                        Type = "redirect",
                        Return_url = $"{_settings.ReturnUrl}"
                    },
                    Metadata = new Metadata
                    {
                        BookingId = bookingId.ToString(),
                        UserId = userId.ToString()
                    },
                    Capture = "true"
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Idempotence-Key");
                _httpClient.DefaultRequestHeaders.Add("Idempotence-Key", Guid.NewGuid().ToString());

                var response = await _httpClient.PostAsync("payments", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return (false, null, null, $"YooKassa error: {responseJson}");
                }

                var paymentResponse = JsonSerializer.Deserialize<YooKassaPaymentResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (paymentResponse == null)
                    return (false, null, null, "Failed to parse YooKassa response");

                return (true, paymentResponse.Id, paymentResponse.Confirmation?.Confirmation_url, null);
            }
            catch (Exception ex)
            {
                return (false, null, null, ex.Message);
            }
        }

        public async Task<bool> CancelPaymentAsync(string paymentId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Remove("Idempotence-Key");
                _httpClient.DefaultRequestHeaders.Add("Idempotence-Key", Guid.NewGuid().ToString());
                var response = await _httpClient.PostAsync($"payments/{paymentId}/cancel", null);
                var responseJson = await response.Content.ReadAsStringAsync();

                Console.WriteLine(responseJson.ToString());

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<(bool success, string? refundId, string? error)> CreateRefundAsync(
                                                                                    string paymentId,
                                                                                    decimal amount,
                                                                                    string description)
        {
            try
            {
                var request = new
                {
                    payment_id = paymentId,
                    amount = new
                    {
                        value = amount.ToString("F2", CultureInfo.InvariantCulture),
                        currency = "RUB"
                    },
                    description = description
                };

                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                Console.WriteLine($"PaymentId: {paymentId}, Amount: {amount}, Description: {description}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                _httpClient.DefaultRequestHeaders.Remove("Idempotence-Key");
                _httpClient.DefaultRequestHeaders.Add("Idempotence-Key", Guid.NewGuid().ToString());

                var response = await _httpClient.PostAsync("refunds", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return (false, null, responseJson);

                using var doc = JsonDocument.Parse(responseJson);
                var refundId = doc.RootElement.GetProperty("id").GetString();

                return (true, refundId, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }
    }
}
