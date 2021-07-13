using myAgency.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace myAgency.Controllers
{
    public class HomeController : Controller
    {
        public myAgencyEntities db = new myAgencyEntities();

        public ActionResult Index()
        {
            List<Property> properties = db.Property.OrderByDescending(x => x.updated_at).Take(4).ToList();
            return View(properties);
        }

        public ActionResult Login()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            return RedirectToAction("Index");
        }

        /*
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }*/
    }
}