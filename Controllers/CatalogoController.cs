using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Amyra.Models;
using Amyra.Data;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;



namespace Amyra.Controllers
{
    
    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _dbcontext;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IDistributedCache _cache;

        public CatalogoController(ILogger<CatalogoController> logger,
                ApplicationDbContext context,
                IDistributedCache cache,
                SignInManager<IdentityUser> signInManager,
                UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _dbcontext = context;
            _cache = cache;
            _userManager = userManager;
            _signInManager = signInManager;

        }


        public async Task<IActionResult> Index(string? searchString)
        {
            
            var productos = from o in _dbcontext.DataProductos select o;
            //SELECT * FROM t_productos -> &
            if(!String.IsNullOrEmpty(searchString)){
                //productos = productos.Where(s => s.Name.Contains(searchString)); //Algebra de bool
                searchString = searchString.ToLower();
                productos = productos.Where(s => s.Name.ToLower().Contains(searchString));
                // & + WHERE name like '%ABC%'
            }
            productos = productos.Where(s => s.Status.Contains("Activo"));
            
            return View(await productos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id){
            Producto objProduct = await _dbcontext.DataProductos.FindAsync(id);
            if(objProduct == null){
                return NotFound();
            }
            var productos = from o in _dbcontext.DataProductos select o;
            productos = productos.Where(s => s.Status.Contains("Activo"));
            ViewBag.Productos = productos.ToList();
            /*ViewBag.RelatedProducts = await _dbcontext.DataProductos
            .Where(p => p.Categoria == objProduct.Categoria && p.Id != objProduct.Id)
            .Take(4)
            .ToListAsync();*/
            return View(objProduct);
        }

        public async Task<IActionResult> Add(int? id){
            var userID = _userManager.GetUserName(User); //sesion
             /*
            IdentityUser myidentity  = await Task.Run(() => _userManager.GetUserAsync(User));
            var roles = await Task.Run(() => _userManager.GetRolesAsync(myidentity));
            if(roles.Contains("admin") ){
            }   
            */
            if(userID == null){
                ViewData["Message"] = "Por favor debe loguearse antes de agregar un producto";
                List<Producto> productos = new List<Producto>();
                return  View("Index",productos);
            }else{
                var producto = await _dbcontext.DataProductos.FindAsync(id);

                Proforma proforma = new Proforma();
                proforma.Producto = producto;
                proforma.Precio = producto.Precio; //precio del producto en ese momento
                proforma.Cantidad = 1;
                proforma.UserID = userID;
                _dbcontext.Add(proforma);
                await _dbcontext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}