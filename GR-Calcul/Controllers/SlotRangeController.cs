using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;


namespace GR_Calcul.Controllers
{
    public class SlotRangeController : Controller
    {
        private CourseModel courseModel = new CourseModel();
        private SlotRangeModel slotRange = new SlotRangeModel();

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
            ViewBag.IdCourse = new SelectList(courseModel.ListCourses(), "ID", "Name");
            ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text");
            return View();
        } 

        //
        // POST: /SlotRange/Create

        [HttpPost]
        public ActionResult Create(SlotRange range)
        {
            string invalidMessage = "";
            foreach (var value in ModelState.Values)
            {
                foreach (var error in value.Errors)
                {
                    invalidMessage += error.ErrorMessage;
                }
            }

            if (ModelState.IsValid)
            {

                SqlException error = slotRange.CreateSlotRange(range);
                if (error == null)
                {
                    return View("Complete", range);
                }
                return View("Error", error);
            }

            //ViewBag.IdCourse = new SelectList(courseModel.ListCourses(), "ID", "Name");
            //ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text");

            return View("Invalid", invalidMessage);
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
