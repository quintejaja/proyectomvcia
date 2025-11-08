//using System.Diagnostics;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using MVC_IA.Models;
//using MVC_IA.Models.DbContext;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims; // NECESARIO para ClaimTypes y ClaimsIdentity
//using Microsoft.AspNetCore.Authentication; // NECESARIO para SignInAsync/SignOutAsync
//using Microsoft.AspNetCore.Authentication.Cookies; // NECESARIO para AuthenticationScheme
//using System.Linq;
//using System.Threading.Tasks;

//namespace MVC_IA.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly ProyectoDbContext db;

//        public HomeController(ILogger<HomeController> logger, ProyectoDbContext _db)
//        {
//            _logger = logger;
//            db = _db;
//        }

//        public IActionResult Index()
//        {
//            // Se mantiene la sesión manual por compatibilidad con Vistas viejas, pero la identidad ahora viene de la cookie.
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }

//        #region Login y Logout (Con Claims/Cookies)

//        [HttpGet]
//        public IActionResult Login(string returnUrl = null)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                // Redirigir si ya está logueado
//                if (User.IsInRole("Admin") || User.IsInRole("Tecnico"))
//                    return RedirectToAction("DashboardTecnico", "Citas");
//                if (User.IsInRole("Cliente"))
//                    return RedirectToAction("DashboardCliente", "Citas");

//                return RedirectToAction("Index");
//            }

//            ViewData["ReturnUrl"] = returnUrl;
//            return View();
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        // CAMBIO CLAVE: Ahora es ASYNC para usar SignInAsync
//        public async Task<IActionResult> Login(Usuario usuario, string returnUrl = null)
//        {
//            var user = db.Usuarios
//              .Where(x => x.Username.Equals(usuario.Username) && x.Password.Equals(usuario.Password))
//              .Include(u => u.Rol) // Necesario para obtener el TipoRol
//                      .FirstOrDefault();

//            if (user != null)
//            {
//                // 1. **INICIO DE SESIÓN CON CLAIMS:** Creamos la identidad y la cookie
//                var claims = new List<Claim>
//        {
//          new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
//          new Claim(ClaimTypes.Name, user.Username),
//          new Claim(ClaimTypes.Role, user.Rol.TipoRol) // CRUCIAL: El valor debe coincidir exactamente con el valor de [Authorize]
//                };

//                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                var authProperties = new AuthenticationProperties
//                {
//                    IsPersistent = true,
//                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
//                };

//                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

//                // 2. Mantenemos la sesión manual (Opcional, pero se mantiene por compatibilidad)
//                HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
//                HttpContext.Session.SetString("username", user.Username);
//                HttpContext.Session.SetString("rol", user.RolId.ToString()); // RolId (1, 2, 3)

//                // 3. Redirección (Prioridad: returnUrl)
//                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
//                {
//                    // Redirigir al destino protegido inmediatamente después del login exitoso.
//                    return Redirect(returnUrl);
//                }

//                // 4. Redirigir según el rol (Si NO hay returnUrl)
//                string tipoRol = user.Rol.TipoRol;

//                if (tipoRol == "Admin" || tipoRol == "Tecnico")
//                {
//                    return RedirectToAction("DashboardTecnico", "Citas");
//                }
//                else if (tipoRol == "Cliente")
//                {
//                    return RedirectToAction("DashboardCliente", "Citas");
//                }

//                return RedirectToAction("Index", "Home");
//            }
//            else
//            {
//                ViewBag.Notification = "Usuario o contraseña incorrectos.";
//            }

//            ViewData["ReturnUrl"] = returnUrl;
//            return View(usuario);
//        }

//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            HttpContext.Session.Clear();

//            return RedirectToAction("Index", "Home");
//        }
//        #endregion

//        #region SignUp (Con Inciio de Sesión Automático)

