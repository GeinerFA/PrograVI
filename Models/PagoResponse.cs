namespace ProyectoPrograVI.Models
{
    public class PagoRequest
    {
        public decimal Monto { get; set; }
        public string NombreTarjeta { get; set; }
        public string NumeroTarjeta { get; set; }
        public string FechaExpiracion { get; set; }
        public string CVV { get; set; }
    }

    public class PagoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
    }
}