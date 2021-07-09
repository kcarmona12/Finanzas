using System;
using System.Collections.Generic;
using System.Linq;
using Finanzas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finanzas.Controllers
{
    [Authorize]
    public class CuentaController : Controller
    {
        private readonly ContextoFinanzas context;
        public CuentaController(ContextoFinanzas context)
        {
            this.context = context;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var cuentas = context.Cuentas
                .Include(o => o.Categoria)
                .Where(o => o.IdUser == LoggedUser().Id)
                .ToList();
            ViewBag.Categorias = context.Categorias
                .ToList();
            ViewBag.Total = cuentas.Sum(o => o.Saldo);
            return View("Index", cuentas);
        }

        [HttpGet]
        public ActionResult Registrar()
        {
            ViewBag.Categorias = context.Categorias
                .ToList();
            return View("Registrar", new Cuenta());
        }

        [HttpPost]
        public ActionResult Registrar(Cuenta cuenta)
        {
            cuenta.IdUser = LoggedUser().Id;
            if (ModelState.IsValid)
            {
                if(cuenta.IdCategoria == 2)
                {
                    cuenta.Limite = cuenta.Saldo;
                    cuenta.Saldo = 0;
                }
                if (cuenta.Saldo != 0 && cuenta.IdCategoria != 2)
                {
                    cuenta.Limite = 0;
                    cuenta.Transaccions = new List<Transaccion>
                    {
                        new Transaccion
                        {
                            Fecha = DateTime.Now,
                            IdTipo = 1,
                            Monto = cuenta.Saldo,
                            Descripcion = "Monto Inicial"
                        }
                    };
                }
                context.Cuentas.Add(cuenta);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Categorias = context.Categorias
                .ToList();
                return View("Registrar", cuenta);
            }
        }

        [HttpGet]
        public ActionResult Transferir()
        {
            var cuentas = context.Cuentas
                .Where(o => o.IdUser == LoggedUser().Id)
                .ToList();
            return View(cuentas);
        }

        [HttpPost]
        public ActionResult Transferir(int CuentaID1, int CuentaID2, decimal amount)
        {
            var transaccion1 = new Transaccion
            {
                IdCuenta = CuentaID1,
                Fecha = DateTime.Now,
                IdTipo = 2,
                Descripcion = "Transferencia",
                Monto = amount * -1
            };
            var transaccion2 = new Transaccion
            {
                IdCuenta = CuentaID2,
                Fecha = DateTime.Now,
                IdTipo = 1,
                Descripcion = "Transferencia",
                Monto = amount
            };

            var cuenta = context.Cuentas.First(o => o.Id == transaccion1.IdCuenta);
            if (transaccion1.IdTipo == 2 && (cuenta.Limite + cuenta.Saldo) <= transaccion1.Monto)
            {
                TempData["Cuenta"] = "El monto del egreso supera el saldo de la cuenta";
                ModelState.AddModelError("Cuenta", "Error");
            }

            if (ModelState.IsValid)
            {
                context.Transaccions.Add(transaccion1);
                context.Transaccions.Add(transaccion2);

                context.SaveChanges();

                ModificaMontoCuenta(CuentaID1);
                ModificaMontoCuenta(CuentaID2);
                return RedirectToAction("Index");
            }
            var cuentas = context.Cuentas
                .Where(o => o.IdUser == LoggedUser().Id)
                .ToList();
            return View(cuentas);
        }

        [HttpGet]
        public ActionResult Yapear()
        {
            var amigo = context.Amigos
                .Where(o => o.IdUP == LoggedUser().Id)
                .Include(o => o.Users)
                .ToList();
            ViewBag.Cuenta = context.Cuentas.Where(o => o.IdUser == LoggedUser().Id).ToList();
            return View(amigo);
        }

        [HttpPost]
        public ActionResult Yapear(int IdAmigo, int IdCuenta, decimal amount)
        {
            var transaccion = new Transaccion
            {
                IdCuenta = IdCuenta,
                Fecha = DateTime.Now,
                IdTipo = 2,
                Descripcion = "Yape gasto",
                Monto = amount * -1
            };

            var cuenta = context.Cuentas.First(o => o.Id == transaccion.IdCuenta);
            if (transaccion.IdTipo == 2 && (cuenta.Limite + cuenta.Saldo) <= transaccion.Monto)
            {
                TempData["Cuenta"] = "El monto del egreso supera el saldo de la cuenta";
                ModelState.AddModelError("Cuenta", "Error");
            }

            if (ModelState.IsValid)
            {
                var cuentaAmigo = context.Cuentas.Where(o => o.IdUser == IdAmigo && o.Nombre == "Yapero").FirstOrDefault();
                var yapeos = new Transaccion();
                if (cuentaAmigo != null)
                {
                    yapeos.IdCuenta = cuentaAmigo.Id;
                    yapeos.Fecha = DateTime.Now;
                    yapeos.IdTipo = 1;
                    yapeos.Descripcion = "Yapeo Ingreso";
                    yapeos.Monto = amount;
                    context.Transaccions.Add(yapeos);
                    context.SaveChanges();
                    ModificaMontoCuenta(cuentaAmigo.Id);
                }
                else
                {
                    var cuentaYape = new Cuenta()
                    {
                        Nombre = "Yapero",
                        IdCategoria = 1,
                        Saldo = amount,
                        Limite = 0,
                        IdUser = IdAmigo
                    };
                    context.Cuentas.Add(cuentaYape);
                    context.SaveChanges();
                    yapeos.IdCuenta = cuentaYape.Id;
                    yapeos.Fecha = DateTime.Now;
                    yapeos.IdTipo = 1;
                    yapeos.Descripcion = "Yapeo Ingreso";
                    yapeos.Monto = amount;
                    context.Transaccions.Add(yapeos);
                    context.SaveChanges();
                    ModificaMontoCuenta(yapeos.Id);
                    ModificaMontoCuenta(cuentaYape.Id);
                }

                context.Transaccions.Add(transaccion);
                context.SaveChanges();
                ModificaMontoCuenta(IdCuenta);
                return RedirectToAction("Index");
            }
            var amigo = context.Amigos
                .Where(o => o.IdUP == LoggedUser().Id)
                .Include(o => o.Users)
                .ToList();
            ViewBag.Cuenta = context.Cuentas.Where(o => o.IdUser == LoggedUser().Id).ToList();
            return View(amigo);
        }

        private void ModificaMontoCuenta(int cuentaId)
        {
            var cuenta = context.Cuentas
                .Include(o => o.Transaccions)
                .FirstOrDefault(o => o.Id == cuentaId);

            var total = cuenta.Transaccions.Sum(o => o.Monto);
            cuenta.Saldo = total;
            context.SaveChanges();
        }

        protected User LoggedUser()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault();
            var user = context.Users.Where(o => o.Username == claim.Value).FirstOrDefault();
            return user;
        }
    }
}
