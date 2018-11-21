using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;

namespace EnglishLearning.Areas.Moderator.Controllers
{
    public class ManageVideosController : Controller
    {
        private EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Moderator/ManageVideos
        public ActionResult Index()
        {
            var video = db.Video.Include(v => v.User);
            return View(video.ToList());
        }

        // GET: Moderator/ManageVideos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Video.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // GET: Moderator/ManageVideos/Create
        public ActionResult Create()
        {
            //ViewBag.UserId = new SelectList(db.User, "UserId", "UserId");
            return View();
        }

        // POST: Moderator/ManageVideos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,VideoHtml,Genre,Type,Name,AuthorLink,UserId,Difficult,Order")] Video video)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var identity = User.Identity.GetUserId();
                    video.UserId = db.User.Where(x => x.IdentityId == identity).Select(x => x.UserId).First();
                }
                catch {
                    video.UserId = 1;
                }
                db.Video.Add(video);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.UserId = new SelectList(db.User, "UserId", "UserId", video.UserId);
            return View(video);
        }

        // GET: Moderator/ManageVideos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Video.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            //ViewBag.UserId = new SelectList(db.User, "UserId", "UserId", video.UserId);
            return View(video);
        }

        // POST: Moderator/ManageVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,VideoHtml,Genre,Type,Name,AuthorLink,UserId,Difficult,Order")] Video video)
        {
            if (ModelState.IsValid)
            {
                db.Entry(video).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.UserId = new SelectList(db.User, "UserId", "UserId", video.UserId);
            return View(video);
        }

        // GET: Moderator/ManageVideos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Video video = db.Video.Find(id);
            if (video == null)
            {
                return HttpNotFound();
            }
            return View(video);
        }

        // POST: Moderator/ManageVideos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Video video = db.Video.Find(id);
            db.Video.Remove(video);
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
