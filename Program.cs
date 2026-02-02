using Microsoft.EntityFrameworkCore;
using gestion_lotes.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("MySql");
// 2. Especifica la versión del servidor
var serverVersion = new MySqlServerVersion(new Version(10, 4, 27));

// 3. Registra el DbContext en el contenedor de servicios
//    Aquí es donde EF Core "aprende" a conectarse a tu BD MySQL.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, serverVersion)
);
builder.Services.AddScoped<IUsuarioRepositorio, RepositorioUsuario>();
builder.Services.AddScoped<ILoteRepositorio, RepositorioLote>();
builder.Services.AddScoped<IRecibo_persona_fisicaRepositorio, RepositorioRecibo_persona_fisica>();
builder.Services.AddScoped<IRecibo_persona_juridicaRepositorio, RepositorioRecibo_persona_juridica>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>//el sitio web valida con cookie
	{
		options.LoginPath = "/Usuario/Login";
		options.LogoutPath = "/Usuario/Logout";
		options.AccessDeniedPath = "/Home/Registringido";
		options.ExpireTimeSpan = TimeSpan.FromMinutes(10);//Tiempo de expiración
	});
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Empleado"));
	options.AddPolicy("Administrador", policy => policy.RequireRole(ClaimTypes.Role,"Administrador"));
});
builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
