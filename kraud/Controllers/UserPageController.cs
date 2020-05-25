using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace kraud.Controllers
{
    public class UserPageController : Controller
    {
        // GET: UserPage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Campanies()
        {
            return View();
        }
    }
}