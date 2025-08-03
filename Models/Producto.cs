namespace ProyectoPrograVI.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int CantStock { get; set; }
        public int IdCategoria { get; set; }
        public string ImagenUrl { get; set; }

        public string? NombreCategoria { get; set; }
    }
}
