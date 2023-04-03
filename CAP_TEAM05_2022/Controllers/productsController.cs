﻿using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class productsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public productsController()
        {
            ViewBag.isCreate = false;
        }
        // GET: products
        public ActionResult Index()
        {
            return View();

        }
        [HttpGet]
        public PartialViewResult _Form()
        {
            ViewBag.isCreate = true;
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
            ViewBag.GroupId = new SelectList(db.groups, "Id", "Name");
            ViewBag.SupplierId = db.customers.Where(x => x.type == Constants.SUPPLIER).ToList();
            return PartialView("_Form", new product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,group_id,category_id,unit,quantity,unit_swap,quantity_swap")] product product, 
                                   string purchase_price, string sell_price, int CheckSwapValue, string price_swap, int SupplierDropdown)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                bool status = true;
                DateTime currentDate = DateTime.Now;
                try
                {
                    int check = db.products.Where(p => p.name == product.name
                                            && p.group_id == product.group_id
                                            && p.category_id == product.category_id
                                            && p.unit == product.unit).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Sản phẩm đã tồn tại trong hệ thống !";
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(product.unit))
                        {
                            status = false;
                            message = "Vui lòng chọn đơn vị của sản phẩm !";
                        }
                        else
                        {
                            if (CheckSwapValue == Constants.UNIT_CONVERT && (product.quantity_swap == null || String.IsNullOrEmpty(product.unit_swap) || String.IsNullOrEmpty(price_swap)))
                            {
                                status = false;
                                message = "Thông tin quy đổi còn trống !";
                            }
                            else
                            {
                                product.status = Constants.SHOW_STATUS;

                                product.created_by = User.Identity.GetUserId();
                                product.sell_price = decimal.Parse(sell_price.Replace(",", "").Replace(".", ""));
                                product.purchase_price = decimal.Parse(purchase_price.Replace(",", "").Replace(".", ""));
                                product.created_at = currentDate;
                                product.code = "SP" + CodeRandom.RandomCode();
                                if (CheckSwapValue != Constants.UNIT_CONVERT)
                                {
                                    product.unit_swap = null;
                                    product.quantity_swap = null;
                                    product.sell_price_swap = null;

                                }
                                else
                                {
                                    if (!String.IsNullOrEmpty(price_swap))
                                    {
                                        product.sell_price_swap = decimal.Parse(price_swap.Replace(",", "").Replace(".", ""));
                                    }
                                }
                                
                                db.products.Add(product);

                                price_product price_Product = new price_product();
                                price_Product.product_id = product.id;
                                price_Product.price = decimal.Parse(sell_price.Replace(",", "").Replace(".", ""));
                                price_Product.updated_at = currentDate;
                                price_Product.unit = product.unit;
                                db.price_product.Add(price_Product);
                                if (CheckSwapValue == Constants.UNIT_CONVERT)
                                {
                                    price_product price_Product_swap = new price_product();
                                    price_Product_swap.product_id = product.id;
                                    price_Product_swap.price = decimal.Parse(price_swap.Replace(",", "").Replace(".", ""));
                                    price_Product_swap.updated_at = currentDate;
                                    price_Product_swap.unit = product.unit_swap;
                                    db.price_product.Add(price_Product_swap);
                                }

                                inventory_order inventory_Order = new inventory_order();
                                inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                                inventory_Order.create_at = currentDate;
                                inventory_Order.update_at = currentDate;
                                inventory_Order.create_by = User.Identity.GetUserId();
                                inventory_Order.Total = decimal.Parse(purchase_price.Replace(",", "").Replace(".", "")) * product.quantity;
                                inventory_Order.state = Constants.PAYED_ORDER;
                                inventory_Order.supplier_id = SupplierDropdown;
                                db.inventory_order.Add(inventory_Order);

                                import_inventory inventory = new import_inventory();
                                inventory.inventory_id = inventory_Order.id;
                                inventory.product_id = product.id;
                                inventory.quantity = product.quantity;
                                inventory.price_import = int.Parse(purchase_price.Replace(",", "").Replace(".", ""));
                                inventory.sold = 0;
                                inventory.sold_swap = 0;
                                inventory.return_quantity = 0;
                                inventory.quantity_remaining = 0;
                                inventory.created_by = User.Identity.GetUserId();
                                inventory.created_at = currentDate;
                                inventory.supplier_id = SupplierDropdown;
                                db.import_inventory.Add(inventory);
                                db.SaveChanges();
                                message = "Tạo sản phẩm thành công";
                                return Json(new { status, message, id = product.id }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    status = false;
                    message = e.Message;
                }
                return Json(new { status, message, id = product.id }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.category_id = new SelectList(db.categories, "id", "code", product.category_id);
            ViewBag.group_id = new SelectList(db.groups, "id", "code", product.group_id);
            ViewBag.created_by = new SelectList(db.users, "id", "name", product.created_by);
            return View("_Form",product);
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

        public ActionResult _PriceHistory(int id)
        {
            var priceHistory = db.price_product.Where(x=> x.product_id == id);
            var product = db.products.Find(id);
            ViewBag.Product = product;
            return PartialView(priceHistory.ToList());

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
            string price_swap, int check_swap, int SupplierDropdown)
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
                        if (check_swap == Constants.UNIT_CONVERT && (quantity_swap == null || String.IsNullOrEmpty(unit_swap) || String.IsNullOrEmpty(price_swap)))
                        {
                            status = false;
                            message = "Thông tin quy đổi còn trống !";
                        }
                        else
                        {
                            product product = new product();
                            product.name = name_product;
                            product.status = Constants.SHOW_STATUS;
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
                            inventory.return_quantity = 0;
                            inventory.quantity_remaining = 0;
                            inventory.created_by = User.Identity.GetUserId();
                            inventory.created_at = DateTime.Now;
                            inventory.supplier_id = SupplierDropdown;
                            db.import_inventory.Add(inventory);
                            db.SaveChanges();
                            message = "Tạo sản phẩm thành công";
                            return Json(new { status, message, id = product.id }, JsonRequestBehavior.AllowGet);
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
                if (check <= Constants.UNIT_CONVERT)
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
            if (product.status == Constants.SHOW_STATUS)
            {
                product.status = Constants.HIDDEN_STATUS;
            }
            else
            {
                product.status = Constants.SHOW_STATUS;
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

        [HttpPost]
        public JsonResult GetSearchValue(string search)
        {
            var product = (from Product in db.products
                           where Product.name.StartsWith(search) && Product.status != 3
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
            emp.name = product.name;
            emp.note = "Số lượng tồn: " + product.quantity + product.unit;
            if (product.quantity_swap != null)
            {
                emp.name_group = string.Format("{0:0,00}", product.sell_price_swap);
                emp.note += "/" + product.quantity_swap + product.unit_swap;
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                if (product.quantity_remaning != null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
                {
                    emp.note += " và " + product.quantity_remaning + product.unit_swap + " lẻ";
                }
            }


            emp.quantity = product.quantity;
            emp.unit = product.unit;
            emp.unit_swap = product.unit_swap;
            emp.name_category = product.sell_price.ToString("N0");
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
        public JsonResult CheckProductNameAvailability(string name)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.products.Where(x => x.name == name).FirstOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

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
