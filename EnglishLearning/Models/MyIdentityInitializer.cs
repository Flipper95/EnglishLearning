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

            //var store = new RoleStore<IdentityRole>(context);
            //var manager = new RoleManager<IdentityRole>(store);
            //List<IdentityRole> identityRoles = new List<IdentityRole>();
            //identityRoles.Add(new IdentityRole() { Name = RoleTypes.Admin });
            //identityRoles.Add(new IdentityRole() { Name = RoleTypes.Secretary });
            //identityRoles.Add(new IdentityRole() { Name = RoleTypes.User });

            //foreach (IdentityRole role in identityRoles)
            //{
            //    manager.Create(role);
            //}

            //// Initialize default user
            //var store = new UserStore<ApplicationUser>(context);
            //var manager = new UserManager<ApplicationUser>(store);
            //ApplicationUser admin = new ApplicationUser();
            //admin.Email = "admin@admin.com";
            //admin.UserName = "admin@admin.com";

            //manager.Create(admin, "1Admin!");
            //manager.AddToRole(admin.Id, RoleTypes.Admin);

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

                var admin = new ApplicationUser { Email = "grim2009@ukr.net", UserName = "grim2009@ukr.net", EmailConfirmed=true };
                string password = "f0dF9_2nfH0";
                var result = userManager.Create(admin, password);
                //somehow admin confirm email
                //var code = userManager.GenerateEmailConfirmationToken(admin.Id);
                //userManager.ConfirmEmail(admin.Id, code);

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