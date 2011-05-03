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
        private SlotRangeModel slotRangeModel = new SlotRangeModel();

        private void InitViewbag()
        {
            ViewBag.IdCourse = new SelectList(courseModel.ListCourses(), "ID", "Name");
            ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text");
        }

        //
        // GET: /SlotRange/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /SlotRange/CourseRanges/5

        public ActionResult CourseRanges(int id)
        {
            InitViewbag();
            List<SlotRange> list = slotRangeModel.GetSlotRangesForCourse(id);
            return View(list);
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
            InitViewbag();
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
                try
                {
                    slotRangeModel.CreateSlotRange(range);
                    ViewBag.Mode = "créée";
                    return View("Complete", range);
                }
                catch (Exception error)
                {
                    ViewBag.ErrorMode = "la création";
                    return View("Error", error);
                }
            }

            return View("Invalid", invalidMessage);
        }

        
        //
        // GET: /SlotRange/Edit/5
 
        public ActionResult Edit(int id)
        {
            SlotRange range = slotRangeModel.GetSlotRange(id);
            if (range == null)
            {
                return View("NoSuchRange");
            }
            else
            {
                ViewBag.IdCourse = new SelectList(courseModel.ListCourses(), "ID", "Name", range.IdCourse);
                ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text", range.SlotDuration);
                return View(range);
            }
        }

        //
        // POST: /SlotRange/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, SlotRange range)
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
                try
                {
                    slotRangeModel.UpdateSlotRange(range);
                    ViewBag.Mode = "mise a jour";
                    return View("Complete", range);
                }
                catch(Exception ex)
                {
                    ViewBag.ErrorMode = "la mise à jour";
                    return View("Error", ex);
                }
            }
            else
            {
                return View("Invalid", invalidMessage);
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
