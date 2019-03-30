using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;
using EnglishLearning.ExtendClasses;

namespace EnglishLearning.Controllers.Exercise
{
    [Authorize]
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
                return base.ShowResult(5);//RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }
            var result = base.Excercise("constructor", 5, 5, StartIndex: StartIndex);
            if (result == null) return RedirectToAction("Index", "Exercise", new { area = "" });
            return View(result);
        }

        protected override List<Word> OperationsWithWordsOnFirstLap(List<Word> words, IOrderedQueryable<Word> query1, int index)
        {
            ViewBag.wordArray = ShuffleWord(words[index]);
            return words;
        }

        protected override List<Word> OperationWithWordsOnEachLap(List<Word> words, int index, int startIndex)
        {
            ViewBag.wordArray = ShuffleWord(words[index]);
            return words;
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