using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using System.Data;
using GR_Calcul.Misc;

namespace GR_Calcul.Controllers
{
    public class CourseController : BaseController
    {
        private CourseModel model = new CourseModel();
        private PersonModel personModel = new PersonModel();
        //
        // GET: /Course/

        [DuffAuthorize(PersonType.ResourceManager, PersonType.Responsible, PersonType.User)]
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // GET: /Course/List

        [DuffAuthorize(PersonType.ResourceManager, PersonType.Responsible, PersonType.User)]
        public ActionResult List()
        {
            switch (SessionManager.GetCurrentUserRole(HttpContext.User.Identity.Name))
            {
                case PersonType.Responsible:
                    return View(model.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)));
                case PersonType.User:
                case PersonType.ResourceManager:
                    return View(model.ListCourses());
                default:
                    SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                    return null;
            }
        }
        //
        // GET: /Course/List

        [DuffAuthorize(PersonType.User)]
        public ActionResult ListMyCourse()
        {
            return View(model.ListMyCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)));
        }

        //
        // GET: /Course/Details/5
        [DuffAuthorize(PersonType.Responsible, PersonType.User)]
        public ActionResult Details(int id)
        {
            switch (SessionManager.GetCurrentUserRole(HttpContext.User.Identity.Name))
            {
                case PersonType.Responsible:
                    return View(model.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)));
                case PersonType.User:
                    int? k = SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name);
                    if (k != null)
                    {
                        if (model.IsUserSubscribed((int)k, id))
                            return View(model.GetCourse(id));
                        else
                        {
                            SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                            return null;
                        }
                    }
                    else
                    {
                        SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                        return null;
                    }
                default:
                    SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                    return null;
            }
        }

        //
        // GET: /Course/Create
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Create()
        {
            var items = personModel.GetResponsibles().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.toString() }).ToList();
            ViewData["Responsibles"] = new SelectList(items, "Value", "Text");

            return View();
        }

        //
        // POST: /Course/Create

        [DuffAuthorize(PersonType.Responsible)]
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

        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id)
        {
            if (IsAuthorized(id))
            {
                Course course = model.GetCourse(id);
                var items = personModel.GetResponsibles().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.toString() }).ToList();
                ViewData["Responsibles"] = new SelectList(items, "Value", "Text", course.Responsible);
                return View(course);
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id, Course course)
        {
            if (IsAuthorized(id))
            {
                try
                {
                    model.UpdateCourse(course);
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    var items = personModel.GetResponsibles().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.toString() }).ToList();
                    ViewData["Responsibles"] = new SelectList(items, "Value", "Text", course.Responsible);
                    ViewData["error"] = e.Message;
                    return View(course);
                }
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        //
        // GET: /Course/Duplicate/5

        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Duplicate(int id)
        {
            if (IsAuthorized(id))
            {
                return View(model.GetCourse(id));
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        //
        // POST: /Course/Duplicate/5

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Duplicate(Course course)
        {
            if (IsAuthorized(course.ID))
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
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        //
        // GET: /Course/Delete/5

        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id)
        {
            if (IsAuthorized(id))
            {
                return View(model.GetCourse(id));
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // POST: /Course/Delete/5

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id, Course course)
        {

            if (IsAuthorized(id))
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
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        private bool IsAuthorized(int courseId)
        {
            Course c = model.GetCourse(courseId);

            int? pId = SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name);
            if (pId != null)
            {
                if (pId == c.Responsible)
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

    }
}
