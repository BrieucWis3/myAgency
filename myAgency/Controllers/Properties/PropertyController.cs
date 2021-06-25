using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using myAgency.Models;
using System.IO;

namespace myAgency.Controllers.Properties
{
    public class PropertyController : Controller
    {
        public myAgencyEntities db = new myAgencyEntities();

        /* le paramètre 'model' est celui transmit depuis la vue 'Index' à partir de la variable tempData laquelle
          correspond au searchModel initial, c'est à dire les éléments du formulaire de filtrage déjà envoyés
          de sorte qu'ils soient affichés tel quels dans les champs préremplis */
        public ActionResult Search(PropertySearchViewModel model)
        {
            PropertySearchViewModel searchModel = new PropertySearchViewModel();

            // on test le cas où des critères de filtrage auraient déjà été renseignés
            if (model.maxPrice != 0 || model.minSurface != 0 || model.options !=null)
            {
                searchModel=model;
            }
            
            /* préparation de la liste des options */
            ViewBag.optionList = getOptions();

            return PartialView("_Search", searchModel);
        }

        // GET: Property
        [HttpGet]
        public ActionResult Index(PropertySearchViewModel searchModel)
        {
            List<Property> properties = db.Property.ToList();

            if (searchModel.minSurface != 0)
            {
                properties = properties.Where(x => x.surface >= searchModel.minSurface).ToList();
            }
            if (searchModel.maxPrice != 0)
            {
                properties = properties.Where(x => x.price <= searchModel.maxPrice).ToList();
            }
            if (searchModel.options!= null && searchModel.options.Count != 0)
            {
                List<Option> filteredOption = db.Option.Where(x => searchModel.options.Contains(x.id)).ToList();

                foreach (Option fo in filteredOption)
                {
                    properties = properties.Where(x => x.Option.Select(o=>o.id).ToList().Contains(fo.id)).ToList();
                }
            }

          /* on transmet le modèle associé au filtrage à la vue Index afin qu'il soit fourni à l'appel de la méthode Search()
             dans cette même vue afin que la méthode intègre les champs du formulaire déjà saisis et devant ainsi être préremplis*/
            TempData["model"] = searchModel;

            return View(properties);
        }

        [HttpGet]
        public ActionResult Show(int? id)
        {
            try
            {
                if(id.HasValue)
                {
                    Property property = db.Property.Find(id);
                    if (property != null)
                    {
                        Dictionary<int, string> heatTypes = new Dictionary<int, string>();
                        heatTypes.Add(0, "Electrique");
                        heatTypes.Add(1, "Gaz");
                        ViewBag.heatTypes = heatTypes;

                        return View(property);
                    }
                    return View("Error");
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return HttpNotFound();
            }
            
        }

        public ActionResult Admin()
        {
            try
            {
                List<Property> properties = db.Property.ToList();
                return View(properties);
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
                /* création de la liste des types de chauffages en traduisant la valeur du champs par son
                 équivalent textuel */
                List<SelectListItem> heatList = new List<SelectListItem>();
                heatList.Add(new SelectListItem { Text = "Electrique", Value = "0"});
                heatList.Add(new SelectListItem { Text = "Gaz", Value = "1" });

                ViewBag.heatTypes = heatList;

                /* préparation de la liste des options */
                ViewBag.optionList = getOptions();

                /* on teste s'il s'agit d'un ajout ou d'une modification d'un bien */ 
                if (id==null)
                {
                    Property property = new Property();
                    return View(property);
                }
                else
                {
                    Property property = db.Property.Find(id);
                    return View(property);
                }
                
            }
            catch
            {
                TempData["error"] = "Une erreur est survenue";
                return RedirectToAction("Admin");
            }
        }

        [HttpPost]
        public ActionResult Edit(Property property)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    TempData["error"] = "Erreur dans la saisie de votre bien";
                    return RedirectToAction("Admin");
                }

                /* gestion du fichier en PJ   */
                var file = Request.Files[0];

                if (file != null && file.ContentLength>0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Fichiers"), fileName);
                    file.SaveAs(path);
                    property.filename = fileName;
                }
                else
                {
                    property.filename = "";
                }
                /* --------------------------------*/

