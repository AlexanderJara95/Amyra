using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;

using Amyra.Data;
using Amyra.Models;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Amyra.Integration;
using Amyra.DTO;

namespace Amyra.Controllers
{
    
    public class PagoController : Controller
    {
        private readonly ILogger<PagoController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly OpenStreetMapApiIntegration _apiIntegration;


        public PagoController(ILogger<PagoController> logger, UserManager<IdentityUser> userManager, ApplicationDbContext context,OpenStreetMapApiIntegration apiIntegration)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
            _apiIntegration = apiIntegration;
        }

        public IActionResult Create(Decimal monto)
        {
            ViewBag.Monto = monto;
            Pago pago = new Pago();
            pago.UserID = _userManager.GetUserName(User);
            pago.MontoTotal = monto;
            return View(pago);
        }

        [HttpPost]
        public IActionResult Pagar(Pago pago)
        {
            pago.PaymentDate = DateTime.UtcNow;
            _context.Add(pago);
            
            var itemsProforma = from o in _context.DataProformas select o;
            itemsProforma = itemsProforma.
                Include(p => p.Producto).
                Where(s => s.UserID.Equals(pago.UserID) && s.Status.Equals("PENDIENTE"));

            Pedido pedido = new Pedido();
            pedido.UserID = pago.UserID;
            pedido.Total = pago.MontoTotal;
            pedido.pago = pago;
            pedido.Status = "PENDIENTE";
            _context.Add(pedido);

            List<DetallePedido> itemsPedido = new List<DetallePedido>();
            foreach(var item in itemsProforma.ToList()){
                DetallePedido detallePedido = new DetallePedido();
                detallePedido.pedido=pedido;
                detallePedido.Precio = item.Precio;
                detallePedido.Producto = item.Producto;
                detallePedido.Cantidad = item.Cantidad;
                itemsPedido.Add(detallePedido);
            }


            _context.AddRange(itemsPedido);

            foreach (Proforma p in itemsProforma.ToList())
            {
                p.Status="PROCESADO";
            }

            _context.UpdateRange(itemsProforma);

            _context.SaveChanges();

            ViewData["Message"] = "El pago se ha registrado y su pedido nro "+ pedido.ID +" esta en camino";
            return View("Gracias");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }  
        [HttpPost]
        public async Task<IActionResult> ReceiveLocation(string latitude, string longitude, decimal monto)
        {
            // Hacer lo que necesites con la ubicación recibida del cliente
            // Puede ser almacenarla en una base de datos, realizar alguna acción específica, etc.
            // location.Latitude contiene la latitud y location.Longitude contiene la longitud

            // Obtener la dirección utilizando OpenStreetMapApiIntegration
            LocationDTO location = await _apiIntegration.GetAddress(latitude, longitude);

            // Devolver la ubicación y la dirección en la respuesta JSON
            if (location != null)
            {
                ViewData["direccion"] = location.Direccion;
                ViewData["ciudad"] = location.Ciudad;
                ViewData["departamento"] = location.Departamento;
                ViewData["pais"] = location.Pais;
            }
            Pago pago = new Pago();
            pago.UserID = _userManager.GetUserName(User);
            pago.MontoTotal = monto;
            return View("Create",pago);
        }

    }
}
