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
        private readonly FunkoShop _repositorio;
        private readonly PagoService _pagoService;
        private const string CarritoSessionKey = "Carrito";

        public CarritoController(PagoService pagoService)
        {
            _repositorio = new FunkoShop();
            _pagoService = pagoService;
        }

        [HttpPost]
        public IActionResult AgregarAlCarrito(int idProducto, int cantidad)
        {
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == idProducto);
            if (producto == null)
                return NotFound();

            var carrito = ObtenerCarrito();

            var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == idProducto);
            if (itemExistente != null)
            {
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    IdProducto = producto.Id,
                    Nombre = producto.Nombre,
                    ImagenUrl = producto.ImagenUrl,
                    Precio = producto.Precio,
                    Cantidad = cantidad
                });
            }

            GuardarCarrito(carrito);

            TempData["Mensaje"] = "Producto agregado al carrito";
            return RedirectToAction("Index", "Productos");
        }

        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        public IActionResult Eliminar(int id)
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(c => c.IdProducto == id);
            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult FinalizarCompra()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                return RedirectToAction("Index");
            }

            decimal total = carrito.Sum(item => item.Subtotal);
            ViewBag.Total = total;

            return View(new PagoRequest { Monto = total });
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> FinalizarCompra(PagoRequest pagoRequest)
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                return RedirectToAction("Index");
            }

            foreach (var item in carrito)
            {
                var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == item.IdProducto);
                if (producto == null || producto.CantStock < item.Cantidad)
                {
                    ModelState.AddModelError("", $"No hay suficiente stock para: {item.Nombre}");
                    ViewBag.Total = carrito.Sum(item => item.Subtotal);
                    return View(pagoRequest);
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Total = carrito.Sum(item => item.Subtotal);
                return View(pagoRequest);
            }


            if (!ModelState.IsValid)
            {
                ViewBag.Total = carrito.Sum(item => item.Subtotal);
                return View(pagoRequest);
            }

            var resultadoPago = await _pagoService.ProcesarPago(pagoRequest);

            if (resultadoPago.Aprobado)
            {
                // ✅ Recuperar datos del usuario desde Session
                var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
                var nombreUsuario = HttpContext.Session.GetString("NombreUsuario");

                var pedido = new Pedido
                {
                    UserId = usuarioId?.ToString() ?? "Anonimo",
                    Usuario = nombreUsuario ?? "Anonimo",   // ✅ aquí guardamos el nombre
                    Fecha = DateTime.Now,                   // 👈 también la fecha
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

                var repoPedido = new FunkoShop();
                int pedidoId = repoPedido.CrearPedido(pedido);

                // Vaciar carrito
                HttpContext.Session.Remove(CarritoSessionKey);

                return RedirectToAction("Confirmacion", new
                {
                    aprobado = true,
                    mensaje = resultadoPago.Mensaje,
                    monto = pedido.Total,
                    usuario = nombreUsuario // 👈 opcional, si quieres mostrarlo en la vista
                });
            }

            // Si no fue aprobado, devolver la vista de pago con el mensaje
            ViewBag.Total = carrito.Sum(item => item.Subtotal);
            ViewBag.Mensaje = resultadoPago.Mensaje;
            return View(pagoRequest);
        }

        [HttpGet]
        public IActionResult Confirmacion(bool aprobado, string mensaje, decimal monto)
        {
            ViewBag.Aprobado = aprobado;
            ViewBag.Mensaje = mensaje;
            ViewBag.Monto = monto;
            return View();
        }

        private List<CarritoItem> ObtenerCarrito()
        {
            var carrito = HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey);
            return carrito ?? new List<CarritoItem>();
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetObject(CarritoSessionKey, carrito);
        }


    }
}