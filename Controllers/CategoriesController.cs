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
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());

            var categories = db.Categories.Where(c => c.HhId == HhId);
            return View(categories.ToList());
        }

        // GET: Categories/Details/5
        public PartialViewResult _Details(int? id)
        {
            Category category = db.Categories.Find(id);
            return PartialView(category);
        }

        // GET: Categories/Create
        public PartialViewResult _Create()
        {
            var HhId = Convert.ToInt32(User.Identity.GetHouseHoldId());
            ViewBag.HhId = HhId;
            ViewBag.CId = new SelectList(db.Categories.Where(p => p.HhId == HhId), "Id", "CName");
            ViewBag.Type = new SelectList(new[] { "expense", "income" }, "Type");
            return PartialView();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CName,Type,HhId")] Category category)
        {
            if (ModelState.IsValid)
            {
                //Category newCategory = new Category();
                //newCategory.HhId = category.HhId;
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", category.HhId);
            return View(category);
        }

        // GET: Categories/Edit/5
        public PartialViewResult _Edit(int? id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", category.HhId);
            ViewBag.Grype = new SelectList(new[] { "expense", "income" }, "Type");
            return PartialView(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CName,Type,HhId")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HhId = new SelectList(db.Households, "Id", "Id", category.HhId);
            return View(category);
        }

        // GET: Categories/Delete/5
        public PartialViewResult _Delete(int? id)
        {            
            Category category = db.Categories.Find(id);
            
            return PartialView(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("_Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
