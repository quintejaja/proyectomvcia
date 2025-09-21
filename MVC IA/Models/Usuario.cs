using System.ComponentModel.DataAnnotations;

namespace MVC_IA.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }
        [Display(Name = "Usuario"), Required(ErrorMessage = "Obligatorio")]
        public string Username { get; set; }
        [Display (Name = "Contraseña"), Required(ErrorMessage = "Obligatorio")]
        public string Password { get; set; }
        //foreign key
        [Display(Name = "Rol"), Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
        public int RolId { get; set; }

        // Propiedad de navegación
        public Rol Rol { get; set; }
    }
}
