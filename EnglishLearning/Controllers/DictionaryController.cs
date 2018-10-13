﻿using System;
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

        //TO DO: решить стоит ли вообще использовать такой код, в плане перехода на другую страницу, если да, сменить параметры с nullable на обычные, расскоментировать
        //<option data-url="@Url.Action("RowsPerPage", new { area = "", id = ViewBag.GroupId,page = Model.CurrentPage, prevPageSize = Model.PageSize, totalPages = Model.TotalPages, pageSize = 10 })" value="10">10</option>
        //TO DO: осторожно хрупкий код, даже не переставлять строки totalPages-1 может быть 0 (когда на первой странице с 100 записями переход на 10 записей)
        public ActionResult RowsPerPage(int id, int? prevPageSize, int? totalPages, int page = 1, int pageSize = 25) {
                //if (prevPageSize != pageSize)
                //{
                //    page = (int)Math.Ceiling(page * ((double)prevPageSize / (double)pageSize));
                //    int newTotalPages = (int)Math.Ceiling((prevPageSize * (totalPages-1)) / (double)pageSize);
                //    if (page > newTotalPages) page = newTotalPages;
                //    if (page <= 0) page = 1;
                //}
            return RedirectToAction("Words", new { id, page, pageSize });
        }

        public ActionResult Search(int id, int pageSize, string search, int page = 1) {
            return RedirectToAction("Words", new { id, page, pageSize,search });
        }

        //[HttpPost]
        //public ActionResult SearchAndRows(int GroupId, int rowsperpage, string searchData, int page = 1) {
        //    return RedirectToAction("Words", new { id = GroupId, page, pageSize = rowsperpage, search = searchData });
        //}

        public ActionResult Words(int id, int page = 1, int pageSize = 25, string search="")
        {
            string userIdentity = User.Identity.GetUserId();
            var user = (from users in db.User where users.IdentityId == userIdentity select users).First();

            var query = (from word in db.Word
                         where word.GroupId == id
                          let learn = ((from learning in db.LearningWord
                                       where learning.WordId == word.WordId && learning.UserId == user.UserId
                                       select true).FirstOrDefault())
                          orderby word.WordId
                          select new WordsDisplay { inLearning = learn == true ? true : false, word = word.Word1, translate = word.Translate, wordId = word.WordId });
            if (!string.IsNullOrWhiteSpace(search)) {
                ViewBag.SearchData = search;
                query = query.Where(x => x.word.Contains(search) || x.translate.Contains(search));
            }
            ViewBag.GroupId = id;
            PagedList<WordsDisplay> list = new PagedList<WordsDisplay>();
            list.CurrentPage = page;
            list.PageSize = pageSize;
            list.TotalRecords = query.Count();
            list.Content = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            return View(list);
        }

        public int EditWord(int id, bool check) {
            LearningWord lw = new LearningWord();
            if (check) {
                int userId = GetUser().UserId;
                lw = (from learningWord in db.LearningWord
                      where learningWord.UserId == userId && learningWord.WordId == id
                      select learningWord).First();
                db.LearningWord.Remove(lw);
                db.SaveChanges();
            }
            else
            {
                lw.AddedDate = DateTime.Now;
                lw.WordId = id;
                lw.LearnPercent = 0;
                lw.UserId = GetUser().UserId;
                db.LearningWord.Add(lw);
                db.SaveChanges();
            }
            return lw.LearningWordId;
        }

        private User GetUser() {
            string userIdentity = User.Identity.GetUserId();
            var user = (from users in db.User where users.IdentityId == userIdentity select users).First();
            return user;
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