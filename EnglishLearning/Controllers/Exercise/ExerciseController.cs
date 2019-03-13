using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class ExerciseController : Controller
    {
        protected EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Exercise
        public ActionResult Index() //int? SuccessPercent, string ErrorMessage = "", string SuccessMessage = ""
        {
            if (TempData["ErrorMessage"] != null) {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View();
        }

        protected int GetCurrentUserId() {
            string userIdentity = User.Identity.GetUserId();
            var userId = (from user in db.User
                          where user.IdentityId == userIdentity
                          select user.UserId).First();
            return userId;
        }

        public ActionResult ShowResult(int count, int max) {
            ViewBag.AnswerCount = count;
            ViewBag.MaxCount = max;
            SessionClear();
            return View();
        }

        protected void SessionClear() {
            Session.Remove("Index");
            Session.Remove("Exercise");
            Session.Remove("AnswerCount");
            Session.Remove("questions");
        }

        public bool CheckResult(int id, string value) {
            List<Word> words = Session["questions"] as List<Word>;
            bool result = CompareResult(words[Convert.ToInt32(Session["Index"])], id, value, Session["Exercise"].ToString());
            if (result)
            {
                Session["AnswerCount"] = Convert.ToInt32(Session["AnswerCount"]) + 1;
                int userId = GetCurrentUserId();
                LearningWord learnedWord = (from lw in db.LearningWord
                                            where lw.WordId == id && lw.UserId == userId
                                            select lw).First();
                learnedWord.ExcerciseDate = DateTime.Now;
                if (learnedWord.LearnPercent >= 100 - 15)
                {
                    learnedWord.LearnPercent = 100;
                    learnedWord.LearnedDate = DateTime.Now.Date;
                }
                else
                {
                    learnedWord.LearnPercent += 15;
                }
                db.SaveChanges();
            }
            return result;
        }

        private bool CompareResult(Word word, int id, string value, string exercise) {
            if (exercise == "translate" || exercise == "listening" || exercise == "constructor") {
                return word.Word1 == value ? true : false;
            }
            if (exercise == "equivalent") {
                return word.Translate == value ? true : false;
            }
            if (exercise == "synonyms") {
                return word.WordId == id ? true : false;
            }
            return false;
        }

    }
}