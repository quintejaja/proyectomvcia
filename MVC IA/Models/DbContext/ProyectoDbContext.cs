using Microsoft.EntityFrameworkCore;

namespace MVC_IA.Models.DbContext
{
    public class ProyectoDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        //TABLAS
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        //Opciones bd (irrelevante)
        public ProyectoDbContext(DbContextOptions<ProyectoDbContext> options) 
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, TipoRol = "Admin" },
                new Rol { IdRol = 2, TipoRol = "Usuario" },
                new Rol { IdRol = 3, TipoRol = "Cliente" },
                new Rol { IdRol = 4, TipoRol = "Tecnico" }
            );
        }
    }
}
