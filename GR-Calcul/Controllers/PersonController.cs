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
        /// <summary>
        /// GET: /Person/
        /// </summary>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Index()
        {
            return RedirectToAction("List");

            //return View(model.ListPerson());
            //return View(PersonModel.ConvertPersons(model.ListPerson()));
        }

        /// <summary>
        /// GET: /Person/List
        /// </summary>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult List()
        {
            //return View(model.ListPerson());
            return View(PersonModel.ConvertPersons(PersonModel.ListPerson()));
        }

        /// <summary>
        /// GET: /Person/Create
        /// </summary>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// POST: /Person/Create
        /// </summary>
        /// <param name="person">person object to create</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Create(Person2 person)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    PersonModel.CreatePerson(person);
                    return RedirectToAction("Index");
                }
                catch (GrException gex)
                {
                    ModelState.AddModelError("", gex.Message);
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
        /// GET: /Person/Edit/5
        /// </summary>
        /// <param name="id">id of the person to edit</param>
        /// <param name="pType">type of the person to edit</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        public ActionResult Edit(int id, PersonType pType)
        {
            Person person = PersonModel.getPerson(id, pType);
            return View(person.toPerson2());
        }

        /// <summary>
        /// POST: /Person/Edit/5
        /// </summary>
        /// <param name="id">id of the person to edit</param>
        /// <param name="person">type of the person to edit</param>
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
                try
                {
                    PersonModel.UpdatePerson(person);
                    return RedirectToAction("Index");
                }
                catch (GrException gex)
                {
                    ModelState.AddModelError("", gex.Message);
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
        /// GET: /Person/Delete/5
        /// </summary>
        /// <param name="id">id of the person to delete</param>
        /// <param name="pType">type of the person to delete</param>
        [DuffAuthorize(PersonType.ResourceManager)] 
        public ActionResult Delete(int id, PersonType pType)
        {
            return View(PersonModel.getPerson(id, pType).toPerson2());
        }

        /// <summary>
        /// POST: /Person/Delete/5
        /// </summary>
        /// <param name="id">id of the person to delete</param>
        /// <param name="person">type of the person to delete</param>
        [DuffAuthorize(PersonType.ResourceManager)]
        [HttpPost]
        public ActionResult Delete(int id, Person2 person)
        {
            try
            {
                PersonModel.DeletePerson(person);
                return RedirectToAction("Index");
            }
            catch (GrException gex)
            {
                ModelState.AddModelError("", gex.Message);

                // get updated data
                Person person_ = PersonModel.getPerson(id, person.pType);

                // update timestamp in case user really wants to delete this
                ModelState.SetModelValue("Timestamp", new ValueProviderResult(person_.Timestamp, "", CultureInfo.InvariantCulture));

                // show new values before user decided to really delete them
                return View(person_.toPerson2());
            }
        }
    }
}
