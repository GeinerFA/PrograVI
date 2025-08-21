using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Data
{
    public class FunkoShop
    {
        // Cadena de conexión a la base de datos (SQL Server)
        private readonly string _connectionString = "Server=JOHAN-MOYA\\SQLEXPRESS;Database=FunkoShop;Trusted_Connection=True;TrustServerCertificate=True;";

        // Método para obtener todos los productos (usando un procedimiento almacenado)
        public List<Producto> GetAll()
        {
            var lista = new List<Producto>(); // Lista donde se guardarán los productos

            // Se abre una conexión con la base de datos
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Se define el comando que ejecutará el procedimiento almacenado
                SqlCommand cmd = new SqlCommand("sp_ObtenerProductos", conn);
                cmd.CommandType = CommandType.StoredProcedure; // Indicamos que es un SP

                conn.Open(); // Se abre la conexión
                SqlDataReader reader = cmd.ExecuteReader(); // Ejecuta el SP y devuelve un lector

                // Mientras existan registros, se van leyendo
                while (reader.Read())
                {
                    // Se agrega cada producto a la lista
                    lista.Add(new Producto
                    {
                        Id = (int)reader["id_producto"], // Convierte el valor a entero
                        Nombre = reader["nombre"].ToString(),
                        Descripcion = reader["descripcion"].ToString(),
                        Precio = (decimal)reader["precio"],
                        CantStock = (int)reader["cant_stock"],
                        IdCategoria = (int)reader["id_categoria"],
                        // La imagen puede ser NULL en la BD, por eso se valida
                        ImagenUrl = reader["imagen_url"] != DBNull.Value ? reader["imagen_url"].ToString() : null,
                        NombreCategoria = reader["nombre_categoria"].ToString()
                    });
                }
            }

            return lista; // Retorna la lista de productos
        }

        // Método para obtener todos los pedidos (reporte básico)
        public List<Pedido> GetAllReporte()
        {
            var lista = new List<Pedido>(); // Lista donde se guardarán los pedidos

            using (var connection = new SqlConnection(_connectionString))
            {
                // Consulta SQL directa para obtener pedidos
                var query = "SELECT Id, Usuario, Fecha, Total, Estado, UserId FROM Pedidos";
                var command = new SqlCommand(query, connection);

                connection.Open(); // Abrir conexión
                var reader = command.ExecuteReader(); // Ejecutar consulta

                // Leer los registros
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
                        Detalles = new List<PedidoDetalle>() // Inicializa lista vacía (se llena después si hace falta)
                    });
                }
            }

            return lista; // Retorna la lista de pedidos
        }

        // Método para insertar un nuevo producto en la base de datos
        public void Insert(Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Se define el comando para ejecutar el procedimiento almacenado
                SqlCommand cmd = new SqlCommand("sp_InsertarProducto", conn);
                cmd.CommandType = CommandType.StoredProcedure; // Es un SP

                // Se agregan los parámetros esperados por el procedimiento
                cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@precio", producto.Precio);
                cmd.Parameters.AddWithValue("@cant_stock", producto.CantStock);
                cmd.Parameters.AddWithValue("@id_categoria", producto.IdCategoria);
                // Si no hay imagen, se guarda como NULL en la base de datos
                cmd.Parameters.AddWithValue("@imagen_url", producto.ImagenUrl ?? (object)DBNull.Value);

                conn.Open(); // Se abre la conexión
                cmd.ExecuteNonQuery(); // Se ejecuta el SP (no devuelve datos, solo inserta)
            }
        }


        // Método para actualizar un producto en la base de datos
        public void Update(Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString)) // Crea la conexión con la BD
            {
                // Se indica que se usará el SP "sp_ActualizarProducto"
                SqlCommand cmd = new SqlCommand("sp_ActualizarProducto", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros que recibe el procedimiento
                cmd.Parameters.AddWithValue("@id_producto", producto.Id);
                cmd.Parameters.AddWithValue("@nombre", producto.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", producto.Descripcion);
                cmd.Parameters.AddWithValue("@precio", producto.Precio);
                cmd.Parameters.AddWithValue("@cant_stock", producto.CantStock);
                cmd.Parameters.AddWithValue("@id_categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@imagen_url", producto.ImagenUrl ?? (object)DBNull.Value); // Si no hay imagen → NULL

                conn.Open(); // Abre conexión
                cmd.ExecuteNonQuery(); // Ejecuta el SP (no devuelve datos)
            }
        }

        // Método para eliminar un producto por su ID
        public void Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Llama al SP que elimina el producto
                SqlCommand cmd = new SqlCommand("sp_EliminarProducto", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Le pasa el ID del producto a eliminar
                cmd.Parameters.AddWithValue("@id_producto", id);

                conn.Open();
                cmd.ExecuteNonQuery(); // Ejecuta eliminación
            }
        }

        // Método para obtener todas las categorías (para dropdowns, filtros, etc.)
        public List<Categoria> GetCategorias()
        {
            var lista = new List<Categoria>(); // Lista de categorías a retornar

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerCategorias", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Mientras existan registros en la consulta
                while (reader.Read())
                {
                    lista.Add(new Categoria
                    {
                        Id = (int)reader["id_categoria"],   // Convierte el ID a int
                        Nombre = reader["nombre"].ToString() // Nombre de la categoría
                    });
                }
            }

            return lista; // Retorna la lista de categorías
        }

        // Método para insertar una nueva categoría
        public void InsertCategoria(Categoria categoria)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_InsertarCategoria", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Se pasa el nombre de la categoría
                cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);

                conn.Open();
                cmd.ExecuteNonQuery(); // Ejecuta la inserción
            }
        }

        // Método para actualizar una categoría existente
        public void UpdateCategoria(Categoria categoria)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_ActualizarCategoria", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros necesarios
                cmd.Parameters.AddWithValue("@id_categoria", categoria.Id);
                cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);

                conn.Open();
                cmd.ExecuteNonQuery(); // Ejecuta la actualización
            }
        }

        // Método para eliminar una categoría por ID
        public void DeleteCategoria(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarCategoria", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // Se pasa el ID de la categoría
                cmd.Parameters.AddWithValue("@id_categoria", id);

                conn.Open();
                cmd.ExecuteNonQuery(); // Ejecuta la eliminación
            }
        }

        // Método para obtener productos filtrados por nombre y/o precio
        public List<Producto> ObtenerProductosFiltrados(string nombre, decimal? precio)
        {
            var lista = new List<Producto>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Consulta base con JOIN para traer categoría asociada
                string query = @"SELECT p.id_producto, p.nombre, p.descripcion, p.precio, 
                         p.cant_stock, p.id_categoria, p.imagen_url, c.nombre AS nombre_categoria
                  FROM tbl_producto p
                  INNER JOIN tbl_categoria c ON p.id_categoria = c.id_categoria
                  WHERE 1 = 1"; // 1=1 facilita agregar condiciones dinámicas

                // Si se especifica un nombre, se agrega el filtro
                if (!string.IsNullOrEmpty(nombre))
                    query += " AND p.nombre LIKE @nombre";

                // Si se especifica un precio, se agrega el filtro
                if (precio.HasValue)
                    query += " AND p.precio <= @precio";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Parámetros para evitar inyección SQL
                    if (!string.IsNullOrEmpty(nombre))
                        cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

                    if (precio.HasValue)
                        cmd.Parameters.AddWithValue("@precio", precio.Value);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Se leen los productos desde la BD
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

        // Método para crear un pedido completo (cabecera + detalles + actualización de stock)
        public int CrearPedido(Pedido pedido)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Se crea una transacción para asegurar consistencia
            using var tran = conn.BeginTransaction();

            try
            {
                // Insertar Pedido principal
                var cmd = new SqlCommand(@"
     INSERT INTO Pedidos (UserId, Usuario, Fecha, Total, Estado)
     OUTPUT INSERTED.Id
     VALUES (@UserId, @Usuario, @Fecha, @Total, @Estado)", conn, tran);

                // Parámetros del pedido
                cmd.Parameters.AddWithValue("@UserId", string.IsNullOrEmpty(pedido.UserId) ? "Anonimo" : pedido.UserId);
                cmd.Parameters.AddWithValue("@Usuario", pedido.Usuario ?? "Anonimo");
                cmd.Parameters.AddWithValue("@Fecha", pedido.Fecha == DateTime.MinValue ? DateTime.Now : pedido.Fecha);
                cmd.Parameters.AddWithValue("@Total", pedido.Total);
                cmd.Parameters.AddWithValue("@Estado", pedido.Estado);

                // Se obtiene el Id del pedido recién insertado
                int pedidoId = (int)cmd.ExecuteScalar();

                // Insertar detalles del pedido y actualizar stock
                foreach (var d in pedido.Detalles)
                {
                    // Insertar detalle del pedido
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

                // Si todo va bien, se confirma la transacción
                tran.Commit();
                return pedidoId;
            }
            catch (Exception ex)
            {
                // Si algo falla, se revierte todo
                tran.Rollback();
                throw new Exception("Error al crear el pedido: " + ex.Message);
            }
        }

        // Método para obtener todos los pedidos registrados
        public List<Pedido> GetAllPedido()
        {
            var lista = new List<Pedido>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Id, Usuario, Fecha, Total, Estado, UserId FROM Pedidos";
                var command = new SqlCommand(query, connection);

                connection.Open();
                var reader = command.ExecuteReader();

                // Se recorren todos los pedidos de la BD
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
                        Detalles = new List<PedidoDetalle>() // Inicializado vacío
                    });
                }
            }

            return lista;
        }

        // Método para obtener un pedido específico por su Id
        public Pedido GetById(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Pedidos WHERE Id=@Id", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();

            // Si encuentra el pedido, lo retorna
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

            // Si no, devuelve null
            return null;
        }

        // Método para actualizar el estado de un pedido específico en la base de datos
        public void ActualizarEstado(int id, string nuevoEstado)
        {
            // Se crea y abre la conexión a la base de datos
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Se define el comando SQL para actualizar el campo Estado de un pedido según su Id
            var cmd = new SqlCommand("UPDATE Pedidos SET Estado=@Estado WHERE Id=@Id", conn);

            // Se agregan los parámetros al comando para evitar inyección SQL
            cmd.Parameters.AddWithValue("@Estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@Id", id);

            // Se ejecuta la consulta (no retorna resultados, solo actualiza)
            cmd.ExecuteNonQuery();
        }

        // Método para obtener todos los pedidos realizados por un usuario en específico
        public List<Pedido> GetByUserId(string userId)
        {
            var lista = new List<Pedido>(); // Lista donde se guardarán los pedidos

            // Se crea y abre la conexión
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Consulta SQL para traer todos los pedidos de un usuario, ordenados por fecha descendente
            var cmd = new SqlCommand("SELECT * FROM Pedidos WHERE UserId=@UserId ORDER BY Fecha DESC", conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            // Se ejecuta la consulta y se leen los resultados
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // Se mapean los datos de cada fila al modelo Pedido
                lista.Add(new Pedido
                {
                    Id = (int)reader["Id"],
                    UserId = reader["UserId"].ToString(),
                    Fecha = (DateTime)reader["Fecha"],
                    Total = (decimal)reader["Total"],
                    Estado = reader["Estado"].ToString()
                });
            }

            return lista; // Se retorna la lista de pedidos del usuario
        }

        // Método para obtener pedidos con filtros dinámicos: por fecha, estado o usuario
        public List<Pedido> ObtenerPedidosFiltrados(DateTime? fecha, string estado, string usuario)
        {
            var lista = new List<Pedido>();

            // Se crea y abre la conexión
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // Consulta base (1=1 permite concatenar condiciones dinámicamente)
            string query = "SELECT * FROM Pedidos WHERE 1=1";

            // Si se pasa una fecha, se agrega a la consulta
            if (fecha.HasValue)
                query += " AND CAST(Fecha AS DATE) = @Fecha";

            // Si se pasa un estado, se agrega a la consulta
            if (!string.IsNullOrEmpty(estado))
                query += " AND Estado = @Estado";

            // Si se pasa un usuario, se agrega a la consulta (LIKE permite coincidencias parciales)
            if (!string.IsNullOrEmpty(usuario))
                query += " AND Usuario LIKE @Usuario";

            // Ordenar resultados de más recientes a más antiguos
            query += " ORDER BY Fecha DESC";

            // Se crea el comando SQL con los filtros
            var cmd = new SqlCommand(query, conn);

            // Se agregan parámetros según los filtros recibidos
            if (fecha.HasValue)
                cmd.Parameters.AddWithValue("@Fecha", fecha.Value.Date);

            if (!string.IsNullOrEmpty(estado))
                cmd.Parameters.AddWithValue("@Estado", estado);

            if (!string.IsNullOrEmpty(usuario))
                cmd.Parameters.AddWithValue("@Usuario", "%" + usuario + "%");

            // Se ejecuta la consulta y se leen los resultados
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                // Se mapean los datos de la BD al modelo Pedido
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

            return lista; // Retorna la lista de pedidos filtrados
        }
        // Método para obtener el detalle completo de un pedido (cabecera + productos)
        public Pedido ObtenerDetallepedido(int pedidoId)
        {
            Pedido pedido = null; // Se inicializa en null, luego se llenará si el pedido existe

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // Se crea un comando para ejecutar el procedimiento almacenado "sp_ObtenerDetallePedido"
                using (SqlCommand cmd = new SqlCommand("sp_ObtenerDetallePedido", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Indica que es un SP
                    cmd.Parameters.AddWithValue("@id_pedido", pedidoId); // Parámetro de entrada

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Primer result set: Datos generales del Pedido
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
                                Detalles = new List<PedidoDetalle>() // Inicializa lista de detalles
                            };
                        }

                        // Segundo result set: Lista de productos (detalles del pedido)
                        if (pedido != null && reader.NextResult()) // Solo si se encontró un pedido
                        {
                            while (reader.Read())
                            {
                                pedido.Detalles.Add(new PedidoDetalle
                                {
                                    Id = (int)reader["id_detalle"],
                                    Cantidad = (int)reader["cantidad"],
                                    Precio = (decimal)reader["precio"],
                                    NombreProducto = reader["Producto"].ToString(),
                                    Subtotal = (decimal)reader["precio"] * (int)reader["cantidad"] // Se calcula subtotal
                                });
                            }
                        }
                    }
                }
            }

            return pedido; // Retorna el pedido con su detalle
        }

        // Método para obtener un reporte de todos los productos más vendidos (agregados por cantidad)
        public List<PedidoDetalle> GetAllDetallesReporte()
        {
            var lista = new List<PedidoDetalle>(); // Lista de resultados

            using (var conn = new SqlConnection(_connectionString))
            {
                // Consulta SQL que agrupa por producto y calcula la cantidad total y subtotal
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
        ORDER BY Cantidad DESC"; // Ordena por mayor cantidad vendida

                var cmd = new SqlCommand(query, conn);
                conn.Open();
                var reader = cmd.ExecuteReader();

                // Se leen todos los productos agrupados
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

            return lista; // Retorna la lista de productos vendidos con totales
        }

        // Método para obtener todos los pedidos de un usuario específico
        public List<Pedido> GetPedidosByUsuarioId(string usuarioId)
        {
            var pedidos = new List<Pedido>(); // Lista de pedidos encontrados

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Consulta SQL para traer los pedidos del usuario ordenados por fecha descendente
                using (SqlCommand cmd = new SqlCommand(@"
        SELECT Id, Usuario, Fecha, Total, Estado, UserId 
        FROM Pedidos 
        WHERE UserId = @UserId 
        ORDER BY Fecha DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", usuarioId); // Parámetro de búsqueda

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Se leen los pedidos uno por uno
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

            return pedidos; // Retorna la lista de pedidos del usuario
        }


        // Método para actualizar el stock de un producto cuando se realiza una venta
        public void ActualizarStockProducto(int productoId, int cantidadVendida)
        {
            // Se crea la conexión a la base de datos con el connection string
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // Query SQL que descuenta del stock la cantidad vendida
                // IMPORTANTE: Solo se actualiza si el stock actual es mayor o igual a la cantidad vendida
                SqlCommand cmd = new SqlCommand(@"
        UPDATE tbl_producto 
        SET cant_stock = cant_stock - @cantidad 
        WHERE id_producto = @productoId AND cant_stock >= @cantidad", conn);

                // Parámetros que se envían a la consulta (evitan SQL Injection)
                cmd.Parameters.AddWithValue("@productoId", productoId); // ID del producto a actualizar
                cmd.Parameters.AddWithValue("@cantidad", cantidadVendida); // Cantidad vendida

                // Se abre la conexión
                conn.Open();

                // Ejecuta la consulta de actualización (no retorna datos, por eso ExecuteNonQuery)
                cmd.ExecuteNonQuery();
            }
        }
    }
}
