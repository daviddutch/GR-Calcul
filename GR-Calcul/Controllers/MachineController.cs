﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;

namespace GR_Calcul.Controllers
{
    public class MachineController : Controller
    {
        private MachineModel model = new MachineModel();
        private RoomModel roomModel = new RoomModel();

        //
        // GET: /Machine/

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // GET: /Machine/List

        public ActionResult List()
        {
            return View(model.ListMachines());
        }

        //
        // GET: /Machine/Details/5

        //public ActionResult Details(int id)
        //{
        //    return View(model.getMachine(id));
        //}

        //
        // GET: /Machine/Create

        public ActionResult Create()
        {
            var items = roomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text");
            return View();
        } 

        //
        // POST: /Machine/Create

        [HttpPost]
        public ActionResult Create(Machine machine)
        {
            try
            {
                // TODO: Add insert logic here
                model.CreateMachine(machine);
                return RedirectToAction("Index");
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                return View();
            }
        }
        
        //
        // GET: /Machine/Edit/5
 
        public ActionResult Edit(int id)
        {
            Machine machine = model.getMachine(id);
            var items = roomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text", machine.id_room);
            return View(machine);
        }

        //
        // POST: /Machine/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Machine machine)
        {
            try
            {
                model.UpdateMachine(machine);
                return RedirectToAction("Index");
            }
            catch (SqlException sqlError)
            {
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                return View();
            }
        }

        ////
        //// GET: /Machine/Delete/5

        public ActionResult Delete(int id)
        {
            return View(model.getMachine(id));
        }

        //
        // POST: /Machine/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                model.DeleteMachine(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
