using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;

namespace EnglishLearning.Areas.Moderator.Controllers
{
    [Authorize(Roles = "moderator")]
    public class ManageWordsController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageWords
        public ActionResult Index(int GroupId)
        {
            ViewBag.Group = GroupId;
            var word = db.Word.Include(w => w.Group).Where(x => x.GroupId == GroupId);
            return View(word.ToList());
        }

        // GET: Moderator/ManageWords/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Word word = db.Word.Find(id);
            if (word == null)
            {
                return HttpNotFound();
            }
            return View(word);
        }

        // GET: Moderator/ManageWords/Create
        public ActionResult Create(int GroupId)
        {
            //ViewBag.GroupId = new SelectList(db.Group, "GroupId", "Name");
            ViewBag.Group = GroupId;
            return View();
        }

        // POST: Moderator/ManageWords/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WordId,Word1,Translate,GroupId,PartOfSpeech,Transcription,Synonyms")] Word word)
        {
            ViewBag.Group = word.GroupId;
            if (ModelState.IsValid)
            {
                word = UploadVoice(word);
                var group = db.Group.Find(word.GroupId);
                group.WordsCount = group.WordsCount + 1;
                db.Entry(group).State = EntityState.Modified;
                db.Word.Add(word);
                db.SaveChanges();
                return RedirectToAction("Index", new { GroupId = word.GroupId});
            }
            //ViewBag.GroupId = new SelectList(db.Group, "GroupId", "Name", word.GroupId);
            return View(word);
        }

        // GET: Moderator/ManageWords/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Word word = db.Word.Find(id);
            if (word == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.Group, "GroupId", "Name", word.GroupId);
            return View(word);
        }

        // POST: Moderator/ManageWords/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WordId,Word1,Translate,GroupId,PartOfSpeech,Voice,Transcription,Synonyms")] Word word)
        {
            if (ModelState.IsValid)
            {
                //word.Voice == null
                db.Entry(word).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { GroupId = word.GroupId });
            }
            ViewBag.GroupId = new SelectList(db.Group, "GroupId", "Name", word.GroupId);
            return View(word);
        }

        // GET: Moderator/ManageWords/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Word word = db.Word.Find(id);
            if (word == null)
            {
                return HttpNotFound();
            }
            return View(word);
        }

        // POST: Moderator/ManageWords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Word word = db.Word.Find(id);
            var group = db.Group.Find(word.GroupId);
            group.WordsCount = group.WordsCount - 1;
            db.Entry(group).State = EntityState.Modified;
            db.Word.Remove(word);
            db.SaveChanges();
            return RedirectToAction("Index", new { GroupId = word.GroupId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private Word UploadVoice(Word word)
        {
            VoiceAPI api = new VoiceAPI();
            byte[] voice = api.UploadVoice(word.Word1);
            word.Voice = voice;
            return word;
        }

        public ActionResult LoadAudio(int id)
        {
            Word w = db.Word.Find(id);
            if (w.Voice.Length > 1)
            {
                return base.File(w.Voice, "audio/wav");
            }
            else return base.File(new byte[1], "audio/wav");
        }

    }
}
