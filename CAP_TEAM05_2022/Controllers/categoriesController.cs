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
    public class categoriesController : Controller
    {
        private QuyThanhEntities db = new QuyThanhEntities();

        // GET: categories
        public ActionResult Index()
        {
            var categories = db.categories.Include(c => c.user).Where(c => c.status != 3).OrderByDescending(c => c.id);
            return View(categories.ToList());
        }
        public ActionResult Create_Category(string name_category)
        {
            string email =  Session["user_email"].ToString();
            user user = db.users.Where(u => u.email == email).FirstOrDefault();
            category category = new category();
            category.name = name_category;
            category.status = 1;
            category.created_by = user.id;
            category.created_at = DateTime.Now;
            category.slug = name_category;
            category.code = "DM" + CodeRandom.RandomCode();
            db.categories.Add(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete_Category(category categorys)
        {
            category categories = db.categories.Find(categorys.id);
            categories.status = 3;
            categories.deleted_at = DateTime.Now;
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Delete_Category", JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStatus_Category(category categorys)
        {
            category categories = db.categories.Find(categorys.id);
            if (categories.status == 1)
            {
                categories.status = 2;
            }
            else
            {
                categories.status = 1;
            }         
            categories.updated_at = DateTime.Now;
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Category", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FindCategory(int category_id)
        {
            category categories = db.categories.Find(category_id);
            var emp = new category();
            emp.id = category_id;
            emp.name = categories.name;       
            return Json(emp);
        }
        public ActionResult Edit_Category(int category_id, string name_category)
        {
            category categories = db.categories.Find(category_id);
            categories.name = name_category;
            categories.updated_at = DateTime.Now;
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category categories = db.categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // GET: categories/Create
        public ActionResult Create()
        {
            ViewBag.created_by = new SelectList(db.users, "id", "name");
            return View();
        }

        // POST: categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,created_by,name,slug,status,created_at,updated_at,deleted_at")] category categories)
        {
            if (ModelState.IsValid)
            {
                db.categories.Add(categories);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.created_by = new SelectList(db.users, "id", "name", categories.created_by);
            return View(categories);
        }

        // GET: categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category categories = db.categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            ViewBag.created_by = new SelectList(db.users, "id", "name", categories.created_by);
            return View(categories);
        }

        // POST: categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,created_by,name,slug,status,created_at,updated_at,deleted_at")] category categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(categories).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.created_by = new SelectList(db.users, "id", "name", categories.created_by);
            return View(categories);
        }

        // GET: categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            category categories = db.categories.Find(id);
            if (categories == null)
            {
                return HttpNotFound();
            }
            return View(categories);
        }

        // POST: categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            category categories = db.categories.Find(id);
            db.categories.Remove(categories);
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
