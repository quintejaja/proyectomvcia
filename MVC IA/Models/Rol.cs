using System.ComponentModel.DataAnnotations;

namespace MVC_IA.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string TipoRol { get; set; }
    }
}
