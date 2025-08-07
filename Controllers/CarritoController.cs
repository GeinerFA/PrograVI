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

            if (!ModelState.IsValid)
            {
                ViewBag.Total = carrito.Sum(item => item.Subtotal);
                return View(pagoRequest);
            }

            var resultadoPago = await _pagoService.ProcesarPago(pagoRequest);

            if (resultadoPago.Aprobado)
            {
                // Limpiar el carrito
                HttpContext.Session.Remove(CarritoSessionKey);

                // Redirigir a la acción de confirmación con los datos necesarios
                return RedirectToAction("Confirmacion", new
                {
                    aprobado = true,
                    mensaje = resultadoPago.Mensaje,
                    monto = carrito.Sum(item => item.Subtotal)
                });
            }
            else
            {
                TempData["MensajeError"] = $"Error en el pago: {resultadoPago.Mensaje}";
                ViewBag.Total = carrito.Sum(item => item.Subtotal);
                return View(pagoRequest);
            }
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