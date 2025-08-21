using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;
using ProyectoPrograVI.Data;
using Microsoft.AspNetCore.Http;
using ProyectoPrograVI.Services;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoPrograVI.Controllers
{
    public class CarritoController : Controller
    {
        // Dependencia hacia el repositorio (acceso a productos, pedidos, etc.)
        private readonly FunkoShop _repositorio;

        // Servicio de pagos para procesar la compra
        private readonly PagoService _pagoService;

        // Clave de sesión donde se guarda el carrito
        private const string CarritoSessionKey = "Carrito";

        // Constructor: recibe el servicio de pagos e inicializa el repositorio
        public CarritoController(PagoService pagoService)
        {
            _repositorio = new FunkoShop();
            _pagoService = pagoService;
        }

        // ==============================================
        // AGREGAR PRODUCTO AL CARRITO
        // ==============================================
        [HttpPost]
        public IActionResult AgregarAlCarrito(int idProducto, int cantidad)
        {
            // Buscar producto en la BD
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == idProducto);
            if (producto == null)
                return NotFound(); // Si no existe, devolvemos 404

            // Recuperar el carrito de la sesión
            var carrito = ObtenerCarrito();

            // Revisar si el producto ya estaba en el carrito
            var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == idProducto);
            if (itemExistente != null)
            {
                // Si ya existe, solo se suma la cantidad
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                // Si no existe, se agrega como un nuevo item
                carrito.Add(new CarritoItem
                {
                    IdProducto = producto.Id,
                    Nombre = producto.Nombre,
                    ImagenUrl = producto.ImagenUrl,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            // Guardamos el carrito en la sesión
            GuardarCarrito(carrito);

            // Mensaje temporal para mostrar en la vista
            TempData["Mensaje"] = "Producto agregado al carrito";

            return RedirectToAction("Index", "Productos"); // Redirige a productos
        }

        // ==============================================
        // MOSTRAR CARRITO
        // ==============================================
        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito); // Retorna la vista con la lista de items
        }

        // ==============================================
        // ELIMINAR PRODUCTO DEL CARRITO
        // ==============================================
        public IActionResult Eliminar(int id)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);
            if (item != null)
            {
                carrito.Remove(item); // Se elimina del carrito
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index"); // Redirige al carrito
        }

        // ==============================================
        // FINALIZAR COMPRA (GET) → muestra el formulario de pago
        // ==============================================
        [HttpGet]
        public IActionResult FinalizarCompra()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                return RedirectToAction("Index"); // Si no hay productos, vuelve al carrito
            }

            decimal total = carrito.Sum(item => item.Subtotal); // Calcula total
            ViewBag.Total = total;

            // Devuelve vista con el monto a pagar
            return View(new PagoRequest { Monto = total });
        }

        // ==============================================
        // FINALIZAR COMPRA (POST) → procesa el pago
        // ==============================================
        [HttpPost]
        public async Task<IActionResult> FinalizarCompra(PagoRequest pagoRequest)
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                return RedirectToAction("Index"); // Si el carrito está vacío
            }

            // Validar stock de cada producto antes de pagar
            foreach (var item in carrito)
            {
                var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == item.IdProducto);
                if (producto == null || producto.CantStock < item.Cantidad)
                {
                    // Si no hay stock suficiente, mostramos error
                    ModelState.AddModelError("", $"No hay suficiente stock para: {item.Nombre}");
                    ViewBag.Total = carrito.Sum(item => item.Subtotal);
                    return View(pagoRequest);
                }
            }

            // Si el modelo de pago no es válido, se vuelve a la vista
            if (!ModelState.IsValid)
            {
                ViewBag.Total = carrito.Sum(item => item.Subtotal);
                return View(pagoRequest);
            }

            // Procesar el pago con el servicio
            var resultadoPago = await _pagoService.ProcesarPago(pagoRequest);

            if (resultadoPago.Aprobado)
            {
                // Recuperar datos del usuario desde Session
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var nombreUsuario = HttpContext.Session.GetString("NombreUsuario");

                // Crear el pedido con sus detalles
                var pedido = new Pedido
                {
                    UserId = usuarioId?.ToString() ?? "Anonimo",
                    Usuario = nombreUsuario ?? "Anonimo",   // Se guarda nombre usuario
                    Fecha = DateTime.Now,                   // Fecha actual
                    Total = carrito.Sum(c => c.Subtotal),   // Total de la compra
                    Estado = "En Proceso",                  // Estado inicial del pedido
                    Detalles = carrito.Select(c => new PedidoDetalle
                    {
                        ProductoId = c.IdProducto,
                        NombreProducto = c.Nombre,
                        Precio = c.Precio,
                        Cantidad = c.Cantidad,
                        Subtotal = c.Subtotal
                    }).ToList()
                };

                // Guardamos el pedido en la BD
                var repoPedido = new FunkoShop();
                int pedidoId = repoPedido.CrearPedido(pedido);

                // Vaciar carrito después de la compra
                HttpContext.Session.Remove(CarritoSessionKey);

                // Redirigir a confirmación
                return RedirectToAction("Confirmacion", new
                {
                    aprobado = true,
                    mensaje = resultadoPago.Mensaje,
                    monto = pedido.Total,
                    usuario = nombreUsuario // opcional, se puede mostrar en vista
                });
            }

            // ❌ Si el pago no fue aprobado, mostrar mensaje
            ViewBag.Total = carrito.Sum(item => item.Subtotal);
            ViewBag.Mensaje = resultadoPago.Mensaje;
            return View(pagoRequest);
        }

        // ==============================================
        // CONFIRMACIÓN DE COMPRA
        // ==============================================
        [HttpGet]
        public IActionResult Confirmacion(bool aprobado, string mensaje, decimal monto)
        {
            ViewBag.Aprobado = aprobado;
            ViewBag.Mensaje = mensaje;
            ViewBag.Monto = monto;
            return View(); // Vista de confirmación
        }

        // ==============================================
        // MÉTODOS PRIVADOS DE APOYO
        // ==============================================

        // Recupera el carrito desde la sesión
        private List<CarritoItem> ObtenerCarrito()
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey);
            return carrito ?? new List<CarritoItem>(); // Si no hay carrito, crea uno nuevo
        }

        // Guarda el carrito en la sesión
        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetObject(CarritoSessionKey, carrito);
        }
    }
}
