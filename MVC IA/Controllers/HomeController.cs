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
        private readonly ProyectoDbContext db; //se declara cosa que accede al contexto de la base de datos

        public HomeController(ILogger<HomeController> logger, ProyectoDbContext _db)
        {
            _logger = logger;
            db = _db; //se guarda el contexto de la db en esta variable
        }

        public IActionResult Index()
        {
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
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Usuario usuario)
        {
            // tomamos la contrase�a repetida desde el formulario manualmente
            string confirmPassword = Request.Form["ConfirmPassword"];

            // validamos si las contrase�as coinciden
            if (usuario.Password != confirmPassword)
            {
                ViewBag.Notification = "Las contrase�as no coinciden.";
                ViewBag.ConfirmPasswordError = "Las contrase�as no coinciden.";
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "IdRol", "TipoRol");
                return View();
            }

            // normalizamos el username que viene del formulario
            usuario.Username = usuario.Username.Trim().ToLower();

            // verificamos si el usuario ya existe (sin importar may�sculas/min�sculas)
            if (db.Usuarios.Any(x => x.Username.ToLower() == usuario.Username))
            {
                ViewBag.Notification = "Este usuario ya existe.";
                ViewBag.Roles = new SelectList(db.Roles.ToList(), "IdRol", "TipoRol");
                return View();
            }

            db.Usuarios.Add(usuario);
            db.SaveChanges();

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
            //busca un usuario con ese nombre y contrase�a exactos
            //var user = db.Usuarios.Where(x =>
            //    x.Username == usuario.Username &&
            //    x.Password == usuario.Password);

            var user = db.Usuarios.Where(x => x.Username.Equals(usuario.Username) && x.Password.Equals(usuario.Password)).FirstOrDefault(); 

            if (user != null)
            {
                //si lo encontr�, guarda en sesi�n y redirige
               // HttpContext.Session.SetString("idUsuario", user.IdUsuario.ToString());
                HttpContext.Session.SetString("username", usuario.Username);
                HttpContext.Session.SetString("password", usuario.Password);

                //HttpContext.Session.SetString("rol", usuario.RolId.ToString());

                return RedirectToAction("Index", "Home");
            }
            else
            {
                //si no lo encontr�, muestra mensaje de error
                ViewBag.Notification = "Usuario o contrase�a incorrectos.";
                
            }
            return View();
        }

        #endregion

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        #region Administraci�n de Usuarios
        public IActionResult Usuarios()
        {
            // Verificar si est� logueado y es Admin
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
            return View();
        }


    }
}
