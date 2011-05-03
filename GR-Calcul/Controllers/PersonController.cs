using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GR_Calcul.Models;
using System.Data.SqlClient;

namespace GR_Calcul.Controllers
{
    public class PersonController : Controller
    {
        private PersonModel model = new PersonModel();
        //
        // GET: /Person/

        public ActionResult Index()
        {
            return View(model.ListPerson());
        }

        //
        // GET: /Person/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Person/Create

        [HttpPost]
        public ActionResult Create(Person person)
        {
            try
            {
                model.CreatePerson(person);
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
        // GET: /Person/Edit/5
 
        public ActionResult Edit(int id, PersonType pType)
        {
            Person person = model.getPerson(id, pType);
            return View(person);
        }

        //
        // POST: /Person/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, Person person)
        {
            try
            {
                model.UpdatePerson(person);
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
        // GET: /Person/Delete/5
 
        public ActionResult Delete(int id, PersonType pType)
        {
            return View(model.getPerson(id, pType));
        }

        //
        // POST: /Person/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, Person person)
        {
            try
            {
                model.DeletePerson(person); 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
