using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using System.Web.Security;
using GR_Calcul.Misc;
using System.Text;
using TaskScheduler;
using System.Globalization;


namespace GR_Calcul.Controllers
{
    /// <summary>
    /// Class containing all actions related to the slotRange module
    /// </summary>
    public class SlotRangeController : BaseController
    {
        private void InitViewbag(int IdCourse)
        {
            //ViewBag.IdCourse = new SelectList(CourseModel.ListCourses(SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name)), "ID", "Name");
            ViewBag.CourseName = CourseModel.GetCourse(IdCourse).Name;// cd: for when locked
            ViewBag.SlotDuration = new SelectList(Slot.durationList, "Text", "Text");
        }

        //
        // GET: /SlotRange/CourseRanges/5
        [DuffAuthorize(PersonType.Responsible, PersonType.ResourceManager)]
        public ActionResult CourseRanges(int id)
        {
            CourseRangesViewModel viewModel = new CourseRangesViewModel();
            Course course = CourseModel.GetCourse(id);
            if (course == null || course.Responsible == SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name))
            {
                InitViewbag(id);
                viewModel.Course = course;
                viewModel.SlotRanges = course.GetSlotRangesForCourse();
                viewModel.Course.Students = CourseModel.getCourseStudents(id);
                return View(viewModel);
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // GET: /SlotRange/ReserveSlotRange/3
        [DuffAuthorize(PersonType.User)]
        public ActionResult ReserveSlotRange(int id)
        {
            InitViewbag(id);
            ReserveSlotRangeViewModel viewModel = new ReserveSlotRangeViewModel();
            Course course = CourseModel.GetCourse(id);
            viewModel.Course = course;
            viewModel.SlotRanges = course.GetValidSlotRangesForCourse();
            int? id_person = SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name);
            viewModel.Reservations = SlotRangeModel.getReservations(id, id_person);
            return View(viewModel);
        }

