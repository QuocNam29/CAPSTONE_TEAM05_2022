using CAP_TEAM05_2022.Helper;
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
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.customers.Where(x => x.type == Constants.SUPPLIER).ToList(), "Id", "Name");
            return View();

        }
        [HttpGet]
        public PartialViewResult _Form(int? id)
        {
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
            ViewBag.SupplierId = db.customers.Where(x => x.type == Constants.SUPPLIER).ToList();
            if (id != null)
            {
                product product = db.products.Find(id);
                return PartialView("_Form", product);
            }
            ViewBag.isCreate = true;
            return PartialView("_Form", new product());
        }

        [HttpGet]
        public PartialViewResult _FormEdit(int? id)
        {
            product product = db.products.Find(id);
            ViewBag.isCreate = true;
            ViewBag.CategoryId = new SelectList(db.categories, "Id", "Name");
            ViewBag.SupplierId = new SelectList(db.customers.Where(x => x.type == Constants.SUPPLIER).ToList(), "Id", "Name");
            return PartialView("_FormEdit", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,group_id,category_id,unit,quantity,unit_swap,quantity_swap")] product product,
                                   string purchase_price, string sell_price, string debt_price, string price_swap, string debt_price_swap, int SupplierDropdown)
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
                            decimal purchasePrice = decimal.Parse(purchase_price.Replace(",", "").Replace(".", ","));
                            decimal sellPrice = decimal.Parse(sell_price.Replace(",", "").Replace(".", ","));
                            decimal debtPrice = decimal.Parse(debt_price.Replace(",", "").Replace(".", ","));
                            decimal debtPriceSwap = 0;
                            decimal priceSwap = 0;
                            if (!String.IsNullOrEmpty(debt_price_swap))
                            {
                                 debtPriceSwap = decimal.Parse(debt_price_swap.Replace(",", "").Replace(".", ","));
                            }
                            else
                            {
                                 debtPriceSwap = debtPrice / product.quantity_swap;
                            }
                            if (!String.IsNullOrEmpty(price_swap))
                            {
                                priceSwap = decimal.Parse(price_swap.Replace(",", "").Replace(".", ","));
                            }
                            else
                            {
                                priceSwap = debtPrice / product.quantity_swap;
                            }

                            product.status = Constants.SHOW_STATUS;
                            product.created_by = User.Identity.GetUserId();
                            product.sell_price = sellPrice;
                            product.sell_price_swap = priceSwap;
                            product.sell_price_debt = debtPrice;
                            product.sell_price_debt_swap = debtPriceSwap;
                            product.purchase_price = purchasePrice;
                            product.created_at = currentDate;
                            product.code = "SP" + CodeRandom.RandomCode();
                            product.supplier_id = SupplierDropdown;
                            db.products.Add(product);

                            price_product price_Product = new price_product();
                            price_Product.product_id = product.id;
                            price_Product.price = sellPrice;
                            price_Product.price_debt = debtPrice;
                            price_Product.updated_at = currentDate;
                            price_Product.unit = product.unit;
                            db.price_product.Add(price_Product);

                            price_product price_Product_swap = new price_product();
                            price_Product_swap.product_id = product.id;
                            price_Product_swap.price = priceSwap;
                            price_Product_swap.price_debt = debtPriceSwap;
                            price_Product_swap.updated_at = currentDate;
                            price_Product_swap.unit = product.unit_swap;
                            db.price_product.Add(price_Product_swap);

                            inventory_order inventory_Order = new inventory_order();
                            inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                            inventory_Order.create_at = currentDate;
                            inventory_Order.update_at = currentDate;
                            inventory_Order.create_by = User.Identity.GetUserId();
                            inventory_Order.Total = purchasePrice * product.quantity;
                            inventory_Order.state = Constants.PAYED_ORDER;
                            inventory_Order.supplier_id = SupplierDropdown;
                            db.inventory_order.Add(inventory_Order);

                            import_inventory inventory = new import_inventory();
                            inventory.inventory_id = inventory_Order.id;
                            inventory.product_id = product.id;
                            inventory.quantity = product.quantity;
                            inventory.price_import = purchasePrice;
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
            return View("_Form", product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,name,group_id,category_id,unit,purchase_price,sell_price,quantity,status,note,created_by,created_at,updated_at,deleted_at,name_group,name_category,unit_swap,quantity_swap,quantity_remaning,sell_price_swap, supplier_id")] product product,
                                    string Editsell_price, string Editprice_swap, string Editdebt_price, string Editdebt_price_swap)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                bool status = true;
                DateTime currentDate = DateTime.Now;
                try
                {
                    bool check = db.products.Where(p => p.name == product.name
                                            && p.group_id == product.group_id
                                            && p.category_id == product.category_id
                                            && p.unit == product.unit
                                            && p.id != product.id).Any();
                    if (check)
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
                            decimal sellPrice = decimal.Parse(Editsell_price.Replace(",", "").Replace(".", ","));
                            decimal debtPrice = decimal.Parse(Editdebt_price.Replace(",", "").Replace(".", ","));
                            decimal debtPriceSwap = 0;
                            decimal priceSwap = 0;
                            if (!String.IsNullOrEmpty(Editdebt_price_swap))
                            {
                                debtPriceSwap = decimal.Parse(Editdebt_price_swap.Replace(",", "").Replace(".", ","));
                            }
                            else
                            {
                                debtPriceSwap = debtPrice / product.quantity_swap;
                            }
                            if (!String.IsNullOrEmpty(Editprice_swap))
                            {
                                priceSwap = decimal.Parse(Editprice_swap.Replace(",", "").Replace(".", ","));
                            }
                            else
                            {
                                priceSwap = debtPrice / product.quantity_swap;
                            }

                            if (sellPrice != product.sell_price || debtPrice != product.sell_price_debt)
                            {
                                price_product price_Product = new price_product();
                                price_Product.product_id = product.id;
                                price_Product.price = sellPrice;
                                price_Product.price_debt = debtPrice;
                                price_Product.updated_at = currentDate;
                                price_Product.unit = product.unit;
                                db.price_product.Add(price_Product);
                            }
                            if (priceSwap != product.sell_price_swap || debtPriceSwap != product.sell_price_debt_swap)
                            {
                                price_product price_Product_swap = new price_product();
                                price_Product_swap.product_id = product.id;
                                price_Product_swap.price = priceSwap;
                                price_Product_swap.price_debt = debtPriceSwap;
                                price_Product_swap.updated_at = currentDate;
                                price_Product_swap.unit = product.unit_swap;
                                db.price_product.Add(price_Product_swap);
                            }
                            product.updated_at = currentDate;
                            product.sell_price = sellPrice;
                            product.sell_price_swap = priceSwap;
                            product.sell_price_debt = debtPrice;
                            product.sell_price_debt_swap = debtPriceSwap;
                            db.Entry(product).State = EntityState.Modified;
                            db.SaveChanges();
                            message = "Cập nhật sản phẩm thành công";
                            return Json(new { status, message, id = product.id }, JsonRequestBehavior.AllowGet);
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
            return View("_Form", product);
        }

        [HttpGet]
        public ActionResult _ProductList( int? category_id, int? supplier_id)
        {
            var links = from l in db.products
                        select l;
            if (category_id != null)
            {
                links = links.Where(p => p.category_id == category_id);
            }
            if (supplier_id != null)
            {
                links = links.Where(p => p.supplier_id == supplier_id);
            }
            return PartialView(links.Where(c => c.status != 3).OrderByDescending(c => c.id));
        }

        [HttpGet]
        public ActionResult _PriceHistory(int id)
        {
            var priceHistory = db.price_product.Where(x => x.product_id == id);
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
            string sell_price, string purchase_price, int quantity_swap, string unit_swap,
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
                    inventory_order invenOrder = inventory.inventory_order;
                    db.import_inventory.Remove(inventory);
                    if (!invenOrder.import_inventory.Any())
                    {
                        db.inventory_order.Remove(invenOrder);
                    }
                }
                var priceProduct = db.price_product.Where(x => x.product_id == product.id).ToList();
                foreach (var item in priceProduct)
                {
                    db.price_product.Remove(item);
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
            emp.sell_price_swap = (int)product.sell_price_swap;
            emp.quantity = product.quantity;
            emp.unit = product.unit;
            emp.unit_swap = product.unit_swap;
            emp.sell_price = (int)product.sell_price;
            emp.sell_price_debt = (int)product.sell_price_debt;
            emp.note = "Số lượng tồn: " + product.quantity + product.unit;
            emp.note += "/" + product.quantity_swap + product.unit_swap;
            emp.note += " và " + product.quantity_remaning + product.unit_swap + " lẻ";
            return Json(emp);
        }

        [HttpPost]
        public JsonResult FindProduct_Select2(int product_id, int? isDebtPrice)
        {
            product product = db.products.Find(product_id);
            var emp = new product();
            emp.id = product.id;
            emp.code = product.code;
            emp.name = product.name;
            emp.name_group = isDebtPrice == 1 ? string.Format("{0:0,00}", product.sell_price_debt_swap) : string.Format("{0:0,00}", product.sell_price_swap);
            emp.quantity = product.quantity;
            emp.unit = product.unit;
            emp.unit_swap = product.unit_swap;
            emp.name_category = isDebtPrice == 1 ? product.sell_price_debt.ToString("N0") :  product.sell_price.ToString("N0");
            emp.note = "Số lượng tồn: " + product.quantity + product.unit;
            emp.note += "/" + product.quantity_swap + product.unit_swap;
            emp.note += " và " + product.quantity_remaning + product.unit_swap + " lẻ";
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
