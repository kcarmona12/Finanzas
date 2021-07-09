using System;
using System.Collections.Generic;
using System.Linq;
using Finanzas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Finanzas.Controllers
{
    public class TransaccionController : Controller
    {
        private readonly ContextoFinanzas _context;
        public TransaccionController(ContextoFinanzas context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult Index(int id)
        {
            var transacciones = _context.Transaccions.
                Include(o => o.Tipo).
                Where(o => o.IdCuenta == id).ToList();
            ViewBag.Cuenta = _context.Cuentas.First(o => o.Id == id);
            return View("Index", transacciones);
        }

        [HttpGet]
        public ActionResult Crear(int id)
        {
            ViewBag.Tipos = _context.Tipos.ToList();
            ViewBag.CuentaId = id;
            return View();
        }

        [HttpPost]
        public ActionResult Crear(Transaccion transaccion)
        {
            var cuenta = _context.Cuentas.First(o => o.Id == transaccion.IdCuenta);

            if (transaccion.IdTipo == 2 && (cuenta.Limite + cuenta.Saldo) >= transaccion.Monto)
                transaccion.Monto *= -1;
            else if (transaccion.IdTipo != 1)
            {
                TempData["Cuenta"] = "El monto del egreso supera el saldo de la cuenta";
                ModelState.AddModelError("Cuenta","Error");
            }
            transaccion.Fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Transaccions.Add(transaccion);
                _context.SaveChanges();
                ModificaMontoCuenta(transaccion.IdCuenta);
                return RedirectToAction("Index", new { id = transaccion.IdCuenta });
            }
            else
            {
                ViewBag.Tipos = _context.Tipos.ToList();
                ViewBag.CuentaId = transaccion.IdCuenta;
                return View(transaccion);
            }
        }

        private void ModificaMontoCuenta(int cuentaId)
        {
            var cuenta = _context.Cuentas
                .Include(o => o.Transaccions)
                .FirstOrDefault(o => o.Id == cuentaId);

            var total = cuenta.Transaccions.Sum(o => o.Monto);
            cuenta.Saldo = total;
            _context.SaveChanges();
        }
    }
}
