using EnglishLearning.Models;
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

            var identity = User.Identity.GetUserId();
            var user = db.User.Where(x => x.IdentityId == identity).First();
            ViewBag.User = user;

            SubTreeComponent root = new SubTreeComponent(0, "Перегляд за групою");
            root = CreateTreeRecursively(root) as SubTreeComponent;

            var url = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext);
            return View("ShowByGroup", (object)root.Print(user, url));
        }

        private TreeComponent CreateTreeRecursively(SubTreeComponent root) {
            var subTreeList = db.LectionGroup.Where(x => x.ParentId == root.Id)
                .OrderBy(x => x.LectionGroupId).Select(x => new SubTreeComponent { Id = x.LectionGroupId, Name = x.Name }).ToList();
            List<TreeComponent> component = subTreeList.ConvertAll<TreeComponent>(x => x);
            root.AddRange(component);
            for (int i = 0; i < subTreeList.Count; i++) {
                var subTree = subTreeList[i];// root.GetChild(i) as SubTreeComponent;
                CreateTreeRecursively(subTree);
            }
            List<TreeItem> leafList = db.Lection
                .Where(x => x.OwnerId == 1 && x.LectionType == root.Id)
                .Select(x => new TreeItem { Id = x.LectionId, Name = x.Name, Difficult = x.Complexity, GroupName = root.Name, Controller = "ShowLection", Action = "Lection" })
                .ToList();
            List<TreeComponent> leaf = leafList.ConvertAll<TreeComponent>(x => x);
            root.AddRange(leaf);
            return root;
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