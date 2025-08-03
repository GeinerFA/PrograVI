// Controllers/CarritoController.cs
using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;
using ProyectoPrograVI.Data;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ProyectoPrograVI.Controllers
{
    public class CarritoController : Controller
    {
        private readonly FunkoShop _repositorio;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string CarritoSessionKey = "Carrito";
        private const string ApiPagosUrl = "https://pagosapi-b8op.onrender.com/api/Pago";

        public CarritoController(FunkoShop repositorio, IHttpClientFactory httpClientFactory)
        {
            _repositorio = repositorio;
            _httpClientFactory = httpClientFactory;
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

        public IActionResult FinalizarCompra()
        {
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                return RedirectToAction("Index");
            }

            var total = carrito.Sum(item => item.Subtotal);
            ViewBag.Total = total;
            return View(new PagoRequest { Monto = total });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcesarPago(PagoRequest pagoRequest)
        {
            // 1. Validar carrito
            var carrito = ObtenerCarrito();
            if (!carrito.Any())
            {
                TempData["ErrorPago"] = "El carrito está vacío";
                return RedirectToAction("Index");
            }

            decimal total = carrito.Sum(item => item.Subtotal);
            string transactionId;

            try
            {
                // 2. Configurar datos de prueba para la API
                var datosPago = new
                {
                    monto = total,
                    nombreTarjeta = "TARJETA SIMULADA",
                    numeroTarjeta = "4111111111111111",
                    fechaExpiracion = "12/30",
                    cvv = "123"
                };

                // 3. Conectar con la API real (pero en modo simulación)
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(3); // Timeout corto

                var response = await client.PostAsJsonAsync(ApiPagosUrl, datosPago);

                // 4. Manejar respuesta (simulada si falla)
                if (response.IsSuccessStatusCode)
                {
                    var resultado = await response.Content.ReadFromJsonAsync<PagoResponse>();
                    transactionId = resultado?.TransactionId ?? GenerarIdSimulado();
                }
                else
                {
                    transactionId = GenerarIdSimulado();
                    Console.WriteLine($"API rechazó simulación: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // 5. Fallback para errores de conexión
                transactionId = GenerarIdSimulado();
                Console.WriteLine($"Error en API: {ex.Message}");
            }

            // 6. Limpiar carrito y redirigir
            HttpContext.Session.Remove(CarritoSessionKey);

            return RedirectToAction("Confirmacion", new
            {
                id = transactionId,
                total = total
            });
        }

        // Método auxiliar para generar IDs simulados
        private string GenerarIdSimulado()
        {
            return $"SIM-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        }

        public IActionResult Confirmacion(string id)
        {
            ViewBag.TransactionId = id;
            return View();
        }

        private List<CarritoItem> ObtenerCarrito()
        {
            return HttpContext.Session.GetObject<List<CarritoItem>>(CarritoSessionKey) ?? new List<CarritoItem>();
        }

        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            HttpContext.Session.SetObject(CarritoSessionKey, carrito);
        }
    }
}