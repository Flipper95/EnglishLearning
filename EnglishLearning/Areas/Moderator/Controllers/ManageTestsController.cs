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
    public class ManageTestsController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();
        private string[] extensions = new string[] { ".mp3" };
        private string pathToSave = "~/App_Data/TestVoice/";

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
        public ActionResult Create([Bind(Include = "TestId,Name,OwnerId,Difficult,TaskCount,TestType,Editable,ExportOwner,Time,Text")] Test test, HttpPostedFileBase file, HttpPostedFileBase testText)
        {
            if (ModelState.IsValid)
            {
                test.Voice = FileOperations.SaveFile(file, Server, pathToSave, extensions);
                ELTask task = new ELTask() { AuthorId = 1, Difficult = test.Difficult, Group = "Test", Test = test, Name = test.Name, Description = "Пройдіть заданий тест" };
                if (testText != null && testText.ContentLength > 0)
                    CreateTestFromFile(testText, test);
                db.ELTask.Add(task);
                db.Test.Add(test);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.User, "UserId", "UserId", test.OwnerId);
            ViewBag.TestType = new SelectList(db.TestGroup, "TestGroupId", "Name", test.TestType);
            return View(test);
        }

        private void CreateTestFromFile(HttpPostedFileBase file, Test test) {
            string result = new StreamReader(file.InputStream).ReadToEnd();
            string[] testdata = result.Split('\r','\n');
            List<string> data = testdata.ToList();
            data = data.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            for (int i = 0; i < data.Count / 2; i++)
            {
                Question quest = new Question();
                List<string> answers = data[i*2 + 1].Split('&').ToList();
                answers = answers.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                quest.Answer.Add(new Answer() { AnswerText = answers[0], Rightness = true });
                for (int j = 1; j < answers.Count; j++) {
                    quest.Answer.Add(new Answer() { AnswerText = answers[j], Rightness = false });
                }
                quest.QuestText = data[i*2];
                test.Question.Add(quest);
            }
            string text = data[0];
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
        public ActionResult Edit([Bind(Include = "TestId,Voice,Name,OwnerId,Difficult,TaskCount,TestType,Editable,ExportOwner,Time,Text")] Test test, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var path = FileOperations.SaveFile(file, Server, pathToSave, extensions);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    FileOperations.DeleteIfExist(Server, test.Voice);
                    test.Voice = path;
                }
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
            FileOperations.DeleteIfExist(Server, test.Voice);
            var task = db.ELTask.Where(x => x.TestId == test.TestId && x.AuthorId == 1);
            if (task.Count() > 0)
            {
                db.ELTask.Remove(task.First());
            }
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
