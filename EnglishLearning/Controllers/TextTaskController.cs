using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class TextTaskController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();
        // GET: TextTask
        public ActionResult Index(string taskName = "")
        {
            var identity = User.Identity.GetUserId();
            var user = db.User.Where(x => x.IdentityId == identity).First();
            var task = (from text in db.TextTask
                            where text.AuthorId == 1
                            select text);
            if (!string.IsNullOrWhiteSpace(taskName))
                task = task.Where(x => x.Name == taskName);
            else
                task = task.Where(x => x.Difficult == user.LvlReading && !x.Name.Contains("Текст на рівень знань"));
            task = task.OrderBy(x => Guid.NewGuid());
            if (task.Count() == 0) return RedirectToAction("Index", "Home");
            var textTask = task.First();
            var temp = textTask.Words.Split(';').ToList();
            ViewBag.Answers = Shuffle.ShuffleList(temp);
            return View(textTask);
        }

        [HttpPost]
        public JsonResult CheckResult(int id, List<string> text) {
            bool result = true;
            var temp = db.TextTask.Where(x => x.TextId == id).First();
            var answer = temp.Words.Split(';');
            for (int i = 0; i < answer.Count(); i++) {
                if (answer[i] != text[i]){
                    result = false;
                    break;
                }
            }
            if (result) {
                SaveUserLvl(temp);
            }
            return Json(answer);
        }

        private void SaveUserLvl(TextTask text)
        {
            if (text.Name.StartsWith("Текст на рівень знань: "))
            {
                var identity = User.Identity.GetUserId();
                var user = db.User.Where(x => x.IdentityId == identity).First();
                string result = user.ObjLvlReading;
                Difficult userLvl = (Difficult)Enum.Parse(typeof(Difficult), user.ObjLvlReading.Replace('-', '_'));
                foreach (Difficult el in Enum.GetValues(typeof(Difficult)))
                {
                    string name = Enum.Format(typeof(Difficult), el, "G");
                    int value = (int)el;
                    if (text.Name == "Текст на рівень знань: " + name)
                    {
                        if (value > (int)userLvl)
                            result = name;
                    }
                }
                result = result.Replace('_', '-');
                user.ObjLvlReading = result;
                db.SaveChanges();
            }
        }


    }
}