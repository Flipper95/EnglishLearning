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

namespace EnglishLearning.Areas.Moderator.Controllers
{
    [Authorize(Roles = "moderator")]
    public class ManageLectionsController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();
        string[] extensions = new string[] { ".pdf" };
        private string pathToSave = "~/App_Data/Lections/";

        // GET: Moderator/ManageLections
        public ActionResult Index()
        {
            var lection = db.Lection.Include(l => l.User).Include(l => l.LectionGroup);
            return View(lection.ToList());
        }

        // GET: Moderator/ManageLections/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lection lection = db.Lection.Find(id);
            if (lection == null)
            {
                return HttpNotFound();
            }
            return View(lection);
        }

        // GET: Moderator/ManageLections/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId");
            ViewBag.LectionType = new SelectList(db.LectionGroup, "LectionGroupId", "Name");//.Where(x => x.ParentId != 0).ToList()
            return View();
        }

        // POST: Moderator/ManageLections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LectionId,Name,OwnerId,LectionType,Description,Editable,ExportOwner,Complexity,ComplexityOrder,LectionPath")] Lection lection, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                lection.LectionPath = FileOperations.SaveFile(file, Server, pathToSave, extensions);
                lection.LectionText = new byte[] { };
                ELTask task = new ELTask() { AuthorId = 1, Difficult = lection.Complexity, Group = "Lection", Lection = lection, Name = lection.Name, Description = "Прочитайте зазначену лекцію" };
                db.ELTask.Add(task);
                db.Lection.Add(lection);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", lection.OwnerId);
            ViewBag.LectionType = new SelectList(db.LectionGroup, "LectionGroupId", "Name", lection.LectionType);
            return View(lection);
        }

        // GET: Moderator/ManageLections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lection lection = db.Lection.Find(id);
            if (lection == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", lection.OwnerId);
            ViewBag.LectionType = new SelectList(db.LectionGroup, "LectionGroupId", "Name", lection.LectionType);
            return View(lection);
        }

        // POST: Moderator/ManageLections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LectionId,Name,OwnerId,LectionType,Description,Editable,ExportOwner,Complexity,ComplexityOrder,LectionPath")] Lection lection, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var path = FileOperations.SaveFile(file, Server, pathToSave, extensions);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    FileOperations.DeleteIfExist(Server, lection.LectionPath);
                    lection.LectionPath = path;
                }
                lection.LectionText = new byte[] { };
                db.Entry(lection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", lection.OwnerId);
            ViewBag.LectionType = new SelectList(db.LectionGroup, "LectionGroupId", "Name", lection.LectionType);
            return View(lection);
        }

        // GET: Moderator/ManageLections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lection lection = db.Lection.Find(id);
            if (lection == null)
            {
                return HttpNotFound();
            }
            return View(lection);
        }

        // POST: Moderator/ManageLections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Lection lection = db.Lection.Find(id);
            FileOperations.DeleteIfExist(Server, lection.LectionPath);
            var task = db.ELTask.Where(x => x.LectionId == lection.LectionId && x.AuthorId == 1);
            if (task.Count() > 0)
            {
                db.ELTask.Remove(task.First());
            }
            db.Lection.Remove(lection);
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
    }
}
