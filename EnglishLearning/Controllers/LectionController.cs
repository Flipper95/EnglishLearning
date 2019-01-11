﻿using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static EnglishLearning.Controllers.LectionChoice;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class LectionController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();
        // GET: Lection
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowByGroup() {
            List<GroupModel> allGroups = new List<GroupModel>();
            allGroups = db.LectionGroup.OrderBy(x => x.ParentId).Select(x => new GroupModel { Id=x.LectionGroupId, ParentId = x.ParentId, Name = x.Name}).ToList();
            var lections = from lection in db.Lection
                           where lection.OwnerId == 1
                           select lection;
            ViewBag.Lections = lections;

            var identity = User.Identity.GetUserId();
            var user = db.User.Where(x => x.IdentityId == identity).First();
            ViewBag.User = user;
            return View(allGroups);
        }

        public PartialViewResult ShowByComplexity(string complexity) {
            var lections = from lection in db.Lection
                           where lection.OwnerId == 1 && lection.Complexity == complexity
                           group lection by lection.ComplexityOrder into g
                           select new LectionChoice
                           {
                               ComplexityOrder = g.Key,
                               lection = (from p in g
                                          select new SimpleLection { Name = p.Name, Description = p.Description, LectionId = p.LectionId }).ToList()
                           };
            return PartialView("LectionsByComplexity", lections.ToList());
        }

        public ActionResult ShowLection(int id) {
            var lection = (from lect in db.Lection
                          where lect.LectionId == id
                          select lect).FirstOrDefault();
            var allLections = from lect in db.Lection
                           where lect.LectionType == lection.LectionType
                           select lect;
            List<Lection> lections = allLections.ToList();
            ViewBag.Index = lections.IndexOf(lection);
            return View(lections);
        }

        public FileStreamResult GetPDF(string filePath) {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/pdf");
        }

        public FileResult DownloadFile(string filePath, string fileName)
        {
            //string filename = filePath.Split('\\').LastOrDefault();
            fileName += ".pdf";
            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/force-download", fileName);
        }

    }

    public class LectionChoice {
        public int ComplexityOrder;
        public List<SimpleLection> lection; //Tuple<string,string>

        public class SimpleLection {
            public string Difficult;
            public string Name;
            public string Description;
            public int LectionId;
        }
    }

}