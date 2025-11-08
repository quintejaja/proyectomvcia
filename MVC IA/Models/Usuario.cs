using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // <--- Añadir este using

namespace MVC_IA.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Display(Name = "Usuario"), Required(ErrorMessage = "Obligatorio")]
        public string Username { get; set; }

        [Display(Name = "Contraseña"), Required(ErrorMessage = "Obligatorio")]
        public string Password { get; set; }

        // foreign key
        [Display(Name = "Rol"), Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        public int RolId { get; set; }

        // Propiedad de navegación del Rol
        public Rol Rol { get; set; }

        // -----------------------------------------------------------------
        // PROPIEDADES DE NAVEGACIÓN AÑADIDAS PARA CITA
        // (Necesarias para mapear las dos relaciones uno-a-muchos con la tabla Cita)
        // -----------------------------------------------------------------

        // 1. Citas donde este usuario es el CLIENTE (el que reserva)
        public ICollection<Cita> CitasComoCliente { get; set; } = new List<Cita>();

        // 2. Citas donde este usuario es el TÉCNICO (el asignado)
        public ICollection<Cita> CitasComoTecnico { get; set; } = new List<Cita>();
    }
}

//using System.ComponentModel.DataAnnotations;

//namespace MVC_IA.Models
//{
//    public class Usuario
//    {
//        [Key]
//        public int IdUsuario { get; set; }
//        [Display(Name = "Usuario"), Required(ErrorMessage = "Obligatorio")]
//        public string Username { get; set; }
//        [Display (Name = "Contraseña"), Required(ErrorMessage = "Obligatorio")]
//        public string Password { get; set; }
//        //foreign key
//        [Display(Name = "Rol"), Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
//        public int RolId { get; set; }

//        // Propiedad de navegación
//        public Rol Rol { get; set; }
//    }
//}
