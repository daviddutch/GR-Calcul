﻿using System;
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
    /// Class containing all actions related to the Person module
    /// </summary>
    public class PersonController : BaseController
    {
        private PersonModel model = new PersonModel();
        //
        // GET: /Person/
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Index()
        {
            return RedirectToAction("List");

            //return View(model.ListPerson());
            //return View(PersonModel.ConvertPersons(model.ListPerson()));
        }

        //
        // GET: /Person/
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult List()
        {
            //return View(model.ListPerson());
            return View(PersonModel.ConvertPersons(model.ListPerson()));
        }

        //
        // GET: /Person/Create
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Person/Create
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Create(Person2 person)
        {
            if (ModelState.IsValid)
            {
                string errMsg = model.CreatePerson(person);

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
        // GET: /Person/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Edit(int id, PersonType pType)
        {
            Person person = model.getPerson(id, pType);
            return View(person.toPerson2());
        }

        //
        // POST: /Person/Edit/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Edit(int id, Person2Edit person)
        {
            if (ModelState.IsValid)
            {
                string errMsg = model.UpdatePerson(person);

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
        // GET: /Person/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)] 
        public ActionResult Delete(int id, PersonType pType)
        {
            return View(model.getPerson(id, pType));
        }

        //
        // POST: /Person/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Delete(int id, Person2 person)
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
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
 