using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Linq;

namespace ProyectoPrograVI.Controllers
{
    public class ReportesController : Controller
    {
        // Repositorio para acceder a los datos de la base
        private readonly FunkoShop _repo;

        // Constructor: inicializa el repositorio
        public ReportesController()
        {
            _repo = new FunkoShop();
        }

        // GET: Vista principal del módulo de reportes
        public IActionResult Index()
        {
            return View(); // Muestra la página principal de reportes
        }

        // POST: Obtener reporte filtrado
        [HttpPost]
        public IActionResult ObtenerReporte(string nombre, decimal? precio)
        {
            // Obtener todos los productos de la base de datos
            var productos = _repo.GetAll();

            // Filtro por nombre (ignora mayúsculas/minúsculas)
            if (!string.IsNullOrEmpty(nombre))
                productos = productos.Where(p => p.Nombre.ToLower().Contains(nombre.ToLower())).ToList();

            // Filtro por precio máximo
            if (precio.HasValue)
                productos = productos.Where(p => p.Precio <= precio.Value).ToList();

            // Retorna un PartialView con los resultados filtrados
            // "_TablaReporte" es una vista parcial que muestra la tabla de productos
            return PartialView("_TablaReporte", productos);
        }
    }
}
