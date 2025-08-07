using System.ComponentModel.DataAnnotations;

namespace ProyectoPrograVI.Models
{
    public class PagoRequest
    {
        public string Nombre { get; set; }                 // requerido
        public string FechaVencimiento { get; set; }       // requerido
        public string Icvv { get; set; }                   // requerido
        public string NumeroTarjeta { get; set; }          // (verifica si también es requerido)
        public decimal Monto { get; set; }                 // (verifica si también es requerido)
    }

    public class PagoResponse
    {
        public bool Aprobado { get; set; }
        public string Mensaje { get; set; }
    }
}