using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Controllers
{
    public class ExerciseController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Exercise
        public ActionResult Index()
        {
            return View();
        }

        private int getCurrentUserId() {
            string userIdentity = User.Identity.GetUserId();
            var userId = (from user in db.User
                          where user.IdentityId == userIdentity
                          select user.UserId).First();
            return userId;
        }

        public ActionResult TranslateExercise() {
            int userId = getCurrentUserId();
            //"SELECT * FROM [Word] AS w WHERE w.WordId IN (SELECT TOP 25 lw.WordId FROM [LearningWord] AS lw WHERE (lw.LearnPercent < 100) AND lw.UserId = @user ORDER BY newid())";
            var query = (from learningWord in db.LearningWord
                         where learningWord.UserId == userId && learningWord.LearnPercent < 100
                         select learningWord);
            int total = query.Count();
            Random rnd = new Random();
            List<ExerciseModel> words = new List<ExerciseModel>();
            for (int i = 0; i < 25; i++) {
                words.Add(new ExerciseModel());
                words[i].lWord = query.ElementAt(rnd.Next(0, total));
                words[i].word = (from word in db.Word
                                 where word.WordId == words[i].lWord.WordId
                                 select word).First();
            }
            
            //.OrderBy(x => Guid.NewGuid()).Take(25)
            return View(words);
        }
    }
}