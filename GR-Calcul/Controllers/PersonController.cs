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
        public ActionResult Edit(int id, Person2 person)
        {
            // all but password !
            if (ModelState.IsValidField("ID") && 
                ModelState.IsValidField("pType") &&
                ModelState.IsValidField("Timestamp") &&
                ModelState.IsValidField("FirstName") &&
                ModelState.IsValidField("LastName") &&
                ModelState.IsValidField("Email") &&
                ModelState.IsValidField("Username")
            )
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
            return View(model.getPerson(id, pType).toPerson2());
        }

        //
        // POST: /Person/Delete/5
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Delete(int id, Person2 person)
        {
            String errMsg = model.DeletePerson(person);

            if (errMsg == "")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", errMsg);

                // get updated data
                Person person_ = (new PersonModel()).getPerson(id, person.pType);

                // update timestamp in case user really wants to delete this
                ModelState.SetModelValue("Timestamp", new ValueProviderResult(person_.Timestamp, "", CultureInfo.InvariantCulture));

                // show new values before user decided to really delete them
                return View(person_.toPerson2()); 
            }
        }
    }
}
