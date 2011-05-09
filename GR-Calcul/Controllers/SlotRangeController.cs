using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using System.Web.Security;
using GR_Calcul.Misc;


namespace GR_Calcul.Controllers
{ 
    public class SlotRangeController : BaseController
    {
        private CourseModel courseModel = new CourseModel();
        private SlotRangeModel slotRangeModel = new SlotRangeModel();

        private void InitViewbag()
        {
            ViewBag.IdCourse = new SelectList(courseModel.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)), "ID", "Name");
            ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text");
        }

        //
        // GET: /SlotRange/CourseRanges/5
        [DuffAuthorize(PersonType.Responsible, PersonType.ResourceManager)]
        public ActionResult CourseRanges(int id)
        {
            InitViewbag();
            CourseRangesViewModel viewModel = new CourseRangesViewModel();
            viewModel.SlotRanges = slotRangeModel.GetSlotRangesForCourse(id);
            viewModel.Course = courseModel.GetCourse(id);
            viewModel.Course.Students = courseModel.getCourseStudents(id);
            return View(viewModel);
        }

        //
        // GET: /SlotRange/CourseRanges/5
        [DuffAuthorize(PersonType.User)]
        public ActionResult ReserveSlotRange(int id)
        {
            InitViewbag();
            ReserveSlotRangeViewModel viewModel = new ReserveSlotRangeViewModel();
            viewModel.SlotRanges = slotRangeModel.GetSlotRangesForCourse(id);
            viewModel.Course = courseModel.GetCourse(id);
            int id_person = 1;
            viewModel.Reservations = slotRangeModel.getReservations(id, id_person);
            return View(viewModel);
        }

        //
        // GET: /SlotRange/Create
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Create()
        {
            InitViewbag();
            return View();
        }

        //
        // POST: /SlotRange/Create

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Create(SlotRange range)
        {
            InitViewbag();
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
                    ModelState.AddModelError("", "Une erreur est survenue: " + error.Message);
                    ViewBag.Error = error.Message;
                    return View(range);
                }
            }
            ModelState.AddModelError("", "Il y a des données incorrectes. Corriger les erreurs!");
            return View(range);
        }

        private bool IsAuthorized(SlotRange range)
        {
            if (range == null)
                return false;
            int? rId = range.GetResponsible();
            if (rId != null)
            {
                int? pId = SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name);
                if (pId != null)
                {
                    if (pId == rId)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }                
            }
            else
            {
                return false;
            }
        }

        //
        // GET: /SlotRange/Edit/5
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id)
        {
            Exception ex = new Exception("Access denied");
            SlotRange range = slotRangeModel.GetSlotRange(id);
            if (range == null)
            {
                return View("NoSuchRange");
            }
            int? rId = range.GetResponsible();
            if (IsAuthorized(range))
            {
                ViewBag.IdCourse = new SelectList(courseModel.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)), "ID", "Name", range.IdCourse);
                ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text", range.SlotDuration);
                return View(range);
            }
            else
            {
                throw ex;
            }
        }

        //
        // POST: /SlotRange/Edit/5

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id, SlotRange range)
        {
            Exception ex = new Exception("Access denied");
            if (IsAuthorized(range))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        slotRangeModel.UpdateSlotRange(range);
                        ViewBag.Mode = "mise a jour";
                        return View("Complete", range);
                    }
                    catch (Exception exx)
                    {
                        ViewBag.ErrorMode = "la mise à jour";
                        return View("Error", exx);
                    }
                }
                ViewBag.IdCourse = new SelectList(courseModel.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)), "ID", "Name", range.IdCourse);
                ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text", range.SlotDuration);

                ModelState.AddModelError("", "Il y a des données incorrectes. Corriger les erreurs!");
                return View(range);

            }
            else
            {
                throw ex;
            }
        }

        //
        // GET: /SlotRange/Delete/5

        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id)
        {
            Exception ex = new Exception("Access denied");
            SlotRange range = slotRangeModel.GetSlotRange(id);
            if (range == null)
            {
                return View("NoSuchRange");
            }
            int? rId = range.GetResponsible();
            if (IsAuthorized(range))
            {
                return View(range);
            }
            else
            {
                throw ex;
            }
        }

        //
        // POST: /SlotRange/Delete/5

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Exception ex = new Exception("Access denied");
            if (IsAuthorized(slotRangeModel.GetSlotRange(id)))
            {
                try
                {
                    slotRangeModel.DeleteSlotRange(id);
                    return RedirectToAction("CourseRanges");
                }
                catch (Exception exx)
                {
                    ViewBag.ErrorMode = "la suppression";
                    return View("Error", exx);
                }
            }
            else
            {
                throw ex;
            }            
        }
    }
}
