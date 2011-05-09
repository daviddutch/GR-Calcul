﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using System.Web.Security;
using GR_Calcul.Misc;
using System.Text;


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
            Course course = CourseModel.GetCourse(id_course);
            viewModel.Course = course;
            viewModel.SlotRanges = course.GetSlotRangesForCourse();
            viewModel.Course.Students = courseModel.getCourseStudents(id);
            viewModel.Course.Students = courseModel.getCourseStudents(id_course);
            return View(viewModel);
        }

        //
        // GET: /SlotRange/ReserveSlotRange/5
        // GET: /SlotRange/CourseRanges/5
        [DuffAuthorize(PersonType.User)]
        public ActionResult ReserveSlotRange(int id)
        {
            InitViewbag();
            ReserveSlotRangeViewModel viewModel = new ReserveSlotRangeViewModel();
            Course course = CourseModel.GetCourse(id);
            viewModel.Course = course;
            viewModel.SlotRanges = course.GetSlotRangesForCourse();
            int id_person = 1;
            viewModel.Reservations = slotRangeModel.getReservations(id, id_person);
            return View(viewModel);
        }

        //// CD test - DELETEME
        //public void TestSlotReservation(string str)
        //{
        //    SlotRange range = new SlotRange(3, DateTime.Parse("2008-11-01T19:35:00.0000000Z"), DateTime.Parse("2008-11-01T19:35:00.0000000Z"), "test", 3, "0x00000000000008AA");
        //    List<Machine> machines = new List<Machine>();
        //    machines.Add(new Machine(1, "test333", "1.1.1.1", 1));
        //    range.InsertCommandXML(new Person(PersonType.User, 1, "Christopher", "Dickinson", str, "as@base.c", "asdf"), new Slot(3, DateTime.Parse("2008-11-01T19:35:00.0000000Z"), DateTime.Parse("2008-11-01T19:35:00.0000000Z")), machines);
        //}

        //// CD test - DELETEME
        //public void TestSlotUnReservation(string str)
        //{
        //    SlotRange range = new SlotRange(3, DateTime.Parse("2008-11-01T19:35:00.0000000Z"), DateTime.Parse("2008-11-01T19:35:00.0000000Z"), "test", 3, "0x00000000000008AA");
        //    range.DeleteCommandXML(str);
        //}

        //
        // GET: /SlotRange/Script/5
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Script(int id)
        {
            SlotRange range = SlotRangeModel.GetSlotRange(id);
            string script = range.GenerateScript();
            return File(Encoding.UTF8.GetBytes(script), "text/plain", string.Format("scripts_cours_{0}.sh", id));
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
            SlotRange range = SlotRangeModel.GetSlotRange(id);
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
            SlotRange range = SlotRangeModel.GetSlotRange(id);
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
            if (IsAuthorized(SlotRangeModel.GetSlotRange(id)))
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
