public class Task<IActionResult> Register(RegistroModel modelo)
{
    // Aquí puedes utilizar las propiedades del modelo recibido, como por ejemplo:
    var nombre = modelo.Nombre;
    var apellido = modelo.Apellido;
    var dni = modelo.Dni;
    var direccion = modelo.Direccion;
    var celular = modelo.Celular;
    var email = modelo.Email;
    var password = modelo.Password;

    // Código para guardar el registro en la base de datos, enviar un correo, etc.

    return RedirectToAction("Index", "Home");
}