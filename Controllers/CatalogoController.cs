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


namespace Amyra.Controllers
{

    public class CatalogoController : Controller
    {
        private readonly ILogger<CatalogoController> _logger;
        private readonly ApplicationDbContext _dbcontext;        
        private readonly IDistributedCache _cache;

        public CatalogoController(ILogger<CatalogoController> logger, ApplicationDbContext context, IDistributedCache cache)
        {
            _logger = logger;
            _dbcontext = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var productos = from o in _dbcontext.DataProductos select o;
            //SELECT * FROM t_productos -> &
            if(!String.IsNullOrEmpty(searchString)){
                productos = productos.Where(s => s.Name.Contains(searchString)); //Algebra de bool
                // & + WHERE name like '%ABC%'
            }
            productos = productos.Where(s => s.Status.Contains("Activo"));
            Response.Headers["Cache-Control"] = "max-age=3600, public";
            return View(productos.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}