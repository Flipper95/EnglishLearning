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
                return base.ShowResult(5);//RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }
            var result = base.Excercise("equivalent", 25, 25, repeat: repeat, StartIndex: StartIndex, RandomStep: 5);
            if (result == null) return RedirectToAction("Index", "Exercise", new { area = "" });
            ViewBag.Repeat = repeat;
            return View(result);
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
