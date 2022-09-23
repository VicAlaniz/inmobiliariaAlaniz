using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme
)
        .AddCookie(options =>
        {
            options.LoginPath = "/Usuarios/Login";
            options.LogoutPath = "/Usuarios/Logout";
            options.AccessDeniedPath = "/Home/AccesoRestringido";
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy =>
    {
        policy.RequireRole("Administrador");
    });
});
builder.Services.AddControllersWithViews();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

/*app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuarios", action = "Login" });
				endpoints.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
				endpoints.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
				
			});
*/
