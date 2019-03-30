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

        protected List<Word> Excercise(string ExerciseName, int MinNumber, int CountToTake, bool repeat = false, int StartIndex = 0, int RandomStep = 0)
        {

            ViewBag.StartIndex = StartIndex;
            int index = new Random().Next(StartIndex, StartIndex + RandomStep);
            Session["Index"] = index;
            ViewBag.Index = index;//1
            Session["Exercise"] = ExerciseName;

            if (StartIndex == 0)
            {
                Session["AnswerCount"] = 0;

                int userId = GetCurrentUserId();
                var query = (from learningWord in db.LearningWord
                             where learningWord.UserId == userId && learningWord.LearnPercent < 100
                             select learningWord);
                if (repeat) query = query.Where(x => x.LearnPercent == 100);
                else query = query.Where(x => x.LearnPercent < 100);
                int total = query.Count();
                if (total < MinNumber)
                {
                    SessionClear();
                    if (repeat) TempData["ErrorMessage"] = "Кількість слів для повторення не достатньо, спочатку вивчіть ще декілька слів";
                    else TempData["ErrorMessage"] = total + " cлів для вправи не достатньо, виберіть додаткових слів на вивчення";
                    return null;
                }

                var query1 = (from word in db.Word
                              where (query).OrderBy(x => Guid.NewGuid()).Take(CountToTake).Any(x => x.WordId == word.WordId)
                              select word).OrderBy(x => Guid.NewGuid());
                var result = OperationsWithQuery(query1);
                if (result == null) return null;
                Session["questions"] = result;
                result = OperationsWithWordsOnFirstLap(result, query1, index);
                return result;
            }
            else
            {
                var result = Session["questions"] as List<Word>;
                result = OperationWithWordsOnEachLap(result, index, StartIndex);
                return result;
            }
        }

        protected virtual ActionResult ShowResult(int MaxAnswers) {
            return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = MaxAnswers });
        }

        protected virtual List<Word> OperationsWithQuery(IOrderedQueryable<Word> query1)
        {
            return query1.ToList();
        }

        protected virtual List<Word> OperationsWithWordsOnFirstLap(List<Word> words, IOrderedQueryable<Word> query1, int index)
        {
            return words;
        }

        protected virtual List<Word> OperationWithWordsOnEachLap(List<Word> words, int index, int startIndex)
        {
            return words;
        }

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