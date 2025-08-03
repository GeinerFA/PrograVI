// Program.cs
using ProyectoPrograVI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<FunkoShop>();
builder.Services.AddHttpClient();

// Configurar DbContext con SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FunkoShopConnection")));

builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // soporte de sesiones
builder.Services.AddHttpContextAccessor(); // acceso a HttpContext

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // habilita el middleware de sesiones
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");

app.Run();