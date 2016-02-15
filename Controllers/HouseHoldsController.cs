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

namespace Financial_Portal.Controllers
{
    public class HouseHoldsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseHolds - the index view will be the dashboard for the HouseHold
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            HouseHold houseHold = db.Households.Find(user.HouseHoldId);
            if (houseHold == null)
            {
                return HttpNotFound();
            }
            return View(houseHold);
        }

        // GET: HouseHolds/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseHold houseHold = db.Households.Include("Users").FirstOrDefault(p => p.Id == id);
            //HouseHold houseHold = db.Households.Find(id);
            if (houseHold == null)
            {
                return HttpNotFound();
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

                var hh = db.Households.FirstOrDefault(h => h.HName == houseHold.HName);  //retrieve the newly created household by name because we don't have the HhId
                //var currUser = db.Users.Find(User.Identity.GetUserId()).HouseHoldId==houseHold.Id;    //now we retrieve the user record and assign the household id
              
                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseHoldId = hh.Id;
               // var currUser = hhId == houseHold.Id; //returns true if both are equal
              
                db.SaveChanges();

                return RedirectToAction("Index", new { id = hh.Id });  //id name must match the id name in the index action passed param


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
            inv.CodeNr = 123456789;
            db.Invites.Add(inv);
            db.SaveChanges();
            var requestor = contact.FName + ' ' + contact.LName;
            var Emailer = new EmailService();
            
            contact.Message = "Please join my financial household. You will have to create an account on the application. Begin by going to Http://jmmcconnell-financial-portal.azurewebsites.net to login and use the code listed below to validate";

            var mail = new IdentityMessage
            {
                Subject = "Invitation to Join",
                Destination = contact.Email,
                Body = "You have received an invitation from: " + requestor + "( " + contact.Email + ") with the following contents. \n\n" + contact.Message + "\n\n Your Invitation Code is: " + inv.CodeNr
            };
            Emailer.SendAsync(mail);

            return RedirectToAction("Details", new { id = inv.HhId });
        }

        // GET: HouseHolds/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseHold houseHold = db.Households.Find(id);
            if (houseHold == null)
            {
                return HttpNotFound();
            }
            return View(houseHold);
        }

        // POST: HouseHolds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HouseHold houseHold = db.Households.Find(id);
            db.Households.Remove(houseHold);
            db.SaveChanges();
            return RedirectToAction("Index");
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

            var CodeNr = Convert.ToInt32(Code);
            var user = db.Users.Find(User.Identity.GetUserId());             //get the current user
            Invite invitation = db.Invites.FirstOrDefault(p => p.EmailInvite == user.Email && p.CodeNr == CodeNr ); //check to see that the invite email matches the current user's email

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
    }
}
