using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlaniz.Models;

namespace InmobiliariaAlaniz.Controllers
{
    public class PropietariosController : Controller
    {
         RepositorioPropietario repo;

        public PropietariosController() {
            repo = new RepositorioPropietario();
        }
        // GET: Propietarios
        public ActionResult Index()
        {
            try{
                var propietario = repo.ObtenerTodos();
                return View(propietario);
            }
            catch (Exception ex) {
                throw;
            }
        }

        // GET: Propietarios/Details/5
        public ActionResult Details(int id)
        { 
            try {
                var propietario = repo.ObtenerPorId(id);
                return View(propietario);
            }
            catch (Exception ex) {
                throw;
            }
            
        }

        // GET: Propietarios/Create
        public ActionResult Create()
        {
            try {
            return View();
            }
            catch (Exception ex) {
                throw;
            }
        }

        // POST: Propietarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid) {
                    repo.Alta(propietario);
                    TempData["Id"] = propietario.Id;
                    return RedirectToAction(nameof(Index));
                }
                else {
                    return View(propietario);
                }
            }
            catch
            {
                throw;
            }
        }

        // GET: Propietarios/Edit/5
        public ActionResult Edit(int id)
        {
            try {
                var propietario = repo.ObtenerPorId(id);
                return View(propietario);
            }
            catch (Exception ex) {
                throw;
            }
            
        }

        // POST: Propietarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Propietario p)
        {
            Propietario prop = null;

            try
            {
                // TODO: Add update logic here
                prop = repo.ObtenerPorId(id);
                prop.Nombre = p.Nombre;
                prop.Apellido = p.Apellido;
                prop.Dni = p.Dni;
                prop.Email = p.Email;
                prop.Telefono = p.Telefono;
                repo.Modificacion(prop);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Propietarios/Delete/5
        public ActionResult Delete(int id)
        {
            try {
                var propietario = repo.ObtenerPorId(id);
                return View(propietario);
            }
            catch (Exception ex) {
                throw;
            }
        }

        // POST: Propietarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Propietario propietario)
        {
            try
            {
                // TODO: Add delete logic here
                repo.Baja(id);
                TempData["Mensaje"] = "Eliminado correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}