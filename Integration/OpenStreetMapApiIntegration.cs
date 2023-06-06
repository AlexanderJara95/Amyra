using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
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
        private readonly ILogger<CurrencyExchangeApiIntegration> _logger;

        private readonly HttpClient httpClient;

        public OpenStreetMapApiIntegration(ILogger<CurrencyExchangeApiIntegration> logger){
            _logger = logger;
            httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Add(API_HEADER_KEY, API_KEY);
        }

        public async Task<Address> GetExchangeRate(double latitude, double longitude){
            
            string requestUrl = $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json&zoom=18&addressdetails=1";
            HttpResponseMessage response = await httpClient.GetAsync(requestUrl);
            try{
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(result);

                    string road = data.address.road;
                    string city = data.address.city;

                    return new Address
                    {
                        Road = road,
                        City = city
                    };
                }
            }catch(Exception ex){
                _logger.LogDebug($"Error al llamar a la API: {ex.Message}");
            }
            return null;
        }

    }
    

}