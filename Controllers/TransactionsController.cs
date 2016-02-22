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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index(int HAccount)
        {
            var accts = db.HAccounts.Find(HAccount);
            var transactions = db.Transactions.Where(t => t.HAccountId == accts.Id).Include(t => t.HAccount);
            if (transactions != null)
                return View(transactions.ToList());
            else
                return View();
        }

        // GET: Transactions/Details/5
        public PartialViewResult _Details(int? id)
        {            
            Transaction transaction = db.Transactions.Find(id);         
            return PartialView(transaction);
        }

        // GET: Transactions/Create
        public PartialViewResult _Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CatId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName");
            ViewBag.HAccountId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName");
            ViewBag.Type = new SelectList(new[] { "expense", "income" }, "Type");
            return PartialView();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CatId,HAccountId,Created,Descript,Type,Reconcile,Amount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var hhacct = db.HAccounts.Find(transaction.HAccountId);
                if (transaction.Type == "income")
                    hhacct.Balance += transaction.Amount;
                else
                    hhacct.Balance -= transaction.Amount;
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Details","HouseAccounts", new { id = hhacct.Id });
            }

            var user = db.Users.Find(User.Identity.GetUserId());

            ViewBag.CatId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName", transaction.CatId);
            ViewBag.HAccountId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName", transaction.HAccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            Transaction transaction = db.Transactions.Find(id);
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName");
            ViewBag.HAcctId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName"); //had to change the viewbag name != the db.Name otherwise the value in the field being displayed would be the first dropdown item
            ViewBag.Grype = new SelectList(new[] { "expense", "income" }, "Type");
            return PartialView(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CatId,HAccountId,Created,Descript,Type,Reconcile,Amount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var oldTrx = (from t in db.Transactions.AsNoTracking()
                              where t.Id == transaction.Id
                              select t).FirstOrDefault();
                //oldTrx = db.Transactions.AsNoTracking().FirstOrDefault(c => c.Id == transaction.Id);    //this is the same as the previous statement - one is LINQ query syntax and this  line is method syntax

                var hhacct = db.HAccounts.Find(transaction.HAccountId);
                if (oldTrx.Type == "income")                //
                    hhacct.Balance -= oldTrx.Amount;        // reverses the original trx
                else                                        //
                    hhacct.Balance += oldTrx.Amount;        //

                if (transaction.Type == "income")           //
                    hhacct.Balance += transaction.Amount;   // adds in the current changes, if any
                else                                        //
                    hhacct.Balance -= transaction.Amount;   //

                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "HouseAccounts", new { id = hhacct.Id });
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName");
            ViewBag.HAcctId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName");
            ViewBag.Grype = new SelectList(new[] { "expense", "income" }, "Type");
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            Transaction transaction = db.Transactions.Find(id);
            return PartialView(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);     //retrieve the current transaction
            var hhacct = db.HAccounts.Find(transaction.HAccountId); //retrieve the Bank Account for the transaction
                        
            transaction.HAccountId = null;
            transaction.CatId = null;
            
            //reverse the balance amount in the Account record
            if (transaction.Type == "income")
                hhacct.Balance -= transaction.Amount;
            else
                hhacct.Balance += transaction.Amount;

            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Details","HouseAccounts", new { id = hhacct.Id });
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
