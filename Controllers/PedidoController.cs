using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc; 
using Microsoft.AspNetCore.Mvc.Rendering; 
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;
using System.Security.Claims; 
using Microsoft.AspNetCore.Http; 

namespace ProyectoPrograVI.Controllers
{
    // Controlador para gestionar pedidos
    public class PedidoController : Controller
    {
        private readonly FunkoShop _repositorio;

        // Constructor -> inicializa el repositorio de datos
        public PedidoController()
        {
            _repositorio = new FunkoShop();
        }

        // ============================
        // LISTAR PEDIDOS
        // ============================
        public IActionResult Index()
        {
            // Obtener usuario y rol desde la sesión
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("Rol");

            List<Pedido> pedidos;

            if (rol == "Administrador")
            {
                // Los administradores ven todos los pedidos
                pedidos = _repositorio.GetAllPedido();
            }
            else
            {
                // Los usuarios normales solo ven sus propios pedidos
                pedidos = _repositorio.GetPedidosByUsuarioId(usuarioId.ToString());
            }

            return View(pedidos); // Pasamos la lista de pedidos a la vista
        }

        // ============================
        // CREAR PEDIDO DESDE EL CARRITO
        // ============================
        [Authorize] // Solo usuarios logueados pueden crear pedidos
        [HttpPost]
        public IActionResult CrearPedido(List<CarritoItem> carrito)
        {
            // Validación: si el carrito está vacío, error
            if (carrito == null || !carrito.Any())
            {
                return BadRequest("El carrito está vacío");
            }

            // Obtener id y nombre del usuario autenticado
            var userId = User.Identity.IsAuthenticated
                ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : "Anonimo";

            var usuario = User.Identity.IsAuthenticated
                ? User.Identity.Name
                : "Anonimo";

            // Construir el pedido con datos del carrito
            var pedido = new Pedido
            {
                UserId = userId,
                Usuario = usuario,
                Fecha = DateTime.Now,
                Total = carrito.Sum(c => c.Subtotal),
                Estado = "En Proceso", // Estado inicial del pedido
                Detalles = carrito.Select(c => new PedidoDetalle
                {
                    ProductoId = c.IdProducto,
                    NombreProducto = c.Nombre,
                    Precio = c.Precio,
                    Cantidad = c.Cantidad,
                    Subtotal = c.Subtotal
                }).ToList()
            };

            // Guardar pedido en BD
            int pedidoId = _repositorio.CrearPedido(pedido);

            return RedirectToAction("Index"); // Redirige a la lista de pedidos
        }

        // ============================
        // EDITAR ESTADO DEL PEDIDO (GET)
        // ============================
        [HttpGet]
        public IActionResult EditarEstado(int id)
        {
            // Verificar permisos -> solo administradores pueden editar estado
            var rol = HttpContext.Session.GetString("Rol");
            if (rol != "Administrador")
            {
                return RedirectToAction("Index", "Home"); // Sin permisos -> redirige
            }

            var pedido = _repositorio.GetById(id);

            if (pedido == null)
            {
                return NotFound();
            }

            // Lista de estados posibles para el pedido (dropdown en la vista)
            ViewBag.Estados = new List<SelectListItem>
            {
                new SelectListItem { Value = "En Proceso", Text = "En Proceso" },
                new SelectListItem { Value = "Enviado", Text = "Enviado" },
                new SelectListItem { Value = "Entregado", Text = "Entregado" }
            };

            return View(pedido);
        }

        // ============================
        // EDITAR ESTADO DEL PEDIDO (POST)
        // ============================
        [HttpPost]
        public IActionResult EditarEstado(int id, string estado)
        {
            if (!string.IsNullOrEmpty(estado))
            {
                // Actualiza el estado en BD
                _repositorio.ActualizarEstado(id, estado);
                return RedirectToAction("Index");
            }

            // Si hubo error, recargar pedido y lista de estados
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

        // ============================
        // DETALLE DE UN PEDIDO
        // ============================
        public IActionResult DetallePedido(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            var rol = HttpContext.Session.GetString("Rol");

            var pedido = _repositorio.ObtenerDetallepedido(id);

            if (pedido == null)
                return NotFound();

            // Solo el admin o el dueño del pedido pueden verlo
            if (rol != "Administrador" && pedido.UserId != usuarioId.ToString())
            {
                return RedirectToAction("Index", "Home"); // Sin permisos
            }

            return View("DetallePedido", pedido); // Muestra la vista con detalle
        }
    }
}
