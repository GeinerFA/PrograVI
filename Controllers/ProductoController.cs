using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;
using ProyectoPrograVI.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ProyectoPrograVI.Controllers
{
    public class ProductosController : Controller
    {
        private readonly FunkoShop _repositorio;

        public ProductosController()
        {
            _repositorio = new FunkoShop();
        }

        // ✅ Index con filtros por nombre, precio y categoría
        public IActionResult Index(string nombre, decimal? precioMin, decimal? precioMax, string categoria)
        {
            var productos = _repositorio.GetAll().AsQueryable();

            // Filtrar por nombre
            if (!string.IsNullOrEmpty(nombre))
                productos = productos.Where(p => p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            // Filtrar por precio mínimo
            if (precioMin.HasValue)
                productos = productos.Where(p => p.Precio >= precioMin.Value);

            // Filtrar por precio máximo
            if (precioMax.HasValue)
                productos = productos.Where(p => p.Precio <= precioMax.Value);

            // Filtrar por categoría
            if (!string.IsNullOrEmpty(categoria))
                productos = productos.Where(p => p.NombreCategoria.Equals(categoria, StringComparison.OrdinalIgnoreCase));

            // Mantener valores en ViewBag para los inputs de filtro
            ViewBag.Nombre = nombre;
            ViewBag.PrecioMin = precioMin;
            ViewBag.PrecioMax = precioMax;
            ViewBag.Categoria = categoria;

            // Lista de categorías para dropdown
            ViewBag.Categorias = _repositorio.GetCategorias()
                                              .Select(c => c.Nombre)
                                              .ToList();

            return View(productos.ToList());
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _repositorio.Insert(producto);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View(producto);
        }

        public IActionResult Edit(int id)
        {
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto producto)
        {
            if (id != producto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _repositorio.Update(producto);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View(producto);
        }

        public IActionResult Delete(int id)
        {
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Detalle(int id)
        {
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        public IActionResult Categoria(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var productos = _repositorio.GetAll()
                                        .Where(p => p.NombreCategoria.ToLower() == id.ToLower())
                                        .ToList();

            if (!productos.Any())
                return View("CategoriaVacia", id);

            ViewBag.Categoria = id;
            return View(productos);
        }

        // ✅ Método privado para convertir lista de categorías en SelectListItem
        private List<SelectListItem> ObtenerSelectListCategorias()
        {
            return _repositorio.GetCategorias()
                               .Select(c => new SelectListItem
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.Nombre
                               }).ToList();
        }
    }
}
