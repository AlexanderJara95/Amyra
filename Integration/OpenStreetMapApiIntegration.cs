using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class Address
{
    public string Road { get; set; } // Nombre de la calle
    public string City { get; set; }
}

namespace Amyra.Integration
{
    public class OpenStreetMapApiIntegration
    {
        private readonly ILogger<OpenStreetMapApiIntegration> _logger;
        private const string API_URL="https://nominatim.openstreetmap.org/reverse";

        private readonly HttpClient httpClient;

        public OpenStreetMapApiIntegration(ILogger<OpenStreetMapApiIntegration> logger)
        {
            _logger = logger;
            httpClient = new HttpClient();
        }

        
        public async Task<Address> GetAddress(string latitude, string longitude){
            double lat = Convert.ToDouble(latitude.Replace(',', '.'));
            double lon = Convert.ToDouble(longitude.Replace(',', '.'));
            string requestUrl = $"{API_URL}?lat={lat}&lon={lon}&format=json&zoom=18&addressdetails=1";
            
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
                return new Address
                    {
                        Road = requestUrl,
                        City = response.StatusCode.ToString(),
                    };
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(result);

                    string road = data.address.road;
                    string city = data.address.city;

                    return new Address
                    {
                        Road = road,
                        City = city
                    };
                }
                else
                {
                    _logger.LogError($"Error al llamar a la API: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al llamar a la API: {ex.Message}");
            }


            return null;
        }     


    }
}
