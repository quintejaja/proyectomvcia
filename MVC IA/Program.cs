using Microsoft.EntityFrameworkCore;
using MVC_IA.Models.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewOptions(options => options.HtmlHelperOptions.ClientValidationEnabled = true); // ✅ ACTIVAR validación del lado cliente

builder.Services.AddDbContext<ProyectoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProyectoDbContext")));

builder.Services.AddSession(); // habilitar uso de sesiones

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession(); // middleware para usar sesión

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