//        [HttpGet]
//        public IActionResult SignUp(string returnUrl = null)
//        {
//            ViewData["ReturnUrl"] = returnUrl;
//            ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//            return View("SignUp");
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        // CAMBIO CLAVE: Ahora es ASYNC para usar SignInAsync
//        public async Task<IActionResult> RegistrarUsuario(Usuario usuario, string returnUrl = null)
//        {
//            string confirmPassword = Request.Form["ConfirmPassword"];

//            // Manejo de errores de contraseñas y unicidad de usuario...
//            if (usuario.Password != confirmPassword)
//            {
//                ViewBag.Notification = "Las contraseñas no coinciden.";
//                ViewBag.ConfirmPasswordError = "Las contraseñas no coinciden.";
//                ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//                ViewData["ReturnUrl"] = returnUrl;
//                return View("SignUp");
//            }

//            usuario.Username = usuario.Username.Trim().ToLower();
//            if (db.Usuarios.Any(x => x.Username.ToLower() == usuario.Username))
//            {
//                ViewBag.Notification = "Este usuario ya existe.";
//                ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//                ViewData["ReturnUrl"] = returnUrl;
//                return View("SignUp");
//            }
//            // Fin del manejo de errores.

//            // Asignar rol por defecto (Cliente = 2, asumiendo que 2 es el Id para Cliente)
//            if (usuario.RolId == 0)
//                usuario.RolId = 2;

//            // 1. Guardar usuario
//            db.Usuarios.Add(usuario);
//            await db.SaveChangesAsync();

//            // 2. **INICIO DE SESIÓN INMEDIATO**
//            var rol = db.Roles.Find(usuario.RolId);

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
//                new Claim(ClaimTypes.Name, usuario.Username),
//                new Claim(ClaimTypes.Role, rol.TipoRol)
//            };

//            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

//            // Mantenemos la sesión manual (para la compatibilidad)
//            HttpContext.Session.SetString("idUsuario", usuario.IdUsuario.ToString());
//            HttpContext.Session.SetString("username", usuario.Username);
//            HttpContext.Session.SetString("rol", usuario.RolId.ToString());


//            // 3. Redirección (Prioridad: returnUrl)
//            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
//            {
//                return Redirect(returnUrl);
//            }

//            // 4. Redirigir según el rol
//            if (rol.TipoRol == "Admin" || rol.TipoRol == "Tecnico")
//            {
//                return RedirectToAction("DashboardTecnico", "Citas");
//            }
//            else if (rol.TipoRol == "Cliente")
//            {
//                return RedirectToAction("DashboardCliente", "Citas");
//            }

//            return RedirectToAction("Index", "Home");
//        }
//        #endregion

//        #region Administración de Usuarios y Utilidades

//        // Se mantiene la lógica de autorización manual aquí, pero el resto de tu app usa [Authorize]
//        public IActionResult Usuarios()
//        {
//            if (HttpContext.Session.GetString("rol") != "1") // Solo Admin (RolId=1) puede acceder
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            var usuarios = db.Usuarios
//                                 .Include(u => u.Rol)
//                                 .Select(u => new
//                                 {
//                                     u.IdUsuario,
//                                     u.Username,
//                                     Rol = u.Rol.TipoRol
//                                 })
//                                 .ToList();

//            ViewBag.Usuarios = usuarios;
//            return View();
//        }

//        [HttpPost]
//        public IActionResult EliminarUsuario(int id)
//        {
//            if (HttpContext.Session.GetString("rol") != "1")
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            var usuario = db.Usuarios.Find(id);
//            if (usuario != null)
//            {
//                db.Usuarios.Remove(usuario);
//                db.SaveChanges();
//            }

//            return RedirectToAction("Usuarios");
//        }

//        public IActionResult Nosotros()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult LoginRegistro()
//        {
//            return View();
//        }

//        public IActionResult Calendario()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult Diagnosticabtn()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult Contacto()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult Servicios()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public class FechaRequest
//        {
//            public string Fecha { get; set; }
//        }

