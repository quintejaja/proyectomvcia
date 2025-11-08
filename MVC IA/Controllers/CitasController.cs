//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MVC_IA.Models;
//using MVC_IA.Models.DbContext;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims; // NECESARIO para Claims
//using System.Linq;
//using System.Threading.Tasks;
//using System;
//using Microsoft.AspNetCore.Authentication; // NECESARIO para SignOutAsync (opcional)
//using Microsoft.AspNetCore.Authentication.Cookies; // NECESARIO para AuthenticationScheme (opcional)

//namespace MVC_IA.Controllers
//{
//    public class CitasController : Controller
//    {
//        private readonly ProyectoDbContext _context;

//        public CitasController(ProyectoDbContext context)
//        {
//            _context = context;
//        }

//        // =========================================================================
//        // Flujo de Reserva
//        // =========================================================================

//        // -----------------------------------------------------------------
//        // ACCIÓN 1: Mostrar la vista de confirmación si NO está logueado
//        // -----------------------------------------------------------------
//        [HttpGet]
//        public IActionResult ConfirmacionReserva(string fecha, string hora)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                // Si el usuario ya está logueado, redirigir a la vista del flujo logueado.
//                return RedirectToAction("ConfirmacionReservaLogueado", new { fecha, hora });
//            }

//            // Aquí se muestra el resumen de la reserva junto con los botones Login/Registro
//            ViewBag.FechaSeleccionada = fecha;
//            ViewBag.HoraSeleccionada = hora;
//            // Nota: En la imagen de tu estructura de carpetas, esta vista se llama 
//            // ConfirmacionReservaNoLogueado.cshtml dentro de Views/Citas/
//            return View("ConfirmacionReservaNoLogueado");
//        }

//        // -----------------------------------------------------------------
//        // ACCIÓN 2: Mostrar la vista de confirmación si SÍ está logueado
//        // -----------------------------------------------------------------
//        [Authorize(Roles = "Cliente")] // Solo clientes pueden reservar
//        [HttpGet]
//        public IActionResult ConfirmacionReservaLogueado(string fecha, string hora)
//        {
//            if (!DateTime.TryParse(fecha, out DateTime parsedDate))
//            {
//                // Manejo de error si la fecha no es válida
//                return RedirectToAction("Calendario", "Home");
//            }

//            // Combinar Fecha y Hora para el modelo
//            // Asumo que 'hora' viene como "HH:mm - HH:mm" (ej. 10:00 - 12:00)
//            // Tomamos solo la hora de inicio:
//            var horaInicioStr = hora.Split('-')[0].Trim();
//            if (!TimeSpan.TryParse(horaInicioStr, out TimeSpan horaInicio))
//            {
//                // Manejo de error si la hora no es válida
//                return RedirectToAction("Calendario", "Home");
//            }

//            DateTime fechaHoraCita = parsedDate.Date.Add(horaInicio);


//            // 1. Prepara el ViewModel para el POST
//            var model = new ReservaConfirmacionViewModel
//            {
//                FechaHoraCita = fechaHoraCita,
//                // Nota: Tu código previo tenía errores al intentar asignar a propiedades de solo lectura
//                // Si FechaDisplay y HoraDisplay están en el ViewModel, deben ser de lectura/escritura (get; set;)
//                // Si no, no intentes asignarles un valor aquí. Dejo la asignación, asumiendo que corregiste el ViewModel.
//                // Revisa: image_247bc8.png muestra errores de solo lectura. Debes corregir el modelo ReservaConfirmacionViewModel.cs.
//                FechaDisplay = parsedDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES")),
//                HoraDisplay = hora
//            };

//            return View(model);
//        }

//        // -----------------------------------------------------------------
//        // ACCIÓN 3: Procesa la confirmación final de la reserva
//        // -----------------------------------------------------------------
//        [Authorize(Roles = "Cliente")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ConfirmarReserva(ReservaConfirmacionViewModel model)
//        {
//            if (ModelState.IsValid)
//            {
//                // **✅ CORRECCIÓN CLAVE: Usar Claims para obtener el ID del usuario logueado**
//                var idClienteStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

//                if (string.IsNullOrEmpty(idClienteStr) || !int.TryParse(idClienteStr, out int idCliente))
//                {
//                    // Si falla la autenticación, cerrar la sesión y redirigir al login
//                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//                    return RedirectToAction("Login", "Home");
//                }

