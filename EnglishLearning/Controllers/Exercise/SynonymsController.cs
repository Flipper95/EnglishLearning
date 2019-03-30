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
    public class SynonymsController : ExerciseController
    {
        public ActionResult SynonymsExercise()
        {
            return SynonymsExercise(0);
        }

        [HttpPost]
        public ActionResult SynonymsExercise(int StartIndex = 0)
        {
            if (StartIndex >= 5)
            {
                return base.ShowResult(5);//RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }
            var result = base.Excercise("synonyms", 25, 30, StartIndex: StartIndex);
            if (result == null) return RedirectToAction("Index", "Exercise", new { area = "" });
            return View(result);
        }

        protected override List<Word> OperationsWithQuery(IOrderedQueryable<Word> query1)
        {
            List<Word> answers;
            try
            {
                answers = DownloadWords(query1, 5);
            }
            catch
            {
                SessionClear();
                TempData["ErrorMessage"] = " Не вдалося завантажити достатньо синонімів для вправи, спробуйте пізніше";
                return null;//RedirectToAction("Index", "Exercise", new { area = "" });
            }
            return answers.ToList();
        }

        protected override List<Word> OperationsWithWordsOnFirstLap(List<Word> words, IOrderedQueryable<Word> query1, int index)
        {
            var notIn = words.Select(x => x.WordId);
            List<Word> model = query1.Where(x => !notIn.Contains(x.WordId)).ToList();
            Session["wrongQuestions"] = model;
            model = model.GetRange(0, 4);
            model.Add(words[index]);
            model = Shuffle.ShuffleList(model);
            ViewBag.Index = model.IndexOf(words[index]);
            return model;
        }

        protected override List<Word> OperationWithWordsOnEachLap(List<Word> words, int index, int startIndex)
        {
            var model = Session["wrongQuestions"] as List<Word>;
            model = model.GetRange(startIndex * 5, 4);
            var tempWord = words[index];
            model.Add(tempWord);
            model = Shuffle.ShuffleList(model);
            ViewBag.Index = model.IndexOf(tempWord);
            return model;
        }

        private List<Word> DownloadWords(IOrderedQueryable<Word> words, int totalCount)
        {
            var wordsWithSyn = words.Where(x => x.Synonyms != null);
            List<Word> result = wordsWithSyn.ToList();
            int count = result.Count();
            if (count < totalCount)
            {
                var wordsWithoutSyn = words.Where(x => x.Synonyms == null);
                var temp = UploadSynonyms(wordsWithoutSyn.ToList(), totalCount - count);
                result = result.Concat(temp).ToList();
                db.SaveChanges();
            }
            try
            {
                result = result.GetRange(0, totalCount);
            }
            catch(Exception ex) {
                throw new Exception("Не вдалося завантажити слова для вправи, спробуйте пізніше.", ex);
            }
            return result;
        }

        private List<Word> UploadSynonyms(List<Word> list, int count)
        {
            SynonymAPI api = new SynonymAPI();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Synonyms == null)
                {
                    if (count == 0) break;
                    List<string> synonyms = api.GetAllSynonyms(list[i].Word1);
                    if (synonyms.Count <= 1)
                    {
                        list.Remove(list[i]);
                    }
                    else
                    {
                        string concatSyn = String.Join("|", synonyms.ToArray());
                        list[i].Synonyms = concatSyn;
                        count--;
                    }
                }
            }
            list.RemoveAll(x => x.Synonyms == null);
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
    }
}