using EnglishLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.ExtendClasses;
using Microsoft.AspNet.Identity;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class TestController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();

        private int userId = 0;
        int UserId {
            get {
                if (userId == 0) {
                    string userIdentity = User.Identity.GetUserId();
                    var temp = db.User.Where(x => x.IdentityId == userIdentity).
                                         Select(x => x.UserId).First();
                    userId = temp;
                }
                return userId;
            }
            set { userId = value; }
        }

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

            string userIdentity = User.Identity.GetUserId();
            var temp = db.User.Where(x => x.IdentityId == userIdentity).First();
            ViewBag.User = temp;
            return View(allGroups);
        }

        public ActionResult Test(int id) {
            //db.Question.Local
            Session["Time"] = DateTime.Now;
            var test = db.Test.Where(x => x.TestId == id).First();
            ViewBag.Name = test.Name;
            ViewBag.Time = test.Time;
            if (!string.IsNullOrWhiteSpace(test.Text)) {
                ViewBag.Text = test.Text;
            }
            if (!string.IsNullOrWhiteSpace(test.Voice)) {
                ViewBag.VoicePath = test.Voice;
            }

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

            if (Session["answers"] is Dictionary<int, List<int>> dict)
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
            //there was code of SaveAnswers[HttpPost] method
            SaveAnswers(answers, prevId);
            //save answer in session dictionary? question id: int[] answers
            return ShowAnswers(choosenId, nextId);
        }

        [HttpPost]
        public void SaveAnswers(object[] answers, int questId) {
            if (answers != null && answers.Length > 0)
            {
                List<int> intanswers = Array.ConvertAll(answers, item => Convert.ToInt32(item)).ToList();//answers.OfType<int>().ToList();
                if (Session["answers"] is Dictionary<int, List<int>> dict)
                {
                    if (dict.ContainsKey(questId))
                        dict[questId] = intanswers;
                    else dict.Add(questId, intanswers);
                }
            }
        }

        private TestHistory CreateHistory(int testId, double percent, string[] result) {
            TestHistory history = new TestHistory
            {
                Answers = result[1],
                Questions = result[0],
                PassDate = DateTime.Now,
                SuccessPercent = percent,
                TestId = testId,
                UserId = UserId
            };
            return history;
        }

        private void SaveHistory(int time, DateTime startTime, double percent, TestHistory history) {
            var minutes = (DateTime.Now - startTime).TotalMinutes;
            if (minutes < time && history.SuccessPercent >= percent)
            {
                db.TestHistory.Add(history);
                db.SaveChanges();
            }
        }

        private double GetMinSuccessPercent(string testName) {
            double minPercent = 50;
            if (testName == "Загальний рівень знань")
            {
                minPercent = 0;
            }
            return minPercent;
        }

        private void SaveUserLvl(Test test, double percent) {
            var user = (from u in db.User
                        where u.UserId == UserId
                        select u).First();
            string result = user.ObjectiveLevel;
            if (test.Name == "Загальний рівень знань" && !user.Tested) {
                if (percent <= 10) result = Enum.Format(typeof(Difficult), Difficult.Beginner, "G");
                else if (percent > 10 && percent <= 28) result = Enum.Format(typeof(Difficult), Difficult.Elementary, "G"); //nameof(Difficult.Elementary)
                else if (percent > 28 && percent <= 46) result = Enum.Format(typeof(Difficult), Difficult.Intermediate, "G");
                else if (percent > 46 && percent <= 64) result = Enum.Format(typeof(Difficult), Difficult.Upper_Intermediate, "G");
                else if (percent > 64 && percent <= 82) result = Enum.Format(typeof(Difficult), Difficult.Advanced, "G");
                else if (percent > 82 && percent <= 100) result = Enum.Format(typeof(Difficult), Difficult.Proficient, "G");
                user.Tested = true;
            }
            else {
                if (percent >= GetMinSuccessPercent(test.Name))
                {
                    Difficult userLvl = (Difficult)Enum.Parse(typeof(Difficult), user.ObjectiveLevel.Replace('-', '_'));
                    foreach (Difficult el in Enum.GetValues(typeof(Difficult))) {
                        string name = Enum.Format(typeof(Difficult), el, "G");
                        int value = (int)el;
                        if (test.Name == "Рівень " + name) {
                            if (value > (int)userLvl)
                                result = name;
                        }
                    }
                    //switch (test.Name)
                    //{
                    //    case ("Рівень Elementary"):
                    //        {
                    //            if (Difficult.Elementary > userLvl)
                    //                result = Enum.Format(typeof(Difficult), Difficult.Elementary, "G");// "Elementary";
                    //            break;
                    //        }
                    //}
                }
            }
            result = result.Replace('_', '-');
            TempData["LevelChanged"] = (result == user.ObjectiveLevel ? false : true);
            user.ObjectiveLevel = result;
            //db.SaveChanges();
            TempData["UserLevel"] = result;
        }

        private void SaveListeningLvl(Test test, double percent) {
            if (test.Name.StartsWith("Рівень сприйняття на слух: ") && percent >= GetMinSuccessPercent(test.Name)) {
                var user = (from u in db.User
                            where u.UserId == UserId
                            select u).First();
                string result = user.ObjLvlListening;
                Difficult userLvl = (Difficult)Enum.Parse(typeof(Difficult), user.ObjLvlListening.Replace('-', '_'));
                foreach (Difficult el in Enum.GetValues(typeof(Difficult))) {
                    string name = Enum.Format(typeof(Difficult), el, "G");
                    int value = (int)el;
                    if (test.Name == "Рівень сприйняття на слух: " + name) {
                        if (value > (int)userLvl)
                            result = name;
                    }
                }
                result = result.Replace('_', '-');
                TempData["LevelChanged"] = (result == user.ObjLvlListening ? false : true);
                user.ObjLvlListening = result;
                TempData["UserLevel"] = result;
            }
        }

        public ActionResult CheckResult(int testId) {
            string[] result = SaveResult(testId, out double percent);
            TestHistory history = CreateHistory(testId, percent, result);
            var test = (from t in db.Test.Include("TestGroup")
                       where t.TestId == testId
                       select t).First();
            //int time = test.TimeInMin;
            if(test.TestGroup.Name == "Рівень знань")
                SaveUserLvl(test, percent);
            if (test.TestGroup.Name == "Тести до озвучених історій")
                SaveListeningLvl(test, percent);
            SaveHistory(test.Time, Convert.ToDateTime(Session["Time"]), GetMinSuccessPercent(test.Name), history);//time
            TempData["history"] = history;
            return RedirectToAction("Result");//, history);
            //rename to check result and create+call show result
        }

        public ActionResult ShowResult(int id) {
            //string userIdentity = User.Identity.GetUserId();
            //var userId = (from user in db.User
            //              where user.IdentityId == userIdentity
            //              select user.UserId).First();
            var history = (from hist in db.TestHistory
                          where hist.TestHistoryId == id && hist.UserId == UserId
                          select hist).First();
            TempData["history"] = history;
            return Result();//history);
        }

        //[NonAction]
        //[HttpPost]
        public ActionResult Result(){//TestHistory history) {
            if (TempData["history"] != null)
            {
                TestHistory history = TempData["history"] as TestHistory;
                var test = (from t in db.Test
                            where t.TestId == history.TestId
                            select t).First();
                ViewBag.Name = test.Name;
                Dictionary<int, List<int>> dict = new Dictionary<int, List<int>>();
                if (Session["answers"] as Dictionary<int, List<int>> != null)
                {
                    dict = Session["answers"] as Dictionary<int, List<int>>;
                }
                else
                {
                    dict = ParseResult(history.Questions, history.Answers);
                }
                var questions = from q in db.Question.Include("Answer")
                                where q.TestId == test.TestId && dict.Keys.Any(x => x == q.QuestionId)
                                select q;
                ViewBag.Answers = dict;

                string level = null;
                if (TempData["UserLevel"] != null) {
                    if (TempData["LevelChanged"] != null)
                    {
                        level = Convert.ToBoolean(TempData["LevelChanged"]) == true ? "Рівень успішно підвищено! " : "Рівень залишився тим самим. ";
                    }
                    level += "Ваш рівень знань: "+TempData["UserLevel"].ToString();
                    ViewBag.Level = level;
                }
                string textResult = "Тест не пройдено(" + history.SuccessPercent + "%)";
                bool success = false;
                if (history.SuccessPercent >= GetMinSuccessPercent(test.Name))
                {
                    textResult = "Тест успішно пройдено(" + history.SuccessPercent + "%)";
                    success = true;
                }
                ViewBag.TextResult = textResult;
                ViewBag.Success = success;
                //ViewBag.LevelChange, ViewBag.UserLevel
                SessionClear();
                return View("ShowResult", questions.ToList());
            }
            else {
                SessionClear();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
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

                    //var tempArr = rightAnswers.Except(userAnswers);
                    int wrongCount = userAnswers.Except(rightAnswers).Count();
                    int rightCount = userAnswers.Count() - wrongCount;//userAnswers.Except(rightAnswers).Count();
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

        public ActionResult GetAudioFile(int id) {
            var voicePath = db.Test.Where(x => x.TestId == id).Select(x => x.Voice).First();
            if (!string.IsNullOrWhiteSpace(voicePath)) {
                return File(Server.MapPath(voicePath), "audio/mp3");
            }
            else return null;
        }

        private void SessionClear() {
            Session.Remove("Time");
            Session.Remove("answers");
            Session.Remove("questions");
        }

    }
}