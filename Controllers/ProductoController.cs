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

        public IActionResult Index()
        {
            var productos = _repositorio.GetAll(); // Devuelve también NombreCategoria
            return View(productos);
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

            // Volver a cargar categorías si ocurre error en validación
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
