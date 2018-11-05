using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    public class GrammarController : Controller
    {

        private int userId = 0;
        int UserId
        {
            get
            {
                if (userId == 0)
                {
                    string userIdentity = User.Identity.GetUserId();
                    var temp = db.User.Where(x => x.IdentityId == userIdentity).
                                         Select(x => x.UserId).First();
                    userId = temp;
                }
                return userId;
            }
            set { userId = value; }
        }

        EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Grammar
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listening(int number = 5, string name = "") {
            if (Session["index"] == null)
            {
                var grammar = GetAllGrammar(name);
                grammar = grammar.Where(x => x.Voice != null && x.Voice.Length > 1).Take(number);
                var result = grammar.ToList();
                Session["questions"] = result;
                Session["index"] = 0;
                Session["number"] = number;
                Session["name"] = name;
                Session["AnswerCount"] = 0;
                ViewBag.Voice = base.File(result[0].Voice, "audio/wav");
                return View(result[0]);
            }
            else {
                int index = Convert.ToInt32(Session["index"]) + 1;
                int endNumber = Convert.ToInt32(Session["number"]);
                if (index > endNumber)
                {
                    string temp = Session["name"].ToString();
                    if (temp != "") {
                        var grpName = (from g in db.GrammarGroup
                                      where g.Name == temp
                                      select (from g2 in db.GrammarGroup
                                              where g2.GroupId == g.ParentId
                                              select g2.Name).First()).First();
                        if (grpName == "Рівень знань") {
                            double percent = Convert.ToDouble(Session["AnswerCount"]) * 100 / endNumber;
                            SaveUserLvl(temp, percent);
                        }
                    }
                    return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = endNumber });
                }
                else
                {
                    var result = (Session["questions"] as List<Grammar>)[index];
                    ViewBag.Voice = base.File(result.Voice, "audio/wav");
                    return View(result);
                }
            }
        }

        private IQueryable<Grammar> GetAllGrammar(string name = "") {
            var grammar = from g in db.Grammar.Include("GrammarGroup")
                          select g;
            if (!string.IsNullOrWhiteSpace(name))
            {
                grammar = grammar.Where(x => x.GrammarGroup.Name == name);
            }
            grammar = grammar.OrderBy(x => Guid.NewGuid());
            return grammar;
        }

        public bool CheckResult(string value)
        {
            var grammar = (Session["questions"] as List<Grammar>)[Convert.ToInt32(Session["index"])];
            bool result = grammar.Text == value ? true : false;
            if (result)
            {
                Session["AnswerCount"] = Convert.ToInt32(Session["AnswerCount"]) + 1;
            }
            return result;
        }

        public string GetAnswer()
        {
            var grammar = (Session["questions"] as List<Grammar>)[Convert.ToInt32(Session["index"])];
            return grammar.Text;
        }

        private void SaveUserLvl(string grpName, double percent)
        {
            var user = (from u in db.User
                        where u.UserId == UserId
                        select u).First();
            string result = user.ObjectiveLevel;
            if (test.Name == "Загальний рівень знань" && !user.Tested)
            {
                if (percent <= 10) result = Enum.Format(typeof(Difficult), Difficult.Beginner, "G");
                else if (percent > 10 && percent <= 28) result = Enum.Format(typeof(Difficult), Difficult.Elementary, "G"); //nameof(Difficult.Elementary)
                else if (percent > 28 && percent <= 46) result = Enum.Format(typeof(Difficult), Difficult.Intermediate, "G");
                else if (percent > 46 && percent <= 64) result = Enum.Format(typeof(Difficult), Difficult.Upper_Intermediate, "G");
                else if (percent > 64 && percent <= 82) result = Enum.Format(typeof(Difficult), Difficult.Advanced, "G");
                else if (percent > 82 && percent <= 100) result = Enum.Format(typeof(Difficult), Difficult.Proficient, "G");
                user.Tested = true;
            }
            else
            {
                if (percent >= GetMinSuccessPercent(test.Name))
                {
                    Difficult userLvl = (Difficult)Enum.Parse(typeof(Difficult), user.ObjectiveLevel.Replace('-', '_'));
                    foreach (Difficult el in Enum.GetValues(typeof(Difficult)))
                    {
                        string name = Enum.Format(typeof(Difficult), el, "G");
                        int value = (int)el;
                        if (test.Name == "Рівень " + name)
                        {
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

    }
}