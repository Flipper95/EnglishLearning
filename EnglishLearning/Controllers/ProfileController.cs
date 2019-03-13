using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {

        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Profile
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = (from u in db.User
                        where u.IdentityId == userId
                        select u).First();
            ViewBag.UserData = user;

            var tasks = from ut in db.UserELTask.Include("ELTask")
                        where ut.UserId == user.UserId && ut.Done == false && ut.ELTask.AuthorId == 1
                        select ut;
            DateTime now = DateTime.Now;
            var toDelete = tasks.Where(x => x.Date < now).ToList();
            var count = toDelete.Count();
            db.UserELTask.RemoveRange(toDelete);
            if ((tasks.Count() - count) < 7) {
                var bannedTasks = tasks.Select(x => x.ELTask.TaskId);

                string[] excludedRepeatGroups = new string[] { "Lection", "Test", "TextTask" };
                var completedTasks = from ut in db.UserELTask.Include("ELTask")
                                     where ut.UserId == user.UserId && ut.Done == true 
                                     && ut.ELTask.AuthorId == 1 && excludedRepeatGroups.Contains(ut.ELTask.Group)
                                     select ut.ELTask.TaskId;
                bannedTasks = bannedTasks.Concat(completedTasks);
                var list = bannedTasks.ToList();
                UserELTask task;
                task = AddNewTaskByGroup("Lection", bannedTasks, user.UserId, user.Level);
                if (task != null) db.UserELTask.Add(task);
                task = AddNewTaskByGroup("Test", bannedTasks, user.UserId, user.Level);
                if (task != null) db.UserELTask.Add(task);
                task = AddNewTaskByGroup("TextTask", bannedTasks, user.UserId, user.LvlReading);
                if (task != null) db.UserELTask.Add(task);
                task = AddNewTaskByGroup("Grammar", bannedTasks, user.UserId, user.LvlWriting);
                if (task != null) db.UserELTask.Add(task);
                task = AddNewTaskByGroup("Other", bannedTasks, user.UserId, user.Level, true);
                if(task != null) db.UserELTask.Add(task);
                task = AddNewTaskByGroup("Word", bannedTasks, user.UserId, user.Level);
                if (task != null) db.UserELTask.Add(task);
            }
            db.SaveChanges();
            return View(user);
        }

        private UserELTask AddNewTaskByGroup(string groupName, IQueryable<int> bannedTasks, int userId, string userLvl, bool withLowerLvl = false) {

            var ban = from ut in db.UserELTask.Include("ELTask")
                      where 
                      ut.UserId == userId && 
                      ut.Done == true && 
                      ut.ELTask.AuthorId == 1 &&
                      ut.ELTask.Group == groupName
                      select ut.ELTask.TaskId;
            ban = bannedTasks.Concat(ban);

            var tasks = from t in db.ELTask
                       where !bannedTasks.Contains(t.TaskId) && 
                       t.AuthorId == 1 &&
                       t.Group == groupName
                       select t;
            var temp = tasks.Where(x => x.Difficult == userLvl);
            if (temp.Count() < 1 || withLowerLvl)
            {
                List<string> lvl = GetLessUserLvl(userLvl);
                if(withLowerLvl)
                    temp = temp.Concat(tasks.Where(x => lvl.Contains(x.Difficult)));
                else temp = tasks.Where(x => lvl.Contains(x.Difficult));
            }
            temp = temp.OrderBy(x => Guid.NewGuid()).Take(1);
            UserELTask task = new UserELTask
            {
                Date = DateTime.Now.AddDays(7),
                UserId = userId
            };
            try
            {
                task.TaskId = temp.Select(x => x.TaskId).First();//el;
            }
            catch {
                task = null;
            }
            return task;
        }

        private List<string> GetLessUserLvl(string user) {
            List<string> difficulties = new List<string>();
            Enum.TryParse(user.Replace('-', '_'), out Difficult userLvl);
            foreach (Difficult el in (Difficult[])Enum.GetValues(typeof(Difficult)))
            {
                if (userLvl > el)
                {
                    difficulties.Add(el.ToString());
                }
            }
            return difficulties;
        }

        [HttpPost]
        public ActionResult EditUserLvl([Bind(Include = "UserId, Level, LvlWriting, LvlListening, LvlReading")] User user) {
            var u = db.User.Find(user.UserId);
            u.Level = user.Level;
            u.LvlListening = user.LvlListening;
            u.LvlReading = user.LvlReading;
            u.LvlWriting = user.LvlWriting;
            db.Entry(u).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult TestUserGrammar(int id)
        {
            var userLvl = (from u in db.User
                           where u.UserId == id
                           select u.ObjectiveLevel).First().Replace('-', '_');
            Difficult current = (Difficult)Enum.Parse(typeof(Difficult), userLvl);
            int index = (int)current + 1;
            string nextLvl = "Рівень ";
            if (Enum.IsDefined(typeof(Difficult), index))
            {
                nextLvl += ((Difficult)index).ToString();
            }
            var testId = (from t in db.Test
                          where t.Name == nextLvl
                          select t.TestId).First();
            return RedirectToAction("Test", "Test", new { area = "", id = testId });
        }

        public ActionResult TestUserWriting(int id)
        {
            var userLvl = (from u in db.User
                           where u.UserId == id
                           select u.ObjLvlWriting).First().Replace('-', '_');
            Difficult current = (Difficult)Enum.Parse(typeof(Difficult), userLvl);
            int index = (int)current + 1;
            string nextLvl = "Рівень ";
            if (Enum.IsDefined(typeof(Difficult), index))
            {
                nextLvl += ((Difficult)index).ToString();
            }
            return RedirectToAction("Listening", "Grammar", new { area = "", number = 10, name = nextLvl });
        }

        public ActionResult TestUserListening(int id)
        {
            var userLvl = (from u in db.User
                           where u.UserId == id
                           select u.ObjLvlListening).First().Replace('-', '_');
            Difficult current = (Difficult)Enum.Parse(typeof(Difficult), userLvl);
            int index = (int)current + 1;
            string nextLvl = "Рівень сприйняття на слух: ";
            if (Enum.IsDefined(typeof(Difficult), index))
            {
                nextLvl += ((Difficult)index).ToString();
            }
            int testId = db.Test.Where(x => x.Name == nextLvl).Select(x => x.TestId).First();
            return RedirectToAction("Test", "Test", new { area = "", id = testId });
        }

        public ActionResult TestUserReading(int id)
        {
            var userLvl = (from u in db.User
                           where u.UserId == id
                           select u.ObjLvlReading).First().Replace('-', '_');
            Difficult current = (Difficult)Enum.Parse(typeof(Difficult), userLvl);
            int index = (int)current + 1;
            string nextLvl = "Текст на рівень знань: ";
            if (Enum.IsDefined(typeof(Difficult), index))
            {
                nextLvl += ((Difficult)index).ToString();
            }
            return RedirectToAction("Index", "TextTask", new { area = "", taskName = nextLvl });
        }

        public ActionResult ShowTasks(string type, bool done = false) {
            var temp = User.Identity.GetUserId();
            var userId = db.User.Where(x => x.IdentityId == temp).Select(x => x.UserId).First();
            var result = (from ut in db.UserELTask.Include("ELTask")
                          where ut.UserId == userId
                          select ut);
            result = result.Where(x => x.Done == done);
            if (type == "Standard")
            {
                result = result.Where(x => x.ELTask.AuthorId == 1);
            }
            else {
                result = result.Where(x => x.ELTask.AuthorId == userId);
            }
            ViewBag.TaskGroup = new List<string> { "Word", "Lection", "Test", "Grammar", "TextTask" };
            return PartialView(result.ToList());
        }

        public ActionResult ShowModal(int id)
        {
            var result = (from ut in db.UserELTask.Include("ELTask")
                          where ut.UserTaskId == id
                          select ut);
            ViewBag.TaskGroup = new List<string> { "Word", "Lection", "Test", "Grammar", "TextTask" };
            return PartialView(result.First());
        }

        public void DeleteTask(int id) {
            var temp = User.Identity.GetUserId();
            var userId = db.User.Where(x => x.IdentityId == temp).Select(x => x.UserId).First();
            var task = (from t in db.ELTask.Include("UserELTask")
                       where t.TaskId == id
                       select t).First();
            if (task.AuthorId == userId) {
                db.ELTask.Remove(task);
            }
            else {
                var userTask = task.UserELTask.Where(x => x.Done == false && x.UserId == userId).First();
                db.UserELTask.Remove(userTask);
            }
            db.SaveChanges();
        }

        public ActionResult RedirectToExecute(int id, int ELTaskId) {
            var elTask = db.ELTask.Find(ELTaskId);
            string name = elTask.Name;
            name = name.ToLower();
            switch (elTask.Group) {
                case("Word"): {
                        switch (name) {
                            case ("listening"): { TaskSaveDone(id); return RedirectToAction("ListeningExercise", "Listening", new { area = "" }); }
                            case ("translate"): { TaskSaveDone(id); return RedirectToAction("TranslateExercise", "Translate", new { area = "" }); }
                            case ("equivalent"): { TaskSaveDone(id); return RedirectToAction("EquivalentExercise", "Equivalent", new { area = "", repeat = false }); }
                            case ("constructor"): { TaskSaveDone(id); return RedirectToAction("ConstructorExercise", "Constructor", new { area = "" }); }
                            case ("synonyms"): { TaskSaveDone(id); return RedirectToAction("SynonymsExercise", "Synonyms", new { area = "" }); }
                            case ("repeat"): { TaskSaveDone(id); return RedirectToAction("EquivalentExercise", "Equivalent", new { area = "", repeat = true }); }
                        }
                        break;
                }
                case ("Lection"): {
                        if (elTask.LectionId != null)
                        {
                            var lection = db.Lection.Where(x => x.LectionId == elTask.LectionId);
                            if (lection.Count() > 0)
                            { 
                                TaskSaveDone(id);
                                return RedirectToAction("ShowLection", "Lection", new { area = "", id = elTask.LectionId });
                            }
                            return RedirectToAction("ShowByGroup", "Lection", new { area = "" });
                        }
                        break;
                }
                case ("Test"): {
                        if (elTask.TestId != null)
                        {
                            var test = db.Test.Where(x => x.TestId == elTask.TestId);
                            if (test.Count() > 0)
                            {
                                TaskSaveDone(id);
                                return RedirectToAction("Test", "Test", new { area = "", id = elTask.TestId });
                            }
                            return RedirectToAction("Index", "Test", new { area = "" });
                        }
                        break;
                }
                case ("Grammar"): {
                        if (elTask.GrammarId != null)
                        {
                            var grammar = db.GrammarGroup.Where(x => x.GroupId == elTask.GrammarId);
                            if (grammar.Count() > 0)
                            {
                                TaskSaveDone(id);
                                return RedirectToAction("Listening", "Grammar", new { area = "", name = grammar.First().Name });
                            }
                        }
                        break;
                }
                case ("TextTask"): {
                        if (elTask.TextId != null)
                        {
                            var text = db.TextTask.Where(x => x.TextId == elTask.TextId);
                            if (text.Count() > 0)
                            {
                                TaskSaveDone(id);
                                return RedirectToAction("Index", "TextTask", new { area = "", taskName = text.First().Name });
                            }
                            return RedirectToAction("Index", "TextTask", new { area = "" });
                        }
                        break;
                }
            }
            return RedirectToAction("Index");
        }

        private bool TaskSaveDone(int id) {
            var task = db.UserELTask.Where(x => x.UserTaskId == id).First();
            if (task != null)
            {
                task.Done = true;
                db.Entry(task).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        //[HttpPost]
        public FileResult Download(string path)
        {
            path = Server.MapPath(path);
            if (System.IO.File.Exists(path))
            {
                var fileType = path.Split('.').Last();
                var mime = "";
                switch (fileType) {
                    case ("docx"): { mime = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break; }
                    case ("doc"): { mime = "application/msword"; break; }
                    case ("pdf"): { mime = "application/pdf"; break; }
                    case ("png"): { mime = "image/png"; break; }
                    case ("jpg"): { mime = "image/jpeg"; break; }
                    default: { mime = "application/octet-stream"; break; }
                }
                return File(path, mime, "Завдання."+fileType);
            }
            else {
                return null;
            }
        }

    }
}