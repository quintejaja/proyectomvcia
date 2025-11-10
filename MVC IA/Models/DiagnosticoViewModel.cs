using System.Collections.Generic;
using System.Linq; // Necesario para IOrderedEnumerable

namespace MVC_IA.Models
{
    // Modelo que representa los datos recolectados del formulario Y los resultados de la IA
    public class DiagnosticoViewModel
    {
        // --- Campos de Datos del Cliente y Equipo (Entrada del Formulario) ---
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }

        // Campo clave para el modelo ML.NET: Recibe el texto de la descripción
        public string Problema { get; set; }

        public string TipoEquipo { get; set; }
        public string SO { get; set; }
        public int? Fabricacion { get; set; }
        public string Pais { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string CodigoPostal { get; set; }

        // --- Propiedades para el Resultado de ML.NET (Salida de la Predicción) ---

        /// <summary>
        /// Categoría predicha con la mayor probabilidad (ej: "hardware").
        /// </summary>
        public string? CategoriaML { get; set; }

        /// <summary>
        /// Colección ordenada de todas las etiquetas y sus probabilidades (scores).
        /// Reemplaza ScoreML para un análisis más completo.
        /// </summary>
        public IOrderedEnumerable<KeyValuePair<string, float>>? Scores { get; set; }
    }
}