//                // Asignar dinámicamente el técnico (Round Robin)
//                var tecnicoAsignadoId = AsignarTecnicoRoundRobin();

//                // 2. Crear la entidad de la reserva
//                var nuevaCita = new Cita
//                {
//                    FechaHora = model.FechaHoraCita,
//                    DescripcionProblema = model.DescripcionProblema,
//                    IdCliente = idCliente, // <<< ID OBTENIDO CORRECTAMENTE DEL CLAIM
//                    TecnicoId = tecnicoAsignadoId,
//                    Estado = "Pendiente"
//                };

//                _context.Citas.Add(nuevaCita);
//                await _context.SaveChangesAsync();

//                // Redirige al éxito (Se confirma que la PK es 'Id')
//                return RedirectToAction("ReservaExitosa", new { id = nuevaCita.Id });
//            }

//            // Si falla la validación, volvemos a mostrar la vista
//            return View("ConfirmacionReservaLogueado", model);
//        }

//        // -----------------------------------------------------------------
//        // ACCIÓN 4: Muestra la pantalla de éxito
//        // -----------------------------------------------------------------
//        public async Task<IActionResult> ReservaExitosa(int id)
//        {
//            var cita = await _context.Citas
//                .Include(c => c.Tecnico)
//                .FirstOrDefaultAsync(c => c.Id == id); // Usa 'Id' como PK

//            if (cita == null)
//            {
//                return NotFound();
//            }

//            return View(cita);
//        }

//        // =========================================================================
//        // Dashboards y Gestión
//        // =========================================================================

//        // **A. DASHBOARD DEL CLIENTE**
//        [Authorize(Roles = "Cliente")]
//        [HttpGet]
//        public IActionResult DashboardCliente()
//        {
//            // **✅ CORRECCIÓN: Usar Claims para obtener el ID del cliente**
//            var idClienteStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (!int.TryParse(idClienteStr, out int idCliente))
//            {
//                return RedirectToAction("Logout", "Home");
//            }

//            var citas = _context.Citas
//                .Where(c => c.IdCliente == idCliente)
//                .Include(c => c.Tecnico) // Incluir Técnico para mostrar el nombre
//                .OrderByDescending(c => c.FechaHora)
//                .ToList();

//            return View(citas);
//        }

//        // **B. DASHBOARD DEL TÉCNICO / ADMINISTRADOR**
//        [Authorize(Roles = "Tecnico,Admin")]
//        [HttpGet]
//        public IActionResult DashboardTecnico()
//        {
//            // Nota: Aquí mantienes la lógica de la sesión para idTecnicoStr y userRole, 
//            // lo cual funciona si el HomeController sigue seteando la sesión, 
//            // pero la práctica recomendada sería usar Claims y User.IsInRole.
//            var idTecnicoStr = HttpContext.Session.GetString("idUsuario");
//            var userRole = HttpContext.Session.GetString("rol");

//            if (!int.TryParse(idTecnicoStr, out int idTecnico))
//            {
//                return RedirectToAction("Logout", "Home");
//            }

//            var citasQuery = _context.Citas.AsQueryable();

//            if (userRole != "1") // Si NO es Admin (es decir, si es Técnico RolId=3)
//            {
//                // Filtra solo por sus citas asignadas
//                citasQuery = citasQuery.Where(c => c.TecnicoId == idTecnico);
//            }
//            // Si es Admin (RolId=1), no se aplica filtro y ve todas.

//            var citas = citasQuery
//                .Include(c => c.Cliente) // Incluir el cliente para el dashboard del técnico
//                .Include(c => c.Tecnico) // Por si acaso (si es Admin)
//                .OrderBy(c => c.FechaHora)
//                .ToList();

//            return View(citas);
//        }

//        // -----------------------------------------------------------------
//        // FUNCIONALIDAD: Asignación de Técnico Round Robin
//        // -----------------------------------------------------------------
//        private int? AsignarTecnicoRoundRobin()
//        {
//            // 1. Obtener los IdUsuario de todos los usuarios con RolId = 3 ("Tecnico")
//            var tecnicosIds = _context.Usuarios
//                                     .Where(u => u.RolId == 3) // Filtrar por RolId = 3 (Técnico)
//                                     .Select(u => u.IdUsuario)
//                                     .ToList();

