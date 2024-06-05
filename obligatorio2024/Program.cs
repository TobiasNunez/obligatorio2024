using Microsoft.EntityFrameworkCore;
using obligatorio2024.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Obligatorio2024Context>(options =>
    options.UseSqlServer("Data Source=FACUNDO;Initial Catalog=obligatorio2024;Integrated Security=true; TrustServerCertificate=True"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

app.Use(async (context, next) =>
{
    var userSession = context.Session.GetString("UsuarioEmail");
    if (string.IsNullOrEmpty(userSession) && !context.Request.Path.Value.Contains("/Usuarios/Login"))
    {
        context.Response.Redirect("/Usuarios/Login");
        return;
    }
    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
