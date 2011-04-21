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
            return View(model.getCourse(id));
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {

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

            return View(model.getCourse(id));
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
            catch
            {
                return View();
            }
        }

        //
        // GET: /Course/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Course/Delete/5

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
