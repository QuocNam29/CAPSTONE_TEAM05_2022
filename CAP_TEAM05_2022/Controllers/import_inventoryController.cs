using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;

namespace CAP_TEAM05_2022.Controllers
{
    [LoginVerification]
    public class import_inventoryController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: import_inventory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InventoryList()
        {
            var import_inventory = db.import_inventory.Include(i => i.user).Include(i => i.product);
            return PartialView(import_inventory.ToList());
        }

        // GET: import_inventory/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            import_inventory import_inventory = db.import_inventory.Find(id);
            if (import_inventory == null)
            {
                return HttpNotFound();
            }
            return View(import_inventory);
        }

        // GET: import_inventory/Create
        public ActionResult Create()
        {
            ViewBag.created_by = new SelectList(db.users, "id", "name");
            ViewBag.product_id = new SelectList(db.products, "id", "code");
            return View();
        }

        // POST: import_inventory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,product_id,quantity,price_import,sold,created_by,created_at,updated_at,deleted_at")] import_inventory import_inventory)
        {
            if (ModelState.IsValid)
            {
                db.import_inventory.Add(import_inventory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.created_by = new SelectList(db.users, "id", "name", import_inventory.created_by);
            ViewBag.product_id = new SelectList(db.products, "id", "code", import_inventory.product_id);
            return View(import_inventory);
        }

        // GET: import_inventory/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            import_inventory import_inventory = db.import_inventory.Find(id);
            if (import_inventory == null)
            {
                return HttpNotFound();
            }
            ViewBag.created_by = new SelectList(db.users, "id", "name", import_inventory.created_by);
            ViewBag.product_id = new SelectList(db.products, "id", "code", import_inventory.product_id);
            return View(import_inventory);
        }

        // POST: import_inventory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,product_id,quantity,price_import,sold,created_by,created_at,updated_at,deleted_at")] import_inventory import_inventory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(import_inventory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.created_by = new SelectList(db.users, "id", "name", import_inventory.created_by);
            ViewBag.product_id = new SelectList(db.products, "id", "code", import_inventory.product_id);
            return View(import_inventory);
        }

        // GET: import_inventory/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            import_inventory import_inventory = db.import_inventory.Find(id);
            if (import_inventory == null)
            {
                return HttpNotFound();
            }
            return View(import_inventory);
        }

        // POST: import_inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            import_inventory import_inventory = db.import_inventory.Find(id);
            db.import_inventory.Remove(import_inventory);
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
