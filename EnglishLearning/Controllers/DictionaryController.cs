using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers
{
    public class DictionaryController : Controller
    {
        // GET: Dictionary
        public ActionResult Index()
        {
            var db = new EnglishLearningEntities();
            var query = from g in db.Group
                        where g.OwnerId == 1
                        select g;
            return View(query);
        }
    }
}