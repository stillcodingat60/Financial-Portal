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

namespace Financial_Portal.Controllers
{
    [AuthorizeHouseHoldRequired]
    public class HouseAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseAccounts
        public ActionResult Index()
        {
            //var user = db.Users.Find(User.Identity.GetUserId());
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
           
            var accountsHhId = db.HAccounts.Where(p => p.HhId == HhId); //a listing of all accounts for this household
                        
            var house = db.Households.Find(HhId);

            var BalList = from acc in house.HAccounts
                       let sum = (from tr in acc.Transactions
                                  where tr.Reconcile == true
                                  select tr.Amount).DefaultIfEmpty().Sum()

                       select new BankBalance()
                       {
                           Account = acc,
                           BankBal = sum
                       };

            //var searchAccts = db.HAccounts.AsQueryable();         // this block of code sets up a search for users whose household id
            //searchAccts = searchAccts.Where(p => p.HhId == HhId);                // matches the currently passed HouseHold id
            //var hAccounts = searchAccts.OrderByDescending(p => p.HAName).ToList();  // and returns the listing to the view

            return View(BalList);
        }

        // GET: HouseAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HouseAccount houseAccount = db.HAccounts.Find(id);

            var trx = db.Transactions.Where(t => t.HAccountId == houseAccount.Id).Include(t => t.Cat);
            decimal bankBal = 0;
            foreach (var rec in trx)
            {
                if (rec.Reconcile == true)
                {
                    if (rec.Type == "income")
                        bankBal += rec.Amount;
                    else
                        bankBal -= rec.Amount;
                }
            }
            ViewBag.bankBal = bankBal;
            return View(houseAccount);
        }

        // GET: HouseAccounts/Create        
        public PartialViewResult _Create()
        {
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
            ViewBag.HhId = HhId;                //need the Household Id because the new account doesn't have any ref to the household

            return PartialView();
        }

        // POST: HouseAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HAName,HhId,Balance")] HouseAccount houseAccount)
        {
            if (ModelState.IsValid)
            {
                var account = db.HAccounts.FirstOrDefault(t => t.HAName == t.HAName);
                if (account != null)
                {
                    Transaction beginBal = new Transaction();
                    beginBal.Amount = houseAccount.Balance;
                    beginBal.Descript = "Beginning Balance";
                    beginBal.Created = DateTime.Now;
                    beginBal.HAccountId = houseAccount.Id;
                    beginBal.Reconcile = true;
                    beginBal.Type = "income";

                    db.Transactions.Add(beginBal);

                    db.HAccounts.Add(houseAccount);

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "An account by this name already exists. Please use a unique name.";
                    return View();
                }
            }

            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", houseAccount.HhId);
            return View(houseAccount);
        }

        // GET: HouseAccounts/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            HouseAccount houseAccount = db.HAccounts.Find(id);

            return PartialView(houseAccount);
        }

        // POST: HouseAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HAName,HhId,Balance")] HouseAccount houseAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(houseAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(houseAccount);
        }

        // GET: HouseAccounts/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            HouseAccount houseAccount = db.HAccounts.Find(id);
            return PartialView(houseAccount);
        }

        // POST: HouseAccounts/Delete/5
        [HttpPost, ActionName("_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            var transactions = db.Transactions.Where(t => t.HAccountId == id);
            foreach (var tr in transactions)
            {
                tr.CatId = null;
                tr.HAccountId = null;
                db.Transactions.Remove(tr);
            }

            HouseAccount houseAccount = db.HAccounts.Find(id);
            db.HAccounts.Remove(houseAccount);

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
    }
}
