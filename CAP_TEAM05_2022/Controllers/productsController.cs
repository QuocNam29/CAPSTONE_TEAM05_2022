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
    public class productsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: products
        public ActionResult Index()
        {
            return View();
         
        }
      
        public ActionResult ProductList(int group_id , int category_id)
        {
            var links = from l in db.products
                        select l;
            if (group_id != -1)
                {
                links = links.Where(p => p.group_id == group_id);
            }
             if (category_id != -1)
            {
                links = links.Where(p => p.category_id == category_id);
            }                                        
            return PartialView(links);

        }
        public ActionResult Create_Product(string name_product, string unit,
            int quantity, int GroupProductDropdown, int CategoryDropdown,
            string sell_price,  string purchase_price
            )
        {
            string email = Session["user_email"].ToString();
            user user = db.users.Where(u => u.email == email).FirstOrDefault();
            product product = new product();
            product.name = name_product;
            product.status = 1;
            product.unit = unit;
            product.category_id = CategoryDropdown;
            product.group_id = GroupProductDropdown;
            product.created_by = user.id;
            product.price = int.Parse(sell_price.Replace(",",""));
            product.created_at = DateTime.Now;
            product.code = "SP" + CodeRandom.RandomCode();
            db.products.Add(product);
            import_inventory inventory = new import_inventory();
            inventory.product_id = product.id;
            inventory.quantity = quantity;
            inventory.price_import = int.Parse(purchase_price.Replace(",", ""));
            inventory.sold = 0;
            inventory.created_by = user.id;
            inventory.created_at = DateTime.Now;
            db.import_inventory.Add(inventory);
            db.SaveChanges();
            Session["notification"] = "Thêm mới thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult Delete_Product(product product)
        {
            product product1 = db.products.Find(product.id);
            product1.status = 3;
            product1.deleted_at = DateTime.Now;
            db.Entry(product1).State = EntityState.Modified;
            db.SaveChanges();
            return Json("Delete_Product", JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStatus_Product(product products)
        {
            product product = db.products.Find(products.id);
            if (product.status == 1)
            {
                product.status = 2;
            }
            else
            {
                product.status = 1;
            }
            product.updated_at = DateTime.Now;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Product", JsonRequestBehavior.AllowGet);
        }
        /*
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
                }*/

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
