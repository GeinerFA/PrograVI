using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Linq;

namespace ProyectoPrograVI.Controllers
{
    public class ReportesPedidosController : Controller
    {
        // Repositorio para acceder a los datos de la base de datos
        private readonly FunkoShop _repo;

        // Constructor: inicializa el repositorio
        public ReportesPedidosController()
        {
            _repo = new FunkoShop();
        }

        // GET: Vista principal del módulo de reportes de pedidos
        public IActionResult Index()
        {
            return View(); // Retorna la vista principal de reportes
        }

        // POST: Obtener reporte filtrado de pedidos
        [HttpPost]
        public IActionResult ObtenerReporte(string usuario, string estado)
        {
            // Se obtienen todos los pedidos desde el repositorio
            var pedidos = _repo.GetAllReporte();

            // 🔹 Filtro por nombre de usuario (ignora mayúsculas/minúsculas)
            if (!string.IsNullOrEmpty(usuario))
                pedidos = pedidos.Where(p => p.Usuario.ToLower().Contains(usuario.ToLower())).ToList();

            // 🔹 Filtro por estado del pedido (ej: "Pendiente", "Completado", etc.)
            if (!string.IsNullOrEmpty(estado))
                pedidos = pedidos.Where(p => p.Estado.ToLower().Contains(estado.ToLower())).ToList();

            // Retorna la vista parcial con los pedidos filtrados
            // "_TablaReporte" es la vista parcial que renderiza la tabla
            return PartialView("_TablaReporte", pedidos);
        }
    }
}
