using EnglishLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Newtonsoft.Json;

namespace EnglishLearning.Controllers
{
    public class StatisticsController : Controller
    {
        EnglishLearningEntities db = new EnglishLearningEntities();

        private int userId = 0;
        int UserId
        {
            //get {
            //    if (Session["userId"] == null) {
            //        string userIdentity = User.Identity.GetUserId();
            //        var temp = db.User.Where(x => x.IdentityId == userIdentity)
            //                          .Select(x => x.UserId).First();
            //        Session["userId"] = temp;
            //    }
            //    return Convert.ToInt32(Session["userId"]);
            //}
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

        // GET: Statistics
        public ActionResult Index()
        {
            var model = (from history in db.TestHistory.Include("Test")
                        where history.UserId == UserId
                        orderby DbFunctions.TruncateTime(history.PassDate) descending
                        select history).ToList();
            return View(model);
        }

        //public JsonResult ChartByCount(string xName, string colName) {
        //    DateTime begin = DateTime.Now.Date.AddDays(-1);
        //    DateTime end = DateTime.Now.Date.AddDays(1);
        //    var query2 = (from history in db.TestHistory
        //                  where history.UserId == UserId && history.PassDate > begin && history.PassDate < end
        //                  group history by DbFunctions.TruncateTime(history.PassDate) into g //need this?
        //                  select new CountDateStat { date = g.Key, count = g.Count() }).ToList();
        //    return CountResult(query2, xName, colName);
        //}

        //public JsonResult CountResult(List<CountDateStat> data, string xName, string colName) {
        //    var barChart = new BarChart();
        //    barChart.cols = new object[] { new { id="date", type="string", label = xName }, new { id = "Yvalue", type = "number", label = colName } };
        //    barChart.rows = new object[data.Count];
        //    for (int i=0; i<data.Count;i++) {
        //        barChart.rows[i] = new { c = new object[] { new { v = data[i].date.Value.ToShortDateString() }, new { v = data[i].count } } };
        //    }
        //    return this.Json(barChart, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult ChartByCount(string xName, string colName, string col2Name, string from, string to)
        {
            DateTime begin, end;
            ConvertToData(from, to, out begin, out end);

            var tests = (from t in db.TestHistory
                        where t.UserId == UserId && t.PassDate > begin && t.PassDate < end
                        select new { IsTest = true, date = t.PassDate }).ToList();
            var parse = tests.Select(x => new { IsTest = x.IsTest, date = x.date as DateTime? });

            var words = (from w in db.LearningWord
                        where w.UserId == UserId && w.LearnPercent == 100 && w.LearnedDate > begin && w.LearnedDate < end
                        select new { IsTest = false, date = w.LearnedDate }).ToList();

            var result = (from x in words.Concat(parse)
                         group x by x.date.Value.Date into og
                         select new CountDateStat { Date = og.Key, CountT = og.Count(o => o.IsTest), CountW = og.Count(c => !c.IsTest) }).ToList();

            return CountResult(result, xName, colName, col2Name);
        }

        [NonAction]
        public JsonResult CountResult(List<CountDateStat> data, string xName, string colName, string col2Name)
        {
            var barChart = new BarChart();
            barChart.cols = new object[] { new { id = "date", type = "string", label = xName }, new { id = "Yvalue", type = "number", label = colName }, new { id = "Y2value", type = "number", label = col2Name } };
            barChart.rows = new object[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                barChart.rows[i] = new { c = new object[] { new { v = data[i].Date.Value.ToShortDateString() }, new { v = data[i].CountT }, new { v = data[i].CountW } } };
            }
            return this.Json(barChart, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChartByTestSuccess(string xName, string colName) {
            var tests = (from test in db.TestHistory.Include("Test")
                        where test.UserId == UserId
                        group test by test.Test.Name into go
                        select new TestSuccessStat { Name = go.Key, Percent = go.Max(x => x.SuccessPercent) }).ToList();
            return TestSuccessResult(tests, xName, colName);
        }

        [NonAction]
        public JsonResult TestSuccessResult(List<TestSuccessStat> data, string xName, string colName) {
            var barChart = new BarChart();
            barChart.cols = new object[] { new { id = "data", type = "string", label = xName }, new { id = "Yvalue", type = "number", label = colName } };
            barChart.rows = new object[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                barChart.rows[i] = new { c = new object[] { new { v = data[i].Name }, new { v = data[i].Percent } } };
            }
            return this.Json(barChart, JsonRequestBehavior.AllowGet);
        }

        private void ConvertToData(string from, string to, out DateTime begin, out DateTime end) {
            if (string.IsNullOrWhiteSpace(from))
                begin = DateTime.Now.Date.AddDays(-1);
            else begin = Convert.ToDateTime(from);

            if (string.IsNullOrWhiteSpace(to))
                end = DateTime.Now.Date.AddDays(1);
            else end = Convert.ToDateTime(to);

            if (begin > end)
            {
                var swap = begin;
                begin = end;
                end = swap;
            }
        }

    }

    //public class CountDateStat {
    //    public DateTime? date;
    //    public int count;
    //}

    public class CountDateStat
    {
        public DateTime? Date;
        public int CountW;
        public int CountT;
    }

    public class TestSuccessStat
    {
        public string Name;
        public double Percent;
    }

    public class BarChart
    {
        public object[] cols { get; set; }
        public object[] rows { get; set; }
    }
}