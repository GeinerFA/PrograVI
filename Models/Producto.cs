namespace ProyectoPrograVI.Models
{
    // Clase que representa un producto dentro de la tienda
    public class Producto
    {
        public int Id { get; set; }                   // Identificador único del producto (PK en la BD)

        public string Nombre { get; set; }            // Nombre del producto (ejemplo: "Funko Batman")

        public string Descripcion { get; set; }       // Descripción del producto (información adicional)

        public decimal Precio { get; set; }           // Precio unitario del producto

        public int CantStock { get; set; }            // Cantidad disponible en inventario (stock actual)

        public int IdCategoria { get; set; }          // Relación con la categoría (FK -> Categoria.Id)

        public string ImagenUrl { get; set; }         // Ruta o URL de la imagen del producto

        public string? NombreCategoria { get; set; }  // Nombre de la categoría (opcional, útil para reportes o vistas)
    }
}
