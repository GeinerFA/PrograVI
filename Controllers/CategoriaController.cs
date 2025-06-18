using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly FunkoShop _repositorio = new FunkoShop();

        public IActionResult Index() => View(_repositorio.GetCategorias());

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _repositorio.InsertCategoria(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Edit(int id)
        {
            var categoria = _repositorio.GetCategorias().FirstOrDefault(c => c.Id == id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _repositorio.UpdateCategoria(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        public IActionResult Delete(int id)
        {
            var categoria = _repositorio.GetCategorias().FirstOrDefault(c => c.Id == id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.DeleteCategoria(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
