using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Amyra.Integration;

namespace Amyra.Controllers.Rest
{
    [ApiController]
    [Route("api/geo")]
    public class LocalizacionApiConroller : ControllerBase
    {
        private readonly OpenStreetMapApiIntegration _apiIntegration;

        public LocalizacionApiConroller(OpenStreetMapApiIntegration apiIntegration)
        {
            _apiIntegration = apiIntegration;
        }

        [HttpPost("location")]
        public async Task<IActionResult> ReceiveLocation([FromBody] LocationModel location)
        {
            // Hacer lo que necesites con la ubicación recibida del cliente
            // Puede ser almacenarla en una base de datos, realizar alguna acción específica, etc.
            // location.Latitude contiene la latitud y location.Longitude contiene la longitud

            // Obtener la dirección utilizando OpenStreetMapApiIntegration
            Address address = await _apiIntegration.GetExchangeRate(location.Latitude, location.Longitude);

            // Devolver la ubicación y la dirección en la respuesta JSON
            return Ok(new
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Road = address?.Road,
                City = address?.City
            });
        }
        public class LocationModel
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Road { get; set; }
            public string City { get; set; }
        }
    }
}
