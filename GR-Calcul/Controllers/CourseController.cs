using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using System.Data;
using GR_Calcul.Misc;
using System.Text;


namespace GR_Calcul.Controllers
{
    /// <summary>
    /// Class containing all actions related to the course module
    /// </summary>
    public class CourseController : BaseController
    {

        /// <summary>
        /// GET: /Course/List
        /// Displays a list of courses
        /// </summary>
        [DuffAuthorize(PersonType.ResourceManager, PersonType.Responsible, PersonType.User)]
        public ActionResult Index()
        {
            switch (SessionManager.GetCurrentUserRole(HttpContext.User.Identity.Name))
            {
                case PersonType.Responsible:
                    return View(CourseModel.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)));
                case PersonType.User:
                    return View(CourseModel.ListCoursesUser(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)));
                case PersonType.ResourceManager:
                    return View(CourseModel.ListCourses());
                default:
                    SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                    return null;
            }
        }

        /// <summary>
        /// GET: /Course/ListMyCourse
        /// Displays the list of course which the user is subscribed to
        /// </summary>
        [DuffAuthorize(PersonType.User)]
        public ActionResult ListMyCourse()
        {
            ViewBag.Title = "Liste de mes cours";
            return View("Index", CourseModel.ListMyCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)));
        }

        /// <summary>
        /// GET: /Course/Create
        /// Form to create a course
        /// </summary>
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Create()
        {
            var items = PersonModel.GetResponsibles().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.toString() }).ToList();
            ViewData["Responsibles"] = new SelectList(items, "Value", "Text");

            return View();
        }


        /// <summary>
        /// POST: /Course/Create
        /// The form to create a course is posted here
        /// </summary>
        /// <param name="course">The form content</param>
        [DuffAuthorize(PersonType.Responsible)]
        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    course.Responsible = (int)SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name);
                    CourseModel.CreateCourse(course);
                    return RedirectToAction("Index");
                }
                catch (GrException gex)
                {
                    System.Diagnostics.Debug.WriteLine(gex.UserMessage);
                    System.Diagnostics.Debug.WriteLine(gex.StackTrace);
                    ModelState.AddModelError("", gex.UserMessage);
                    return View(course);
                }
            }
            else
            {
                // addinge extra error message here in case JS is deactivated on client.
                ModelState.AddModelError("", "vous avez envoyé des données invalides");
                return View(course);
            }

        }

        /// <summary>
        /// GET: /Course/Script
        /// Generate a script for a given course
        /// </summary>
        /// <param name="id">The id of the course</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Script(int id)
        {
            Course course = CourseModel.GetCourse(id);
            string allScripts = course.GenerateAllScripts();
            return File(Encoding.UTF8.GetBytes(allScripts),"text/plain",string.Format("scripts_cours_{0}.sh", id));
        }

        /// <summary>
        /// GET: /Course/Edit
        /// Form to edit a course
        /// </summary>
        /// <param name="id">Id of the course</param>
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id)
        {
            if (IsAuthorized(id))
            {
                return View(CourseModel.GetCourse(id));
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        /// <summary>
        /// POST: /Course/Edit
        /// The form content of the edit course is sended here
        /// </summary>
        /// <param name="id">The id of the course</param>
        /// <param name="course">The course content</param>
        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id, Course course)
        {
            if (IsAuthorized(id))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        CourseModel.UpdateCourse(course);
                        return RedirectToAction("Index");
                    }
                    catch (GrException e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                        System.Diagnostics.Debug.WriteLine(e.StackTrace);
                        ModelState.AddModelError("", e.UserMessage);
                        return View(course);
                    }
                }
                else
                {
                    // addinge extra error message here in case JS is deactivated on client.
                    ModelState.AddModelError("", "vous avez envoyé des données invalides");
                    return View(course);
                }
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        /// <summary>
        /// GET: /Course/Duplicate
        /// Open a form to create a course with data of an other one
        /// </summary>
        /// <param name="id">Id of the course to duplicate</param>
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Duplicate(int id)
        {
            if (IsAuthorized(id))
            {
                Course course = CourseModel.GetCourse(id);
                course.DuplDestDate = DateTime.Now;
                return View(course);
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        /// <summary>
        /// POST: /Course/Duplicate
        /// The data of the duplicate form is sended here
        /// </summary>
        /// <param name="course">Course data</param>
        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Duplicate(Course course)
        {
            if (IsAuthorized(course.ID))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        int id_course = CourseModel.CreateCourse(course);
                        List<SlotRange> slotRanges = course.GetSlotRangesForCourse();
                        int days = 0;
                        foreach (SlotRange sr in slotRanges)
                        {
                            if (days == 0)
                            {
                                TimeSpan span = course.DuplDestDate - sr.StartRes;
                                days = (int)span.TotalDays;
                            }
                                
                            SlotRangeModel.DuplicateSlotRange(sr, days, id_course);
                        }
                        return RedirectToAction("Index");
                    }
                    catch (GrException gex)
                    {
                        System.Diagnostics.Debug.WriteLine(gex.UserMessage);
                        System.Diagnostics.Debug.WriteLine(gex.StackTrace);
                        ModelState.AddModelError("", gex.UserMessage);
                        return View(course);
                    }
                }
                else
                {
                    // addinge extra error message here in case JS is deactivated on client.
                    ModelState.AddModelError("", "vous avez envoyé des données invalides");
                    return View(course);
                }
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        /// <summary>
        /// GET: /Course/Delete
        /// Page to confirm the delete of a course
        /// </summary>
        /// <param name="id">Id of the course to delete</param>
        /// <returns>The html page to be displayed</returns>
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id)
        {
            if (IsAuthorized(id))
            {
                return View(CourseModel.GetCourse(id));
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        /// <summary>
        /// POST: /Course/Delete
        /// The course is deleted
        /// </summary>
        /// <param name="id">Id of the course to be deleted</param>
        /// <param name="course">The course to be deleted</param>
        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id, Course course)
        {

            if (IsAuthorized(id))
            {
                try
                {
                    CourseModel.DeleteCourse(course);

                    return RedirectToAction("Index");
                }
                catch (GrException gex)
                {
                    System.Diagnostics.Debug.WriteLine(gex.UserMessage);
                    System.Diagnostics.Debug.WriteLine(gex.StackTrace);
                    ModelState.AddModelError("", gex.UserMessage);
                    return View(CourseModel.GetCourse(id));
                }
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        /// <summary>
        /// GET: /Course/Unsubscribe
        /// Displays the confirmation to unsubscribe from a course
        /// </summary>
        /// <param name="id">The id of the course</param>
        [DuffAuthorize(PersonType.User)]
        public ActionResult Unsubscribe(int id)
        {
            return View(CourseModel.GetCourse(id));
        }
        /// <summary>
        /// POST: /Course/Unsubscribe
        /// Unsubscribe from the course
        /// </summary>
        /// <param name="id">Id of the course</param>
        /// <param name="course">The data of the course</param>
        [HttpPost]
        [DuffAuthorize(PersonType.User)]
        public ActionResult Unsubscribe(int id, Course course)
        {
            try
            {
                course.Unsubscribe(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name), course);

                return RedirectToAction("Index");
            }
            catch (GrException gex)
            {
                System.Diagnostics.Debug.WriteLine(gex.UserMessage);
                System.Diagnostics.Debug.WriteLine(gex.StackTrace);
                ModelState.AddModelError("", gex.UserMessage);
                return View();
            }
        }
        /// <summary>
        /// GET: /Course/Subscribe
        /// Displays the page to enter the key of the course before subsribing to it
        /// </summary>
        /// <param name="id">id of the course</param>
        [DuffAuthorize(PersonType.User)]
        public ActionResult Subscribe(int id)
        {
            Course course = CourseModel.GetCourse(id);
            course.Key = "";
            return View(course);
        }

        /// <summary>
        /// POST: /Course/Subscribe
        /// Subscribe to the course
        /// </summary>
        /// <param name="id">Id of the course</param>
        /// <param name="course">Data of the course</param>
        [HttpPost]
        [DuffAuthorize(PersonType.User)]
        public ActionResult Subscribe(int id, Course course)
        {            
            try
            {
                if (CourseModel.GetCourse(id).Key == course.Key)
                {
                    course.Subscribe(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name));
                    return RedirectToAction("ListMyCourse");
                }
                else
                {
                    ModelState.AddModelError("", "La clefs n'est pas correct.");
                    return View(course);
                }
            }
            catch (GrException gex)
            {
                System.Diagnostics.Debug.WriteLine(gex.UserMessage);
                System.Diagnostics.Debug.WriteLine(gex.StackTrace);
                ModelState.AddModelError("", gex.UserMessage);
                return View();
            }
        }
        /// <summary>
        /// Check if the user has specifics rights on a course
        /// </summary>
        /// <param name="courseId">The id of the course to check</param>
        private bool IsAuthorized(int courseId)
        {
            Course c = CourseModel.GetCourse(courseId);

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
