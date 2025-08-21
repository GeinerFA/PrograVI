using System.Diagnostics; 
using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Controllers
{
    // Controlador principal de la aplicaci�n
    public class HomeController : Controller
    {
        // Inyecci�n de dependencias para el logger (sirve para registrar mensajes en consola o archivos)
        private readonly ILogger<HomeController> _logger;

        // Constructor que recibe un objeto logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Acci�n principal -> Muestra la p�gina de inicio (Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }

        // Acci�n secundaria -> Muestra la vista de "Privacidad" (Privacy.cshtml)
        public IActionResult Privacy()
        {
            return View();
        }

        // Acci�n para manejar errores de la aplicaci�n
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Devuelve la vista Error.cshtml con un modelo que contiene el RequestId
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
