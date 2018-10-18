using EnglishLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    public class LectionController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();
        // GET: Lection
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowByGroup() {
            List<LectionGroup> allGroups = new List<LectionGroup>();
            allGroups = db.LectionGroup.OrderBy(x => x.ParentId).ToList();
            var lections = from lection in db.Lection select lection;
            ViewBag.Lections = lections;
            return View(allGroups);
        }
    }
}