//        [HttpPost]
//        public JsonResult ObtenerDisponibilidad([FromBody] FechaRequest request)
//        {
//            if (DateTime.TryParse(request.Fecha, out DateTime fechaSeleccionada))
//            {
//                // Lógica de disponibilidad...
//                if (fechaSeleccionada.DayOfWeek == DayOfWeek.Saturday || fechaSeleccionada.DayOfWeek == DayOfWeek.Sunday)
//                {
//                    return Json(new List<string>());
//                }

//                var horasDisponibles = new List<string>();
//                for (int hora = 10; hora <= 16; hora += 2)
//                {
//                    string inicio = $"{hora:00}:00";
//                    string fin = $"{(hora + 2):00}:00";
//                    horasDisponibles.Add($"{inicio} - {fin}");
//                }

//                return Json(horasDisponibles);
//            }
//            return Json(new List<string>());
//        }

//        public IActionResult ReservaDetalles(string fecha, string hora)
//        {
//            ViewBag.FechaSeleccionada = fecha;
//            ViewBag.HoraSeleccionada = hora;

//            try
//            {
//                if (DateTime.TryParse(fecha, out DateTime parsedDate))
//                {
//                    ViewBag.FechaDisplay = parsedDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES"));
//                }
//                else
//                {
//                    ViewBag.FechaDisplay = "Fecha no válida";
//                }
//            }
//            catch
//            {
//                ViewBag.FechaDisplay = fecha;
//            }

//            return View();
//        }
//        #endregion
//    }
//}
//using System.Diagnostics;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using MVC_IA.Models;
//using MVC_IA.Models.DbContext;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims; // NECESARIO para ClaimTypes y ClaimsIdentity
//using Microsoft.AspNetCore.Authentication; // NECESARIO para SignInAsync/SignOutAsync
//using Microsoft.AspNetCore.Authentication.Cookies; // NECESARIO para AuthenticationScheme
//using System.Linq;
//using System.Threading.Tasks;

//namespace MVC_IA.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;
//        private readonly ProyectoDbContext db;

//        public HomeController(ILogger<HomeController> logger, ProyectoDbContext _db)
//        {
//            _logger = logger;
//            db = _db;
//        }

//        public IActionResult Index()
//        {
//            // Se mantiene la sesión manual por compatibilidad con Vistas viejas, pero la identidad ahora viene de la cookie.
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

//        }
//        #region Login y Logout (Con Claims/Cookies)

//        [HttpGet]
//        public IActionResult Login(string returnUrl = null)
//        {
//            if (User.Identity.IsAuthenticated)
//            {
//                // 1. Si ya está autenticado, PRIORIZAR el returnUrl.
//                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
//                {
//                    return Redirect(returnUrl);
//                }

//                // 2. Si no hay returnUrl, redirigir al Dashboard apropiado
//                if (User.IsInRole("Admin") || User.IsInRole("Tecnico"))
//                    return RedirectToAction("DashboardTecnico", "Citas");
//                if (User.IsInRole("Cliente"))
//                    return RedirectToAction("DashboardCliente", "Citas");

//                return RedirectToAction("Index");
//            }

//            ViewData["ReturnUrl"] = returnUrl;
//            return View();
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(Usuario usuario, string returnUrl = null)
//        {
//            var user = db.Usuarios
//              .Where(x => x.Username.Equals(usuario.Username) && x.Password.Equals(usuario.Password))
//              .Include(u => u.Rol) // Necesario para obtener el TipoRol
//              .FirstOrDefault();

//            if (user != null)
//            {
//                string tipoRol = user.Rol.TipoRol;

//                // 1. **INICIO DE SESIÓN CON CLAIMS:** Creamos la identidad y la cookie
//                var claims = new List<Claim>
//                {
//                    new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
//                    new Claim(ClaimTypes.Name, user.Username),
//                    new Claim(ClaimTypes.Role, tipoRol) // CRUCIAL para [Authorize(Roles="...")]
//                };

