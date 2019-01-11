using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;

namespace EnglishLearning.Areas.Moderator.Controllers
{
    [Authorize(Roles = "moderator")]
    public class ManageTextTasksController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageTextTasks
        public ActionResult Index()
        {
            var textTask = db.TextTask.Include(t => t.User);
            return View(textTask.ToList());
        }

        // GET: Moderator/ManageTextTasks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextTask textTask = db.TextTask.Find(id);
            if (textTask == null)
            {
                return HttpNotFound();
            }
            return View(textTask);
        }

        // GET: Moderator/ManageTextTasks/Create
        public ActionResult Create()
        {
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId");
            return View();
        }

        // POST: Moderator/ManageTextTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "TextId,Name,Text,Words,Difficult")] TextTask textTask)
        {
            if (ModelState.IsValid)
            {
                if (textTask.Words.EndsWith(";")) textTask.Words = textTask.Words.TrimEnd(';');
                textTask.AuthorId = 1;
                ELTask task = new ELTask() { AuthorId = 1, Difficult = textTask.Difficult, Group = "TextTask", TextTask = textTask, Name = textTask.Name, Description = "Прочитайте текст та вставте в проміжки слова" };
                db.ELTask.Add(task);
                db.TextTask.Add(textTask);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", textTask.AuthorId);
            return View(textTask);
        }

        // GET: Moderator/ManageTextTasks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextTask textTask = db.TextTask.Find(id);
            if (textTask == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", textTask.AuthorId);
            return View(textTask);
        }

        // POST: Moderator/ManageTextTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "TextId,AuthorId,Name,Text,Words,Difficult")] TextTask textTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(textTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.AuthorId = new SelectList(db.User, "UserId", "Level", textTask.AuthorId);
            return View(textTask);
        }

        // GET: Moderator/ManageTextTasks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextTask textTask = db.TextTask.Find(id);
            if (textTask == null)
            {
                return HttpNotFound();
            }
            return View(textTask);
        }

        // POST: Moderator/ManageTextTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TextTask textTask = db.TextTask.Find(id);
            var task = db.ELTask.Where(x => x.TextId == textTask.TextId && x.AuthorId == 1);
            if (task.Count() > 0)
            {
                db.ELTask.Remove(task.First());
            }
            db.TextTask.Remove(textTask);
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
