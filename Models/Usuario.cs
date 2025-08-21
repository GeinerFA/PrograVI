using System.ComponentModel.DataAnnotations;

namespace ProyectoPrograVI.Models
{
    // Clase que representa un usuario del sistema (usuario comun o administrador)
    public class Usuario
    {
        public int IdUsuario { get; set; }
        // Identificador único del usuario (Primary Key en la BD)

        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }
        // Nombre del usuario (obligatorio)

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }
        // Correo electrónico del usuario (obligatorio y validado con formato de email)

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        // Contraseña del usuario (obligatoria y tratada como campo de tipo Password)

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string Telefono { get; set; }
        // Teléfono de contacto del usuario (obligatorio)

        public string Direccion { get; set; }
        // Dirección física del usuario (puede ser usada para envíos)

        public string Rol { get; set; }
        // Rol del usuario en el sistema (ejemplo: "usuario comun", "Administrador")
    }
}
