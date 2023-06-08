using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Amyra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using Amyra.Models;
using Amyra.Integration;


namespace Amyra.Controllers
{
    public class ProformaController : Controller
    {
        private readonly ILogger<ProformaController> _logger;
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly OpenStreetMapApiIntegration _apiIntegration;

        public ProformaController(
            ILogger<ProformaController> logger,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager
            )
        {
            _logger = logger;
            _dbcontext = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var userIDSession = _userManager.GetUserName(User);

            //SELECT * FROM Proforma p,Producto pr WHERE p.productId=pr.Id And p.UserId=? And p.status='PENDIENTE' 
            var items = from o in _dbcontext.DataProformas select o;
            items = items.Include(p => p.Producto).
                    Where(w => w.UserID.Equals(userIDSession) &&
                     w.Status.Equals("PENDIENTE"));
            var itemsCarrito = items.ToList();
            //Fila1 1234, Shampo; Precio, Cantidad
            //Fila2 12345, Shampo3; Precio, Cantidad
            var total = itemsCarrito.Sum(c => c.Cantidad * c.Precio);

            //MEMORIA
            dynamic model = new ExpandoObject();
            model.montoTotal = total;
            model.elementosCarrito = itemsCarrito;

            //Carrito carrito = new Carrito();
            //carrito.total = total;
            //carrito.itemsCarrito = itemsCarrito;

            //return View(carrito);
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public class Producto{
            public String Name {get; set;}
            public String ImageUrl {get; set;}
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? Id){
                if (Id == null){
                    return NotFound();
                }

                var proforma = await _dbcontext.DataProformas.FindAsync(Id);
                if(proforma == null){
                    return NotFound();
                }
                _dbcontext.DataProformas.Remove(proforma);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

        [HttpPost]
        public async Task<IActionResult> Agregar(int productoId){
            // Obtener el producto según el ID
            var producto = await _dbcontext.DataProductos.FindAsync(productoId);

            if (producto == null){
                return NotFound();
            }

            // Crear un nuevo objeto Proforma y asignar los valores
            var proforma = new Proforma{
                Producto = producto,
                Cantidad = 1,
                Precio = producto.Precio,
                Status = "PENDIENTE",
                UserID = _userManager.GetUserName(User)
            };

            // Agregar la proforma al contexto de la base de datos
            _dbcontext.DataProformas.Add(proforma);
            await _dbcontext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> LimpiarCarrito(){
            var userIDSession = _userManager.GetUserName(User);

            // Obtener todas las proformas del usuario actual
            var proformas = await _dbcontext.DataProformas
                .Where(p => p.UserID.Equals(userIDSession))
                .ToListAsync();

            // Eliminar las proformas del contexto de la base de datos
            _dbcontext.DataProformas.RemoveRange(proformas);
            await _dbcontext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult PagarTotal(){
            // Lógica para procesar el pago y generar el resultado de la transacción
            // Crear un modelo para mostrar el resultado de la transacción
            var model = new PagoTotalViewModel
            {
                TransaccionExitosa = true, // Cambia esto según la lógica de tu aplicación
                Mensaje = "La transacción se realizó exitosamente." // Cambia esto según la lógica de tu aplicación
            };

            return View("PagarTotal", model);
        }

        public class PagoTotalViewModel{
            public bool TransaccionExitosa { get; set; }
            public string Mensaje { get; set; }
        }      

    }
}