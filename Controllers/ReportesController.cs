using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Linq;

namespace ProyectoPrograVI.Controllers
{
    public class ReportesController : Controller
    {
        private readonly FunkoShop _repo;

        public ReportesController()
        {
            _repo = new FunkoShop();
        }

        public IActionResult Index()
        {
            return View(); // Vista principal del módulo de reportes
        }

        [HttpPost]
        public IActionResult ObtenerReporte(string nombre, decimal? precio)
        {
            var productos = _repo.GetAll();

            if (!string.IsNullOrEmpty(nombre))
                productos = productos.Where(p => p.Nombre.ToLower().Contains(nombre.ToLower())).ToList();

            if (precio.HasValue)
                productos = productos.Where(p => p.Precio <= precio.Value).ToList();

            return PartialView("_TablaReporte", productos);
        }
    }
}
