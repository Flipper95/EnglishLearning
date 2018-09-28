using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace EnglishLearning.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //var db = new EnglishModel();
            var db = new EnglishLearningEntities();
            var query = from g in db.Group
                            where g.OwnerId == 1
                            select g;
            //var query = from g in db.Group
            //            where g.OwnerId == 1
            //            select g;
            return View(query);//query
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            IList<string> roles = new List<string>();
            ApplicationUserManager userManager = HttpContext.GetOwinContext()
                                            .GetUserManager<ApplicationUserManager>();
            ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            if (user != null)
                roles = userManager.GetRoles(user.Id);
            return View(roles);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}