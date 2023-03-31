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
    public class products1Controller : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: products1
        public ActionResult Index()
        {
            var products = db.products.Include(p => p.category).Include(p => p.group).Include(p => p.user);
            return View(products.ToList());
        }

        // GET: products1/Details/5
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

        // GET: products1/Create
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.categories, "id", "code");
            ViewBag.group_id = new SelectList(db.groups, "id", "code");
            ViewBag.created_by = new SelectList(db.users, "id", "name");
            return View();
        }

        // POST: products1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,name,group_id,category_id,unit,purchase_price,sell_price,quantity,status,note,created_by,created_at,updated_at,deleted_at,name_group,name_category,unit_swap,quantity_swap,quantity_remaning,sell_price_swap")] product product)
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

        // GET: products1/Edit/5
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

        // POST: products1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,name,group_id,category_id,unit,purchase_price,sell_price,quantity,status,note,created_by,created_at,updated_at,deleted_at,name_group,name_category,unit_swap,quantity_swap,quantity_remaning,sell_price_swap")] product product)
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

        // GET: products1/Delete/5
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

        // POST: products1/Delete/5
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