                if(property.id != 0) // en cas d'édition
                {
                    property.updated_at = DateTime.Now;
                    var entry = db.Entry(property); // on crée notre entry pour l'édition de l'entité property

                    /* mise à jour des options associées à la property en cours d'édition */
                    if (property.Option.Count > 0)
                    {
                        /* les options que l'on a cochées dans notre formulaire */
                        List<Option> selectedOptions = new List<Option>();

                        /* on récupère les informations complètes de ces options (id + name) à partir de l'attribut property.Option
                         * de notre propriété et qui est renseigné par le formulaire mais avec l'id seulement */
                        foreach (Option option in property.Option)
                        {
                            if(option.id != 0)
                            {
                                Option currentOption = db.Option.Find(option.id);
                                selectedOptions.Add(currentOption);
                            }
                        }

                        /* une fois les options voulues récupérées, on supprime les options n'ayant que l'id rattachées à
                         l'élément property courant */
                        property.Option.Clear();
                        entry.State = EntityState.Modified; // obligatoire pour ne pas faire un Load() dans le vide

                        // Load() permet de charger les éléments Option initialement associées en base avec notre property courante
                        entry.Collection(i => i.Option).Load();

                        // on récupère l'id de chacun de ces élements Option pour s'en servir dans le parcours qui va suivre 
                        List<int> insertedOptionsIds = new List<int>();
                        insertedOptionsIds = property.Option.Select(x => x.id).ToList();
                        /*foreach (var option in property.Option)
                        {
                            insertedOptionsIds.Add(option.id);
                        }*/

                        /* on parcours les options sélectionnées dans le formulaire, stockées dans notre liste selectedOption
                         * précédemment créée pour que dans le cas où certaines de ces options soient initialement absentes parmi 
                         * les options déjà associées à notre property, on les ajoute (peut être inutile) */
                        foreach (Option option in selectedOptions)
                        {
                            if(property.Option.Where(x=>x.id==option.id).Count()==0)
                            {
                                property.Option.Add(option);
                            }    
                        }
                        /* enfin, on parcours les ids des options déjà insérées afin que s'il y en a qui ne sont pas dans la liste
                         * de celles que l'on veut ajouter un les supprimer */
                        foreach(int id in insertedOptionsIds)
                        {
                            if (selectedOptions.Where(x => x.id == id).Count() == 0)
                            {
                                Option optionToRemove = property.Option.Where(x => x.id == id).FirstOrDefault();
                                property.Option.Remove(optionToRemove);
                            }
                        }
                        // on enregistre une seconde fois les modifications pour mettre à jour les options
                        entry.State = EntityState.Modified;
                    }
                    else
                    {
                        entry.State = EntityState.Modified;
                        entry.Collection(i => i.Option).Load();
                        if (property.Option.Count() > 0)
                        {
                            List<Option> insertedOptions = new List<Option>();
                            insertedOptions = property.Option.ToList();

                            foreach (Option option in insertedOptions)
                            {
                                property.Option.Remove(option);
                            }
                            entry.State = EntityState.Modified;
                        }
                        
                    }

                    db.SaveChanges();
                    TempData["success"] = "Bien numéro " + property.id + " modifié avec succès";
                }
                else // cas d'un ajout
                {
                    property.created_at = DateTime.Now;
                    property.updated_at= DateTime.Now;
                    if (property.Option.Count > 0)
                    {
                        List<Option> selectedOptions = new List<Option>();
                        foreach (Option option in property.Option)
                        {
                            Option currentOption = db.Option.Find(option.id);
                            selectedOptions.Add(currentOption);
                        }

                        property.Option.Clear();

                        foreach(var option in selectedOptions)
                        {
                            property.Option.Add(option);
                        }
                    }
                    db.Property.Add(property);
                    db.SaveChanges();
                    TempData["success"] = "Bien numéro " + property.id + " ajouté avec succès";
                }
                return RedirectToAction("Admin");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                TempData["error"] = "Erreur dans la saisie de votre bien";
                return RedirectToAction("Admin");
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                Property property = db.Property.Find(id);
                if(property.Option.Count > 0)
                {
                    var entry = db.Entry(property);
                    property.Option.Clear();
                    entry.State = EntityState.Modified;
                }
                db.Property.Remove(property);

                db.SaveChanges();
                TempData["success"] = "Bien numéro " + property.id + " supprimé avec succès";

                return RedirectToAction("Admin");
            }
            catch(Exception e)
            {
                TempData["error"] = "Erreur dans la suppression de votre bien";
                return RedirectToAction("Admin");
            }
        }

        public List<SelectListItem> getOptions()
        {
            List<Option> options = db.Option.ToList();
            List<SelectListItem> optionList = new List<SelectListItem>();
            foreach (var item in options)
            {
                optionList.Add(new SelectListItem { Text = item.name, Value = item.id.ToString() });
            }

            return optionList;
        }

    }
}