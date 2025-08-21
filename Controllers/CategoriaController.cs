using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Data;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Controllers
{
    // Controlador encargado de manejar las operaciones CRUD de Categorías
    public class CategoriasController : Controller
    {
        // Instancia del repositorio que maneja la lógica de base de datos (FunkoShop)
        private readonly FunkoShop _repositorio = new FunkoShop();

        // ===========================
        // LISTAR CATEGORÍAS
        // ===========================
        // Muestra la lista de categorías disponibles en la BD
        public IActionResult Index() => View(_repositorio.GetCategorias());

        // ===========================
        // CREAR CATEGORÍA (GET)
        // ===========================
        // Devuelve la vista con el formulario para crear una nueva categoría
        public IActionResult Create() => View();

        // ===========================
        // CREAR CATEGORÍA (POST)
        // ===========================
        // Recibe la categoría desde el formulario y la guarda en la BD
        [HttpPost]
        public IActionResult Create(Categoria categoria)
        {
            if (ModelState.IsValid) // Valida que los datos sean correctos
            {
                _repositorio.InsertCategoria(categoria); // Inserta en la BD
                return RedirectToAction(nameof(Index));   // Redirige a la lista
            }
            return View(categoria); // Si falla la validación, regresa la vista con errores
        }

        // ===========================
        // EDITAR CATEGORÍA (GET)
        // ===========================
        // Busca la categoría por ID y muestra el formulario de edición
        public IActionResult Edit(int id)
        {
            var categoria = _repositorio.GetCategorias().FirstOrDefault(c => c.Id == id);
            if (categoria == null) return NotFound(); // Si no existe, retorna 404
            return View(categoria);
        }

        // ===========================
        // EDITAR CATEGORÍA (POST)
        // ===========================
        // Recibe la categoría modificada y actualiza en la BD
        [HttpPost]
        public IActionResult Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound(); // Validación de ID
            if (ModelState.IsValid)
            {
                _repositorio.UpdateCategoria(categoria); // Actualiza en la BD
                return RedirectToAction(nameof(Index));  // Vuelve a la lista
            }
            return View(categoria); // Si falla, regresa la vista con errores
        }

        // ===========================
        // ELIMINAR CATEGORÍA (GET)
        // ===========================
        // Muestra la vista de confirmación antes de borrar
        public IActionResult Delete(int id)
        {
            var categoria = _repositorio.GetCategorias().FirstOrDefault(c => c.Id == id);
            if (categoria == null) return NotFound(); // Si no existe, 404
            return View(categoria);
        }

        // ===========================
        // ELIMINAR CATEGORÍA (POST)
        // ===========================
        // Elimina la categoría de la BD después de confirmar
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.DeleteCategoria(id); // Borra de la BD
            return RedirectToAction(nameof(Index)); // Redirige a la lista
        }
    }
}
