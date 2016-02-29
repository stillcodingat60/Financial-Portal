using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Financial_Portal.Models;
using Microsoft.AspNet.Identity;
using System.Configuration;
using Newtonsoft.Json;

namespace Financial_Portal.Controllers
{
    public class HouseHoldsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseHolds - the index view will be the dashboard for the HouseHold
        [AuthorizeHouseHoldRequired]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user == null)
            {
                ViewBag.Message = "You are not a user on this system. Please register first!";
                return View();
            }

            HouseHold houseHold = db.Households.Find(user.HouseHoldId);
            if (houseHold == null)
            {
                return HttpNotFound();
            }
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
            
            return View(houseHold);
        }

        // GET: HouseHolds/Details/5
        [Authorize]
        public ActionResult Details()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            if (user.HouseHoldId == null)
            {
                ViewBag.Message = "You are not a user on this system. Please register first!";
                return View();
            }

            HouseHold houseHold = db.Households.Include("Users").Include("HAccounts").FirstOrDefault(p => p.Id == user.HouseHoldId);

            if (houseHold == null)
            {
                ViewBag.Message = " Please create or join a household first!";
                return View();
            }

            //var searchUsers = db.Users.AsQueryable();                               // this block of code sets up a search for users whose household id
            //searchUsers = searchUsers.Where(p => p.HouseHoldId == houseHold.Id);    // matches the currently passed HouseHold id
            //var hhUsers = searchUsers.OrderByDescending(p => p.LastName).ToList();  // and returns the listing to the view

            return View(houseHold);
        }

        // GET: HouseHolds/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HouseHolds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HName")] HouseHold houseHold)
        {
            if (ModelState.IsValid)
            {
                db.Households.Add(houseHold);
                db.SaveChanges();                   //save the newly created household

                //var hh = db.Households.FirstOrDefault(h => h.HName == houseHold.HName);  //retrieve the newly created household by name because we don't have the HhId

                //var currUser = db.Users.Find(User.Identity.GetUserId()).HouseHoldId==houseHold.Id;    //now we retrieve the user record and assign the household id

                var searchCats = db.Categories.AsQueryable();
                searchCats = searchCats.Where(p => p.HhId == null);         //add the initial list of seeded categories
                var CatList = new List<Category>();                         //for each newly created HouseHold
                foreach (var eachCat in searchCats)
                {
                    var category = new Category()
                    {
                        CName = eachCat.CName,
                        HhId = houseHold.Id,
                        Type = eachCat.Type
                    };
                    CatList.Add(category);
                }
                db.Categories.AddRange(CatList);

                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseHoldId = houseHold.Id;

                db.SaveChanges();            //saves the hhid to ApplicationUser

                return RedirectToAction("Details", new { id = houseHold.Id });  //id name must match the id name in the index action passed param


                // var CatName = db.Categories.OrderByDescending(c => c.CName).ToList(); get list of categories to initialize the new Household
                //next we want to loop through each category record, assign it a HhId and save it. Do that in the view?

            }

            return View(houseHold);
        }

        [HttpPost]
        public ActionResult Invite(ContactMessage contact)     //sent here from the HouseHold details page
        {
            var inv = new Invite();
            inv.EmailInvite = contact.Email;
            inv.HhId = contact.HhId;
            inv.CodeNr = KeyGenerator.GetUniqueKey(7);
            db.Invites.Add(inv);
            db.SaveChanges();
            var requestor = contact.FName + ' ' + contact.LName;
            var Emailer = new EmailService();

            contact.Message = "Please join my financial household. You will have to create an account on the application. Begin by going to Http://jmm-financialportal.azurewebsites.net to login and use the code listed below to validate";

            var mail = new IdentityMessage
            {
                Subject = "Invitation to Join",
                Destination = contact.Email,
                Body = "You have received an invitation with the following contents. \n\n" + contact.Message + "\n\n Your Invitation Code is: " + inv.CodeNr
            };
            Emailer.SendAsync(mail);

            return RedirectToAction("Details", new { id = inv.HhId });
        }

        // GET: HouseHolds/Delete/5
        public ActionResult Delete()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseHoldId = null;
            db.SaveChanges();
            return RedirectToAction("Create");

        }

        // POST: HouseHolds/Delete/5
        [AuthorizeHouseHoldRequired]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string Y, string N)
        {
            if (!String.IsNullOrEmpty(Y))
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseHoldId = null;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: HouseHolds/Join
        public ActionResult Join()
        {
            return View();
        }


        // POST: HouseHolds/Join
        [HttpPost]
        public ActionResult Join(string Code)
        {
            if (Code == null)
            {
                ViewBag.errorMessage = "Please enter a valid code.";
                return View();
            }

            //var CodeNr = Convert.ToInt32(Code);
            var user = db.Users.Find(User.Identity.GetUserId());             //get the current user
            Invite invitation = db.Invites.FirstOrDefault(p => p.EmailInvite == user.Email && p.CodeNr == Code); //check to see that the invite email matches the current user's email

            if (invitation == null)
            {                                                                //because someone may have gotten the invite email and tried to join
                ViewBag.errorMessage = "Sorry, there is no invitation on file for you";
                return View();                                               //without using the specific email address that was invited!
            }

            HouseHold houseHold = db.Households.Find(invitation.HhId);   //get the household for the invitation
            if (houseHold == null)
            {
                ViewBag.errorMessage = "HOUSEHOLD NOT FOUND. Please enter a valid code.";
                return View();
            }

            user.HouseHoldId = houseHold.Id;                             // setting the User with the Household Id makes him part of the HouseHold
            db.SaveChanges();
            return RedirectToAction("Details", new { id = houseHold.Id });

            //return View();
        }

        public ActionResult GetChart()
        {
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
            var house = db.Households.Find(HhId);

            var acctsHhId = db.HAccounts.Where(p => p.HhId == HhId);
            var budgetsHhId = db.Budgets.Where(b => b.HhId == HhId);

            var totMonthlyInc = acctsHhId.
                SelectMany(t => t.Transactions).                //use SelectMany when you have multiple lists
                Where(t => t.Type == "income" && t.CatId != null).
                Select(t => t.Amount).
                DefaultIfEmpty().
                Sum();

            var totMonthlyExp = acctsHhId.
                 SelectMany(t => t.Transactions).
                 Where(t => t.Type == "expense").
                 Select(t => t.Amount).
                 DefaultIfEmpty().
                 Sum();

            var totMonthlyBud = budgetsHhId.
                 Select(b => b.BAmount).
                 DefaultIfEmpty().
                 Sum();

            ViewBag.totMonthlyInc = totMonthlyInc;
            ViewBag.totMonthlyExp = totMonthlyExp;
            ViewBag.totMonthlyBud = totMonthlyBud;

            var donutHole = new[] { new { label = "Income", value = (decimal)totMonthlyInc }, new { label = "Expenses", value = (decimal)totMonthlyExp }, new { label = "Budget", value = (decimal)totMonthlyBud } };

            var chartData = (from catd in house.Categories
                             where catd.Type.Equals("expense")
                             let act = (from jmm in catd.Transactions
                                        where jmm.Created.Month.Equals(DateTime.Now.Month)
                                        select jmm.Amount).DefaultIfEmpty().Sum()
                             let bud = (from mrm in catd.Budgets
                                        select mrm.BAmount).DefaultIfEmpty().Sum()

                             select new
                             {
                                 y = catd.CName,
                                 a = act,
                                 b = bud
                             }).ToArray();            
            
            //donut data
            var donutData = (from cats in house.Categories
                             let sum = (from tr in cats.Budgets
                                        select tr.BAmount).DefaultIfEmpty().Sum()
                             let bSum = (from b in cats.Budgets
                                         select b.BAmount).DefaultIfEmpty().Sum()

                             select new
                             {
                                 label = cats.CName,
                                 value = sum
                             }).ToArray();

            var data = new
            {
                donut = donutData,
                bar = chartData,
                donut2 = donutHole,
                totMonthlyInc = totMonthlyInc,
                totMonthlyExp = totMonthlyExp,
                totMonthlyBud = totMonthlyBud
            };

            return Content(JsonConvert.SerializeObject(data), "application/json");

        }

    }
}
