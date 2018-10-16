using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers.Exercise
{
    public class ListeningController : ExerciseController
    {

        public ActionResult ListeningExercise()
        {
            return ListeningExercise(0);
        }

        [HttpPost]
        public ActionResult ListeningExercise(int StartIndex = 0)
        {
            if (StartIndex >= 5)
            {
                return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }

            ViewBag.StartIndex = StartIndex;
            int index = new Random().Next(StartIndex, StartIndex + 0);
            Session["Index"] = index;
            ViewBag.Index = index;
            Session["Exercise"] = "listening";

            if (StartIndex == 0)
            {
                Session["AnswerCount"] = 0;

                int userId = getCurrentUserId();
                var query = (from learningWord in db.LearningWord
                                where learningWord.UserId == userId && learningWord.LearnPercent < 100
                                select learningWord);

                int total = query.Count();
                if (total < 5)
                {
                    SessionClear();
                    ViewBag.ErrorMessage = total + " cлів для вправи не достатньо, виберіть додаткових слів на вивчення";
                    return View("Index");
                }

                var query1 = (from word in db.Word
                              where (query).OrderBy(x => Guid.NewGuid()).Take(5).Any(x => x.WordId == word.WordId)
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
            if (w.Voice.Length > 1)
            {
                return base.File(w.Voice, "audio/wav");
            }
            else return base.File(new byte[1], "audio/wav");
        }

        public string GetAnswer() {
            Word w = (Session["questions"] as List<Word>)[Convert.ToInt32(Session["Index"])];
            return w.Word1;
        }

    }
}