namespace ProyectoPrograVI.Models
{
    // Clase que representa el detalle de un pedido (los productos comprados dentro de un pedido)
    public class PedidoDetalle
    {
        public int Id { get; set; }                 // Identificador único del detalle (PK en la tabla de detalles)

        public int PedidoId { get; set; }           // Relación con el Pedido (FK -> Pedido.Id)

        public int ProductoId { get; set; }         // Relación con el Producto (FK -> Producto.Id)

        public string NombreProducto { get; set; }  // Nombre del producto comprado

        public decimal Precio { get; set; }         // Precio unitario del producto en el momento de la compra

        public int Cantidad { get; set; }           // Cantidad de unidades compradas de este producto

        public decimal Subtotal { get; set; }       // Subtotal = Precio * Cantidad
    }
}
