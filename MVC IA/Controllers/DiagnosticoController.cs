using Microsoft.AspNetCore.Mvc;
using MVC_IA.Models;
using static MVC_IA.MLModel; // Importante: Accede a las clases y métodos de la IA
using System.Linq; 

namespace MVC_IA.Controllers
{
    public class DiagnosticoController : Controller
    {
        // Método GET (Muestra el formulario original)
        // La ruta asume que el archivo de tu formulario es Views/Home/DiagnosticaBtn.cshtml
        public IActionResult Index()
        {
            return View("~/Views/Home/DiagnosticaBtn.cshtml"); 
        }

        // Método POST (Procesa el formulario y llama a la IA)
        [HttpPost]
        public IActionResult ProcesarDiagnostico(DiagnosticoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Si falla la validación, regresa al formulario
                return View("~/Views/Home/DiagnosticaBtn.cshtml", model); 
            }

            // --- 1. Ejecución de la IA ---
            var mlInput = new MLModel.ModelInput()
            {
                // Mapeo perfecto: Problema del formulario -> Titulo del modelo
                Titulo = model.Problema 
            };

            // Ejecuta la predicción y obtiene TODAS las probabilidades
            var labeledScores = MLModel.PredictAllLabels(mlInput);
            var topPrediction = labeledScores.FirstOrDefault();

            // Almacena los resultados en el ViewModel
            model.CategoriaML = topPrediction.Key; 
            model.Scores = labeledScores; 

            // 2. Redirige a la vista de resultados
            return View("Resultado", model);
        }
    }
}