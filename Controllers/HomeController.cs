using System.Diagnostics; 
using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Controllers
{
    // Controlador principal de la aplicación
    public class HomeController : Controller
    {
        // Inyección de dependencias para el logger (sirve para registrar mensajes en consola o archivos)
        private readonly ILogger<HomeController> _logger;

        // Constructor que recibe un objeto logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Acción principal -> Muestra la página de inicio (Index.cshtml)
        public IActionResult Index()
        {
            return View();
        }

        // Acción secundaria -> Muestra la vista de "Privacidad" (Privacy.cshtml)
        public IActionResult Privacy()
        {
            return View();
        }

        // Acción para manejar errores de la aplicación
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
