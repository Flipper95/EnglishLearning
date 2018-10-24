using EnglishLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;

namespace EnglishLearning.Controllers
{
    public class TestController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();
        // GET: Test
        public ActionResult Index()
        {
            List<GroupModel> allGroups = new List<GroupModel>();
            allGroups = (from testG in db.TestGroup
                         orderby testG.ParentId
                         select new GroupModel {
                             Id = testG.TestGroupId,
                             Name = testG.Name,
                             ParentId = testG.ParentId
                         }).ToList();
                //db.LectionGroup.OrderBy(x => x.ParentId).ToList();
            var tests = from test in db.Test
                           where test.OwnerId == 1
                           select test;
            ViewBag.Tests = tests;
            return View(allGroups);
        }

        public ActionResult Test(int id) {
            //db.Question.Local
            Session["Time"] = DateTime.Now;
            ViewBag.Time = 15;
            var test = db.Test.Where(x => x.TestId == id).First();
            ViewBag.Name = test.Name;

            IQueryable<Question> questions = (from quest in db.Question
                            where quest.TestId == id
                            select quest).OrderBy(x => Guid.NewGuid());
            int count = questions.Count();
            if (count >= test.TaskCount)
                questions = questions.Take(test.TaskCount);
            else questions = questions.Take(count);

            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
            Session["answers"] = dict;

            var result = questions.ToList();
            Session["questions"] = result.Select(x => x.QuestionId).ToList();

            return View(result);
        }

        public PartialViewResult ShowAnswers(int id, int? nextId) {
            string question = (from quest in db.Question
                           where quest.QuestionId == id
                           select quest.QuestText).FirstOrDefault();
            ViewBag.Question = question;
            ViewBag.NextId = nextId;

            Dictionary<int, List<int>> dict = Session["answers"] as Dictionary<int, List<int>>;
            if(dict != null)
            {
                if (dict.ContainsKey(id))
                    ViewBag.Checked = dict[id];
            }

            var answers = from answer in db.Answer
                          where answer.QuestionId == id
                          select answer;
            var result = Shuffle.ShuffleList(answers.ToList());
            return PartialView(result);
        }

        [HttpPost]
        public PartialViewResult ShowAnswers(int? nextId, int prevId, int choosenId, object[] answers) {
            //if (answers != null && answers.Length > 0)
            //{
            //    List<int> intanswers = Array.ConvertAll(answers, item => Convert.ToInt32(item)).ToList();//answers.OfType<int>().ToList();
            //    Dictionary<int, List<int>> dict = Session["answers"] as Dictionary<int, List<int>>;
            //    if (dict != null)
            //    {
            //        if (dict.ContainsKey(prevId))
            //            dict[prevId] = intanswers;
            //        else dict.Add(prevId, intanswers);
            //    }
            //}
            SaveAnswers(answers, prevId);
            //save answer in session dictionary? question id: int[] answers
            return ShowAnswers(choosenId, nextId);
        }

        [HttpPost]
        public void SaveAnswers(object[] answers, int questId) {
            if (answers != null && answers.Length > 0)
            {
                List<int> intanswers = Array.ConvertAll(answers, item => Convert.ToInt32(item)).ToList();//answers.OfType<int>().ToList();
                Dictionary<int, List<int>> dict = Session["answers"] as Dictionary<int, List<int>>;
                if (dict != null)
                {
                    if (dict.ContainsKey(questId))
                        dict[questId] = intanswers;
                    else dict.Add(questId, intanswers);
                }
            }
        }

        public ActionResult CheckResult(int testId) {
            double percent;
            string[] result = SaveResult(testId, out percent);
            TestHistory history = new TestHistory();
            history.Answers = result[1];
            history.Questions = result[0];
            history.PassDate = DateTime.Now;
            history.SuccessPercent = percent;
            history.TestId = testId;
            //history.UserId = 
            if ((DateTime.Now - Convert.ToDateTime(Session["Time"])).TotalMinutes < 15)
            {
                //save (before send user)
                //db.TestHistory.Add(history);
                //db.SaveChanges();
            }
            else {
                //dont save, only show
            }
            //Dictionary<int, List<int>> diction = ParseResult(result[0], result[1]);
            return Result(history);
            //rename to check result and create+call show result
        }

        public ActionResult ShowResult(int historyId) {
            var history = (from hist in db.TestHistory
                          where hist.TestHistoryId == historyId //&& hist.UserId == 
                          select hist).First();
            return Result(history);
        }

        private ActionResult Result(TestHistory history) {
            var test = (from t in db.Test
                       where t.TestId == history.TestId
                       select t).First();
            ViewBag.Name = test.Name;
            Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
            if (Session["answers"] as Dictionary<int, List<int>> != null) {
                dict = Session["answers"] as Dictionary<int, List<int>>;
            }
            else {
                dict = ParseResult(history.Questions, history.Answers);
            }
            var questions = from q in db.Question.Include("Answer")
                            where q.TestId == test.TestId && dict.Keys.Any(x => x == q.QuestionId)
                            select q;
            ViewBag.Answers = dict;

            string textResult = "Тест не пройдено(" + history.SuccessPercent + "%)";
            bool success = false;
            if (history.SuccessPercent > 50)
            {
                textResult = "Тест успішно пройдено(" + history.SuccessPercent + "%)";
                success = true;
            }
            ViewBag.TextResult = textResult;
            ViewBag.Success = success;

            return View("ShowResult", questions.ToList());
        }

        private Dictionary<int, List<int>> ParseResult(string questions, string answers) {
            Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();
            string[] strKeys = questions.Split(' ');
            string[] strAnswers = answers.Split('\n');
            for (int i=0;i<strKeys.Length-1;i++) {
                string[] questAnswers = strAnswers[i].Split(' ');
                List<int> values = new List<int>();
                foreach (var el in questAnswers)
                {
                    if(!string.IsNullOrWhiteSpace(el))
                        values.Add(Convert.ToInt32(el));
                }
                result.Add(Convert.ToInt32(strKeys[i]), values);
            }
            return result;
        }

        private string[] SaveResult(int testId, out double percent) {
            string[] result = new string[2] { "", "" };
            percent = 0;
            List<int> questionsId = Session["questions"] as List<int>;
            Dictionary<int, List<int>> dict = Session["answers"] as Dictionary<int, List<int>>;

            var questions = from quest in db.Question
                            where quest.TestId == testId
                            && questionsId.Any(x => x == quest.QuestionId)
                            select quest;

            foreach (var el in questions) {

                List<int> rightAnswers = el.Answer.Where(x => x.Rightness == true)
                                           .Select(x => x.AnswerId).ToList();
                int answerCount = rightAnswers.Count;

                if (dict.ContainsKey(el.QuestionId))
                {
                    List<int> userAnswers = dict[el.QuestionId];

                    int wrongCount = rightAnswers.Except(userAnswers).Count();
                    int rightCount = userAnswers.Count() - userAnswers.Except(rightAnswers).Count();
                    int count = rightCount - wrongCount > 0 ? rightCount - wrongCount : 0;

                    percent += (count * 100) / answerCount;
                    result[1] += String.Join(" ", userAnswers);
                }
                else {
                    if(answerCount == 0) percent += 100;
                }
                result[0] += el.QuestionId + " ";
                result[1] += "\n";
            }
            percent = percent / questions.Count();
            return result;
        }

    }
}