//            if (!tecnicosIds.Any())
//            {
//                return null; // No hay técnicos
//            }

//            // 2. Contar las citas pendientes (Estado = "Pendiente") para cada uno de esos técnicos.
//            var citasPendientes = _context.Citas
//                // Solo contamos citas pendientes asignadas a los técnicos activos
//                .Where(c => c.TecnicoId.HasValue && tecnicosIds.Contains(c.TecnicoId.Value) && c.Estado == "Pendiente")
//                .GroupBy(c => c.TecnicoId)
//                .Select(g => new { TecnicoId = g.Key, Conteo = g.Count() })
//                .ToDictionary(x => x.TecnicoId, x => x.Conteo);

//            // 3. Inicializar el conteo a 0 para los técnicos que no tienen citas
//            var conteoFinal = tecnicosIds.ToDictionary(id => id, id => 0);
//            foreach (var par in citasPendientes)
//            {
//                // Aseguramos que solo se manejen IDs válidos
//                if (par.Key.HasValue)
//                {
//                    conteoFinal[par.Key.Value] = par.Value;
//                }
//            }

//            // 4. Encontrar el Técnico con el conteo más bajo y devolver su ID.
//            var tecnicoMenosOcupado = conteoFinal
//                .OrderBy(x => x.Value) // Ordenar por el menor número de citas pendientes
//                .ThenBy(x => x.Key) // Desempate por ID (o podrías usar un campo como la fecha de última asignación)
//                .FirstOrDefault();

//            return tecnicoMenosOcupado.Key;
//        }


//        // -----------------------------------------------------------------
//        // Acciones de Gestión de Citas (ej: Cambiar Estado)
//        // -----------------------------------------------------------------

//        [Authorize(Roles = "Tecnico,Admin")]
//        public async Task<IActionResult> CambiarEstado(int id, string estado)
//        {
//            var cita = await _context.Citas.FindAsync(id);

//            if (cita == null)
//            {
//                return NotFound();
//            }

//            // Validación de estado: asegurar que solo se permitan estados válidos
//            if (estado == "En Curso" || estado == "Completada" || estado == "Cancelada")
//            {
//                cita.Estado = estado;
//                await _context.SaveChangesAsync();
//            }

//            // Redirigir al dashboard correspondiente
//            return RedirectToAction("DashboardTecnico");
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_IA.Models;
using MVC_IA.Models.DbContext;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies; // Necesario para DateTime
using System.Security.Claims;

namespace MVC_IA.Controllers
{
    // Asegúrate de que tu controlador esté dentro de un namespace (es buena práctica)
    public class CitasController : Controller
    {
        private readonly ProyectoDbContext _context;

        public CitasController(ProyectoDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // Flujo de Reserva
        // =========================================================================

        // -----------------------------------------------------------------
        // ACCIÓN 1: Mostrar la vista de confirmación si NO está logueado
        // -----------------------------------------------------------------
        [HttpGet]
        public IActionResult ConfirmacionReserva(string fecha, string hora)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Si el usuario ya está logueado, redirigir a la vista del flujo logueado.
                return RedirectToAction("ConfirmacionReservaLogueado", new { fecha, hora });
            }

            // Aquí se muestra el resumen de la reserva junto con los botones Login/Registro
            ViewBag.FechaSeleccionada = fecha;
            ViewBag.HoraSeleccionada = hora;
            string returnUrl = Url.Action("ConfirmacionReservaLogueado", "Citas", new { fecha = fecha, hora = hora });
            return View("ConfirmacionReservaNoLogueado"); // Debe haber una vista llamada Views/Citas/ConfirmacionReserva.cshtml
        }

