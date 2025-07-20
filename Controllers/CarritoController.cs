using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;
using ProyectoPrograVI.Data;
using Microsoft.AspNetCore.Http;

namespace ProyectoPrograVI.Controllers
{
    public class CarritoController : Controller
    {
        private readonly FunkoShop _repositorio;

        public CarritoController()
        {
            _repositorio = new FunkoShop();
        }

        private const string CarritoSessionKey = "Carrito";

        // Método para agregar al carrito
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

        // Mostrar el carrito
        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        // Eliminar producto del carrito
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

        // Métodos auxiliares para la sesión
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
