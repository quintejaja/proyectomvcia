using Microsoft.EntityFrameworkCore;
using MVC_IA.Models; // Necesitas este using para acceder a Usuario, Rol, Cita

namespace MVC_IA.Models.DbContext
{
    public class ProyectoDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        // TABLAS
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Cita> Citas { get; set; } // Asegurar que el DbSet de Cita esté aquí
        

        // Opciones bd (irrelevante)
        public ProyectoDbContext(DbContextOptions<ProyectoDbContext> options)
        : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // -------------------------------------------------------------------------
            // 1. Configuración de la relación Cita a Cliente (Usuario que hizo la reserva)
            // Usa IdCliente (FK) y CitasComoCliente (propiedad de navegación en Usuario)
            // -------------------------------------------------------------------------
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Cliente) // Propiedad de navegación en Cita
                .WithMany(u => u.CitasComoCliente) // Colección de navegación en Usuario
                .HasForeignKey(c => c.IdCliente) // Clave foránea en Cita
                                                 // Usamos Restrict para evitar la eliminación en cascada en la base de datos
                .OnDelete(DeleteBehavior.Restrict);

            // -------------------------------------------------------------------------
            // 2. Configuración de la relación Cita a Técnico (Usuario asignado)
            // Usa TecnicoId (FK) y CitasComoTecnico (propiedad de navegación en Usuario)
            // -------------------------------------------------------------------------
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Tecnico) // Propiedad de navegación en Cita (puede ser nula)
                .WithMany(u => u.CitasComoTecnico) // Colección de navegación en Usuario
                .HasForeignKey(c => c.TecnicoId) // Clave foránea en Cita (es nullable: int?)
                .IsRequired(false) // Confirma que la FK no es obligatoria (nullable en Cita.cs)
                .OnDelete(DeleteBehavior.Restrict); // Usamos Restrict para evitar conflictos de múltiples cascadas

            // -------------------------------------------------------------------------
            // 3. Seed Data (Datos Iniciales)
            // -------------------------------------------------------------------------
            modelBuilder.Entity<Rol>().HasData(
                new Rol { IdRol = 1, TipoRol = "Admin" },
                new Rol { IdRol = 2, TipoRol = "Cliente" }, // Recomiendo usar solo un rol para 'cliente'
                new Rol { IdRol = 3, TipoRol = "Tecnico" } // Un rol claro para técnicos
            );

            // NOTA: Tu Rol original tenía Id=3 para Cliente y 4 para Técnico. 
            // He simplificado a 1=Admin, 2=Cliente, 3=Tecnico para claridad. 
            // Si tu base de datos ya está creada, mantén los Id originales.

            base.OnModelCreating(modelBuilder);
        }
    }
}
//using Microsoft.EntityFrameworkCore;
//using MVC_IA.Models; // Necesitas este using para acceder a Usuario, Rol, Cita

//namespace MVC_IA.Models.DbContext
//{
//    public class ProyectoDbContext : Microsoft.EntityFrameworkCore.DbContext
//    {
//        //TABLAS
//        public DbSet<Usuario> Usuarios { get; set; }
//        public DbSet<Rol> Roles { get; set; }

//        public DbSet<Cita> Citas { get; set; }

//        //Opciones bd (irrelevante)
//        public ProyectoDbContext(DbContextOptions<ProyectoDbContext> options)
//        : base(options) { }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            // 1. Configuración de la relación Cita a Técnico (FK: TecnicoId)
//            // ESTA RELACIÓN DEBE SER RESTRICT para evitar el ciclo de eliminación en cascada.
//            modelBuilder.Entity<Cita>()
//                .HasOne(c => c.Tecnico) // La propiedad de navegación que representa al técnico
//                .WithMany()             // No necesitamos mapear la colección de citas en el modelo Usuario
//                .HasForeignKey(c => c.TecnicoId)
//                .OnDelete(DeleteBehavior.Restrict); // <-- SOLUCIÓN: Desactiva la eliminación en cascada

//            // 2. Configuración de la relación Cita a Usuario/Cliente (FK: UsuarioId)
//            // Se puede mantener como Cascade, ya que solo una ruta puede tenerla.
//            modelBuilder.Entity<Cita>()
//                .HasOne(c => c.Usuario)
//                .WithMany()
//                .HasForeignKey(c => c.UsuarioId)
//                .OnDelete(DeleteBehavior.Cascade);

//            // Aseguramos que la base.OnModelCreating se llame antes de HasData si es necesario, 
//            // pero si la pones al final, hereda cualquier configuración de EF Core.
//            base.OnModelCreating(modelBuilder);

//            // 3. Seed Data (Datos Iniciales)
//            modelBuilder.Entity<Rol>().HasData(
//                new Rol { IdRol = 1, TipoRol = "Admin" },
//                new Rol { IdRol = 2, TipoRol = "Usuario" }, // Asumo que este es el rol general
//                new Rol { IdRol = 3, TipoRol = "Cliente" }, // Opcional si Usuario/Cliente son lo mismo
//                new Rol { IdRol = 4, TipoRol = "Tecnico" }
//            );
//        }
//    }
//}