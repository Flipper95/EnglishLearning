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
    public class ManageTestsController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageTests
        public ActionResult Index()
        {
            var test = db.Test.Include(t => t.User).Include(t => t.TestGroup);
            return View(test.ToList());
        }

        // GET: Moderator/ManageTests/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // GET: Moderator/ManageTests/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId");
            ViewBag.TestType = new SelectList(db.TestGroup, "TestGroupId", "Name");
            return View();
        }

        // POST: Moderator/ManageTests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TestId,Name,OwnerId,Difficult,TaskCount,TestType,Editable,ExportOwner,Time")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Test.Add(test);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", test.OwnerId);
            ViewBag.TestType = new SelectList(db.TestGroup, "TestGroupId", "Name", test.TestType);
            return View(test);
        }

        // GET: Moderator/ManageTests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", test.OwnerId);
            ViewBag.TestType = new SelectList(db.TestGroup, "TestGroupId", "Name", test.TestType);
            return View(test);
        }

        // POST: Moderator/ManageTests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TestId,Name,OwnerId,Difficult,TaskCount,TestType,Editable,ExportOwner,Time")] Test test)
        {
            if (ModelState.IsValid)
            {
                db.Entry(test).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", test.OwnerId);
            ViewBag.TestType = new SelectList(db.TestGroup, "TestGroupId", "Name", test.TestType);
            return View(test);
        }

        // GET: Moderator/ManageTests/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Test.Find(id);
            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        // POST: Moderator/ManageTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Test test = db.Test.Find(id);
            db.Test.Remove(test);
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
