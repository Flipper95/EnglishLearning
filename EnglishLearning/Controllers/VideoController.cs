using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    public class VideoController : Controller
    {

        EnglishLearningEntities db = new EnglishLearningEntities();
        //Difficult difficult;

        // GET: Video
        public ActionResult Index()
        {
            var identity = User.Identity.GetUserId();
            var user = db.User.Where(x => x.IdentityId == identity).First();
            Enum.TryParse(user.Level.Replace('-', '_'), out Difficult complexity);
            //difficult = complexity;
            //ViewBag.UserLvl = difficult;
            TempData["UserLvl"] = complexity;

            var genre = db.Video.Select(x => x.Genre).Distinct().ToList();
            return View(genre);
        }

        public ActionResult GetVideoByGenre(string genre) {
            //orderby video.Order
            //var videos = (from video in db.Video
            //              where video.Genre == genre
            //              orderby video.Order
            //              group video by video.Type into v
            //              select v).ToDictionary(g => g.Key, g => g.ToList());
            ViewBag.UserLvl = TempData["UserLvl"];
            var videos = db.Video.Where(x => x.Genre == genre).OrderBy(x => x.Order)
                           .GroupBy(x => x.Type)
                           .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Order).ToList());
                           //.OrderBy(x => x.Value.OrderBy(y => y.Order));
                                                      //.OrderBy(x => x.First().Order)
            return PartialView("VideoAccordion", videos);
        }

        public string GetVideo(int id) {
            var videoTag = db.Video.Where(x => x.Id == id).Select(x => x.VideoHtml).First();
            return videoTag;
        }
    }
}