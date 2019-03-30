using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class TranslateController : ExerciseController
    {
        public ActionResult TranslateExercise()
        {
            return TranslateExercise(0);
        }

        [HttpPost]
        public ActionResult TranslateExercise(int StartIndex = 0)
        {
            if (StartIndex >= 25)
            {
                return base.ShowResult(5);//RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }

            var result = base.Excercise("translate", 25, 25, StartIndex: StartIndex, RandomStep: 5);
            if (result == null) return RedirectToAction("Index", "Exercise", new { area = "" });
            return View(result);
        }
    }
}