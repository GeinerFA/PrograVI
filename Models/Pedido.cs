using System;
using System.Collections.Generic;

namespace ProyectoPrograVI.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string Usuario { get; set; }  // <- Debes agregarlo si usas Usuario
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public string Direccion { get; set; }

        public string Estado { get; set; }
        public string UserId { get; set; }
        public List<PedidoDetalle> Detalles { get; set; }
    }
}
