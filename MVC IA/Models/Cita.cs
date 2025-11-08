using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Asegúrate de que las entidades Usuario y Rol estén en este o un namespace accesible
// using MVC_IA.Models; 

namespace MVC_IA.Models
{
    public class Cita
    {
        [Key]
        public int Id { get; set; }

        [Required]
        // La fecha y hora exacta de inicio de la cita
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(250)]
        public string DescripcionProblema { get; set; }

        // -----------------------------------------------------------------
        // RELACIÓN CON EL CLIENTE (USUARIO QUE RESERVA)
        // -----------------------------------------------------------------
        [Required]
        [Display(Name = "Cliente")]
        // Clave foránea del Cliente (Usuario que solicitó la cita)
        public int IdCliente { get; set; }

        [ForeignKey("IdCliente")]
        public Usuario Cliente { get; set; }


        // -----------------------------------------------------------------
        // RELACIÓN CON EL TÉCNICO (OPCIONAL AL INICIO)
        // -----------------------------------------------------------------
        [Display(Name = "Técnico Asignado")]
        // Hacemos el Id nullable (?) ya que el técnico se asigna después.
        public int? TecnicoId { get; set; }

        [ForeignKey("TecnicoId")]
        // La propiedad de navegación también debe ser nullable si TecnicoId lo es.
        public Usuario? Tecnico { get; set; }

        // -----------------------------------------------------------------
        // ESTADO DE LA CITA
        // -----------------------------------------------------------------
        [Required]
        [StringLength(50)]
        // Estado: "Pendiente", "Confirmada", "Cancelada", "Completada"
        public string Estado { get; set; }
    }
}
//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MVC_IA.Models // Asegúrate de que este namespace sea el correcto para tus otros modelos
//{
//    public class Cita
//    {
//        [Key]
//        public int Id { get; set; }

//        [Required]
//        // La fecha y hora exacta de inicio de la cita
//        public DateTime FechaHora { get; set; }

//        [Required]
//        [StringLength(250)]
//        public string DescripcionProblema { get; set; }

//        // Clave foránea al Usuario que hizo la reserva (asumo que usas Identity/string)
//        [Required]
//        public int UsuarioId { get; set; }
//        // Puedes agregar una propiedad de navegación si la necesitas:
//        // [ForeignKey("UsuarioId")]
//        // public Usuario Usuario { get; set; } 
//        public Usuario Usuario { get; set; }
//        // Clave foránea al Técnico asignado
//        [Required]
//        public int TecnicoId { get; set; }
//        // También puedes agregar la propiedad de navegación para el Técnico

//        [Required]
//        [StringLength(50)]
//        // Estado: "Pendiente", "Confirmada", "Cancelada", "Completada"
//        public string Estado { get; set; }


//        [ForeignKey("TecnicoId")]
//        public Usuario Tecnico { get; set; } // El técnico es un Usuario
//    }
//}