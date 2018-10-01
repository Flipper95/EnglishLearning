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

        public ActionResult Words(int id) {
            string userIdentity = User.Identity.GetUserId();
            var user = (from users in db.User where users.IdentityId == userIdentity select users).First();

            int page = 1;
            int pageSize = 100;

            //var query = (from word in db.Word join learning in db.LearningWord on word.WordId equals learning.WordId
            //             where learning.UserId == user.UserId && word.GroupId == id
            //             orderby word.WordId
            //             select new { word, learning}).Skip((page - 1) * pageSize).Take(pageSize);
            var query2 = (from word in db.Word
                          join learning in db.LearningWord on word.WordId equals learning.WordId into WordGroup
                          from item in WordGroup.DefaultIfEmpty()
                          where word.GroupId == id && (item.UserId == user.UserId || item == null)
                          orderby word.WordId
                          select new { InLearning = item == null ? false : true, word = word.Word1, translate = word.Translate, wordId = word.WordId }).Skip((page - 1) * pageSize).Take(pageSize);//.Select(c=>c.ToExpando());//left outer join
            //List<object[]> list = new List<object[]>{ new object[] { query2 } };
            //List<dynamic> list = new List<dynamic>();
            //foreach (dynamic el in query2) {
            //    list.Add(el.ToExpando());
            //}
            //ViewBag.Words = query2;
            var list = query2.AsEnumerable().Select(x => x.ToExpando()).ToList();// { dynamic d = new ExpandoObject(); d.InLearning = x.InLearning; d.word = x.word; d.translate = x.translate; return d; }).ToList();
            return View(list);
        }

    }

    public static class Extensions
    {
        public static ExpandoObject ToExpando(this object anonymousObject)
        {
            IDictionary<string, object> anonymousDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(anonymousObject);
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var item in anonymousDictionary)
                expando.Add(item);
            return (ExpandoObject)expando;
        }
    }
}