using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlaniz.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace InmobiliariaAlaniz.Controllers
{
    public class UsuariosController : Controller
    {
        RepositorioUsuario repo;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment) {
            this.configuration = configuration;
            this.environment = environment;
            repo = new RepositorioUsuario();
        }
        // GET: Usuarios
        
        public ActionResult Index()
        {
            try{
                var usuario = repo.ObtenerTodos();
                return View(usuario);
            }
            catch (Exception ex) {
                throw;
            }
        }

        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            try {
                var usuario = repo.ObtenerPorId(id);
                return View(usuario);
            }
            catch (Exception ex) {
                throw;
            }
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            try {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
            }
            catch (Exception ex) {
                throw;
            }
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
          
           try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: u.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                u.Clave = hashed;
                //u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)Roles.Empleado;
                //var nombreAleatorio = Guid.NewGuid();
                //int res = repo.Alta(u);
                if (u.AvatarFile != null && u.Id > 0){
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path)){
                        Directory.CreateDirectory(path);
                    }
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    
                }
                repo.Alta(u);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex){
                ViewBag.Roles = Usuario.ObtenerRoles();
                throw;
            }
        
        }
           [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["returnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login) {
          
          try{
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid){
                        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                var entidad = repo.ObtenerPorEmail(login.Usuario);
                if (entidad == null || entidad.Clave != hashed) {
                    ModelState.AddModelError("", "Correo o clave incorrectos");
                    TempData["returnUrl"] = returnUrl;
                    return View();
                }
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, entidad.Email),
                    new Claim("FullName", entidad.Nombre + " " + entidad.Apellido),
                    new Claim(ClaimTypes.Role, entidad.RolesName),
                };
                var ClaimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(ClaimsIdentity));
                
                TempData.Remove("returnUrl");
                return Redirect(returnUrl);
                }
                
                TempData["returnUrl"] = returnUrl;
                return View();
                
            }
            catch (Exception ex){
                
                throw;
            }
        
        }
        [Authorize]
             public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var usu = repo.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", usu);
        }

        // GET: Usuarios/Edit/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Edit(int id) {
            try {
            ViewData["Title"] = "Editar usuario";
            var usuario = repo.ObtenerPorId(id);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(usuario);
            }
            catch (Exception ex) {
                throw;
            }
            
        }
       
  

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u)
        {
             Usuario usu = null;
            try
            {
                usu = repo.ObtenerPorId(id);
                usu.Nombre = u.Nombre;
                usu.Apellido = u.Apellido;
                //usu.Clave = u.Clave;
                usu.Email = u.Email;
                usu.Rol = u.Rol;
       
             if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                        u.Avatar = Path.Combine("/Uploads", fileName);
                    }
                usu.Avatar = u.Avatar;
                }
               /* string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: usu.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                usu.Clave = hashed;*/
                repo.Modificacion(usu);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult EditPass(int id, PassModel p) {
            try {
                Usuario original = repo.ObtenerPorId(id);
                    var passVieja = p.passOld;
                    var passNueva = p.passNew;
                    string hashedOld = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: passVieja,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    if(original.pass == hashedOld && passNueva == p.pass) {
                        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: p.pass,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));

                        original.pass = hashed;
                    } else {
                        TempData["Mensaje"] = "Contraseña no coincide";
                    }
                ru.Modificar(original);

                return RedirectToAction(nameof(Index));
            } catch (Exception ex) {
                throw;
            }
        }
*/
        // GET: Usuarios/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id)
        {
             try {
                var usuario = repo.ObtenerPorId(id);
                return View(usuario);
            }
            catch (Exception ex) {
                throw;
            }
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Delete(int id, Usuario usuario)
        {
           try {
                // TODO: Add delete logic here
                repo.Baja(id);
                TempData["Mensaje"] = "Eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) {
                throw;
            }
        }
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
    
}