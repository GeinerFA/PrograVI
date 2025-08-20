using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

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
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("Rol");

            List<Pedido> pedidos;

            if (rol == "Administrador")
            {
                // Administradores ven todos los pedidos
                pedidos = _repositorio.GetAllPedido();
            }
            else
            {
                // Usuarios normales ven solo sus pedidos
                pedidos = _repositorio.GetPedidosByUsuarioId(usuarioId.ToString());
            }

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
            // Verificar permisos antes de editar
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
            {
                return RedirectToAction("Index", "Home"); // O mostrar error de permisos
            }

            var pedido = _repositorio.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

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
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("Rol");

            var pedido = _repositorio.ObtenerDetallepedido(id);

            if (pedido == null)
                return NotFound();

            // Verificar que el usuario tenga permiso para ver este pedido
            if (rol != "Administrador" && pedido.UserId != usuarioId.ToString())
            {
                return RedirectToAction("Index", "Home"); // O mostrar error de permisos
            }

            return View("DetallePedido", pedido);
        }


    }
}
