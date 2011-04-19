using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;

namespace GR_Calcul.Controllers
{
    public class SlotRangeController : Controller
    {
        //
        // GET: /SlotRange/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /SlotRange/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /SlotRange/Create

        public ActionResult Create()
        {

            //return View(new SlotRangeFormViewModel());

            ViewData["SlotDuration"] = SlotRangeModels.blabla;
            //ViewData["SlotDuration"] = new List<SelectListItem>(SlotRangeModels.blabla);
            //ViewData["SlotDuration"] = new List<int>(SlotRangeModels.blabla);
            return View();


            //return View(new SlotRangeModels());
        } 

        //
        // POST: /SlotRange/Create

        [HttpPost]
        public ActionResult Create(SlotRangeModels range)
        {
            ViewData["SlotDuration"] = SlotRangeModels.blabla;
            //return View("Complete", range);
            
            if (ModelState.IsValid)
                return View("Complete", range);
            return View(range);
            
        }
        
        //
        // GET: /SlotRange/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /SlotRange/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /SlotRange/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /SlotRange/Delete/5

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
