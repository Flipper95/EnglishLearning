using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;
using EnglishLearning.ExtendClasses;

namespace EnglishLearning.Controllers.Exercise
{
    public class ConstructorController : ExerciseController
    {

        public ActionResult ConstructorExercise()
        {
            return ConstructorExercise(0);
        }

        [HttpPost]
        public ActionResult ConstructorExercise(int StartIndex = 0)
        {
            if (StartIndex >= 5)
            {
                return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }

            ViewBag.StartIndex = StartIndex;
            int index = new Random().Next(StartIndex, StartIndex);
            Session["Index"] = index;
            ViewBag.Index = index;
            Session["Exercise"] = "constructor";

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
                    ViewBag.ErrorMessage = "Кількість слів для вправи перекладу не достатньо, виберіть додаткових слів на вивчення";
                    return View("Index");
                }

                var query1 = (from word in db.Word
                              where (query).OrderBy(x => Guid.NewGuid()).Take(5).Any(x => x.WordId == word.WordId)
                              select word).OrderBy(x => Guid.NewGuid());
                var result = query1.ToList();
                Session["questions"] = result;
                ViewBag.wordArray = ShuffleWord(result[index]);
                return View(result);
            }
            else
            {
                var result = Session["questions"] as List<Word>;
                ViewBag.wordArray = ShuffleWord(result[index]);
                return View(result);
            }
        }

        private char[] ShuffleWord(Word word)
        {
            char[] wordArray = Shuffle.ShuffleList(word.Word1.ToList()).ToArray();
            return wordArray;
        }

        public string GetAnswer()
        {
            Word w = (Session["questions"] as List<Word>)[Convert.ToInt32(Session["Index"])];
            return w.Word1;
        }

    }
}