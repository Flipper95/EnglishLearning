﻿using EnglishLearning.ExtendClasses;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    [Authorize]
    public class GrammarController : Controller
    {

        EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Grammar
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Constructor(int startIndex = 0, int number = 5, string name = "")
        {
                ViewBag.Index = 1;
            int index = 0;
            if (Session["index"] == null || startIndex == 0)
            {
                var grammar = GetAllGrammar(name);
                var count = grammar.Count();
                if (count >= number) grammar = grammar.Take(number);
                else grammar = grammar.Take(count);
                Session["questions"] = grammar.ToList();//result;
                Session["index"] = 0;
                Session["number"] = count >= number ? number : count;
                Session["AnswerCount"] = 0;
            }
            else {
                index = Convert.ToInt32(Session["index"]) + 1;
                Session["index"] = index;
                int endNumber = Convert.ToInt32(Session["number"]);
                if (index >= endNumber)
                {
                    return RedirectToAction("ShowResult", "Exercise", new { count = Session["AnswerCount"], max = endNumber });
                }
            }
            var gr = (Session["questions"] as List<Grammar>)[index];
            ViewBag.Translate = gr.Translate;
            ViewBag.SentenceIndex = index;
            return View();
        }

        public ActionResult ConstructorBlock(int sentenceIndex, int wordIndex){
            ViewBag.WordIndex = wordIndex;
            var sentences = Session["questions"] as List<Grammar>;
            List<string> model = new List<string>();
            if (sentences[sentenceIndex].Text.Split(' ').Count() > wordIndex)
            {
                model = GetNextConstructorWords(sentences, sentenceIndex, wordIndex);
                model = Shuffle.ShuffleList(model);
            }
            return PartialView(model);
        }

        private List<string> GetNextConstructorWords(List<Grammar> sentences, int sentenceIndex, int wordIndex ) {
            var temp = ClearFromSymbols(sentences[sentenceIndex].Text).Split(' ');
            var answer = temp[wordIndex];
            List<string> words = new List<string>
            {
                answer
            };
            var consonant = DoubleConsonant(answer);
            if (consonant != answer) words.Add(consonant);
            if (wordIndex > 0) words.Add(temp[wordIndex - 1]);
            words = words.Concat(GetRandomWords(sentences, sentenceIndex)).ToList();
            return words;
        }

        private List<string> GetRandomWords(List<Grammar> sentences, int exceptIndex) {
            Random rnd = new Random();
            List<string> result = new List<string>();
            for (int i = 0; i < sentences.Count; i++)
            {
                if (i != exceptIndex)
                {
                    var temp = ClearFromSymbols(sentences[i].Text).Split(' ');
                    int index = rnd.Next(0, temp.Count() - 1);
                    result.Add(temp[index]);
                    if (temp.Count() > index + 1) result.Add(temp[index + 1]);
                    if (index > 0) result.Add(temp[index - 1]);
                    var rndWord = DoubleConsonant(temp[index]);
                    if (rndWord != temp[index]) result.Add(rndWord);
                }
            }
            return result;
        }

        private string DoubleConsonant(string value) {
            //index of any
            var index = Regex.Match(value, @"%m|n|p%", RegexOptions.RightToLeft).Index;
            var result = value;
            if (index != 0)
            {
                result = value.Substring(0, index+1);
                result += value.Substring(index, value.Length - index);
            }
            return result;
        }

        private char[] SwapRandomChars(char[] value) {
            Random rnd = new Random();
            int i = rnd.Next(0, value.Length);
            int j = rnd.Next(0, value.Length);
            var temp = value[i];
            value[i] = value[j];
            value[j] = temp;
            return value;
        }

        public ActionResult Listening(int startIndex = 0, int number = 5, string name = "") {
            if (Session["index"] == null || startIndex == 0 )
            {
                var grammar = GetAllGrammar(name);
                grammar = grammar.Where(x => x.Voice != null);
                var count = grammar.Count();
                if (count >= number) grammar = grammar.Take(number);
                else grammar = grammar.Take(count);
                var result = grammar.ToList();
                Session["questions"] = result;
                ViewBag.Index = 1;
                Session["index"] = 0;
                Session["number"] = count >= number ? number : count;
                Session["name"] = name;
                Session["AnswerCount"] = 0;
                return View(result[0]);
            }
            else {
                ViewBag.Index = 1;
                ViewBag.Random = Guid.NewGuid();
                int index = Convert.ToInt32(Session["index"]) + 1;
                Session["index"] = index;
                int endNumber = Convert.ToInt32(Session["number"]);
                if (index >= endNumber)
                {
                    string temp = Session["name"].ToString();
                    if (temp != "") {
                        var grpName = (from g in db.GrammarGroup
                                      where g.Name == temp
                                      select (from g2 in db.GrammarGroup
                                              where g2.GroupId == g.ParentId
                                              select g2.Name).FirstOrDefault()).FirstOrDefault();
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
            var text = grammar.Text.ToLower();
            text = ClearFromSymbols(text);
            value = ClearFromSymbols(value.ToLower());
            value = WebUtility.HtmlDecode(value);
            value = value.Replace("\u00A0", " ");
            bool result = text == value ? true : false;
            if (result)
            {
                Session["AnswerCount"] = Convert.ToInt32(Session["AnswerCount"]) + 1;
            }
            return result;
        }

        private string ClearFromSymbols(string value) {
            var text = Regex.Replace(value, @"[^0-9a-zA-Zа-яА-ЯїієІЇЄ\s'-]+", "");
            return text;
        }

        public string GetAnswer()
        {
            var grammar = (Session["questions"] as List<Grammar>)[Convert.ToInt32(Session["index"])];
            return grammar.Text;
        }

        private void SaveUserLvl(string grpName, double percent)
        {
            string userIdentity = User.Identity.GetUserId();
            var user = (from u in db.User
                        where u.IdentityId == userIdentity
                        select u).First();
            string result = user.ObjLvlListening;
                if (percent >= 80)
                {
                    Difficult userLvl = (Difficult)Enum.Parse(typeof(Difficult), user.ObjLvlListening.Replace('-', '_'));
                    foreach (Difficult el in Enum.GetValues(typeof(Difficult)))
                    {
                        string name = el.ToString();
                        int value = (int)el;
                        if (grpName == "Рівень " + name)
                        {
                            if (value > (int)userLvl)
                                result = name;
                        }
                    }
                }
            result = result.Replace('_', '-');
            user.ObjLvlListening = result;
            db.SaveChanges();
        }

        public FileContentResult LoadAudio(string NoCache)
        {
            var result = (Session["questions"] as List<Grammar>)[Convert.ToInt32(Session["index"])];
            if (result.Voice.Length > 1)
            {
                return base.File(result.Voice, "audio/wav");
            }
            else return base.File(new byte[1], "audio/wav");
        }

    }
}