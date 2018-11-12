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
                var temp = from t in db.ELTask
                           where !bannedTasks.Contains(t.TaskId) && t.AuthorId == 1
                           select t.TaskId;
                foreach (var el in temp) {
                    UserELTask task = new UserELTask();
                    task.Date = now.AddDays(7);
                    task.TaskId = el;
                    task.UserId = user.UserId;
                    db.UserELTask.Add(task);
                }
            }
            db.SaveChanges();
            return View();
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

        public ActionResult TestUserListening(int id)
        {
            var userLvl = (from u in db.User
                           where u.UserId == id
                           select u.ObjLvlListening).First().Replace('-', '_');
            Difficult current = (Difficult)Enum.Parse(typeof(Difficult), userLvl);
            int index = (int)current + 1;
            string nextLvl = "Рівень ";
            if (Enum.IsDefined(typeof(Difficult), index))
            {
                nextLvl += ((Difficult)index).ToString();
            }
            return RedirectToAction("Listening", "Grammar", new { area = "", number = 10, name = nextLvl });
        }

        public ActionResult ShowTasks(string type) {
            //IQueryable<Task> result;
            var temp = User.Identity.GetUserId();
            var userId = db.User.Where(x => x.IdentityId == temp).Select(x => x.UserId).First();
            var result = (from ut in db.UserELTask.Include("ELTask")
                          where ut.UserId == userId && ut.Done == false
                          select ut);
            if (type == "Standard")
            {
                result = result.Where(x => x.ELTask.AuthorId == 1);
            }
            else {
                result = result.Where(x => x.ELTask.AuthorId == userId);
            }
            return PartialView(result.ToList());
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
            //var userTask = from ut in db.UserELTask
            //               where ut.TaskId == id && ut.UserId == userId
            //               select ut
        }

        public ActionResult Edit(int id) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELTask eLTask = db.ELTask.Find(id);
            if (eLTask == null)
            {
                return HttpNotFound();
            }
            return PartialView("EditTask", eLTask);
        }

        [HttpPost]
        public void EditTask(ELTask eLTask, string date, bool? done, HttpPostedFileBase result, HttpPostedFileBase file)
        {

            if (ModelState.IsValid)
            {
                var temp = User.Identity.GetUserId();
                var userId = db.User.Where(x => x.IdentityId == temp).Select(x => x.UserId).First();
                var path = SaveFile(file);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (!string.IsNullOrWhiteSpace(eLTask.DocumentPath))
                    {
                        if (System.IO.File.Exists(Server.MapPath(eLTask.DocumentPath)))
                            System.IO.File.Delete(Server.MapPath(eLTask.DocumentPath));
                    }
                    eLTask.DocumentPath = path;
                }
                eLTask.AuthorId = userId;
                db.Entry(eLTask).State = EntityState.Modified;
                UserELTask userTask = (from ut in db.UserELTask
                                       where ut.TaskId == eLTask.TaskId && ut.UserId == userId
                                       select ut).First();
                DateTime dateNotify;
                try
                {
                    dateNotify = Convert.ToDateTime(date);
                }
                catch {
                    dateNotify = DateTime.Now.AddDays(1);
                }
                userTask.Date = dateNotify;
                userTask.Done = Convert.ToBoolean(done);
                var resPath = SaveFile(result);
                if (!string.IsNullOrWhiteSpace(resPath))
                {
                    if (!string.IsNullOrWhiteSpace(userTask.ResultDocPath))
                    {
                        if (System.IO.File.Exists(Server.MapPath(userTask.ResultDocPath)))
                            System.IO.File.Delete(Server.MapPath(userTask.ResultDocPath));
                    }
                    userTask.ResultDocPath = resPath;
                }
                db.Entry(userTask).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index", "Profile", new { area = "" });
            }
            //RedirectToAction("Edit", new { id = eLTask.TaskId });
            //return View(eLTask);
        }

        private string SaveFile(HttpPostedFileBase file)
        {
            var path = "";
            if (file != null && file.ContentLength > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                if (extension == ".pdf" || extension == ".doc" || extension == ".docx")
                {
                    var fileName = Path.GetFileName(file.FileName);
                    fileName = Guid.NewGuid().ToString() + extension;
                    path = "~/App_Data/TaskDocuments/";
                    var tempPath = Path.Combine(Server.MapPath(path), fileName);
                    path = path + fileName;
                    file.SaveAs(tempPath);
                }
            }
            return path;
        }

        //[HttpPost]
        public FileResult Download(string path)
        {
            path = Server.MapPath(path);
            if (System.IO.File.Exists(path))
            {
                //byte[] fileBytes = System.IO.File.ReadAllBytes(path);
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
                //var response = new FileContentResult(fileBytes, mime);
                //string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(path, mime, "Завдання."+fileType);
            }
            else {
                return null;
            }
        }

    }
}