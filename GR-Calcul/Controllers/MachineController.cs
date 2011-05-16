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

        /// <summary>
        /// GET: /Machine/
        /// </summary>
        [DuffAuthorize(PersonType.ResourceManager, PersonType.Responsible)]
        public ActionResult Index()
        {
            return View(MachineModel.ListMachines());
        }


        /// <summary>
        /// GET: /Machine/Create
        /// </summary>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Create()
        {
            var items = RoomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text");
            return View();
        } 

        /// <summary>
        /// POST: /Machine/Create
        /// </summary>
        /// <param name="machine">the machine to create</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Create(Machine machine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MachineModel.CreateMachine(machine);
                    return RedirectToAction("Index");
                }
                catch (GrException gex)
                {
                    ModelState.AddModelError("", gex.UserMessage);
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", Messages.invalidData);
                return View();
            }
        }
        
        /// <summary>
        /// GET: /Machine/Edit/5
        /// </summary>
        /// <param name="id">the id of the machine to edit</param>
        [DuffAuthorize(PersonType.ResourceManager)] 
        public ActionResult Edit(int id)
        {
            Machine machine = MachineModel.getMachine(id);
            var items = RoomModel.ListRooms().Select(x => new SelectListItem() { Value = x.ID.ToString(), Text = x.Name.ToString() }).ToList();
            ViewData["Rooms"] = new SelectList(items, "Value", "Text", machine.id_room);
            return View(machine);
        }

        /// <summary>
        /// POST: /Machine/Edit/5
        /// </summary>
        /// <param name="id">id of the machine to edit</param>
        /// <param name="machine">the machine object containing the new data</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Edit(int id, Machine machine)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MachineModel.UpdateMachine(machine);
                    return RedirectToAction("Index");
                }
                catch (GrException gex)
                {
                    ModelState.AddModelError("", gex.UserMessage);
                    return View();
                }
            }
            else
            {
                // addinge extra error message here in case JS is deactivated on client.
                ModelState.AddModelError("", Messages.invalidData);
                return View();
            }
        }

        /// <summary>
        /// GET: /Machine/Delete/5
        /// </summary>
        /// <param name="id">the id of the machine to delete</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Delete(int id)
        {
            return View(MachineModel.getMachine(id));
        }

        /// <summary>
        /// POST: /Machine/Delete/5
        /// </summary>
        /// <param name="id">id of the machine to delete</param>
        /// <param name="machine">machine object to delete</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Delete(int id, Machine machine)
        {
            try
            {
                MachineModel.DeleteMachine(machine);
                return RedirectToAction("Index");
            }
            catch (GrException gex)
            {
                ModelState.AddModelError("", gex.UserMessage);

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