//                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//                var authProperties = new AuthenticationProperties
//                {
//                    IsPersistent = true,
//                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
//                };

//                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

//                // 2. Mantenemos la sesión manual (Opcional, pero se mantiene por compatibilidad)
//                HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
//                HttpContext.Session.SetString("username", user.Username);
//                HttpContext.Session.SetString("rol", user.RolId.ToString()); // RolId (1, 2, 3)

//                // 3. Redirección (Prioridad: returnUrl)
//                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
//                {
//                    // Redirigir al destino protegido inmediatamente después del login exitoso.
//                    return Redirect(returnUrl);
//                }

//                // 4. Redirigir según el rol (Si NO hay returnUrl)
//                if (tipoRol == "Admin" || tipoRol == "Tecnico")
//                {
//                    return RedirectToAction("DashboardTecnico", "Citas");
//                }
//                else if (tipoRol == "Cliente")
//                {
//                    return RedirectToAction("DashboardCliente", "Citas");
//                }

//                return RedirectToAction("Index", "Home");
//            }
//            else
//            {
//                ViewBag.Notification = "Usuario o contraseña incorrectos.";
//            }

//            ViewData["ReturnUrl"] = returnUrl;
//            return View(usuario);
//        }

//        public async Task<IActionResult> Logout()
//        {
//            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//            HttpContext.Session.Clear();

//            return RedirectToAction("Index", "Home");
//        }
//        #endregion

//        #region SignUp (Con Inciio de Sesión Automático)

//        [HttpGet]
//        public IActionResult SignUp(string returnUrl = null)
//        {
//            ViewData["ReturnUrl"] = returnUrl;
//            ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//            return View("SignUp");
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> RegistrarUsuario(Usuario usuario, string returnUrl = null)
//        {
//            string confirmPassword = Request.Form["ConfirmPassword"];

//            // Manejo de errores de contraseñas y unicidad de usuario...
//            if (usuario.Password != confirmPassword)
//            {
//                ViewBag.Notification = "Las contraseñas no coinciden.";
//                ViewBag.ConfirmPasswordError = "Las contraseñas no coinciden.";
//                ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//                ViewData["ReturnUrl"] = returnUrl;
//                return View("SignUp");
//            }

//            usuario.Username = usuario.Username.Trim().ToLower();
//            if (db.Usuarios.Any(x => x.Username.ToLower() == usuario.Username))
//            {
//                ViewBag.Notification = "Este usuario ya existe.";
//                ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//                ViewData["ReturnUrl"] = returnUrl;
//                return View("SignUp");
//            }
//            // Fin del manejo de errores.

//            // Asignar rol por defecto
//            var rolCliente = db.Roles.FirstOrDefault(r => r.TipoRol == "Cliente");
//            if (rolCliente != null)
//                usuario.RolId = rolCliente.IdRol;
//            else
//            {
//                // Si no existe el rol Cliente, mostrar error y volver.
//                ViewBag.Notification = "Error de configuración: El rol 'Cliente' no existe en la base de datos.";
//                ViewBag.Roles = new SelectList(db.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
//                ViewData["ReturnUrl"] = returnUrl;
//                return View("SignUp");
//            }


//            // 1. Guardar usuario
//            db.Usuarios.Add(usuario);
//            await db.SaveChangesAsync();

//            // 2. **INICIO DE SESIÓN INMEDIATO**
//            // CRUCIAL: Debemos buscar el rol después de guardar para asegurarnos de que el TipoRol esté disponible
//            var rol = await db.Roles.FindAsync(usuario.RolId);

//            if (rol == null)
//            {
//                // Esto no debería pasar si la lógica anterior funciona, pero es un buen control
//                ViewBag.Notification = "Error fatal al asignar el rol.";
//                return View("SignUp");
//            }

