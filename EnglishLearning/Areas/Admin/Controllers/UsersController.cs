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
            //IList<string> roles = new List<string>();
            //ApplicationUserManager userManager = HttpContext.GetOwinContext()
            //                                .GetUserManager<ApplicationUserManager>();
            //var context = new ApplicationDbContext();

            var query = from users in context.Users
                        select users;
            //ApplicationUser user = userManager.FindByEmail(User.Identity.Name);
            //if (user != null)
            //    roles = userManager.GetRoles(user.Id);
            //ViewBag.Roles = GetRoles().ToList();
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
                //ViewBag.Roles = GetRoles().ToList();
                return View(query);
            }
        }

        [HttpPost]
        public ActionResult UserRole(string RoleEmail, string submit) {
            ViewBag.UserRoleModified = true;
            ViewBag.UserModified = RoleEmail;
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
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

        //В данному функціоналі немає сенсу, нові ролі не впливають на доступ (можливо застосовувати IAuthorizationFilter)
        //[HttpPost]
        //public ActionResult RoleEdit(string Role, string submit)
        //{
        //    ViewBag.RoleModified = true;
        //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        //    if (submit == "add")
        //    {
        //        IdentityRole newRole = new IdentityRole(Role);
        //        if (!roleManager.RoleExists(Role))
        //        {
        //            roleManager.Create(newRole);
        //        }
        //    }
        //    if (submit == "delete")
        //    {
        //        var query = from roles in context.Roles
        //                    where roles.Name == Role
        //                    select roles;
        //        if (roleManager.RoleExists(Role))
        //        {
        //            roleManager.Delete(query.First());
        //        }
        //    }
        //    var query1 = from users in context.Users
        //                 select users;
        //    return View("Index", query1);
        //}

        private IQueryable<IdentityRole> GetRoles() {
            var query1 = from roles in context.Roles
                         select roles;
            return query1;
        }
    }
}