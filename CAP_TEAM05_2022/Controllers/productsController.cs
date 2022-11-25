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
