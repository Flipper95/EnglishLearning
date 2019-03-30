using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers.Exercise
{
    [Authorize]
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
                return base.ShowResult(5);
            }
            var result = base.Excercise("listening", 5, 25, StartIndex: StartIndex);
            if(result == null) return RedirectToAction("Index", "Exercise", new { area = "" });
            return View(result);
        }

        protected override List<Word> OperationsWithQuery(IOrderedQueryable<Word> query1)
        {
            var result = DownloadWords(query1, 5);
            if (result.Count < 5)
            {
                SessionClear();
                TempData["ErrorMessage"] = " Не вдалося завантажити озвучування cлова для вправи, спробуйте пізніше";
                return null;
                //return RedirectToAction("Index", "Exercise", new { area = "" });
            }
            return result;
        }

        private List<Word> DownloadWords(IOrderedQueryable<Word> words, int totalCount) {
            var query = words.Where(x => x.Voice != null);
            int count = query.Count();
            List<Word> result = query.ToList();
            if (count < totalCount) {
                var query2 = words.Where(x => x.Voice == null);//.Take(totalCount - count);
                result = result.Concat(UploadVoice(query2.ToList(), totalCount - count)) as List<Word>;
                db.SaveChanges();
            }
            return result;
        }

        private List<Word> UploadVoice(List<Word> list, int count)
        {
            VoiceAPI api = new VoiceAPI();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if(list[i].Voice == null)
                {
                    if (count == 0) break;
                    byte[] voice = api.UploadVoice(list[i].Word1);
                    list[i].Voice = voice;
                    if (list[i].Voice.Length <= 1)
                    {
                        list.Remove(list[i]);
                    }
                    else count--;
                }
            }
            list.RemoveAll(x => x.Voice == null);
            return list;
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