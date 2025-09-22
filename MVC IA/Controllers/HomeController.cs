using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_IA.Models;
using MVC_IA.Models.DbContext;

namespace MVC_IA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProyectoDbContext db;

        public HomeController(ILogger<HomeController> logger, ProyectoDbContext _db)
        {
            _logger = logger;
            db = _db;
        }

        public IActionResult Index()
        {
            // Get username from session and pass it to the view
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region SignUp
        public IActionResult SignUp()
        {
            ViewBag.Roles = new SelectList(db.Roles.ToList(), "IdRol", "TipoRol");
            return View("SignUp");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(Usuario usuario)
        {
            // tomamos la contraseña repetida desde el formulario manualmente
            string confirmPassword = Request.Form["ConfirmPassword"];

            // validamos si las contraseñas coinciden
            if (usuario.Password != confirmPassword)
            {
                ViewBag.Notification = "Las contraseñas no coinciden.";
                ViewBag.ConfirmPasswordError = "Las contraseñas no coinciden.";
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "IdRol", "TipoRol");
                return View("SignUp");
            }

            // normalizamos el username
            usuario.Username = usuario.Username.Trim().ToLower();

            // verificamos si el usuario ya existe
            if (db.Usuarios.Any(x => x.Username.ToLower() == usuario.Username))
            {
                ViewBag.Notification = "Este usuario ya existe.";
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "IdRol", "TipoRol");
                return View("SignUp");
            }

            // si no tiene rol asignado, lo ponemos por defecto en 2 (usuario común)
            if (usuario.RolId == 0)
                usuario.RolId = 2;

            db.Usuarios.Add(usuario);
            db.SaveChanges();

            // iniciamos sesión
            HttpContext.Session.SetString("idUsuario", usuario.IdUsuario.ToString());
            HttpContext.Session.SetString("username", usuario.Username);
            HttpContext.Session.SetString("rol", usuario.RolId.ToString());

            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Login - LogOut
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Usuario usuario)
        {
            var user = db.Usuarios
                .Where(x => x.Username.Equals(usuario.Username) && x.Password.Equals(usuario.Password))
                .FirstOrDefault();

            if (user != null)
            {
                // Guardamos los datos en sesión
                HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("rol", user.RolId.ToString());

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Notification = "Usuario o contraseña incorrectos.";
            }

            return View();
        }
        #endregion

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        #region Administración de Usuarios
        public IActionResult Usuarios()
        {
            // Verificar si está logueado y es Admin
            if (HttpContext.Session.GetString("rol") != "1")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuarios = db.Usuarios
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
            // Solo admins pueden eliminar
            if (HttpContext.Session.GetString("rol") != "1")
            {
                return RedirectToAction("Index", "Home");
            }

            var usuario = db.Usuarios.Find(id);
            if (usuario != null)
            {
                db.Usuarios.Remove(usuario);
                db.SaveChanges();
            }

            return RedirectToAction("Usuarios");
        }
        #endregion

        public IActionResult Nosotros()
        {
            // Pasar el username a la vista Nosotros también si es necesario
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }

        public IActionResult LoginRegistro()
        {
            return View();
        }

        public IActionResult Reparaciones()
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

        public IActionResult Contacto()
        {
            var username = HttpContext.Session.GetString("username");
            ViewBag.Username = username;
            return View();
        }
    }
}