namespace MVC_IA.Models
{
    public class ReservaConfirmacionViewModel
    {
        // Datos de la cita que recibimos del TempData y enviamos al POST
        public DateTime FechaHoraCita { get; set; }

        // Datos adicionales para el problema del usuario
        public string DescripcionProblema { get; set; }

        // Propiedades de ayuda para la vista (Display)
        public string? FechaDisplay { get; set; } // <<< CORRECCIÓN
        public string? HoraDisplay { get; set; }  // <<< CORRECCIÓN;
    }
}
