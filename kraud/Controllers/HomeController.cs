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
    [RequireHttps]
    public class HomeController : Controller
    {
        private const string XsrfKey = "XsrfId";
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
                    IdentityResult result =UserManager.Create(admin, "Admin");
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

        public ActionResult GoogleLogin(string returnUrl)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleLoginCallback",
                    new { returnUrl = returnUrl })
            };

            HttpContext.GetOwinContext().Authentication.Challenge(properties, "Google");
            return new HttpUnauthorizedResult();
        }

        [AllowAnonymous]
        public async Task<ActionResult> GoogleLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            ApplicationUser user = await UserManager.FindAsync(loginInfo.Login);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.DefaultUserName,
                };

                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
                else
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
            }

            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                DefaultAuthenticationTypes.ApplicationCookie);

            ident.AddClaims(loginInfo.ExternalIdentity.Claims);

            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, ident);

            return Redirect(returnUrl ?? "/");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Запрос перенаправления к внешнему поставщику входа
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Home", new { ReturnUrl = returnUrl }));
        }


        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            ApplicationUser user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.DefaultUserName,
                };

                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return View("Error", result.Errors);
                }
                else
                {
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        return View("Error", result.Errors);
                    }
                }
            }

            ClaimsIdentity ident = await UserManager.CreateIdentityAsync(user,
                DefaultAuthenticationTypes.ApplicationCookie);

            ident.AddClaims(loginInfo.ExternalIdentity.Claims);

            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = false
            }, ident);

            return Redirect(returnUrl ?? "/");
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
            var identity = Thread.CurrentPrincipal.Identity;
            var thisuser = UserManager.FindByName(identity.Name);
            var isadmin =await UserManager.IsInRoleAsync(thisuser.Id, "admin");    
            if (!isadmin) { return Json(false); }
            else {
                if (thisuser.Id == Id) { return Json(false); }
                else
                {
                    var user = await UserManager.FindByIdAsync(Id);
                    var result = await UserManager.DeleteAsync(user);
                    if (result.Succeeded) { return Json(true); }
                    else { return Json(false); }
                }             
            }     
        }

        [Authorize(Roles = "admin")]
        public async Task<JsonResult> AdminChange(string Id)
        {
            using (ApplicationContext context = new ApplicationContext())
            {
                var identity = Thread.CurrentPrincipal.Identity;
                var thisuser = UserManager.FindByName(identity.Name);
                var isadmin = await UserManager.IsInRoleAsync(thisuser.Id, "admin");
                if (!isadmin) { return Json(false); }
                else
                {
                    var user = context.Users.Where(x => x.Id == Id).FirstOrDefault();
                    if (user.IsAdmin) { UserManager.RemoveFromRole(Id, "admin"); user.IsAdmin = false; }
                    else if (!user.IsAdmin) { UserManager.AddToRole(Id, "admin"); user.IsAdmin = true; }
                    context.SaveChanges();
                    return Json(true);
                }           
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

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }

    
}