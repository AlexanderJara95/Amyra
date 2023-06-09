using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Globalization;
using Amyra.DTO;

public class Address
{
    public string Road { get; set; } // Nombre de la calle
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }
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

        
        public async Task<LocationDTO> GetAddress(string latitude, string longitude){
            
            string requestUrl = $"{API_URL}?lat={latitude}&lon={longitude}&format=json&zoom=18&addressdetails=1";
                        
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Referrer = new Uri("https://amyra.onrender.com/"); // Reemplaza con tu URL de referencia
                var response = await httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(result);

                    string direccion = data.address.building + " "+data.address.house_number+", "+ data.address.road;
                    string ciudad = data.address.city;
                    string departamento = data.address.state;
                    string pais = data.address.country;
                    return new LocationDTO
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        Direccion = direccion,
                        Ciudad = ciudad,
                        Departamento = departamento,
                        Pais = pais,
                    };

                }else
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
