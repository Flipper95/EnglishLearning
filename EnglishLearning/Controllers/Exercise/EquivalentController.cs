using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers.Exercise
{
    [Authorize]
    public class EquivalentController : ExerciseController
    {
        public ActionResult EquivalentExercise(bool repeat)
        {
            return EquivalentExercise(0, repeat);
        }

        [HttpPost]
        public ActionResult EquivalentExercise(int StartIndex = 0, bool repeat = false)
        {
            if (StartIndex >= 25)
            {
                return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }

            ViewBag.StartIndex = StartIndex;
            int index = new Random().Next(StartIndex, StartIndex + 5);
            Session["Index"] = index;
            ViewBag.Index = index;
            Session["Exercise"] = "equivalent";

            if (StartIndex == 0)
            {
                Session["AnswerCount"] = 0;

                int userId = GetCurrentUserId();
                var query = (from learningWord in db.LearningWord
                             where learningWord.UserId == userId && learningWord.LearnPercent < 100
                             select learningWord);
                if (repeat) query = query.Where(x => x.LearnPercent == 100);
                else query = query.Where(x => x.LearnPercent < 100);
                ViewBag.Repeat = repeat;

                int total = query.Count();
                if (total < 25)
                {
                    SessionClear();
                    if(repeat) TempData["ErrorMessage"] = "Кількість слів для повторення не достатньо, спочатку вивчіть ще декілька слів";
                    else TempData["ErrorMessage"] = "Кількість слів для вправи з еквівалентами не достатньо, виберіть додаткових слів на вивчення";
                    return RedirectToAction("Index", "Exercise", new { area = "" });
                    //ViewBag.ErrorMessage = total+" cлів для вправи не достатньо, виберіть додаткових слів на вивчення";
                    //return View("Index");
                }

                var query1 = (from word in db.Word
                              where (query).OrderBy(x => Guid.NewGuid()).Take(25).Any(x => x.WordId == word.WordId)
                              select word).OrderBy(x => Guid.NewGuid());
                var result = query1.ToList();
                Session["questions"] = result;
                return View(result);
            }
            else
            {
                var result = Session["questions"] as List<Word>;
                return View(result);
            }
        }

        public ActionResult LoadAudio(string value)
        {
            Word w = (Session["questions"] as List<Word>)[Convert.ToInt32(Session["Index"])];
            if (w.Voice != null)
            {
                if (w.Voice.Length > 1)
                {
                    return base.File(w.Voice, "audio/wav");
                }
                return base.File(new byte[1], "audio/wav");
            }
            else return base.File(new byte[1], "audio/wav");
        }

    }
}
