using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;
using GR_Calcul.Misc;
using System.Globalization;


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
            return View(MachineModel.ListMachines());
        }


        //
        // GET: /Machine/Create
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Create()
        {
            var items = RoomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text");
            return View();
        } 

        //
        // POST: /Machine/Create
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Create(Machine machine)
        {
            if (ModelState.IsValid)
            {
                string errMsg = MachineModel.CreateMachine(machine);
                
                if (errMsg == "")
                {
                    return RedirectToAction("Index");
                }

                else
                {
                    ModelState.AddModelError("", errMsg);
                    return View();
                }
            }
            else
            {
                // addinge extra error message here in case JS is deactivated on client.
                ModelState.AddModelError("", "vous avez envoyé des données invalides");
                return View();
            }
        }
        
        //
        // GET: /Machine/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)] 
        public ActionResult Edit(int id)
        {
            Machine machine = MachineModel.getMachine(id);
            var items = RoomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text", machine.id_room);
            return View(machine);
        }

        //
        // POST: /Machine/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Edit(int id, Machine machine)
        {
            if (ModelState.IsValid)
            {
                string errMsg = MachineModel.UpdateMachine(machine);

                if (errMsg == "")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", errMsg);
                    return View();
                }
            }
            else
            {
                // addinge extra error message here in case JS is deactivated on client.
                ModelState.AddModelError("", "vous avez envoyé des données invalides");
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
            String errMsg = MachineModel.DeleteMachine(machine);

            if(errMsg == ""){
                return RedirectToAction("Index");
            }
            else{
                ModelState.AddModelError("", errMsg);

                // get updated data
                Machine machine_ = MachineModel.getMachine(id);

                // update timestamp in case user really wants to delete this
                ModelState.SetModelValue("Timestamp", new ValueProviderResult(machine_.Timestamp, "", CultureInfo.InvariantCulture));

                // show new values before user decided to really delete them
                return View(machine_);
            }
        }
    }
}
