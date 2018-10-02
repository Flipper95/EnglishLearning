using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EnglishLearning.Models;
using Microsoft.AspNet.Identity;

namespace EnglishLearning.Controllers
{
    public class DictionaryController : Controller
    {

        EnglishLearningEntities db = new EnglishLearningEntities();

        // GET: Dictionary
        public ActionResult Index()
        {
            var query = from g in db.Group
                        where g.OwnerId == 1
                        select g;
            return View(query);
        }

        public ActionResult Words(int id, int page = 1, int pageSize = 100) {
            string userIdentity = User.Identity.GetUserId();
            var user = (from users in db.User where users.IdentityId == userIdentity select users).First();

            var query = (from word in db.Word
                          join learning in db.LearningWord on word.WordId equals learning.WordId into WordGroup
                          from item in WordGroup.DefaultIfEmpty()
                          where word.GroupId == id && (item.UserId == user.UserId || item == null)
                          orderby word.WordId
                          select new WordsDisplay { inLearning = item == null ? false : true, word = word.Word1, translate = word.Translate, wordId = word.WordId });//.Select(c=>c.ToExpando());//left outer join
            //var list = query2.AsEnumerable().Select(x => x.ToExpando()).ToList();// { dynamic d = new ExpandoObject(); d.InLearning = x.InLearning; d.word = x.word; d.translate = x.translate; return d; }).ToList();
            //Word w = new Word();
            ViewBag.GroupId = id;
            PagedList<WordsDisplay> list = new PagedList<WordsDisplay>();
            list.CurrentPage = page;
            list.PageSize = pageSize;
            list.TotalRecords = query.Count();
            list.Content = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            return View(list);
        }

        public int EditWord(int id, bool check) {
            string value = "";
            value += "1";
            return id;
        }

    }

    public class PagedList<T>
    {
        public List<T> Content { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalRecords / PageSize); }
        }
    }

    public class WordsDisplay
    {
        public int wordId;
        public string word;
        public string translate;
        public bool inLearning;
    }

}