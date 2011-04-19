using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;

namespace GR_Calcul.Controllers
{
    public class MachineController : Controller
    {
        //
        // GET: /Machine/

        public ActionResult Index()
        {
            List<MachineModel> articles = new List<MachineModel>();

            MachineModel article1 = new MachineModel();
            article1.Name = "First Machine";
            article1.IP = "192.168.1.1";

            articles.Add(article1);

            MachineModel article2 = new MachineModel();
            article2.Name = "Second Machine";
            article2.IP = "192.168.1.2";

            articles.Add(article2);

            return View(articles);
        }

        //
        // GET: /Machine/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Machine/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Machine/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /Machine/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Machine/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        ////
        //// GET: /Machine/Delete/5
 
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //
        // POST: /Machine/Delete/5

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