//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
//                new Claim(ClaimTypes.Name, usuario.Username),
//                new Claim(ClaimTypes.Role, rol.TipoRol)
//            };

//            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

//            // Mantenemos la sesión manual (para la compatibilidad)
//            HttpContext.Session.SetString("idUsuario", usuario.IdUsuario.ToString());
//            HttpContext.Session.SetString("username", usuario.Username);
//            HttpContext.Session.SetString("rol", usuario.RolId.ToString());


//            // 3. Redirección (Prioridad: returnUrl)
//            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
//            {
//                return Redirect(returnUrl);
//            }

//            // 4. Redirigir según el rol
//            if (rol.TipoRol == "Admin" || rol.TipoRol == "Tecnico")
//            {
//                return RedirectToAction("DashboardTecnico", "Citas");
//            }
//            else if (rol.TipoRol == "Cliente")
//            {
//                return RedirectToAction("DashboardCliente", "Citas");
//            }

//            return RedirectToAction("Index", "Home");
//        }
//        #endregion

//        #region Administración de Usuarios y Utilidades

//        // CORREGIDO: Ahora usa User.IsInRole("Admin") para verificar la autorización.
//        public IActionResult Usuarios()
//        {
//            if (!User.IsInRole("Admin"))
//            {
//                // Redirige si el usuario no tiene el rol de Administrador en el Claim.
//                return RedirectToAction("Index", "Home");
//            }

//            var usuarios = db.Usuarios
//                                     .Include(u => u.Rol)
//                                     .Select(u => new
//                                     {
//                                         u.IdUsuario,
//                                         u.Username,
//                                         Rol = u.Rol.TipoRol
//                                     })
//                                     .ToList();

//            ViewBag.Usuarios = usuarios;
//            return View();
//        }

//        [HttpPost]
//        public IActionResult EliminarUsuario(int id)
//        {
//            // CORREGIDO: Usa User.IsInRole("Admin")
//            if (!User.IsInRole("Admin"))
//            {
//                return RedirectToAction("Index", "Home");
//            }

//            var usuario = db.Usuarios.Find(id);
//            if (usuario != null)
//            {
//                db.Usuarios.Remove(usuario);
//                db.SaveChanges();
//            }

//            return RedirectToAction("Usuarios");
//        }

//        public IActionResult Nosotros()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult LoginRegistro()
//        {
//            return View();
//        }

//        public IActionResult Calendario()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult Diagnosticabtn()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult Contacto()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public IActionResult Servicios()
//        {
//            var username = HttpContext.Session.GetString("username");
//            ViewBag.Username = username;
//            return View();
//        }

//        public class FechaRequest
//        {
//            public string Fecha { get; set; }
//        }

//        [HttpPost]
//        public JsonResult ObtenerDisponibilidad([FromBody] FechaRequest request)
//        {
//            if (DateTime.TryParse(request.Fecha, out DateTime fechaSeleccionada))
//            {
//                // Lógica de disponibilidad...
//                if (fechaSeleccionada.DayOfWeek == DayOfWeek.Saturday || fechaSeleccionada.DayOfWeek == DayOfWeek.Sunday)
//                {
//                    return Json(new List<string>());
//                }

//                var horasDisponibles = new List<string>();
//                for (int hora = 10; hora <= 16; hora += 2)
//                {
//                    string inicio = $"{hora:00}:00";
//                    string fin = $"{(hora + 2):00}:00";
//                    horasDisponibles.Add($"{inicio} - {fin}");
//                }

//                return Json(horasDisponibles);
//            }
//            return Json(new List<string>());
//        }

//        public IActionResult ReservaDetalles(string fecha, string hora)
//        {
//            ViewBag.FechaSeleccionada = fecha;
//            ViewBag.HoraSeleccionada = hora;

