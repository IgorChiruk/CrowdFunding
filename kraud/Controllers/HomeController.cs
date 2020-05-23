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
                    ApplicationUser admin = new ApplicationUser() { UserName = "Admin" };
                    UserManager.Create(admin, "Admin");
                    UserManager.AddToRole(admin.Id, "admin");
                }
                return View();
            }          
        }

        [Authorize(Roles = "admin")]
        public ActionResult Users()
        {    
            return View();
        }


        public JsonResult GetUserName()
        {
            var identity =Thread.CurrentPrincipal.Identity;
            if (identity.IsAuthenticated)
            {
                return Json(identity.Name);
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
                };

                IdentityResult result =await UserManager.CreateAsync(newuser, loginModel.Password);
                //UserManager.AddToRole(admin.Id, "admin");
                if (result.Succeeded)
                {
                    //UserManager.AddToRole(newuser.Id, "user");
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