        // -----------------------------------------------------------------
        // ACCIÓN 2: Mostrar la vista de confirmación si SÍ está logueado
        // -----------------------------------------------------------------
        [Authorize]
        [HttpGet]
        public IActionResult ConfirmacionReservaLogueado(string fecha, string hora)
        {
            if (!DateTime.TryParse(fecha, out DateTime parsedDate))
            {
                // Manejo de error si la fecha no es válida
                return RedirectToAction("Calendario", "Home");
            }

            // Combinar Fecha y Hora para el modelo
            // Asumo que 'hora' viene como "HH:mm - HH:mm" (ej. 10:00 - 12:00)
            // Tomamos solo la hora de inicio:
            var horaInicioStr = hora.Split('-')[0].Trim();
            if (!TimeSpan.TryParse(horaInicioStr, out TimeSpan horaInicio))
            {
                // Manejo de error si la hora no es válida
                return RedirectToAction("Calendario", "Home");
            }

            DateTime fechaHoraCita = parsedDate.Date.Add(horaInicio);


            // 1. Prepara el ViewModel para el POST
            var model = new ReservaConfirmacionViewModel
            {
                FechaHoraCita = fechaHoraCita,
                FechaDisplay = parsedDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES")),
                HoraDisplay = hora
            };

            return View(model); // Debe haber una vista Views/Citas/ConfirmacionReservaLogueado.cshtml
        }

        // -----------------------------------------------------------------
        // ACCIÓN 3: Procesa la confirmación final de la reserva
        // -----------------------------------------------------------------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(ReservaConfirmacionViewModel model)
        {
            // Nota: Se asume que ReservaConfirmacionViewModel solo tiene FechaHoraCita y DescripcionProblema
            if (ModelState.IsValid)
            {
                // **Recuperar el ID del usuario logueado de la sesión**
                var idClienteStr = HttpContext.Session.GetString("idUsuario");
                if (!int.TryParse(idClienteStr, out int idCliente))
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //nuevo
                    // Si falla el parseo, el usuario no debería estar autorizado, pero por seguridad, deslogueamos
                    return RedirectToAction("Logout", "Home");
                }

                // Asignar dinámicamente el técnico (Round Robin)
                var tecnicoAsignadoId = AsignarTecnicoRoundRobin();

                // 2. Crear la entidad de la reserva
                var nuevaCita = new Cita
                {
                    FechaHora = model.FechaHoraCita,
                    DescripcionProblema = model.DescripcionProblema,
                    IdCliente = idCliente, // <<< CORRECCIÓN APLICADA: Usa IdCliente de la sesión
                    TecnicoId = tecnicoAsignadoId,
                    Estado = "Pendiente"
                };

                
                try
                {
                    _context.Citas.Add(nuevaCita);
                    await _context.SaveChangesAsync();
                    
                    return RedirectToAction("ReservaExitosa", new { id = nuevaCita.Id });
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Error al guardar la cita. Verifique si hay técnicos disponibles.");
                }
            }

