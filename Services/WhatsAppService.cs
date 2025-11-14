using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RlucenaWHT.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        // Placeholders for your actual data
        private readonly string _metaApiUrl = "https://graph.facebook.com/v15.0/YOUR_PHONE_NUMBER_ID/messages";
        private readonly string _apiToken = "YOUR_API_TOKEN";

        public WhatsAppService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
        }

        public async Task<bool> EnviarAvisoCita(string mobileNumber, string appointmentTime, string clientName)
        {
            // Manual JSON construction
            var jsonPayload = $@"{{
                ""messaging_product"": ""whatsapp"",
                ""to"": ""{mobileNumber}"",
                ""type"": ""template"",
                ""template"": {{
                    ""name"": ""aviso_cita"",
                    ""language"": {{ ""code"": ""es_ES"" }},
                    ""components"": [
                        {{
                            ""type"": ""body"",
                            ""parameters"": [
                                {{ ""type"": ""text"", ""text"": ""{clientName}"" }},
                                {{ ""type"": ""text"", ""text"": ""{appointmentTime}"" }}
                            ]
                        }}
                    ]
                }}
            }}";

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_metaApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to send WhatsApp message. Status: {response.StatusCode}, Response: {errorContent}");
                    return false;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error sending WhatsApp message: {e.Message}");
                return false;
            }
        }
    }
}
