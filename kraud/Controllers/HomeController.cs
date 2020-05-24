using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using kraud.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;


namespace kraud.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {        
            using(ApplicationContext db = new ApplicationContext())
            {
                if (!db.Roles.Any())
                {
                    ApplicationRole adminrole = new ApplicationRole() { Name="admin" };
                    ApplicationRole userrole = new ApplicationRole() { Name = "user" };
                    RoleManager.Create(adminrole);
                    RoleManager.Create(userrole);
                }
                if (!db.Users.Any())
                {
                    ApplicationUser admin = new ApplicationUser() { UserName = "Admin", IsAdmin=true };
                    UserManager.Create(admin, "Admin");
                    UserManager.AddToRole(admin.Id, "admin");
                }
                return View();
            }          
        }

        public ActionResult MainPage()
        {
            return PartialView();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Users()
        {
            var userlist = UserManager.Users.ToList();
            return View(userlist);
        }


        public JsonResult GetUserName()
        {
            var identity =Thread.CurrentPrincipal.Identity;
            if (identity.IsAuthenticated)
            {
                Dictionary<string, string> user = new Dictionary<string, string>();
                user.Add("UserName", identity.Name);
                var IsAdmin = UserManager.Users.Where(x => x.UserName == identity.Name).FirstOrDefault().IsAdmin;
                user.Add("IsAdmin", IsAdmin.ToString());
                return Json(user);
            }
            else { return Json(false); }
        }

        public async Task<JsonResult> Login(LoginModel loginModel)
        {
            ApplicationUser user = await UserManager.FindAsync(loginModel.Login, loginModel.Password);
            if (user == null)
            {
                return Json(false);
            }
            else
            {
                ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignOut();
                AuthenticationManager.SignIn(new AuthenticationProperties
                {
                    IsPersistent = true
                }, claim);
                return Json(user.UserName);
            }
                
        }

        public async Task<JsonResult> Register(LoginModel loginModel)
        {
            ApplicationUser user = await UserManager.FindByNameAsync(loginModel.Login);
            if (user == null)
            {
                ApplicationUser newuser = new ApplicationUser
                {
                    UserName = loginModel.Login,
                    IsAdmin = false
                };

                IdentityResult result =await UserManager.CreateAsync(newuser, loginModel.Password);           
                if (result.Succeeded)
                {
                    UserManager.AddToRole(newuser.Id, "user");
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(newuser,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    return Json(true);
                }
                else {return Json(false);}                         
            }
            else
            {
                return Json(false);
            }
        }

        [Authorize(Roles = "admin")]
        public async Task<JsonResult> DeleteUser(string Id)
        {
            var user = await UserManager.FindByIdAsync(Id);
            var result =await UserManager.DeleteAsync(user);
            if (result.Succeeded) { return Json(true); }
            else { return Json(false); }          
        }

        [Authorize(Roles = "admin")]
        public JsonResult AdminChange(string Id)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                var user = context.Users.Where(x => x.Id == Id).FirstOrDefault();
                if (user.IsAdmin) { UserManager.RemoveFromRole(Id, "Admin"); user.IsAdmin = false;  }
                else if(!user.IsAdmin) { UserManager.AddToRole(Id, "Admin"); user.IsAdmin = true; }
                context.SaveChanges();
                return Json(true);
            }
            
        }

        public JsonResult LogOut()
        {
            AuthenticationManager.SignOut();
            return Json(true);
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        private ApplicationRoleManager RoleManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
    }
}