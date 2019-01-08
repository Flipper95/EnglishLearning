using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;

namespace EnglishLearning.Areas.Moderator.Controllers
{
    public class ManageGrammarsController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageGrammars
        public ActionResult Index()
        {
            var grammar = db.Grammar.Include(g => g.GrammarGroup).Include(g => g.User);
            return View(grammar.ToList());
        }

        // GET: Moderator/ManageGrammars/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar grammar = db.Grammar.Find(id);
            if (grammar == null)
            {
                return HttpNotFound();
            }
            return View(grammar);
        }

        // GET: Moderator/ManageGrammars/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.GrammarGroup.Where(x => x.ParentId != 0), "GroupId", "Name");
            ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId");
            return View();
        }

        // POST: Moderator/ManageGrammars/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,Translate,GroupId,AuthorId")] Grammar grammar, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                byte[] bytes = null;
                if (file != null && file.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        bytes = binaryReader.ReadBytes(file.ContentLength);
                    }
                }
                if (bytes == null)
                    grammar.Voice = UploadVoice(grammar.Text);
                else grammar.Voice = bytes;
                db.Grammar.Add(grammar);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(db.GrammarGroup.Where(x => x.ParentId != 0), "GroupId", "Name", grammar.GroupId);
            ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", grammar.AuthorId);
            return View(grammar);
        }

        private byte[] UploadVoice(string sentence) {
            VoiceAPI api = new VoiceAPI();
            byte[] result = api.UploadVoice(sentence);
            if (result.Length <= 1) {
                result = null;
            }
            return result;
        }

        public ActionResult LoadAudio(int id)
        {
            Grammar grammar = db.Grammar.Find(id);
            if (grammar.Voice.Length > 1)
            {
                return base.File(grammar.Voice, "audio/wav");
            }
            else return base.File(new byte[1], "audio/wav");
        }

        // GET: Moderator/ManageGrammars/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar grammar = db.Grammar.Find(id);
            if (grammar == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.GrammarGroup.Where(x => x.ParentId != 0), "GroupId", "Name", grammar.GroupId);
            ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", grammar.AuthorId);
            return View(grammar);
        }

        // POST: Moderator/ManageGrammars/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Voice,Text,Translate,GroupId,AuthorId")] Grammar grammar)
        {
            if (ModelState.IsValid)
            {
                db.Entry(grammar).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(db.GrammarGroup.Where(x => x.ParentId != 0), "GroupId", "Name", grammar.GroupId);
            ViewBag.AuthorId = new SelectList(db.User, "UserId", "UserId", grammar.AuthorId);
            return View(grammar);
        }

        // GET: Moderator/ManageGrammars/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grammar grammar = db.Grammar.Find(id);
            if (grammar == null)
            {
                return HttpNotFound();
            }
            return View(grammar);
        }

        // POST: Moderator/ManageGrammars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Grammar grammar = db.Grammar.Find(id);
            db.Grammar.Remove(grammar);
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
