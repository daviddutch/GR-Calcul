using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;

namespace GR_Calcul.Controllers
{
    public class MachineController : Controller
    {
        private MachineModel model = new MachineModel();

        //
        // GET: /Machine/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // GET: /Machine/List

        public ActionResult List()
        {
            return View(model.ListMachines());
        }

        //
        // GET: /Machine/Details/5

        public ActionResult Details(int id)
        {
            return View(model.getMachine(id));
        }

        //
        // GET: /Machine/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Machine/Create

        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        
        //
        // GET: /Machine/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View(model.getMachine(id));
        }

        //
        // POST: /Machine/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Machine machine)
        {
            try
            {
                // TODO: Add update logic here
                model.UpdateMachine(machine);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        ////
        //// GET: /Machine/Delete/5
 
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //
        // POST: /Machine/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
