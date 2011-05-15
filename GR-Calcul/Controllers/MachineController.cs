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
    /// <summary>
    /// Class containing all actions related to the machine module
    /// </summary>
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
            return View(MachineModel.ListMachines());
        }


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
                MachineModel.CreateMachine(machine);
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
            Machine machine = MachineModel.getMachine(id);
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
                MachineModel.UpdateMachine(machine);
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
            return View(MachineModel.getMachine(id));
        }

        //
        // POST: /Machine/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Delete(int id, Machine machine)
        {
            try
            {
                MachineModel.DeleteMachine(machine);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                if (e.Message.Equals("timestamp"))
                    ModelState.AddModelError("", "L'élément à été modifier. Veuillez revérifier les infos avant de confirmer la suppresion.");
                else
                    ModelState.AddModelError("", e.Message);
                return View(MachineModel.getMachine(id));
            }
        }
    }
}
