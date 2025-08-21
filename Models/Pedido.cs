using System;                        
using System.Collections.Generic;     

namespace ProyectoPrograVI.Models
{
    // Clase que representa un pedido realizado por un usuario
    public class Pedido
    {
        public int Id { get; set; }                 // Identificador único del pedido en la BD

        public string Usuario { get; set; }         // Nombre del usuario que realizó el pedido (se usa para mostrar quién compró)

        public DateTime Fecha { get; set; }         // Fecha en que se realizó el pedido

        public decimal Total { get; set; }          // Monto total del pedido (suma de los subtotales de los productos)

        public string Direccion { get; set; }       // Dirección de entrega del pedido

        public string Estado { get; set; }          // Estado actual del pedido (ej: "Pendiente", "Enviado", "Entregado")

        public string UserId { get; set; }          // ID del usuario que hizo el pedido (enlazado al sistema de autenticación)

        public List<PedidoDetalle> Detalles { get; set; } // Lista con los detalles del pedido (productos comprados, cantidades, precios)
    }
}
