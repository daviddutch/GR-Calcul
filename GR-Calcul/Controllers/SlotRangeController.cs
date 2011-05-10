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
using TaskScheduler;


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
            Course course = CourseModel.GetCourse(id);
            viewModel.Course = course;
            viewModel.SlotRanges = course.GetSlotRangesForCourse();
            viewModel.Course.Students = courseModel.getCourseStudents(id);
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

                    // schedule linux script to be sent to resource manager by email 
                    ScheduleEmail(range);

                    return View("Complete", range);
                }
                catch (Exception error)
                {
                    ModelState.AddModelError("", "Une erreur est survenue.");
                    ViewBag.Error = error.Message;

                    System.Diagnostics.Debug.WriteLine(error.Message);
                    System.Diagnostics.Debug.WriteLine(error.StackTrace);
                    
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

                // schedule linux script to be sent to resource manager by email 
                ScheduleEmail(range);

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

            // Principals // doesn't work yet !
            //task.Principal.RunLevel = _TASK_RUNLEVEL.TASK_RUNLEVEL_HIGHEST;
            //task.Principal.UserId = "NT AUTHORITY\\LOCAL SERVICE";
            //task.Principal.LogonType = _TASK_LOGON_TYPE.TASK_LOGON_SERVICE_ACCOUNT;

            ITimeTrigger trigger = (ITimeTrigger)task.Triggers.Create(_TASK_TRIGGER_TYPE2.TASK_TRIGGER_TIME);
            trigger.Id = "EmailTriggerForSlotRange_" + range.id_slotRange;
            DateTime dt = range.EndRes.Add(new System.TimeSpan(0, 1, 0, 0)); // EndRes + 1h
            trigger.StartBoundary = dt.ToString("yyyy-MM-ddTHH:MM:ss");
            trigger.EndBoundary = dt.Add(new System.TimeSpan(0, 1, 0, 0)).ToString("yyyy-MM-ddTHH:MM:ss"); // remove the task from active tasks 1h later
            trigger.ExecutionTimeLimit = "PT2M"; // 2 minutes

            IExecAction action = (IExecAction)task.Actions.Create(_TASK_ACTION_TYPE.TASK_ACTION_EXEC);
            action.Id = "EmailActionForSlotRange_" + range.id_slotRange;
            action.Path = "C:\\script.vbs";
            action.Arguments = range.id_slotRange.ToString();

            // TODO: we may need to specify that this task shall be executed even if no user is logged in
            // "Local System" with VARIANT VT_EMPTY ??

            ITaskFolder root = scheduler.GetFolder("\\");
            IRegisteredTask regTask = root.RegisterTaskDefinition(
                "EmailTaskForSlotRange_" + range.id_slotRange,
                task,
                (int)_TASK_CREATION.TASK_CREATE_OR_UPDATE,
                null, // username - we're using the logged in user of this web app
                null, // password - we're using the logged in user of this web app
                _TASK_LOGON_TYPE.TASK_LOGON_INTERACTIVE_TOKEN); // for simplicity, we're using the logged in user of this web app

            regTask.Run(null);
        }

        // this method filters to allow only localhost requests
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

            // send script to resourceManager(s) via E-Mail
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.From = new System.Net.Mail.MailAddress("dontreply@gr-calcul.com");

            // TODO get email address of resource manager
            message.To.Add(new System.Net.Mail.MailAddress("chris.fribourg@gmail.com"));
            message.IsBodyHtml = false;
            message.Subject = "Script Linux pour le SlotRange '" + range.id_slotRange;
            message.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            message.Body = script;

            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Send(message);

            return View(range); // can be implemented for testing purposes
        }
    }
}
