using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using System.Data;

namespace GR_Calcul.Controllers
{
    public class CourseController : Controller
    {
        private CourseModel model = new CourseModel();
        private PersonModel personModel = new PersonModel();
        //
        // GET: /Course/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // GET: /Course/List

        public ActionResult List()
        {
            
            return View(model.ListCourses());
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id)
        {
            return View(model.GetCourse(id));
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            var items = personModel.GetResponsibles().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.toString() }).ToList();
            ViewData["Responsibles"] = new SelectList(items, "Value", "Text");

            return View();
        }

        //
        // POST: /Course/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            try
            {
                model.CreateCourse(course);
                return RedirectToAction("Index");
            }
            catch (SqlException sqlError)
            {
                Console.WriteLine(sqlError);
                return View();
            }
        }

        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id)
        {
            Course course = model.GetCourse(id);
            var items = personModel.GetResponsibles().Select(x => new SelectListItem (){ Value = x.ID.ToString(), Text = x.toString() }).ToList();
            ViewData["Responsibles"] = new SelectList(items, "Value", "Text", course.Responsible);
            return View(course);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Course course)
        {
            try
            {
                model.UpdateCourse(course);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {

                ViewData["error"] = e.Message;
                return View();
            }
        }
        //
        // GET: /Course/Duplicate/5

        public ActionResult Duplicate(int id)
        {

            return View(model.GetCourse(id));
        }
        //
        // POST: /Course/Duplicate/5

        [HttpPost]
        public ActionResult Duplicate(Course course)
        {
            try
            {
                model.CreateCourse(course);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        //
        // GET: /Course/Delete/5

        public ActionResult Delete(int id)
        {
            return View(model.GetCourse(id));
        }

        //
        // POST: /Course/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Course course)
        {
            try
            {
                model.DeleteCourse(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
