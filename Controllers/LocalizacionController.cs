using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Amyra.DTO;
using Amyra.Integration;

namespace Amyra.Controllers
{
    public class LocalizacionController : Controller
    {
        private readonly ILogger<TipoCambioController> _logger;
        private readonly OpenStreetMapApiIntegration _apiIntegration;

        public LocalizacionController(ILogger<TipoCambioController> logger,
        OpenStreetMapApiIntegration apiIntegration)
        {
            _logger = logger;
            _apiIntegration = apiIntegration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveLocation(string latitude, string longitude)
        {
            // Hacer lo que necesites con la ubicación recibida del cliente
            // Puede ser almacenarla en una base de datos, realizar alguna acción específica, etc.
            // location.Latitude contiene la latitud y location.Longitude contiene la longitud

            // Obtener la dirección utilizando OpenStreetMapApiIntegration
            Address address = await _apiIntegration.GetAddress(latitude, longitude);

            // Devolver la ubicación y la dirección en la respuesta JSON
            return Ok(new
            {
                Road = address?.Road,
                City = address?.City
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}