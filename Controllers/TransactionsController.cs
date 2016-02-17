﻿using System;
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
        public ActionResult Index(int? HAccount)
        {                      
            var transactions = db.Transactions.Include(t => t.Cat).Include(t => t.HAccount);
            if (transactions != null)
                return View(transactions.ToList());
            else
                return View();
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CatId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName");
            ViewBag.HAccountId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName");
            ViewBag.Type = new SelectList(new[] { "income", "expense" }, "Type");
            return View();
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
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var user = db.Users.Find(User.Identity.GetUserId());            

            ViewBag.CatId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName", transaction.CatId);
            ViewBag.HAccountId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName", transaction.HAccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CatId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName");
            ViewBag.HAccountId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName");
            ViewBag.Type = new SelectList(new[] { "income", "expense" }, "Type");
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CatId,HAccountId,Created,Descript,Reconcile,Amount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var user = db.Users.Find(User.Identity.GetUserId());
            ViewBag.CatId = new SelectList(db.Categories.Where(p => p.HhId == user.HouseHoldId), "Id", "CName");
            ViewBag.HAccountId = new SelectList(db.HAccounts.Where(q => q.HhId == user.HouseHoldId), "Id", "HAName");
            ViewBag.Type = new SelectList(new[] { "income", "expense" }, "Type");
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            transaction.HAccountId = null;
            transaction.CatId = null;
            
            db.Transactions.Remove(transaction);
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