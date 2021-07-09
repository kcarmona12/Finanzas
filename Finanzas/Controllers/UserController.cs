using Finanzas.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Finanzas.Controllers
{
    public class UserController : Controller
    {
        private readonly ContextoFinanzas context;

        public UserController(ContextoFinanzas context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = context.Users
                .Where(o => o.Username == username && o.Password == password)
                .FirstOrDefault();
            if (user != null)
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Login");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                HttpContext.SignInAsync(claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("Login", "Usuario o contraseña incorrectos.");
            return View();
        }
        [HttpGet]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return View("Login");
        }

        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(User user, string passwordConf)
        {
            if (user.Password != passwordConf)
                ModelState.AddModelError("PasswordConf", "Las contraseñas no coinciden");

            if (ModelState.IsValid)
            {
                context.Users.Add(user);
                context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View("Registrar", user);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Listar()
        {
            var usuarios = context.Users.ToList();
            return View(usuarios);
        }

        [Authorize]
        [HttpGet]
        public ActionResult ListarSolicitud()
        {
            var solicitudes = context.Solicitudes.
                Where(o => o.IdAmigo == LoggedUser().Id).
                ToList();
            return View(solicitudes);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Solicitud(Solicitud solicitudes, int idUser)
        {
            solicitudes.IdUsuario = LoggedUser().Id;
            solicitudes.IdAmigo = idUser;
            solicitudes.Mensaje = LoggedUser().Username + " Te ha mandado una solicitud de amistad";



            var solicitud = context.Solicitudes.ToList();
            var amigos = context.Amigos.ToList();

            foreach (var item in solicitud)
            {
                if (item.IdUsuario == LoggedUser().Id && item.IdAmigo == idUser)
                {
                    TempData["SOLICITUDOK"] = "Solicituda ya enviada";
                    ModelState.AddModelError("Error", "Solicituda ya enviada");
                }
            }

            foreach (var item in amigos)
            {
                if (item.IdUP == LoggedUser().Id && item.IdUS == idUser || item.IdUS == LoggedUser().Id && item.IdUP == idUser)
                {
                    TempData["SOLICITUDOK"] = "Ya son amigos";
                    ModelState.AddModelError("Error", "Solicituda ya enviada");
                }
            }

            if (ModelState.IsValid)
            {
                TempData["SOLICITUD"] = "Solicitud enviada";
                context.Solicitudes.Add(solicitudes);
                context.SaveChanges();
                return RedirectToAction("Listar");
            }
            return RedirectToAction("Listar");
        }
        [Authorize]
        [HttpGet]
        public ActionResult AceptarSolicitud(Amigo amigo1, Amigo amigo2, int idSolicitud)
        {
            var solicitudes = context.Solicitudes.Where(o => o.Id == idSolicitud).ToList();
            foreach (var item in solicitudes)
            {
                amigo1.IdUP = item.IdAmigo;
                amigo1.IdUS = item.IdUsuario;
                context.Amigos.Add(amigo1);
                context.SaveChanges();
                amigo2.IdUP = item.IdUsuario;
                amigo2.IdUS = item.IdAmigo;
                context.Amigos.Add(amigo2);
                context.SaveChanges();

                var solicitud = context.Solicitudes.Where(o => o.Id == idSolicitud).FirstOrDefault();
                context.Solicitudes.Remove(solicitud);
                context.SaveChanges();
                return RedirectToAction("ListarSolicitud");
            }
            return RedirectToAction("ListarSolicitud");

        }
        protected User LoggedUser()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = context.Users.Where(o => o.Username == claim.Value).FirstOrDefault();
            return user;
        }
    }
}
