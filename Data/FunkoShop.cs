using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoPrograVI.Models;


namespace ProyectoPrograVI.Data
{
    public class FunkoShop
    {

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
                        //NombreCategoria = reader["nombre_categoria"].ToString() // <-- Aquí el nombre de la categoría
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
                cmd.Parameters.AddWithValue("@imagen_url", producto.ImagenUrl);
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
                cmd.Parameters.AddWithValue("@imagen_url", producto.ImagenUrl);
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
    }
}


