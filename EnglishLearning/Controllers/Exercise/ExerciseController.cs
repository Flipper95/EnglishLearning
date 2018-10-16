using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    public class ExerciseController : Controller
    {
        protected EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Exercise
        public ActionResult Index() //int? SuccessPercent, string ErrorMessage = "", string SuccessMessage = ""
        {
            //ViewBag.SuccessMessage = SuccessMessage;
            //ViewBag.SuccessPercent = SuccessPercent;
            return View();
        }

        protected int getCurrentUserId() {
            string userIdentity = User.Identity.GetUserId();
            var userId = (from user in db.User
                          where user.IdentityId == userIdentity
                          select user.UserId).First();
            return userId;
        }

        public ActionResult ShowResult(int count, int max) {
            ViewBag.AnswerCount = count;
            ViewBag.MaxCount = max;
            //ViewBag.SuccessMessage = "Кількість правильних відповідей в останній вправі" + count + " з " + max;
            SessionClear();
            return View();
        }

        protected void SessionClear() {
            Session.Remove("Index");
            Session.Remove("Exercise");
            Session.Remove("AnswerCount");
            Session.Remove("questions");
        }

        //public ActionResult TranslateExercise() {
        //    return TranslateExercise(0);
        //}

        //[HttpPost]
        //public ActionResult TranslateExercise(int StartIndex = 0) {
        //    if (StartIndex >= 25) {
        //        return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5});
        //    }
        //    ViewBag.StartIndex = StartIndex;
        //    Session["Index"] = new Random().Next(StartIndex, StartIndex + 5);
        //    Session["Exercise"] = "translate";
        //    ViewBag.Index = new Random().Next(StartIndex, StartIndex + 5);
        //    if (StartIndex == 0)
        //    {
        //        Session["AnswerCount"] = 0;
        //        int userId = getCurrentUserId();
        //        //"SELECT * FROM [Word] AS w WHERE w.WordId IN (SELECT TOP 25 lw.WordId FROM [LearningWord] AS lw WHERE (lw.LearnPercent < 100) AND lw.UserId = @user ORDER BY newid())";
        //        var query = (from learningWord in db.LearningWord
        //                     where learningWord.UserId == userId && learningWord.LearnPercent < 100
        //                     select learningWord);
        //        //orderby learningWord.WordId
        //        int total = query.Count();
        //        if (total < 25)
        //        {
        //            SessionClear();
        //            ViewBag.ErrorMessage = "Кількість слів для вправи перекладу не достатньо, виберіть додаткових слів на вивчення";
        //            return View("Index");
        //            //return RedirectToAction("PassToIndex", "Exercise",
        //            //    new { ErrorMessage = "Кількість слів для вправи перекладу не достатньо, виберіть додаткових слів на вивчення" });
        //        }
        //        var query1 = (from word in db.Word
        //                      where (query).OrderBy(x => Guid.NewGuid()).Take(25).Any(x => x.WordId == word.WordId)
        //                      select word).OrderBy(x => Guid.NewGuid());
        //        var result = query1.ToList();
        //        Session["questions"] = result;
        //        return View(result);
        //    }
        //    else {
        //        var result = Session["questions"] as List<Word>;
        //        return View(result);
        //    }
        //    //var watch1 = System.Diagnostics.Stopwatch.StartNew();
        //    //Random rnd = new Random();
        //    //List<ExerciseModel> words = new List<ExerciseModel>();
        //    //List<int> random = new List<int>();
        //    //while (random.Count < 25) {
        //    //    int number = rnd.Next(0, total);
        //    //    if (!random.Contains(number))
        //    //    {
        //    //        random.Add(number);
        //    //    }
        //    //}
        //    //for (int i = 0; i < 25; i++) {
        //    //    words.Add(new ExerciseModel());
        //    //    int randNumber = random[i];
        //    //    words[i].lWord = query.OrderBy(x=>x.WordId).Skip(randNumber).First();
        //    //    int searchIndex = words[i].lWord.WordId;
        //    //    words[i].word = (from word in db.Word
        //    //                     where word.WordId == searchIndex
        //    //                     select word).First();
        //    //}
        //    //watch1.Stop();
        //    //var elapsedMs1 = watch1.ElapsedMilliseconds;
        //    ////.OrderBy(x => Guid.NewGuid()).Take(25)
        //    //return View(words);
        //}

        public bool CheckResult(int id, string value) {
            List<Word> words = Session["questions"] as List<Word>;
            bool result = CompareResult(words[Convert.ToInt32(Session["Index"])], value, Session["Exercise"].ToString());
            if (result)
            {
                Session["AnswerCount"] = Convert.ToInt32(Session["AnswerCount"]) + 1;
                int userId = getCurrentUserId();
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

        private bool CompareResult(Word word, string value, string exercise) {
            if (exercise == "translate" || exercise == "listening" || exercise == "constructor") {
                return word.Word1 == value ? true : false;
            }
            if (exercise == "equivalent") {
                return word.Translate == value ? true : false;
            }
            return false;
        }

    }
}