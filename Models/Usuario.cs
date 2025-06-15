namespace ProyectoPrograVI.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public string Password { get; set; }
    }
}