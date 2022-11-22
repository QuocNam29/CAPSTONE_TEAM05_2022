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
using OfficeOpenXml;

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
            return PartialView(links.Where(c => c.status != 3).OrderByDescending(c => c.id));

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
            product.sell_price = int.Parse(sell_price.Replace(",",""));
            product.purchase_price = int.Parse(purchase_price.Replace(",", ""));
            product.quantity = quantity;
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
      
        [HttpPost]
        public JsonResult FindProduct(int Product_id)
        {
            product product = db.products.Find(Product_id);
            var emp = new product();
            emp.id = Product_id;
            emp.name = product.name;
            emp.unit = product.unit;
            emp.quantity = product.quantity;
            emp.sell_price = product.sell_price;
            emp.purchase_price = product.purchase_price;
            emp.group_id = product.group_id;
            emp.category_id = product.category_id;
            return Json(emp);
        }
        public JsonResult UpdateProduct(product Products)
        {
            product product = db.products.Find(Products.id);
            product.name = Products.name;
           
            product.unit = Products.unit;
            product.category_id = Products.category_id;
            product.group_id = Products.group_id;     
            product.sell_price = Products.sell_price;
            product.purchase_price = Products.purchase_price;
            product.quantity = Products.quantity;
            product.updated_at = DateTime.Now;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            string message = "Record Saved Successfully ";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }

        public void ExportExcel(int group_id, int category_id)
        {
            var list = from l in db.products
                        select l;
            if (group_id != -1)
            {
                list = list.Where(p => p.group_id == group_id);
            }
            if (category_id != -1)
            {
                list = list.Where(p => p.category_id == category_id);
            }
            list = list.Where(c => c.status != 3).OrderByDescending(c => c.id);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
                ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("SanPham");
                Sheet.Cells["A1"].Value = "Mã sản phẩm";
                Sheet.Cells["B1"].Value = "Tên sản phẩm";
                Sheet.Cells["C1"].Value = "Nhóm hàng";
                Sheet.Cells["D1"].Value = "Danh mục";
                Sheet.Cells["E1"].Value = "Đơn vị";
                Sheet.Cells["F1"].Value = "Giá bán";
                Sheet.Cells["G1"].Value = "Giá nhập";
                Sheet.Cells["H1"].Value = "Số lượng nhập";
                Sheet.Cells["I1"].Value = "Đã bán";
                int row = 2;// dòng bắt đầu ghi dữ liệu
                foreach (var item in list)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.name;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.group.name;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.category.name;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.unit;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.sell_price;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.purchase_price;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.quantity;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.import_inventory.Where(i => i.product_id == item.id).Sum(s => s.sold);
                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename=" + "San_Pham.xlsx");
                Response.BinaryWrite(ep.GetAsByteArray());
                Response.End();
            
        }

        /*   public ActionResult getProduct()
           {

               return Json(db.products.Select(x => new
               {
                   productID = x.id,
                   productName = x.name,
                   productUnit = x.unit,
                   productQuatity = x.quantity,
                   productSell_price = x.sell_price,
                   productPurchase_price = x.purchase_price,
                   productGroup_id = x.group_id,
                   productCategory_id = x.category_id,
               }).ToList(), JsonRequestBehavior.AllowGet) ;
           }*/
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
