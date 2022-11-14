using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Models;

namespace CAP_TEAM05_2022.Controllers
{
    public class productsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: products
        public ActionResult Index()
        {
            var products = db.products.Include(p => p.category).Include(p => p.group).Include(p => p.user);
            return View(products.ToList());
        }

        // GET: products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: products/Create
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.categories, "id", "code");
            ViewBag.group_id = new SelectList(db.groups, "id", "code");
            ViewBag.created_by = new SelectList(db.users, "id", "name");
            return View();
        }

        // POST: products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,name,slug,group_id,category_id,unit,price_1,price_2,price_3,price_4,discount,status,note,created_by,created_at,updated_at,deleted_at")] product product)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.category_id = new SelectList(db.categories, "id", "code", product.category_id);
            ViewBag.group_id = new SelectList(db.groups, "id", "code", product.group_id);
            ViewBag.created_by = new SelectList(db.users, "id", "name", product.created_by);
            return View(product);
        }

        // GET: products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.category_id = new SelectList(db.categories, "id", "code", product.category_id);
            ViewBag.group_id = new SelectList(db.groups, "id", "code", product.group_id);
            ViewBag.created_by = new SelectList(db.users, "id", "name", product.created_by);
            return View(product);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,name,slug,group_id,category_id,unit,price_1,price_2,price_3,price_4,discount,status,note,created_by,created_at,updated_at,deleted_at")] product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.category_id = new SelectList(db.categories, "id", "code", product.category_id);
            ViewBag.group_id = new SelectList(db.groups, "id", "code", product.group_id);
            ViewBag.created_by = new SelectList(db.users, "id", "name", product.created_by);
            return View(product);
        }

        // GET: products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            product product = db.products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product product = db.products.Find(id);
            db.products.Remove(product);
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