            // Si falla la validación, volvemos a mostrar la vista
            return View("ConfirmacionReservaLogueado", model);
        }

        // -----------------------------------------------------------------
        // ACCIÓN 4: Muestra la pantalla de éxito
        // -----------------------------------------------------------------
        public async Task<IActionResult> ReservaExitosa(int id)
        {
            var cita = await _context.Citas
                .Include(c => c.Tecnico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            return View(cita); // Debe haber una vista Views/Citas/ReservaExitosa.cshtml
        }

        [Authorize(Roles = "Tecnico,Admin,Cliente")]
        [HttpGet]
        public async Task<IActionResult> DetallesCita(int id)
        {
            // Buscamos la cita por su ID, incluyendo Cliente y Técnico
            var cita = await _context.Citas
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cita == null)
            {
                return NotFound();
            }

            // --- 2. LÓGICA DE SEGURIDAD ---
            var currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int.TryParse(currentUserIdStr, out int currentUserId);

            // Si el usuario es Admin, puede ver todo.
            if (User.IsInRole("Admin"))
            {
                return View(cita); // El Admin sigue
            }

            // Si el usuario es Cliente, SÓLO puede ver la cita si es suya.
            if (User.IsInRole("Cliente") && cita.IdCliente != currentUserId)
            {
                return Forbid(); // ¡Acceso denegado! No es su cita.
            }

            // Si el usuario es Técnico, SÓLO puede ver la cita si está asignado a ella.
            if (User.IsInRole("Tecnico") && cita.TecnicoId != currentUserId)
            {
                return Forbid(); // ¡Acceso denegado! No es su cita.
            }

            // Si pasó todos los filtros, mostramos la vista.
            return View(cita);
        }

        // =========================================================================
        // Dashboards y Gestión
        // =========================================================================

        // **A. DASHBOARD DEL CLIENTE**
        [Authorize(Roles = "Cliente")] // Nota: Esta decoración necesita configuración de roles en Program.cs
        [HttpGet]
        public IActionResult DashboardCliente()
        {
            //var idClienteStr = HttpContext.Session.GetString("idUsuario");
            var idClienteStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idClienteStr, out int idCliente))
            {
                return RedirectToAction("Logout", "Home");
            }

            var citas = _context.Citas
                .Where(c => c.IdCliente == idCliente)
                .Include(c => c.Tecnico) // Incluir Técnico para mostrar el nombre
                .OrderByDescending(c => c.FechaHora)
                .ToList();

            return View(citas); // La vista espera List<Cita>
        }

        // **B. DASHBOARD DEL TÉCNICO / ADMINISTRADOR**
        [Authorize(Roles = "Tecnico,Admin")] // Permite acceso a Técnicos (RolId=3) y Administradores (RolId=1)
        [HttpGet]
       
        public IActionResult DashboardTecnico()
        {
            var idUsuarioStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool esAdmin = User.IsInRole("Admin");

            if (!int.TryParse(idUsuarioStr, out int idUsuario))
            {
                return RedirectToAction("Logout", "Home");
            }

            var citasQuery = _context.Citas.AsQueryable();

            if (!esAdmin)
            {
                citasQuery = citasQuery.Where(c => c.TecnicoId == idUsuario);
            }

            var citas = citasQuery
                .Include(c => c.Cliente)
                .Include(c => c.Tecnico)
                .OrderBy(c => c.FechaHora)
                .ToList();

            return View(citas);
        }

        // -----------------------------------------------------------------
        // FUNCIONALIDAD: Asignación de Técnico Round Robin
        // -----------------------------------------------------------------
        private int? AsignarTecnicoRoundRobin()
        {
           
            var tecnicosIds = _context.Usuarios
                                         .Include(u => u.Rol)
                                         .Where(u => u.Rol.TipoRol == "Tecnico")
                                         .Select(u => u.IdUsuario)
                                         .ToList();

            if (!tecnicosIds.Any())
            {
                return null;
            }

            var citasPendientes = _context.Citas
                .Where(c => c.TecnicoId.HasValue && tecnicosIds.Contains(c.TecnicoId.Value) && c.Estado == "Pendiente")
                .GroupBy(c => c.TecnicoId)
                .Select(g => new { TecnicoId = g.Key, Conteo = g.Count() })
                .ToDictionary(x => x.TecnicoId, x => x.Conteo);

            var conteoFinal = tecnicosIds.ToDictionary(id => id, id => 0);
            foreach (var par in citasPendientes)
            {
                if (par.Key.HasValue)
                {
                    conteoFinal[par.Key.Value] = par.Value;
                }
            }

            var tecnicoMenosOcupado = conteoFinal
                .OrderBy(x => x.Value)
                .ThenBy(x => x.Key)
                .FirstOrDefault();

            return tecnicoMenosOcupado.Key;


            // -----------------------------------------------------------------
            // Acciones de Gestión de Citas (ej: Cambiar Estado)
            // -----------------------------------------------------------------

            //[Authorize(Roles = "Tecnico,Admin")]
            //public async Task<IActionResult> CambiarEstado(int id, string estado)
            //{
            //    var cita = await _context.Citas.FindAsync(id);

            //    if (cita == null)
            //    {
            //        return NotFound();
            //    }

            //    // Validación de estado: asegurar que solo se permitan estados válidos
            //    if (estado == "En Curso" || estado == "Completada" || estado == "Cancelada")
            //    {
            //        cita.Estado = estado;
            //        await _context.SaveChangesAsync();
            //    }

            //    // Redirigir al dashboard correspondiente
            //    return RedirectToAction("DashboardTecnico");
            //}
            
        }
        [Authorize(Roles = "Tecnico,Admin")]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            var cita = await _context.Citas.FindAsync(id);

            if (cita == null)
            {
                return NotFound();
            }

            if (estado == "En Curso" || estado == "Completada" || estado == "Cancelada")
            {
                cita.Estado = estado;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("DashboardTecnico");
        }
    } 
}