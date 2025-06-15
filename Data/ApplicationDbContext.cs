using Microsoft.EntityFrameworkCore;
using ProyectoPrograVI.Models;

namespace ProyectoPrograVI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}