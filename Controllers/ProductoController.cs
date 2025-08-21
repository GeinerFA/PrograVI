using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;
using ProyectoPrograVI.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace ProyectoPrograVI.Controllers
{
    public class ProductosController : Controller
    {
        // Repositorio para acceder a la base de datos
        private readonly FunkoShop _repositorio;

        public ProductosController()
        {
            // Se inicializa el repositorio
            _repositorio = new FunkoShop();
        }

        // Index con filtros por nombre, precio y categoría
        public IActionResult Index(string nombre, decimal? precioMin, decimal? precioMax, string categoria)
        {
            // Se obtiene la lista de productos en forma de consulta (para poder filtrar)
            var productos = _repositorio.GetAll().AsQueryable();

            // Filtrar por nombre (contiene el texto ingresado, sin importar mayúsculas/minúsculas)
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

            // Guardar valores en ViewBag para que se mantengan en la vista
            ViewBag.Nombre = nombre;
            ViewBag.PrecioMin = precioMin;
            ViewBag.PrecioMax = precioMax;
            ViewBag.Categoria = categoria;

            // Cargar lista de categorías para el dropdown
            ViewBag.Categorias = _repositorio.GetCategorias()
                                              .Select(c => c.Nombre)
                                              .ToList();

            // Retorna la vista con la lista filtrada de productos
            return View(productos.ToList());
        }

        // GET: Crear un nuevo producto
        public IActionResult Create()
        {
            // Se cargan las categorías para el dropdown
            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View();
        }

        // POST: Crear producto en la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid) // Validar que el modelo sea correcto
            {
                _repositorio.Insert(producto); // Guardar producto en la BD
                return RedirectToAction(nameof(Index)); // Volver al listado
            }

            // Si hay error, volver a cargar categorías y mostrar formulario
            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View(producto);
        }

        // GET: Editar un producto
        public IActionResult Edit(int id)
        {
            // Buscar producto por ID
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound(); // Si no existe, retornar error 404

            // Pasar lista de categorías a la vista
            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View(producto);
        }

        // POST: Guardar cambios de un producto editado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Producto producto)
        {
            if (id != producto.Id) // Validar que el ID coincida
                return NotFound();

            if (ModelState.IsValid) // Validar modelo
            {
                _repositorio.Update(producto); // Actualizar en BD
                return RedirectToAction(nameof(Index));
            }

            // Si hay error, volver a cargar categorías
            ViewBag.Categorias = ObtenerSelectListCategorias();
            return View(producto);
        }

        // GET: Confirmación para eliminar un producto
        public IActionResult Delete(int id)
        {
            // Buscar producto por ID
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto); // Mostrar vista de confirmación
        }

        // POST: Confirmar eliminación
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _repositorio.Delete(id); // Eliminar de la BD
            return RedirectToAction(nameof(Index)); // Volver al listado
        }

        // GET: Mostrar detalle de un producto
        public IActionResult Detalle(int id)
        {
            // Buscar producto por ID
            var producto = _repositorio.GetAll().FirstOrDefault(p => p.Id == id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // GET: Mostrar productos por categoría
        public IActionResult Categoria(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            // Buscar productos que pertenezcan a la categoría
            var productos = _repositorio.GetAll()
                                        .Where(p => p.NombreCategoria.ToLower() == id.ToLower())
                                        .ToList();

            // Si no hay productos en esa categoría, mostrar vista especial
            if (!productos.Any())
                return View("CategoriaVacia", id);

            ViewBag.Categoria = id; // Pasar nombre de la categoría a la vista
            return View(productos);
        }

        // Método privado para armar un dropdown de categorías
        private List<SelectListItem> ObtenerSelectListCategorias()
        {
            return _repositorio.GetCategorias()
                               .Select(c => new SelectListItem
                               {
                                   Value = c.Id.ToString(), // ID como valor
                                   Text = c.Nombre           // Nombre como texto
                               }).ToList();
        }
    }
}
