using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Linq;

namespace ProyectoPrograVI.Controllers
{
    public class ReportesPedidosController : Controller
    {
        private readonly FunkoShop _repo;

        public ReportesPedidosController()
        {
            _repo = new FunkoShop();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ObtenerReporte(string usuario, string estado)
        {
            var pedidos = _repo.GetAllReporte();

            if (!string.IsNullOrEmpty(usuario))
                pedidos = pedidos.Where(p => p.Usuario.ToLower().Contains(usuario.ToLower())).ToList();

            if (!string.IsNullOrEmpty(estado))
                pedidos = pedidos.Where(p => p.Estado.ToLower().Contains(estado.ToLower())).ToList();

            return PartialView("_TablaReporte", pedidos);
        }
    }
}
