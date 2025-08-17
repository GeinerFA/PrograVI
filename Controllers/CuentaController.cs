using Microsoft.AspNetCore.Mvc;
using ProyectoPrograVI.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CuentaController : Controller
{
    private readonly string _connectionString = "Server=GEINER\\SQLEXPRESS;Database=FunkoShop;Trusted_Connection=True;TrustServerCertificate=True;";
    private readonly string _connectionString = "Server=DESKTOP-KFPIU8T\\SQLEXPRESS;Database=FunkoShop;Trusted_Connection=True;TrustServerCertificate=True;";

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.Mensaje = "Email y contraseña son requeridos";
            return View();
        }

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("sp_LoginUsuario", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        HttpContext.Session.SetInt32("UsuarioId", (int)reader["id_usuario"]);
                        HttpContext.Session.SetString("NombreUsuario", reader["nombre"].ToString());
                        HttpContext.Session.SetString("Rol", reader["rol"].ToString());
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
        }

        ViewBag.Mensaje = "Email o contraseña incorrectos.";
        return View();
    }

    public IActionResult Registrar()
    {
        ViewBag.Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "Administrador", Text = "Administrador" },
            new SelectListItem { Value = "Usuario", Text = "Usuario" }
        };
        return View();
    }

    [HttpPost]
    public IActionResult Registrar(Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Administrador", Text = "Administrador" },
                new SelectListItem { Value = "Usuario", Text = "Usuario" }
            };
            return View(usuario);
        }

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Email", usuario.Email);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", usuario.Direccion ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Rol", usuario.Rol);
                cmd.Parameters.AddWithValue("@Password", usuario.Password);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return RedirectToAction("Login");
                }
                catch (SqlException ex)
                {
                    ModelState.AddModelError("", "Error al registrar: " + ex.Message);
                    ViewBag.Roles = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "Administrador", Text = "Administrador" },
                        new SelectListItem { Value = "Usuario", Text = "Usuario" }
                    };
                    return View(usuario);
                }
            }
        }
    }

    public IActionResult Perfil()
    {
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null)
            return RedirectToAction("Login");

        var usuario = new Usuario();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("sp_ObtenerUsuarioPorId", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuarioId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario.IdUsuario = (int)reader["id_usuario"];
                        usuario.Nombre = reader["nombre"].ToString();
                        usuario.Email = reader["email"].ToString();
                        usuario.Telefono = reader["telefono"].ToString();
                        usuario.Direccion = reader["direccion"]?.ToString();
                        usuario.Rol = reader["rol"].ToString();
                    }
                }
            }
        }

        return View(usuario);
    }

    [HttpPost]
    public IActionResult ActualizarPerfil(Usuario usuario)
    {
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null || usuario.IdUsuario != usuarioId)
        {
            return RedirectToAction("Login");
        }

        if (!ModelState.IsValid)
        {
            return View("Perfil", usuario);
        }

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            using (SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Email", usuario.Email);
                cmd.Parameters.AddWithValue("@Telefono", usuario.Telefono);
                cmd.Parameters.AddWithValue("@Direccion", usuario.Direccion ?? (object)DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Actualizar datos en la sesión
        HttpContext.Session.SetString("NombreUsuario", usuario.Nombre);
        HttpContext.Session.SetString("Email", usuario.Email);

        TempData["Mensaje"] = "Perfil actualizado correctamente";
        return RedirectToAction("Perfil");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}