//            try
//            {
//                if (DateTime.TryParse(fecha, out DateTime parsedDate))
//                {
//                    ViewBag.FechaDisplay = parsedDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES"));
//                }
//                else
//                {
//                    ViewBag.FechaDisplay = "Fecha no válida";
//                }
//            }
//            catch
//            {
//                ViewBag.FechaDisplay = fecha;
//            }

//            return View();
//        }
//        #endregion
//    }
//}

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_IA.Models;
using MVC_IA.Models.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // NECESARIO para ClaimTypes y ClaimsIdentity
using Microsoft.AspNetCore.Authentication; // NECESARIO para SignInAsync/SignOutAsync
using Microsoft.AspNetCore.Authentication.Cookies; // NECESARIO para AuthenticationScheme
using System.Linq;
using System.Threading.Tasks;
//using Acr.UserDialogs;
//using Fluent.Infrastructure.FluentModel;

namespace MVC_IA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoDbContext db;
        private readonly ProyectoDbContext _context; // Correctly typed field for the database context

        public HomeController(ProyectoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Se mantiene la sesión manual por compatibilidad con Vistas viejas, pero la identidad ahora viene de la cookie.
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Login y Logout (Con Claims/Cookies)

        
        [HttpGet]
        public IActionResult Login()
        {
            // El returnUrl se pasa como query parameter desde el botón de la vista
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // CAMBIO CLAVE: Ahora es ASYNC para usar SignInAsync
        public async Task<IActionResult> Login(Usuario usuario, string returnUrl = null)
        {
            var user = _context.Usuarios
                .Where(x => x.Username.Equals(usuario.Username) && x.Password.Equals(usuario.Password))
                .Include(u => u.Rol) // Necesario para obtener el TipoRol
                .FirstOrDefault();

            if (user != null)
            {
                // 1. **INICIO DE SESIÓN CON CLAIMS:** Creamos la identidad y la cookie
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Rol.TipoRol) // CRUCIAL para [Authorize(Roles="...")]
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                // 2. Mantenemos la sesión manual (Opcional)
                HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("rol", user.RolId.ToString()); // RolId (1, 2, 3)

                // 3. Redirección (Prioridad: returnUrl)
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // 4. Redirigir según el rol
                string tipoRol = user.Rol.TipoRol;

                if (tipoRol == "Admin" || tipoRol == "Tecnico")
                {
                    return RedirectToAction("DashboardTecnico", "Citas");
                }
                else if (tipoRol == "Cliente")
                {
                    return RedirectToAction("DashboardCliente", "Citas");
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Usuario o contraseña incorrectos.";
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(usuario);
        }
        

        // CAMBIO CLAVE: Ahora es ASYNC y usa SignOutAsync
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region SignUp (Con Incio de Sesión Automático)

        [HttpGet]
        public IActionResult SignUp(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
            ViewBag.Roles = new SelectList(_context.Roles
        .Where(r => r.TipoRol == "Cliente" || r.TipoRol == "Tecnico")
        .ToList(), "IdRol", "TipoRol");
            return View("SignUp");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // CAMBIO CLAVE: Ahora es ASYNC para usar SignInAsync
        public async Task<IActionResult> RegistrarUsuario(Usuario usuario, string returnUrl = null)
        {
            string confirmPassword = Request.Form["ConfirmPassword"];

            // Manejo de errores de contraseñas y unicidad de usuario...
            if (usuario.Password != confirmPassword)
            {
                ViewBag.Notification = "Las contraseñas no coinciden.";
                ViewBag.ConfirmPasswordError = "Las contraseñas no coinciden.";
                ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
                ViewData["ReturnUrl"] = returnUrl;
                return View("SignUp");
            }

            usuario.Username = usuario.Username.Trim().ToLower();
            if (_context.Usuarios.Any(x => x.Username.ToLower() == usuario.Username))
            {
                ViewBag.Notification = "Este usuario ya existe.";
                ViewBag.Roles = new SelectList(_context.Roles.Where(r => r.TipoRol == "Cliente").ToList(), "IdRol", "TipoRol");
                ViewData["ReturnUrl"] = returnUrl;
                return View("SignUp");
            }
            // Fin del manejo de errores.

            // Asignar rol por defecto (Cliente = 2, asumiendo que 2 es el Id para Cliente)
            //if (usuario.RolId == 0)
            //    usuario.RolId = 2;

            // 1. Guardar usuario
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // 2. **INICIO DE SESIÓN INMEDIATO**
            var rol = _context.Roles.Find(usuario.RolId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, rol.TipoRol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Mantenemos la sesión manual (para la compatibilidad)
            HttpContext.Session.SetString("idUsuario", usuario.IdUsuario.ToString());
            HttpContext.Session.SetString("username", usuario.Username);
            HttpContext.Session.SetString("rol", usuario.RolId.ToString());


            // 3. Redirección (Prioridad: returnUrl)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // 4. Redirigir según el rol
            if (rol.TipoRol == "Admin" || rol.TipoRol == "Tecnico")
            {
                return RedirectToAction("DashboardTecnico", "Citas");
            }
            else if (rol.TipoRol == "Cliente")
            {
                return RedirectToAction("DashboardCliente", "Citas");
            }

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Administración de Usuarios y Utilidades

        // Se mantiene la lógica de autorización manual aquí, pero el resto de tu app usa [Authorize]
        public IActionResult Usuarios()
        {
            if (HttpContext.Session.GetString("rol") != "1") // Solo Admin (RolId=1) puede acceder
            {
                return RedirectToAction("Index", "Home");
            }

            var usuarios = _context.Usuarios
                                 .Include(u => u.Rol)
                                 .Select(u => new
                                 {
                                     u.IdUsuario,
                                     u.Username,
                                     Rol = u.Rol.TipoRol
                                 })
                                 .ToList();

            ViewBag.Usuarios = usuarios;
            return View();
        }

        [HttpPost]
        public IActionResult EliminarUsuario(int id)
        {
            if (HttpContext.Session.GetString("rol") != "1")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }

            return RedirectToAction("Usuarios");
        }

        public IActionResult Nosotros()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        public IActionResult LoginRegistro()
        {
            return View();
        }

        public IActionResult Calendario()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        public IActionResult Diagnosticabtn()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        public IActionResult Contacto()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        public IActionResult Servicios()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        public class FechaRequest
        {
            public string Fecha { get; set; }
        }

        [HttpPost]
        public JsonResult ObtenerDisponibilidad([FromBody] FechaRequest request)
        {
            if (DateTime.TryParse(request.Fecha, out DateTime fechaSeleccionada))
            {
                // Lógica de disponibilidad...
                if (fechaSeleccionada.DayOfWeek == DayOfWeek.Saturday || fechaSeleccionada.DayOfWeek == DayOfWeek.Sunday)
                {
                    return Json(new List<string>());
                }

                var horasDisponibles = new List<string>();
                for (int hora = 10; hora <= 16; hora += 2)
                {
                    string inicio = $"{hora:00}:00";
                    string fin = $"{(hora + 2):00}:00";
                    horasDisponibles.Add($"{inicio} - {fin}");
                }

                return Json(horasDisponibles);
            }
            return Json(new List<string>());
        }

        public IActionResult ReservaDetalles(string fecha, string hora)
        {
            ViewBag.FechaSeleccionada = fecha;
            ViewBag.HoraSeleccionada = hora;

            try
            {
                if (DateTime.TryParse(fecha, out DateTime parsedDate))
                {
                    ViewBag.FechaDisplay = parsedDate.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("es-ES"));
                }
                else
                {
                    ViewBag.FechaDisplay = "Fecha no válida";
                }
            }
            catch
            {
                ViewBag.FechaDisplay = fecha;
            }

            return View();
        }
        #endregion
    }
}