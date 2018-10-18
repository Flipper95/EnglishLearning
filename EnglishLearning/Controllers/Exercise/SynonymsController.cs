using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;

namespace EnglishLearning.Controllers.Exercise
{
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
                return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = 5 });
            }

            ViewBag.StartIndex = StartIndex;
            int index = new Random().Next(StartIndex, StartIndex);
            Session["Index"] = index;
            //ViewBag.Index = index;
            Session["Exercise"] = "synonyms";

            if (StartIndex == 0)
            {
                Session["AnswerCount"] = 0;

                int userId = getCurrentUserId();
                var query = (from learningWord in db.LearningWord
                             where learningWord.UserId == userId && learningWord.LearnPercent < 100
                             select learningWord);

                int total = query.Count();
                if (total < 25)
                {
                    SessionClear();
                    ViewBag.ErrorMessage = total + " cлів для вправи не достатньо, виберіть додаткових слів на вивчення";
                    return View("Index");
                }

                var query1 = (from word in db.Word
                              where (query).OrderBy(x => Guid.NewGuid()).Take(30).Any(x => x.WordId == word.WordId)
                              select word).OrderBy(x => Guid.NewGuid());
                List<Word> answers;
                try
                {
                    answers = DownloadWords(query1, 5);
                }
                catch {
                    SessionClear();
                    ViewBag.ErrorMessage = total + " cлів для вправи не достатньо, виберіть додаткових слів на вивчення";
                    return View("Index");
                }
                Session["questions"] = answers.ToList();
                var notIn = answers.Select(x => x.WordId);
                List<Word> model = query1.Where(x => !notIn.Contains(x.WordId)).ToList();
                Session["wrongQuestions"] = model;
                //ViewBag.Answer = answers[index];
                model = model.GetRange(StartIndex*5, 4);
                model.Add(answers[index]);
                model = Shuffle.ShuffleList(model);
                ViewBag.Index = model.IndexOf(answers[index]);
                return View(model);
            }
            else
            {
                var model = Session["wrongQuestions"] as List<Word>;
                model = model.GetRange(StartIndex*5, 4);
                //ViewBag.Answer = (Session["answers"] as List<Word>)[index];
                var tempWord = (Session["questions"] as List<Word>)[index];
                model.Add(tempWord);
                model = Shuffle.ShuffleList(model);
                ViewBag.Index = model.IndexOf(tempWord);
                return View(model);
            }
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
                        //Synonym syn = new Synonym();
                        //syn.FirstWordId = list[i].WordId;
                        //syn.Synonyms = concatSyn;
                        //ICollection<Synonym> synCollection = new List<Synonym> { syn };
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