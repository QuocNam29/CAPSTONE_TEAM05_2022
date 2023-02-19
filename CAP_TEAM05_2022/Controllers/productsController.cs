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
using Microsoft.AspNet.Identity;

namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class productsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: products
        public ActionResult Index()
        {
            return View();

        }

        public ActionResult _ProductList(int group_id, int category_id)
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
        public ActionResult _Inventory_ProductList()
        {
            var links = from l in db.products
                        select l;

            return PartialView(links.Where(c => c.status != 3).OrderByDescending(c => c.id));

        }
        public ActionResult Create_Product(string name_product, string unit,
            int quantity, int GroupProductDropdown, int CategoryDropdown,
            string sell_price, string purchase_price, int? quantity_swap, string unit_swap,
            string price_swap, int check_swap )
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.products.Where(p => p.name == name_product
                                        && p.group_id == GroupProductDropdown
                                        && p.category_id == CategoryDropdown
                                        && p.unit == unit).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Sản phẩm đã tồn tại trong hệ thống !";
                }
                else
                {
                    if (String.IsNullOrEmpty(unit))
                    {
                        status = false;
                        message = "Vui lòng chọn đơn vị của sản phẩm !";
                    }
                    else
                    {
                        if (check_swap == 1 && (quantity_swap == null || String.IsNullOrEmpty(unit_swap) || String.IsNullOrEmpty(price_swap)))
                        {
                            status = false;
                            message = "Thông tin quy đổi còn trống !";
                        }
                        else
                        {


                            product product = new product();
                            product.name = name_product;
                            product.status = 1;
                            product.unit = unit;
                            product.category_id = CategoryDropdown;
                            product.group_id = GroupProductDropdown;
                            product.created_by = User.Identity.GetUserId();
                            product.sell_price = decimal.Parse(sell_price.Replace(",", "").Replace(".", ""));
                            product.purchase_price = decimal.Parse(purchase_price.Replace(",", "").Replace(".", ""));
                            product.quantity = quantity;
                            product.created_at = DateTime.Now;
                            product.code = "SP" + CodeRandom.RandomCode();
                            product.quantity_swap = quantity_swap;
                            product.unit_swap = unit_swap;
                            if (!String.IsNullOrEmpty(price_swap))
                            {
                                product.sell_price_swap = decimal.Parse(price_swap.Replace(",", "").Replace(".", ""));
                            }
                            db.products.Add(product);
                            import_inventory inventory = new import_inventory();
                            inventory.product_id = product.id;
                            inventory.quantity = quantity;
                            inventory.price_import = int.Parse(purchase_price.Replace(",", "").Replace(".", ""));
                            inventory.sold = 0;
                            inventory.sold_swap = 0;
                            inventory.created_by = User.Identity.GetUserId();
                            inventory.created_at = DateTime.Now;
                            db.import_inventory.Add(inventory);
                            db.SaveChanges();
                            message = "Tạo sản phẩm thành công";
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_Product(product product)
        {
            bool status = true;
            string mess = "";
            try
            {
              
                int check = db.import_inventory.Where(i => i.product_id == product.id && i.sold == 0).Count();
                if (check <= 1)
                {
                    import_inventory inventory = db.import_inventory.Where(i => i.product_id == product.id && i.sold == 0).FirstOrDefault();
                    db.import_inventory.Remove(inventory);
                }
                product product1 = db.products.Find(product.id);
                db.products.Remove(product1);
                db.SaveChanges();
                mess = "Xóa sản phẩm thành công";
                db.SaveChanges();
            }
            catch (Exception)
            {

                status = false;
                mess = "Xóa thất bại ! (Sản phẩm đang được liên kết với chức năng khác trong hệ thống)";
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
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

        public JsonResult UpdateProduct(int Product_id, string name_product, string unit,
            int GroupProductDropdown, int CategoryDropdown,
            string sell_price)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.products.Where(p => p.name == name_product
                                       && p.group_id == GroupProductDropdown
                                       && p.category_id == CategoryDropdown
                                       && p.unit == unit 
                                       && p.id != Product_id).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Sản phẩm đã tồn tại trong hệ thống !";
                }
                else
                {
                 
                    product product = db.products.Find(Product_id);
                    product.name = name_product;
                    product.unit = unit;
                    product.category_id = CategoryDropdown;
                    product.group_id = GroupProductDropdown;
                    product.sell_price = int.Parse(sell_price.Replace(",", "").Replace(".", ""));
                    product.updated_at = DateTime.Now;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Chỉnh sửa thông tin sản phẩm thành công ";
                    
                }
            }
            catch (Exception e)
            {

                message = e.Message;
                status = false;
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Import_Stock(int Product_id, int quantity, string purchase_price)
        {
            string message = "";
            bool status = true;
            try
            {
               
                product product = db.products.Find(Product_id);
                product.quantity += quantity;
                product.updated_at = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                import_inventory import = new import_inventory();
                import.product_id = Product_id;
                import.quantity = quantity;
                import.price_import = int.Parse(purchase_price.Replace(",", "").Replace(".", ""));
                import.sold = 0;
                import.created_at = DateTime.Now;
                import.created_by = User.Identity.GetUserId();
                db.import_inventory.Add(import);

                db.SaveChanges();
                message = "Nhập số lượng sản phẩm thành công ";
                status = true;
            }
            catch (Exception e)
            {

                message = e.Message;
                status = false;
            }

            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSearchValue(string search)
        {
            var product = (from Product in db.products
                           where Product.name.StartsWith(search) && Product.status != 3 && Product.@group.status != 3 && Product.category.status != 3
                           select new
                           {
                               label = Product.name,
                               val = Product.id
                           }).ToList();

            return Json(product);
        }

        [HttpPost]
        public JsonResult FindProduct_name(int product_id)
        {
            product product = db.products.Find(product_id);
            var emp = new product();
            emp.id = product.id;
            emp.code = product.code;
            emp.name = product.sell_price.ToString("N0");
            emp.note = "Số lượng tồn: " + product.quantity + product.unit;
            if (product.quantity_swap != null)
            {
                emp.name_group = string.Format("{0:0,00}", product.sell_price_swap);
                emp.note += "/" + product.quantity_swap + product.unit_swap;
                if (product.quantity_remaning != null)
                {
                    emp.note += " và " + product.quantity_remaning  + product.unit_swap + " lẻ";
                }
            }
           
                
            emp.quantity = product.quantity;
            emp.unit = product.unit;
            emp.unit_swap = product.unit_swap;
            return Json(emp);
        }

        public JsonResult GetProductList(string searchTerm)
        {
            var productList = db.products.ToList();

            if (searchTerm != null)
            {
                productList = db.products.Where(x => x.name.Contains(searchTerm)).ToList();
            }

            var modifiedData = productList.Select(x => new
            {
                id = x.id,
                text = x.name
            });
            return Json(modifiedData, JsonRequestBehavior.AllowGet);
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
