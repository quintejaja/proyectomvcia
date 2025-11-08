using Microsoft.EntityFrameworkCore;
using MVC_IA.Models.DbContext;
using Microsoft.AspNetCore.Authentication.Cookies; // 🚨 NECESARIO para la autenticación por cookies
using System; // Necesario para TimeSpan

var builder = WebApplication.CreateBuilder(args);

// ==========================================================
// 1. CONFIGURACIÓN DE SERVICIOS
// ==========================================================
builder.Services.AddControllersWithViews()
    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true);

builder.Services.AddDbContext<ProyectoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectoDbContext")));

// Configuración de Sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de vida de la sesión
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🚨 CONFIGURACIÓN DE AUTENTICACIÓN CON COOKIES
// Esto le dice al framework cómo leer y generar el ticket de identidad del usuario.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Login";          // Redirige aquí si se requiere Login
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });
// FIN DE LA CONFIGURACIÓN DE AUTENTICACIÓN

var app = builder.Build();

// ==========================================================
// 2. CONFIGURACIÓN DEL PIPELINE DE PETICIÓN (ORDEN CRÍTICO)
// ==========================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🚨 ORDEN CRÍTICO: Authentication debe ir antes de Authorization
app.UseAuthentication(); // Lee la cookie y establece HttpContext.User
app.UseAuthorization();  // Verifica los permisos y roles de HttpContext.User

app.UseSession(); // El middleware de Sesión

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
//using Microsoft.EntityFrameworkCore;
//using MVC_IA.Models.DbContext;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews()
//    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true); // ✅ ACTIVAR validación del lado cliente

//builder.Services.AddDbContext<ProyectoDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectoDbContext")));

//builder.Services.AddSession(); // habilitar uso de sesiones

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.UseSession(); // middleware para usar sesión

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
