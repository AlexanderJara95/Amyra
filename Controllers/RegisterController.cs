/*using Amyra.Models;
using Microsoft.AspNetCore.Mvc;

namespace Amyra.Controllers
{
    public class RegisterController : Controller
    {
        // GET: /register/
        public IActionResult Index()
        {
            return View();
        }

        // POST: /register/
        [HttpPost]
        public IActionResult Index(Register model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Guardar los datos de registro en la base de datos.
                return RedirectToAction("Success");
            }

            return View(model);
        }

        // GET: /register/success
        public IActionResult Success()
        {
            return View();
        }
    }
}
*/