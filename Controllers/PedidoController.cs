using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Security.Claims; // 👈 para acceder al userId

namespace ProyectoPrograVI.Controllers
{
    public class PedidoController : Controller
    {
        private readonly FunkoShop _repositorio;

        public PedidoController()
        {
            _repositorio = new FunkoShop();
        }

        // Listar todos los pedidos
        public IActionResult Index()
        {
            var pedidos = _repositorio.GetAllPedido();
            return View(pedidos);
        }

        // Crear un nuevo pedido desde el carrito
        [Authorize] // opcional: si quieres que solo usuarios logueados puedan comprar
        [HttpPost]
        public IActionResult CrearPedido(List<CarritoItem> carrito)
        {
            if (carrito == null || !carrito.Any())
            {
                return BadRequest("El carrito está vacío");
            }

            // Obtener el id del usuario autenticado
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : "Anonimo";

            var usuario = User.Identity.IsAuthenticated
                ? User.Identity.Name
                : "Anonimo";

            // Construir pedido
            var pedido = new Pedido
            {
                UserId = userId,
                Usuario = usuario,
                Fecha = DateTime.Now,
                Total = carrito.Sum(c => c.Subtotal),
                Estado = "En Proceso",
                Detalles = carrito.Select(c => new PedidoDetalle
                {
                    ProductoId = c.IdProducto,
                    NombreProducto = c.Nombre,
                    Precio = c.Precio,
                    Cantidad = c.Cantidad,
                    Subtotal = c.Subtotal
                }).ToList()
            };

            // Guardar en la BD
            int pedidoId = _repositorio.CrearPedido(pedido);

            return RedirectToAction("Index");
        }

        // Editar estado (GET)
        [HttpGet]
        public IActionResult EditarEstado(int id)
        {
            var pedido = _repositorio.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            // Lista de estados
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "En Proceso", Text = "En Proceso" },
                new SelectListItem { Value = "Enviado", Text = "Enviado" },
                new SelectListItem { Value = "Entregado", Text = "Entregado" }
            };

            return View(pedido);
        }

        // Editar estado (POST)
        [HttpPost]
        public IActionResult EditarEstado(int id, string estado)
        {
            if (!string.IsNullOrEmpty(estado))
            {
                _repositorio.ActualizarEstado(id, estado);
                return RedirectToAction("Index");
            }

            var pedido = _repositorio.GetById(id);
            if (pedido == null)
            {
                return NotFound();
            }

            // Recargar estados si hubo error
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "En Proceso", Text = "En Proceso" },
                new SelectListItem { Value = "Enviado", Text = "Enviado" },
                new SelectListItem { Value = "Entregado", Text = "Entregado" }
            };

            return View(pedido);
        }
        public IActionResult DetallePedido(int id)
        {
            var repo = new FunkoShop();
            var pedido = repo.ObtenerDetallepedido(id);
            if (pedido == null) return NotFound();

            return View("DetallePedido", pedido); // fuerza a usar DetallePedido.cshtml
        }


    }
}
