using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaAlaniz.Models;
using Microsoft.AspNetCore.Authorization;

namespace InmobiliariaAlaniz.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        RepositorioPago repo;
        RepositorioContrato repositorioContrato;
  

        public PagosController() {
            repo = new RepositorioPago();
            repositorioContrato = new RepositorioContrato();
          
        }
      
        public ActionResult Index()
        {
            try{
                var pago = repo.ObtenerTodos();
                 /*if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];*/
                return View(pago);
            }
            catch (Exception ex) {
                throw;
            }
        }
        public ActionResult Details(int id)
        {
            try {
                var contrato = repo.ObtenerPorId(id);
                return View(contrato);
            }
            catch (Exception ex) {
                throw;
            }
        }

        
        public ActionResult Create(int id)
        {
            try {
                if (id > 0) 
                {
                    Contrato contrato = repositorioContrato.ObtenerPorId(id);
                    if (contrato.Id == 0)
                    {
                        TempData["mensaje"] = "Contrato no encontrado";
                        return View();
                    }

                    ViewBag.Contrato = contrato;
                    return View();
                }
                ViewBag.Contratos = repositorioContrato.ObtenerTodos();
                return View();
            }
            catch (Exception ex) {
                throw;
            }
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int id, Pago pago)
        {
           try
            {
                 if (id > 0 && pago.IdContrato == 0)
                    {
                        pago.Id = 0;
                        pago.IdContrato = id;
                    }
                        var res = repo.Alta(pago);
                        if (res > 0)
                        {
                            TempData["mensaje"] = "Pago guardado correctamente";
                            return RedirectToAction(nameof(Index));
                        } else
                        {
                            TempData["mensaje"] = "Error al cargar";
                            return RedirectToAction(nameof(Create));
                        }
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        
        public ActionResult Edit(int id)
        {
            try {
                var pago = repo.ObtenerPorId(id);
                ViewBag.Contratos = repositorioContrato.ObtenerTodos();

          
                return View(pago);
            }
            catch (Exception ex) {
                throw;
            }
            
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Pago pago)
        {
            Pago p = null;

            try
            {
                // TODO: Add update logic here
                p = repo.ObtenerPorId(id);
                p.FechaPago = pago.FechaPago;
                p.Importe = pago.Importe;
                p.IdContrato = pago.IdContrato;             
                repo.Modificacion(p);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
        public ActionResult Delete(int id)
        {
             try {
                var pago = repo.ObtenerPorId(id);
               
                return View(pago);
            }
            catch (Exception ex) {
                throw;
            }
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public ActionResult Delete(int id, Pago pago)
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