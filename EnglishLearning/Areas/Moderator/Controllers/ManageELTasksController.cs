﻿using System;
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
    public class ManageELTasksController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageELTasks
        public ActionResult Index()
        {
            var eLTask = db.ELTask;/*.Include(e => e.User);*/
            return View(eLTask.ToList());
        }

        // GET: Moderator/ManageELTasks/Details/5
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
        public ActionResult Create()
        {
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTask(ELTask eLTask, string date, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                eLTask.DocumentPath = SaveFile(file);
                eLTask.AuthorId = GetUser(eLTask);
                db.ELTask.Add(eLTask);
                UserELTask userTask = new UserELTask();
                userTask.Date = Convert.ToDateTime(date);
                userTask.ELTask = eLTask;
                var userId = User.Identity.GetUserId();
                int result = db.User.Where(x => x.IdentityId == userId).Select(x => x.UserId).First();
                userTask.UserId = result;
                db.UserELTask.Add(userTask);
                db.SaveChanges();
                return RedirectToAction("Index","Profile", new { area = ""});
            }

            return View(eLTask);
        }

        // POST: Moderator/ManageELTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTask(ELTask eLTask, string date, bool done, HttpPostedFileBase result, HttpPostedFileBase file) {

            if (ModelState.IsValid)
            {
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
                db.Entry(eLTask).State = EntityState.Modified;
                var userId = User.Identity.GetUserId();
                int temp = db.User.Where(x => x.IdentityId == userId).Select(x => x.UserId).First();
                UserELTask userTask = (from ut in db.UserELTask
                                       where ut.TaskId == eLTask.TaskId && ut.UserId == temp
                                       select ut).First();
                userTask.Date = Convert.ToDateTime(date);
                userTask.Done = done;
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
                return RedirectToAction("Index", "Profile", new { area = "" });
            }
            return View(eLTask);
        }

        // POST: Moderator/ManageELTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TaskId,Name,Description,Text,Group,DocumentPath,Difficult")] ELTask eLTask, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
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
                db.Entry(eLTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", eLTask.AuthorId);
            return View(eLTask);
        }

        // GET: Moderator/ManageELTasks/Delete/5
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