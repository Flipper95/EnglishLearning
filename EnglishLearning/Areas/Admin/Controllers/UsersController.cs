using EnglishLearning.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {

        ApplicationDbContext context = new ApplicationDbContext();

        public UsersController() {
            ViewBag.Roles = GetRoles().ToList();
        }

        // GET: Admin/Users
        public ActionResult Index()
        {
            var query = from users in context.Users
                        select users;
            return View(query);
        }

        [HttpPost]
        public ActionResult Index(string SearchEmail) {
            if (string.IsNullOrEmpty(SearchEmail))
            {
                return Index();
            }
            else
            {
                var query = from users in context.Users
                            where users.Email.Contains(SearchEmail)
                            select users;
                return View(query);
            }
        }

        [HttpPost]
        public ActionResult UserRole(string RoleEmail, string submit) {
            ViewBag.UserRoleModified = true;
            ViewBag.UserModified = RoleEmail;
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var user = from users in context.Users
                       where users.Email == RoleEmail
                       select users;
            if (submit == "add") {
                userManager.AddToRole(user.First().Id, Request.Form["UserRoleEdit"].ToString());
            }
            if (submit == "delete") {
                userManager.RemoveFromRole(user.First().Id, Request.Form["UserRoleEdit"].ToString());
            }
            var query = from users in context.Users
                        select users;
            return View("Index", query);
        }

        private IQueryable<IdentityRole> GetRoles() {
            var query1 = from roles in context.Roles
                         select roles;
            return query1;
        }
    }
}