using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace EnglishLearning.Models
{
    public class MyIdentityInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context) {

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "moderator" };
            var role3 = new IdentityRole { Name = "teacher" };
            var role4 = new IdentityRole { Name = "user" };

            if (roleManager.FindByName(role1.Name) == null && roleManager.FindByName(role2.Name) == null
                && roleManager.FindByName(role3.Name) == null && roleManager.FindByName(role4.Name) == null)
            {
                roleManager.Create(role1);
                roleManager.Create(role2);
                roleManager.Create(role3);
                roleManager.Create(role4);

                var adminFile = System.IO.File.ReadLines(HttpContext.Current.Server.MapPath("~/App_Data/admin.txt"));
                var adminEmail = adminFile.ElementAt(0);
                var adminPassword = adminFile.ElementAt(1);

                var admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail, EmailConfirmed=true };
                string password = adminPassword;
                var result = userManager.Create(admin, password);

                if (result.Succeeded)
                {
                    userManager.AddToRole(admin.Id, role1.Name);
                    userManager.AddToRole(admin.Id, role2.Name);
                    userManager.AddToRole(admin.Id, role3.Name);
                    userManager.AddToRole(admin.Id, role4.Name);
                }
            }

            base.Seed(context);
        }
    }
}