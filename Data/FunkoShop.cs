using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Data
{
    public class FunkoShop
    {
        private readonly string _connectionString = "Server=DEKTOP-EDWIN-MA\\SQLEXPRESS;Database=FunkoShop;Trusted_Connection=True;TrustServerCertificate=True;";
        public List<Producto> GetAll()
        {
            var lista = new List<Producto>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerProductos", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Producto
                    {
                        Id = (int)reader["id_producto"],
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString(),
                        Precio = (decimal)reader["precio"],
                        CantStock = (int)reader["cant_stock"],
                        IdCategoria = (int)reader["id_categoria"],
                        ImagenUrl = reader["imagen_url"] != DBNull.Value ? reader["imagen_url"].ToString() : null,
                        NombreCategoria = reader["nombre_categoria"].ToString()
                    });
                }
            }
            return lista;
        }

        public List<Pedido> GetAllReporte()
        {
            var lista = new List<Pedido>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Id, Usuario, Fecha, Total, Estado, UserId FROM Pedidos";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Pedido
                    {
                        Id = reader.GetInt32(0),
                        Usuario = reader.GetString(1),
                        Fecha = reader.GetDateTime(2),
                        Total = reader.GetDecimal(3),
                        Estado = reader.GetString(4),
                        UserId = reader.GetString(5),
                        Detalles = new List<PedidoDetalle>()
                    });
                }
            }

            return lista;
        }
        public void Insert(Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarProducto", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@precio", producto.Precio);
                cmd.Parameters.AddWithValue("@cant_stock", producto.CantStock);
                cmd.Parameters.AddWithValue("@id_categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@imagen_url", producto.ImagenUrl ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Update(Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarProducto", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_producto", producto.Id);
                cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@precio", producto.Precio);
                cmd.Parameters.AddWithValue("@cant_stock", producto.CantStock);
                cmd.Parameters.AddWithValue("@id_categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@imagen_url", producto.ImagenUrl ?? (object)DBNull.Value);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarProducto", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_producto", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Categoria> GetCategorias()
        {
            var lista = new List<Categoria>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerCategorias", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Categoria
                    {
                        Id = (int)reader["id_categoria"],
                        Nombre = reader["nombre"].ToString()
                    });
                }
            }
            return lista;
        }

        public void InsertCategoria(Categoria categoria)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarCategoria", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateCategoria(Categoria categoria)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarCategoria", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_categoria", categoria.Id);
                cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteCategoria(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarCategoria", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_categoria", id);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<Producto> ObtenerProductosFiltrados(string nombre, decimal? precio)
        {
            var lista = new List<Producto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = @"SELECT p.id_producto, p.nombre, p.descripcion, p.precio, 
                                p.cant_stock, p.id_categoria, p.imagen_url, c.nombre AS nombre_categoria
                         FROM tbl_producto p
                         INNER JOIN tbl_categoria c ON p.id_categoria = c.id_categoria
                         WHERE 1 = 1";

                if (!string.IsNullOrEmpty(nombre))
                    query += " AND p.nombre LIKE @nombre";

                if (precio.HasValue)
                    query += " AND p.precio <= @precio";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (!string.IsNullOrEmpty(nombre))
                        cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

                    if (precio.HasValue)
                        cmd.Parameters.AddWithValue("@precio", precio.Value);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new Producto
                        {
                            Id = (int)reader["id_producto"],
                            Nombre = reader["nombre"].ToString(),
                            Descripcion = reader["descripcion"].ToString(),
                            Precio = (decimal)reader["precio"],
                            CantStock = (int)reader["cant_stock"],
                            IdCategoria = (int)reader["id_categoria"],
                            ImagenUrl = reader["imagen_url"] != DBNull.Value ? reader["imagen_url"].ToString() : null,
                            NombreCategoria = reader["nombre_categoria"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        //Pruebaaaaa
        public int CrearPedido(Pedido pedido)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                // Insertar Pedido
                var cmd = new SqlCommand(@"
            INSERT INTO Pedidos (UserId, Usuario, Fecha, Total, Estado)
            OUTPUT INSERTED.Id
            VALUES (@UserId, @Usuario, @Fecha, @Total, @Estado)", conn, tran);

                cmd.Parameters.AddWithValue("@UserId", string.IsNullOrEmpty(pedido.UserId) ? "Anonimo" : pedido.UserId);
                cmd.Parameters.AddWithValue("@Usuario", pedido.Usuario ?? "Anonimo");
                cmd.Parameters.AddWithValue("@Fecha", pedido.Fecha == DateTime.MinValue ? DateTime.Now : pedido.Fecha);
                cmd.Parameters.AddWithValue("@Total", pedido.Total);
                cmd.Parameters.AddWithValue("@Estado", pedido.Estado);

                int pedidoId = (int)cmd.ExecuteScalar();

                // Insertar Detalles y actualizar stock
                foreach (var d in pedido.Detalles)
                {
                    // Insertar detalle
                    var cmdDetalle = new SqlCommand(@"
                INSERT INTO PedidoDetalles (PedidoId, ProductoId, NombreProducto, Precio, Cantidad, Subtotal)
                VALUES (@PedidoId, @ProductoId, @NombreProducto, @Precio, @Cantidad, @Subtotal)", conn, tran);

                    cmdDetalle.Parameters.AddWithValue("@PedidoId", pedidoId);
                    cmdDetalle.Parameters.AddWithValue("@ProductoId", d.ProductoId);
                    cmdDetalle.Parameters.AddWithValue("@NombreProducto", d.NombreProducto);
                    cmdDetalle.Parameters.AddWithValue("@Precio", d.Precio);
                    cmdDetalle.Parameters.AddWithValue("@Cantidad", d.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@Subtotal", d.Subtotal);

                    cmdDetalle.ExecuteNonQuery();

                    // Actualizar stock del producto
                    var cmdStock = new SqlCommand(@"
                UPDATE tbl_producto 
                SET cant_stock = cant_stock - @cantidad 
                WHERE id_producto = @productoId", conn, tran);

                    cmdStock.Parameters.AddWithValue("@productoId", d.ProductoId);
                    cmdStock.Parameters.AddWithValue("@cantidad", d.Cantidad);
                    cmdStock.ExecuteNonQuery();
                }

                tran.Commit();
                return pedidoId;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                // Log the error (consider using ILogger)
                throw new Exception("Error al crear el pedido: " + ex.Message);
            }
        }

        public List<Pedido> GetAllPedido()
        {
            var lista = new List<Pedido>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Id, Usuario, Fecha, Total, Estado, UserId FROM Pedidos";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Pedido
                    {
                        Id = reader.GetInt32(0),
                        Usuario = reader.GetString(1),
                        Fecha = reader.GetDateTime(2),
                        Total = reader.GetDecimal(3),
                        Estado = reader.GetString(4),
                        UserId = reader.GetString(5),
                        Detalles = new List<PedidoDetalle>()
                    });
                }
            }

            return lista;
        }

        public Pedido GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Pedidos WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Pedido
                {
                    Id = (int)reader["Id"],
                    UserId = reader["UserId"].ToString(),
                    Fecha = (DateTime)reader["Fecha"],
                    Total = (decimal)reader["Total"],
                    Estado = reader["Estado"].ToString()
                };
            }
            return null;
        }

        public void ActualizarEstado(int id, string nuevoEstado)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("UPDATE Pedidos SET Estado=@Estado WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.ExecuteNonQuery();
        }

        public List<Pedido> GetByUserId(string userId)
        {
            var lista = new List<Pedido>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Pedidos WHERE UserId=@UserId ORDER BY Fecha DESC", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pedido
                {
                    Id = (int)reader["Id"],
                    UserId = reader["UserId"].ToString(),
                    Fecha = (DateTime)reader["Fecha"],
                    Total = (decimal)reader["Total"],
                    Estado = reader["Estado"].ToString()
                });
            }
            return lista;
        }

        public List<Pedido> ObtenerPedidosFiltrados(DateTime? fecha, string estado, string usuario)
        {
            var lista = new List<Pedido>();
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            string query = "SELECT * FROM Pedidos WHERE 1=1";

            if (fecha.HasValue)
                query += " AND CAST(Fecha AS DATE) = @Fecha";

            if (!string.IsNullOrEmpty(estado))
                query += " AND Estado = @Estado";

            if (!string.IsNullOrEmpty(usuario))
                query += " AND Usuario LIKE @Usuario";

            query += " ORDER BY Fecha DESC";

            var cmd = new SqlCommand(query, conn);

            if (fecha.HasValue)
                cmd.Parameters.AddWithValue("@Fecha", fecha.Value.Date);

            if (!string.IsNullOrEmpty(estado))
                cmd.Parameters.AddWithValue("@Estado", estado);

            if (!string.IsNullOrEmpty(usuario))
                cmd.Parameters.AddWithValue("@Usuario", "%" + usuario + "%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Pedido
                {
                    Id = (int)reader["Id"],
                    UserId = reader["UserId"].ToString(),
                    Usuario = reader["Usuario"].ToString(),
                    Fecha = (DateTime)reader["Fecha"],
                    Total = (decimal)reader["Total"],
                    Estado = reader["Estado"].ToString()
                });
            }

            return lista;
        }
        public Pedido ObtenerDetallepedido(int pedidoId)
        {
            Pedido pedido = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetallePedido", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_pedido", pedidoId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Primer result set: Pedido
                        if (reader.Read())
                        {
                            pedido = new Pedido
                            {
                                Id = (int)reader["Id"],
                                UserId = reader["UserId"].ToString(),
                                Usuario = reader["Usuario"].ToString(),
                                Fecha = (DateTime)reader["Fecha"],
                                Total = (decimal)reader["Total"],
                                Estado = reader["Estado"].ToString(),
                                Direccion = reader["Direccion"].ToString(),
                                Detalles = new List<PedidoDetalle>()
                            };
                        }

                        // Segundo result set: Detalles (solo si pedido != null)
                        if (pedido != null && reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                pedido.Detalles.Add(new PedidoDetalle
                                {
                                    Id = (int)reader["id_detalle"],
                                    Cantidad = (int)reader["cantidad"],
                                    Precio = (decimal)reader["precio"],
                                    NombreProducto = reader["Producto"].ToString(),
                                    Subtotal = (decimal)reader["precio"] * (int)reader["cantidad"]
                                });
                            }
                        }
                    }
                }
            }

            return pedido;
        }

        public List<PedidoDetalle> GetAllDetallesReporte()
        {
            var lista = new List<PedidoDetalle>();

            using (var conn = new SqlConnection(_connectionString))
            {
                string query = @"
            SELECT 
                p.id_producto AS ProductoId,
                p.nombre AS NombreProducto,
                SUM(d.Cantidad) AS Cantidad,
                d.Precio,
                SUM(d.Cantidad * d.Precio) AS Subtotal
            FROM tbl_detalle_pedido d
            INNER JOIN tbl_producto p ON d.id_producto = p.id_producto
            GROUP BY p.id_producto, p.nombre, d.Precio
            ORDER BY Cantidad DESC";

                var cmd = new SqlCommand(query, conn);
                conn.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new PedidoDetalle
                    {
                        ProductoId = (int)reader["ProductoId"],
                        NombreProducto = reader["NombreProducto"].ToString(),
                        Cantidad = (int)reader["Cantidad"],
                        Precio = (decimal)reader["Precio"],
                        Subtotal = (decimal)reader["Subtotal"]
                    });
                }
            }

            return lista;
        }

        public List<Pedido> GetPedidosByUsuarioId(string usuarioId)
        {
            var pedidos = new List<Pedido>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT Id, Usuario, Fecha, Total, Estado, UserId 
            FROM Pedidos 
            WHERE UserId = @UserId 
            ORDER BY Fecha DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", usuarioId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pedidos.Add(new Pedido
                            {
                                Id = (int)reader["Id"],
                                Usuario = reader["Usuario"].ToString(),
                                Fecha = (DateTime)reader["Fecha"],
                                Total = (decimal)reader["Total"],
                                Estado = reader["Estado"].ToString(),
                                UserId = reader["UserId"].ToString()
                            });
                        }
                    }
                }
            }

            return pedidos;
        }

        public void ActualizarStockProducto(int productoId, int cantidadVendida)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(@"
            UPDATE tbl_producto 
            SET cant_stock = cant_stock - @cantidad 
            WHERE id_producto = @productoId AND cant_stock >= @cantidad", conn);

                cmd.Parameters.AddWithValue("@productoId", productoId);
                cmd.Parameters.AddWithValue("@cantidad", cantidadVendida);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    }
}
