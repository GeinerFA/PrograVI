namespace ProyectoPrograVI.Models
{
    // Clase que representa un ítem dentro del carrito de compras
    public class CarritoItem
    {
        // Identificador único del producto
        public int IdProducto { get; set; }

        // Nombre del producto que se agregará al carrito
        public string Nombre { get; set; }

        // URL de la imagen del producto (para mostrarlo en la vista)
        public string ImagenUrl { get; set; }

        // Precio unitario del producto
        public decimal Precio { get; set; }

        // Cantidad de unidades de este producto en el carrito
        public int Cantidad { get; set; }

        // Subtotal calculado automáticamente (Precio * Cantidad)
        // Es una propiedad de solo lectura (no necesita set)
        public decimal Subtotal => Precio * Cantidad;
    }
}
