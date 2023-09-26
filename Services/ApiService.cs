using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace WebApplicationIceCreamProject.Services
{
	public class ApiService
	{
        private readonly HttpClient _httpClient;
        private readonly string _apiKey; // Replace with your API key

        public ApiService(string apiKey)
        {
            // Increase the timeout to 5 minutes (300 seconds)
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(300)
            };
            _apiKey = apiKey;
        }

        public async Task<T> GetApiResponseAsync<T>(string apiUrl)
        {
            try
            {
                // Set any required headers or parameters for the API request here
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize the API response to the specified type (T)
                    T responseData = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(responseContent);
                    return responseData;
                }
                else
                {
                    // Handle API error response here
                    throw new Exception($"API request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception($"API request failed: {ex.Message}");
            }
        }
    }
}
