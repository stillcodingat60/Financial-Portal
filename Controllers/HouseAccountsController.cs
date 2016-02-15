using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Financial_Portal.Models;

namespace Financial_Portal.Controllers
{
    public class HouseAccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseAccounts
        public ActionResult Index()
        {
            var hAccounts = db.HAccounts.Include(h => h.Hh);
            return View(hAccounts.ToList());
        }

        // GET: HouseAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseAccount houseAccount = db.HAccounts.Find(id);
            if (houseAccount == null)
            {
                return HttpNotFound();
            }
            return View(houseAccount);
        }

        // GET: HouseAccounts/Create
        public ActionResult Create()
        {
            ViewBag.HhId = new SelectList(db.Households, "Id", "Id");
            return View();
        }

        // POST: HouseAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,HAName,HhId")] HouseAccount houseAccount)
        {
            if (ModelState.IsValid)
            {
                db.HAccounts.Add(houseAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", houseAccount.HhId);
            return View(houseAccount);
        }

        // GET: HouseAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseAccount houseAccount = db.HAccounts.Find(id);
            if (houseAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", houseAccount.HhId);
            return View(houseAccount);
        }

        // POST: HouseAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,HAName,HhId")] HouseAccount houseAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(houseAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", houseAccount.HhId);
            return View(houseAccount);
        }

        // GET: HouseAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseAccount houseAccount = db.HAccounts.Find(id);
            if (houseAccount == null)
            {
                return HttpNotFound();
            }
            return View(houseAccount);
        }

        // POST: HouseAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
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