        //
        // GET: /SlotRange/Reserve/5
        [DuffAuthorize(PersonType.User)]
        public JsonResult Reserve(int id, bool reserve)
        {
            try
            {
                if (reserve)
                {
                    SlotRangeModel.ReserveSlot(id, (int) SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name));
                }
                else
                {
                    SlotRangeModel.UnReserveSlot(id, SessionManager.GetCurrentUserId(HttpContext.User.Identity.Name));
                }
                return new JsonResult { Data = new { Success = true, Message = "Bravo" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (GrException gex)
            {
                return new JsonResult { Data = new { Success = false, Message = gex.UserMessage }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }

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
        public ActionResult Create(int IdCourse)
        {
            InitViewbag(IdCourse);
            return View();
        }

        //
        // POST: /SlotRange/Create

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Create(SlotRange range)
        {
            InitViewbag(range.IdCourse);
            if (ModelState.IsValid)
            {
                try
                {
                    SlotRangeModel.CreateSlotRange(range);
                    ViewBag.Mode = "créée";

                    // schedule linux script to be sent to resource manager by email 
                    ScheduleEmail(range);

                    return View("Complete", range);
                }
                catch (GrException gex)
                {
                    ModelState.AddModelError("", gex.UserMessage);
                    ViewBag.Error = gex.UserMessage;

                    System.Diagnostics.Debug.WriteLine(gex.UserMessage);
                    System.Diagnostics.Debug.WriteLine(gex.StackTrace);
                    
                    return View(range);
                }
            }
            ModelState.AddModelError("", Messages.invalidData);
            
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


        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Duplicate(int id)
        {
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
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }
        
        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Duplicate(int id, SlotRange target)
        {
            SlotRange source = SlotRangeModel.GetSlotRange(id);
            if (source == null)
            {
                return View("NoSuchRange");
            }
            int? rId = source.GetResponsible();
            if (IsAuthorized(source))
            {
                TimeSpan span= target.StartRes - source.StartRes;
                int days = (int)span.TotalDays;
                SlotRangeModel.DuplicateSlotRange(source, days, source.IdCourse);
                return RedirectToAction("CourseRanges", new { id = source.IdCourse});
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // GET: /SlotRange/Edit/5
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id)
        {
            SlotRange range = SlotRangeModel.GetSlotRange(id);
            if (range == null)
            {
                return View("NoSuchRange");
            }
            int? rId = range.GetResponsible();
            if (IsAuthorized(range))
            {
                InitViewbag(range.IdCourse);

                return View(range);
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // POST: /SlotRange/Edit/5
        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Edit(int id, SlotRange range)
        {
            if (IsAuthorized(range))
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        SlotRangeModel.UpdateSlotRange(range);

                        // schedule linux script to be sent to resource manager by email 
                        ScheduleEmail(range);

                        ViewBag.Mode = "mise a jour";
                        return View("Complete", range);
                    }
                    catch (GrException gex)
                    {
                        ModelState.AddModelError("", gex.UserMessage);
                        return View(range);
                    }
                }
                InitViewbag(range.IdCourse);

                ModelState.AddModelError("", Messages.invalidData);

                return View(range);

            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // GET: /SlotRange/Delete/5

        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id)
        {
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
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }
        }

        //
        // POST: /SlotRange/Delete/5

        [HttpPost]
        [DuffAuthorize(PersonType.Responsible)]
        public ActionResult Delete(int id, SlotRange range)
        {
            SlotRange sr = SlotRangeModel.GetSlotRange(id);
            if (IsAuthorized(SlotRangeModel.GetSlotRange(id)))
            {
                try
                {
                    SlotRangeModel.DeleteSlotRange(id, range);
                    return RedirectToAction("CourseRanges", new { id= sr.IdCourse});
                }
                catch (GrException gex)
                {

                    ModelState.AddModelError("", gex.UserMessage);

                    // get updated data
                    SlotRange range_ = SlotRangeModel.GetSlotRange(id);

                    // update timestamp in case user really wants to delete this
                    ModelState.SetModelValue("Timestamp", new ValueProviderResult(range_.Timestamp, "", CultureInfo.InvariantCulture));

                    // show new values before user decided to really delete them
                    return View(range_);

                    //ViewBag.ErrorMode = "la suppression";
                    //return View("Error", exx);
                }
            }
            else
            {
                SessionManager.RedirectAccessDenied(HttpContext.Request.RequestContext);
                return null;
            }            
        }

        // CREATE OR UPDATE, c.f. RegisterTaskDefinition()
        [DuffAuthorize(PersonType.Responsible)]
        private void ScheduleEmail(SlotRange range)
        {
            // create new Scheduled Task (email)
            TaskSchedulerClass scheduler = new TaskSchedulerClass();

            // TODO: here we may need to specify the user
            scheduler.Connect(null, null, null, null); // CD: use current machine, username, password, domain

            // Registration
            ITaskDefinition task = scheduler.NewTask(0);
            task.RegistrationInfo.Author = "GRcalcul";
            task.RegistrationInfo.Description = "email linux script task";
            task.RegistrationInfo.Date = DateTime.Now.ToString("yyyy-MM-ddTHH:MM:ss");

            // Settings
            task.Settings.MultipleInstances = _TASK_INSTANCES_POLICY.TASK_INSTANCES_IGNORE_NEW;
            task.Settings.DisallowStartIfOnBatteries = false;
            task.Settings.StopIfGoingOnBatteries = false;
            task.Settings.AllowHardTerminate = true;
            task.Settings.StartWhenAvailable = true;
            task.Settings.RunOnlyIfNetworkAvailable = true;
            task.Settings.AllowDemandStart = true;
            task.Settings.Hidden = false;
            task.Settings.WakeToRun = true;
            task.Settings.ExecutionTimeLimit = "PT1H"; // 1 hour
            task.Settings.DeleteExpiredTaskAfter = "PT12M"; // 1 year

            ITimeTrigger trigger = (ITimeTrigger)task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
            trigger.Id = "EmailTriggerForSlotRange_" + range.id_slotRange;
            DateTime dt = range.EndRes.Add(new System.TimeSpan(1, 0, 0, 0)); // Midnight after EndRes
            trigger.StartBoundary = dt.ToString("yyyy-MM-ddTHH:MM:ss");
            trigger.EndBoundary = dt.Add(new System.TimeSpan(0, 0, 1, 0)).ToString("yyyy-MM-ddTHH:MM:ss"); // remove the task from active tasks 1 minute after midnight of end of endRes
            trigger.ExecutionTimeLimit = "PT2M"; // 2 minutes

            IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Id = "EmailActionForSlotRange_" + range.id_slotRange;
            action.Path = "C:\\script.vbs";
            action.Arguments = range.id_slotRange.ToString();

            ITaskFolder root = scheduler.GetFolder("\\");
            IRegisteredTask regTask = root.RegisterTaskDefinition(
                "EmailTaskForSlotRange_" + range.id_slotRange,
                task,
                (int)_TASK_CREATION.TASK_CREATE_OR_UPDATE,
                null, // username - we're using the logged in user of this web app
                null, // password - we're using the logged in user of this web app
                _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN); // for simplicity, we're using the logged in Windows user who is running this web app
        }

        // this method uses host-based authentication
        /// <summary>
        /// GET: /SlotRange/EmailScript/3
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EmailScript(int id)
        {
            if (Request.Url.Host != "localhost" && Request.UserHostAddress != "127.0.0.1")
            {
                throw new Exception("access denied");
            }

            SlotRange range = SlotRangeModel.GetSlotRange(id);
            if (range == null)
            {
                throw new Exception("invalid id");
            }

            string script = range.GenerateScript();

            // get email address of resource manager
            List<Person> persons = PersonModel.GetResourceManagers();
            string resMgrs = PersonModel.GetEmailCSV(persons);

            // send script to resourceManager(s) via E-Mail
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            //message.From = new System.Net.Mail.MailAddress("dontreply@gr-calcul.com");
            message.To.Add(resMgrs);
            message.IsBodyHtml = false;
            message.Subject = "Script Linux pour le SlotRange '" + range.id_slotRange + "'";
            message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            message.From = new System.Net.Mail.MailAddress("info@gr-calcul.com");
            message.Body = script;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Send(message);

            // for demo backup, also write to a file


            return View(range); // can be implemented for testing purposes
        }
    }
}
