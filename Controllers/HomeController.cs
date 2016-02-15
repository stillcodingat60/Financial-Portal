using Financial_Portal.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Financial_Portal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Helper helper = new Helper();
            var hh = helper.GetHousehold(User.Identity.GetUserId());
            if (hh == null)
            {

            }
            return View();
           
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}