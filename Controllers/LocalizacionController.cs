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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}