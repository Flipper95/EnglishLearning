using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Areas.Moderator.Models;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;

namespace EnglishLearning.Areas.Moderator.Controllers
{
    //[Authorize(Roles = "admin, moderator")]
    //can use [OverrideAuthorization] + [Authorize(Roles = "admin, moderator, user")] for except rules
    public class ManageELTasksController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageELTasks
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Index()
        {
            var eLTask = db.ELTask;/*.Include(e => e.User);*/
            return View(eLTask.ToList());
        }

        // GET: Moderator/ManageELTasks/Details/5
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELTask eLTask = db.ELTask.Find(id);
            if (eLTask == null)
            {
                return HttpNotFound();
            }
            return View(eLTask);
        }

        // GET: Moderator/ManageELTasks/Create
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Create()
        {
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId");
            return View();
        }

        [Authorize(Roles = "admin, moderator, user")]
        public ActionResult CreateTaskModal() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moderator, user")]
        public void CreateTask(ELTask eLTask, string date, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                eLTask.DocumentPath = SaveFile(file);
                eLTask.AuthorId = GetUser(eLTask);
                if (string.IsNullOrEmpty(eLTask.Name)) eLTask.Name = "";
                db.ELTask.Add(eLTask);
                UserELTask userTask = new UserELTask();
                DateTime tempDate;
                try
                {
                    tempDate = Convert.ToDateTime(date);
                }
                catch {
                    tempDate = DateTime.Now.AddDays(7);
                }
                userTask.Date = tempDate;
                userTask.ELTask = eLTask;
                var userIdentity = User.Identity.GetUserId();
                int userId = db.User.Where(x => x.IdentityId == userIdentity).Select(x => x.UserId).First();
                userTask.UserId = userId;
                db.UserELTask.Add(userTask);
                db.SaveChanges();
            }
        }

        // POST: Moderator/ManageELTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Create(ELTask eLTask, HttpPostedFileBase file) // authorid ,DocumentPath   [Bind(Include = "TaskId,Name,Description,Text,Group,Difficult")] ELTask eLTask
        {
            if (ModelState.IsValid)
            {
                eLTask.DocumentPath = SaveFile(file);
                eLTask.AuthorId = GetUser(eLTask);
                db.ELTask.Add(eLTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "Level", eLTask.AuthorId);
            return View(eLTask);
        }

        // GET: Moderator/ManageELTasks/Edit/5
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELTask eLTask = db.ELTask.Find(id);
            if (eLTask == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", eLTask.AuthorId);
            return View(eLTask);
        }

        [Authorize(Roles = "admin, moderator, user")]
        public ActionResult EditTaskModal(int? id) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELTask eLTask = db.ELTask.Find(id);
            if (eLTask == null)
            {
                return HttpNotFound();
            }
            return PartialView(eLTask);
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator, user")]
        public void EditTask(ELTask eLTask, HttpPostedFileBase file) {

            if (ModelState.IsValid)
            {
                eLTask = EditTaskData(eLTask, file);
                db.Entry(eLTask).State = EntityState.Modified;
                //var userId = User.Identity.GetUserId();
                //int temp = db.User.Where(x => x.IdentityId == userId).Select(x => x.UserId).First();
                //UserELTask userTask = (from ut in db.UserELTask
                //                       where ut.TaskId == eLTask.TaskId && ut.UserId == temp
                //                       select ut).First();
                //userTask.Date = Convert.ToDateTime(date);
                //userTask.Done = done;
                //var resPath = SaveFile(result);
                //if (!string.IsNullOrWhiteSpace(resPath))
                //{
                //    if (!string.IsNullOrWhiteSpace(userTask.ResultDocPath))
                //    {
                //        if (System.IO.File.Exists(Server.MapPath(userTask.ResultDocPath)))
                //            System.IO.File.Delete(Server.MapPath(userTask.ResultDocPath));
                //    }
                //    userTask.ResultDocPath = resPath;
                //}
                //db.Entry(userTask).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch { }
                //return RedirectToAction("Index", "Profile", new { area = "" });
            }
            //return View(eLTask);
        }

        // POST: Moderator/ManageELTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Edit([Bind(Include = "TaskId,Name,Description,Text,Group,DocumentPath,Difficult")] ELTask eLTask, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                eLTask = EditTaskData(eLTask, file);
                db.Entry(eLTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", eLTask.AuthorId);
            return View(eLTask);
        }

        [Authorize(Roles = "admin, moderator")]
        private ELTask EditTaskData(ELTask eLTask, HttpPostedFileBase file) {
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
            eLTask.AuthorId = GetUser(eLTask);
            return eLTask;
        }

        [Authorize(Roles = "admin, moderator, user")]
        public ActionResult EditUserTaskModal(int? id) {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserELTask eLUserTask = db.UserELTask.Find(id);
            if (eLUserTask == null)
            {
                return HttpNotFound();
            }
            return PartialView(eLUserTask);
        }

        [HttpPost]
        [Authorize(Roles = "admin, moderator, user")]
        public void EditUserTask(UserELTask eLUserTask, HttpPostedFileBase result) {
            var path = SaveFile(result);
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (!string.IsNullOrWhiteSpace(eLUserTask.ResultDocPath))
                {
                    if (System.IO.File.Exists(Server.MapPath(eLUserTask.ResultDocPath)))
                        System.IO.File.Delete(Server.MapPath(eLUserTask.ResultDocPath));
                }
                eLUserTask.ResultDocPath = path;
            }
            //save don`t work
            db.Entry(eLUserTask).State = EntityState.Modified;
            db.SaveChanges();
        }

        // GET: Moderator/ManageELTasks/Delete/5
        [Authorize(Roles = "admin, moderator")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ELTask eLTask = db.ELTask.Find(id);
            if (eLTask == null)
            {
                return HttpNotFound();
            }
            return View(eLTask);
        }

        // POST: Moderator/ManageELTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, moderator")]
        public ActionResult DeleteConfirmed(int id)
        {
            ELTask eLTask = db.ELTask.Find(id);
            if (!string.IsNullOrWhiteSpace(eLTask.DocumentPath))
            {
                if (System.IO.File.Exists(Server.MapPath(eLTask.DocumentPath)))
                    System.IO.File.Delete(Server.MapPath(eLTask.DocumentPath));
            }
            db.ELTask.Remove(eLTask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private int GetUser(ELTask task) {
            int result = 1;
            if (User.IsInRole("moderator"))
            {
                result = db.User.Where(x => x.UserId == 1).Select(x => x.UserId).First();
            }
            else
            {
                var userId = User.Identity.GetUserId();
                result = db.User.Where(x => x.IdentityId == userId).Select(x => x.UserId).First();
            }
            return result;
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

    }
}
