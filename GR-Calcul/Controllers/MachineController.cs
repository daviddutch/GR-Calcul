using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using GR_Calcul.Misc;

namespace GR_Calcul.Controllers
{
    public class MachineController : BaseController
    {
        private MachineModel model = new MachineModel();
        private RoomModel roomModel = new RoomModel();

        //
        // GET: /Machine/
        [DuffAuthorize(PersonType.ResourceManager, PersonType.Responsible)]
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //
        // GET: /Machine/List
        [DuffAuthorize(PersonType.ResourceManager, PersonType.Responsible)]
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
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Create()
        {
            var items = roomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text");
            return View();
        } 

        //
        // POST: /Machine/Create
        [DuffAuthorize(PersonType.ResourceManager)]
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
        [DuffAuthorize(PersonType.ResourceManager)] 
        public ActionResult Edit(int id)
        {
            Machine machine = model.getMachine(id);
            var items = roomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text", machine.id_room);
            return View(machine);
        }

        //
        // POST: /Machine/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)]
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
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Delete(int id)
        {
            return View(model.getMachine(id));
        }

        //
        // POST: /Machine/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)]
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
