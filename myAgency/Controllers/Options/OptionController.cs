using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using myAgency.Models;

namespace myAgency.Controllers.Options
{
    public class OptionController : Controller
    {
        public myAgencyEntities db = new myAgencyEntities();

        // GET: Option
        
        public ActionResult Index()
        {
            return RedirectToAction("Admin");
        }

        public ActionResult Admin()
        {
            try
            {
                List<Option> options = db.Option.ToList();
                return View(options);
            }
            catch
            {
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                Option option;
                if (id.HasValue) // cas d'une édition
                {
                    option = db.Option.Find(id);
                }
                else // cas d'un ajout
                {
                    option = new Option();
                }
                return View(option);
            }
            catch(Exception e)
            {
                TempData["error"] = "Une erreur est survenue";
                return RedirectToAction("Admin");
            }

        }

        [HttpPost]
        public ActionResult Edit(Option option)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["error"] = "Erreur dans la saisie de votre option";
                    return RedirectToAction("Admin");
                }

                if (option.id != 0) // cas d'une édition
                {
                    var entry = db.Entry(option);
                    entry.State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "Option numéro " + option.id + " modifiée avec succès";
                }
                else // cas d'un ajout
                {
                    db.Option.Add(option);
                    db.SaveChanges();
                    TempData["success"] = "Option numéro " + option.id + " enregistrée avec succès";
                }
                return RedirectToAction("Admin");
            }
            catch(Exception e)
            {
                TempData["error"] = "Erreur dans la saisie de votre bien";
                return RedirectToAction("Admin");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Option option = db.Option.Find(id);
                if (option.Property.Count > 0)
                {
                    var entry = db.Entry(option);
                    option.Property.Clear();
                    entry.State = EntityState.Modified;
                }
                db.Option.Remove(option);

                db.SaveChanges();
                TempData["success"] = "Option numéro " + option.id + " supprimé avec succès";

                return RedirectToAction("Admin");
            }
            catch (Exception e)
            {
                TempData["error"] = "Erreur dans la suppression de votre option";
                return RedirectToAction("Admin");
            }
        }
    }
}