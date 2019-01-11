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
    public class ManageAnswersController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageAnswers
        public ActionResult Index(int id = 0)
        {
            var answer = db.Answer.Include(a => a.Question);
            if (id != 0)
            {
                answer = answer.Where(x => x.QuestionId == id);
                ViewBag.QuestId = id;
            }
            return View(answer.ToList());
        }

        // GET: Moderator/ManageAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answer.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // GET: Moderator/ManageAnswers/Create
        public ActionResult Create(int id = 0)
        {
            if (id != 0)
                ViewBag.QuestionId = new SelectList(db.Question.Where(x => x.QuestionId == id), "QuestionId", "QuestText");
            else
                ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestText");
            return View();
        }

        // POST: Moderator/ManageAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AnswerId,QuestionId,AnswerText,Rightness")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Answer.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestText", answer.QuestionId);
            return View(answer);
        }

        // GET: Moderator/ManageAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answer.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestText", answer.QuestionId);
            return View(answer);
        }

        // POST: Moderator/ManageAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AnswerId,QuestionId,AnswerText,Rightness")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestText", answer.QuestionId);
            return View(answer);
        }

        // GET: Moderator/ManageAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answer.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Moderator/ManageAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer answer = db.Answer.Find(id);
            db.Answer.Remove(answer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult ClearQuestId() {
            ViewBag.QuestId = null;
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
