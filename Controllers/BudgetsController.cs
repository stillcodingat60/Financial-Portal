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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        public ActionResult Index()
        {
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
            var budgets = db.Budgets.Where(b => b.HhId == HhId);
            ViewBag.HName = db.Households.Find(HhId);
            var bHhId = db.Budgets.Find(HhId);
            if (bHhId != null)
                return View(budgets.ToList());
            else
                ViewBag.Message("You need to Create a budget first");
            return View();
        }

        // GET: Budgets/Details/5
        public PartialViewResult _Details(int? id)
        {
            Budget budget = db.Budgets.Find(id);
            return PartialView(budget);
        }

        // GET: Budgets/Create
        public ActionResult CreateNewBudget()
        {
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
            var category = db.Categories.Where(c => c.HhId == HhId);
            var budgets = new List<Budget>();
            foreach (var catg in category)
            {
                var budget = new Budget()
                {
                    HhId = HhId,
                    Type = catg.Type,
                    BName = catg.CName,
                    Frequency = 12,
                    CatId = catg.Id
                };
                budgets.Add(budget);
            }
            db.Budgets.AddRange(budgets);
            db.SaveChanges();
            
            //ViewBag.CatId = new SelectList(db.Categories, "Id", "CName");
            return RedirectToAction("Index");
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,BName,CatId,Frequency,HhId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Budgets.Add(budget);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CatId = new SelectList(db.Categories, "Id", "CName", budget.CatId);
            return View(budget);
        }

        // GET: Budgets/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            Budget budget = db.Budgets.Find(id);
            
            ViewBag.CatId = new SelectList(db.Categories, "Id", "CName", budget.CatId);
            return PartialView(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,BName,CatId,Frequency,HhId")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CatId = new SelectList(db.Categories, "Id", "CName", budget.CatId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        public PartialViewResult _Delete(int? id)
        {
            Budget budget = db.Budgets.Find(id);
            return PartialView(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            db.Budgets.Remove(budget);
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
