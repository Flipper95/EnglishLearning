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
    public class ManageGrammarGroupsController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageGrammarGroups
        public ActionResult Index()
        {
            return View(db.GrammarGroup.ToList());
        }

        // GET: Moderator/ManageGrammarGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrammarGroup grammarGroup = db.GrammarGroup.Find(id);
            if (grammarGroup == null)
            {
                return HttpNotFound();
            }
            return View(grammarGroup);
        }

        // GET: Moderator/ManageGrammarGroups/Create
        public ActionResult Create()
        {
            ViewBag.ParentId = new SelectList(db.GrammarGroup, "GroupId", "Name");
            return View();
        }

        // POST: Moderator/ManageGrammarGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,Name,Difficult,ParentId")] GrammarGroup grammarGroup)
        {
            if (ModelState.IsValid)
            {
                ELTask task = new ELTask() { AuthorId = 1, Difficult = grammarGroup.Difficult, Group = "Grammar", GrammarGroup = grammarGroup, Name = grammarGroup.Name, Description = "Виконайте граматичну вправу для заданої групи" };
                db.ELTask.Add(task);
                db.GrammarGroup.Add(grammarGroup);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.GrammarGroup, "GroupId", "Name", grammarGroup.ParentId);
            return View(grammarGroup);
        }

        // GET: Moderator/ManageGrammarGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrammarGroup grammarGroup = db.GrammarGroup.Find(id);
            if (grammarGroup == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(db.GrammarGroup, "GroupId", "Name", grammarGroup.ParentId);
            return View(grammarGroup);
        }

        // POST: Moderator/ManageGrammarGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,Name,Difficult,ParentId")] GrammarGroup grammarGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grammarGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(db.GrammarGroup, "GroupId", "Name", grammarGroup.ParentId);
            return View(grammarGroup);
        }

        // GET: Moderator/ManageGrammarGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrammarGroup grammarGroup = db.GrammarGroup.Find(id);
            if (grammarGroup == null)
            {
                return HttpNotFound();
            }
            return View(grammarGroup);
        }

        // POST: Moderator/ManageGrammarGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GrammarGroup grammarGroup = db.GrammarGroup.Find(id);
            var task = db.ELTask.Where(x => x.GrammarId == grammarGroup.GroupId && x.AuthorId == 1);
            if (task.Count() > 0)
            {
                db.ELTask.Remove(task.First());
            }
            db.GrammarGroup.Remove(grammarGroup);
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
