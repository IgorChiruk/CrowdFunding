using Microsoft.Owin;
using Owin;
using kraud.Models;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security;
using System;


[assembly: OwinStartup(typeof(kraud.App_Start.Startup))]

namespace kraud.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<ApplicationContext>(ApplicationContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Index"),            
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.UseFacebookAuthentication(
               appId: "559513334758269",
               appSecret: "25d65b7cec62921ccf7831d5dd2d068e");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {           
                ClientId = "827718720304-p82c2ialtfllkt825gavprdmqrjcmpan.apps.googleusercontent.com",
                ClientSecret = "WaPRC7739_MkyGaxj_rcxRHl",
                CallbackPath = new PathString("/GoogleLoginCallback"),
            });

        }
    }
}


//<add name = "IdentityDb" providerName="System.Data.SqlClient" connectionString="Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\IdentityDb.mdf;Integrated Security=True;" />
//<add name = "IdentityDb" providerName="System.Data.SqlClient" connectionString="Data Source=mer13.database.windows.net;Initial Catalog=MER13;User ID=mer13;Password=Tenar1561;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" />