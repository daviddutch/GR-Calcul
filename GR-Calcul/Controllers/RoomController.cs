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
    /// Class containing all actions related to the room module
    /// </summary>
    public class RoomController : BaseController
    {

        private RoomModel roomModel = new RoomModel();

        //
        // GET: /Room/
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Index()
        {
            return View(RoomModel.ListRooms());
        }

        //
        // GET: /Room/Create
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Room/Create
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Create(Room room)
        {
            try
            {
                RoomModel.CreateRoom(room);
                return RedirectToAction("Index");
            }
            catch (Exception sqlError)
            {
                ViewBag.ErrorMessage = sqlError.Message;
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                return View("Error");
            }
        }
        
        //
        // GET: /Room/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Edit(int id)
        {
            return View(RoomModel.GetRoom(id));
        }

        //
        // POST: /Room/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Edit(int id, Room room)
        {
            try
            {
                RoomModel.UpdateRoom(room);
                return RedirectToAction("Index");
            }
            catch (Exception sqlError)
            {
                ViewBag.ErrorMessage = sqlError.Message;
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                return View("Error");
            }
        }

        //
        // GET: /Room/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Delete(int id)
        {
            return View(RoomModel.GetRoom(id));
        }

        //
        // POST: /Room/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                RoomModel.DeleteRoom(id);
                return RedirectToAction("Index");
            }
            catch(Exception sqlError)
            {
                ViewBag.ErrorMessage = sqlError.Message;
                System.Diagnostics.Debug.WriteLine(sqlError.Message);
                System.Diagnostics.Debug.WriteLine(sqlError.StackTrace);
                return View("Error");
            }
        }
    }
}
