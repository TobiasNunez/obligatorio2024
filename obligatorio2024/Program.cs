using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;
using obligatorio2024.Service;
using Microsoft.AspNetCore.Session;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Obligatorio2024Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registro de servicios
builder.Services.AddScoped<CurrencyService>();
builder.Services.AddScoped<PagoService>();
builder.Services.AddHttpClient<WeatherService>();
// Añadir el WeatherService

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
app.UseSession();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var userSession = context.Session.GetString("UsuarioEmail");
    var path = context.Request.Path.Value.ToLower();

    // Allow access to Home, Login, and static files without redirection
    if (string.IsNullOrEmpty(userSession) &&
        !path.Contains("/usuarios/login") &&
        !path.Contains("/home") &&
        !path.StartsWith("/css") &&
        !path.StartsWith("/js") &&
        !path.StartsWith("/images"))
    {
        context.Response.Redirect("/Home/Index");
        return;
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
