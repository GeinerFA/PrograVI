using System.ComponentModel.DataAnnotations; 

namespace ProyectoPrograVI.Models
{
    // Clase que representa la información enviada al procesar un pago
    public class PagoRequest
    {
        public string Nombre { get; set; }                 // Nombre del titular de la tarjeta (requerido)

        public string FechaVencimiento { get; set; }       // Fecha de vencimiento de la tarjeta (MM/AA) (requerido)

        public string Icvv { get; set; }                   // Código de seguridad de la tarjeta (CVV) (requerido)

        public string NumeroTarjeta { get; set; }          // Número de la tarjeta (debería ser requerido y validado con longitud y formato)

        public decimal Monto { get; set; }                 // Monto de la transacción (también debería validarse como requerido y mayor a 0)
    }

    // Clase que representa la respuesta del sistema de pagos después de procesar el PagoRequest
    public class PagoResponse
    {
        public bool Aprobado { get; set; }                 // Indica si el pago fue aprobado o rechazado

        public string Mensaje { get; set; }                // Mensaje descriptivo del resultado (ejemplo: "Pago exitoso", "Fondos insuficientes")

        public string IdTransaccion { get; set; }          // Identificador único de la transacción generada

        public string MetodoPago { get; set; }             // Método de pago utilizado (ejemplo: "Tarjeta de crédito", "Tarjeta de débito")

        public DateTime Fecha { get; set; } = DateTime.Now; // Fecha en que se registró la transacción (por defecto, la actual)
    }
}
