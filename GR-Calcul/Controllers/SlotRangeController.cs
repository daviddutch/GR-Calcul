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
    public class SlotRangeController : Controller
    {
        private CourseModel courseModel = new CourseModel();
        private SlotRangeModel slotRangeModel = new SlotRangeModel();

        private void InitViewbag()
        {
            ViewBag.IdCourse = new SelectList(courseModel.ListCourses(), "ID", "Name");
            ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text");
        }

        public class User
        {
            public string UserName { get; set; }
            public UserRole Role { get; set; }

            public static User GetCurrentUser { get; set; }

            public Boolean IsInRole(UserRole[] roles)
            {
                foreach(UserRole r in roles){
                    if(r.Equals(Role))
                        return true;
                }
                return false;
            }

            public static void CreateInstance(string u, UserRole r){
                GetCurrentUser = new User();
                GetCurrentUser.UserName = u;
                GetCurrentUser.Role = r;
            }

        }

        //
        //GET: /SlotRange/SignIn/user

        public string SignIn()
        {
            string userName = "thomas";
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Value cannot be null or empty.", "userName");

            FormsAuthentication.SetAuthCookie(userName, true);

            User.CreateInstance(userName, UserRole.ResourceManager);

            return "himode";
        }

        //
        // GET: /SlotRange/CourseRanges/5

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

        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }

        //
        // GET: /SlotRange/Create
        [DuffAuthorize(UserRole.Responsible)]
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
            InitViewbag();
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
                    ViewBag.Error = error.Message;
                    return View(range);
                }
            }
            ViewBag.Error = invalidMessage;
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
