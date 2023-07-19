using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    public class inventory_orderController : Controller
    {
        public inventory_orderController()
        {
            ViewBag.isCreate = false;
        }
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: inventory_order
        public ActionResult Index()
        {
            var inventory_order = db.inventory_order.Include(i => i.customer).Include(i => i.user);
            return View(inventory_order.ToList());
        }
        public ActionResult ScriptContainer()
        {
            return PartialView();
        }
        public ActionResult _HistoryInventory(int order_customer, int? method)
        {
            var HistoryOrder = db.inventory_order.Where(o => o.supplier_id == order_customer);
            if (method == Constants.DEBT_ORDER)
            {
                HistoryOrder = HistoryOrder.Where(s => s.state == Constants.DEBT_ORDER);
            }
            return PartialView(HistoryOrder.OrderByDescending(o => o.id).ToList());
        }
        public ActionResult _InventoryList(DateTime? date_Start, DateTime? date_End, int? supplierID)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.inventory_order.Include(i => i.customer).Include(i => i.user).Where(s => s.is_old_debt == false 
                                                    && s.create_at >= date_Start && s.create_at <= date_End
                                                    || s.create_at.Day == date_Start.Value.Day
                                                    && s.create_at.Month == date_Start.Value.Month
                                                    && s.create_at.Year == date_Start.Value.Year
                                                    || s.create_at.Day == date_End.Value.Day
                                                    && s.create_at.Month == date_End.Value.Month
                                                    && s.create_at.Year == date_End.Value.Year);
            if (supplierID != null)
            {
                sales = sales.Where(x => x.supplier_id == supplierID);
            }
            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _InventoryDetails(int inventor_id)
        {
            var inventory = db.inventory_order.Find(inventor_id);
            ViewBag.Inventory = inventory;
            var OrderDetailsList = db.import_inventory.Where(o => o.inventory_id == inventor_id);
            return PartialView(OrderDetailsList.ToList());
        }

        // GET: inventory_order/Create
        public ActionResult Create()
        {
            ViewBag.Product = db.products.Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == Constants.SUPPLIER && c.status == Constants.SHOW_STATUS), "id", "name");
            ViewBag.isCreate = true;
            return View();
        }
        public ActionResult CreateDetails(int? supplierId)
        {
            ViewBag.Product = db.products.Where(x=> x.supplier_id == supplierId).Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == Constants.SUPPLIER && c.status == Constants.SHOW_STATUS), "id", "name");
            ViewBag.isCreate = true;
            return PartialView("CreateDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,supplier_id,create_at,update_at,create_by,state,Total,payment,debt")] inventory_order inventory_order,
            List<import_inventory> importInventory, int method, string Repayment, string[] saleprice, string[] debtPrice, string[] priceImport)
        {
            string message = "";
            bool status = true;
            try
            {
                if (importInventory == null || importInventory.Count == 0)
                {
                    ModelState.AddModelError("importInventoryError", "Vui lòng nhập chi tiết các sản phẩm nhập kho");
                     message = "Vui lòng nhập chi tiết các sản phẩm cần nhập kho";
                     status = false;
                }
                if (ModelState.IsValid)
                {

                    DateTime currentDate = DateTime.Now;
                    decimal total = 0;
                    int i = 0;
                    foreach (var inventory in importInventory)
                    {

                        inventory.inventory_id = inventory_order.id;
                        inventory.sold = 0;
                        inventory.sold_swap = 0;
                        inventory.return_quantity = 0;
                        inventory.quantity_remaining = 0;
                        inventory.created_by = User.Identity.GetUserId();
                        inventory.created_at = currentDate;
                        inventory.created_at = currentDate;
                        inventory.supplier_id = inventory_order.supplier_id;
                        inventory.price_import = decimal.Parse(priceImport[i].Replace(",", "").Replace(".", ""));
                        db.import_inventory.Add(inventory);
                        total += inventory.price_import * inventory.quantity;


                        product product = db.products.Find(inventory.product_id);
                        decimal sellPrice = decimal.Parse(saleprice[i].Replace(",", "").Replace(".", ""));
                        decimal debtPrice1 = decimal.Parse(debtPrice[i].Replace(",", "").Replace(".", ""));
                        decimal priceSwap = sellPrice / product.quantity_swap;
                        decimal debtPriceSwap = debtPrice1 / product.quantity_swap;
                        if (sellPrice != product.sell_price || debtPrice1 != product.sell_price_debt)
                        {
                            price_product price_Product = new price_product();
                            price_Product.product_id = product.id;
                            price_Product.price = sellPrice;
                            price_Product.price_debt = debtPrice1;
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
                        product.quantity += inventory.quantity;
                        product.sell_price = sellPrice;
                        product.sell_price_debt = debtPrice1;
                        product.sell_price_swap = priceSwap;
                        product.sell_price_debt_swap = debtPriceSwap;
                        db.Entry(product).State = EntityState.Modified;
                        i++;
                    }
                    var latestInventoryOrder = db.inventory_order.OrderByDescending(x => x.id).FirstOrDefault(s => s.create_at.Day == currentDate.Day
                         && s.create_at.Month == currentDate.Month
                         && s.create_at.Year == currentDate.Year
                         && !s.is_old_debt);
                    int InventoryOrderId = latestInventoryOrder != null ? int.Parse(latestInventoryOrder.code.Split('-').Last()) + 1 : 1;

                    inventory_order.code = $"MPN-{DateTime.Now:ddMMyy}-{InventoryOrderId:D3}";
                    inventory_order.create_at = currentDate;
                    inventory_order.update_at = currentDate;
                    inventory_order.create_by = User.Identity.GetUserId();
                    inventory_order.Total = total;
                    inventory_order.state = method;
                    inventory_order.is_old_debt = false;
                    if (method == Constants.DEBT_ORDER)
                    {
                        decimal payment = decimal.Parse(Repayment.Replace(",", "").Replace(".", ""));
                        inventory_order.payment = payment;
                        inventory_order.pay_debt = 0;
                        inventory_order.debt = inventory_order.Total - inventory_order.payment;
                        if (payment > total)
                        {
                            message = "Số tiền trả trước đang lớn hơn tổng giá trị đơn nhập hàng !";
                            status = false;
                            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    db.inventory_order.Add(inventory_order);
                    if (method == Constants.DEBT_ORDER)
                    {

                        var check_debt = db.debts.Where(d => d.inventory_order.supplier_id == inventory_order.supplier_id && d.inventory_id != null).Count();
                        if (check_debt > 0)
                        {
                            var last_debt = db.debts.Where(d => d.inventory_order.supplier_id == inventory_order.supplier_id && d.inventory_id != null).OrderByDescending(o => o.id).FirstOrDefault();
                            debt debt = new debt();
                            debt.inventory_id = inventory_order.id;
                            debt.paid = inventory_order.payment;
                            debt.created_at = currentDate;
                            debt.created_by = User.Identity.GetUserId();
                            debt.total = last_debt.total + inventory_order.payment;
                            debt.debt1 = total;
                            debt.remaining = last_debt.remaining + (total - debt.paid);
                            db.debts.Add(debt);

                            var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == inventory_order.supplier_id && d.inventory_id != null).OrderByDescending(o => o.id).FirstOrDefault();
                            customer_debt customer_Debt = new customer_debt();
                            customer_Debt.inventory_id = inventory_order.id;
                            customer_Debt.created_at = currentDate;
                            customer_Debt.created_by = User.Identity.GetUserId();
                            customer_Debt.customer_id = inventory_order.supplier_id;
                            customer_Debt.debt = (total - debt.paid);
                            customer_Debt.remaining = last_debt.remaining + (total - debt.paid);
                            db.customer_debt.Add(customer_Debt);
                        }
                        else
                        {
                            debt debt = new debt();
                            debt.inventory_id = inventory_order.id;
                            debt.paid = inventory_order.payment;
                            debt.created_at = currentDate;
                            debt.created_by = User.Identity.GetUserId();
                            debt.total = inventory_order.payment;
                            debt.debt1 = total;
                            debt.remaining = total - debt.paid;
                            db.debts.Add(debt);

                            customer_debt customer_Debt = new customer_debt();
                            customer_Debt.inventory_id = inventory_order.id;
                            customer_Debt.created_at = currentDate;
                            customer_Debt.created_by = User.Identity.GetUserId();
                            customer_Debt.customer_id = inventory_order.supplier_id;
                            customer_Debt.debt = total - debt.paid;
                            customer_Debt.remaining = total - debt.paid;
                            db.customer_debt.Add(customer_Debt);
                        }
                    }
                    db.SaveChanges();
                    message = "Tạo đơn nhập hàng thành công.";
                    return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
           
            ViewBag.Uniform = db.products.Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == Constants.SUPPLIER && c.status == Constants.SHOW_STATUS), "id", "name");
            ViewBag.isCreate = true;
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete_InventoryOrder(inventory_order order)
        {
            bool status = true;
            string mess = "";
            try
            {
                inventory_order inventory_Order = db.inventory_order.Find(order.id);
                foreach (var item in inventory_Order.import_inventory.ToList())
                {
                    if (item.sold > 0 || item.quantity_remaining > 0)
                    {
                        status = false;
                        mess = "Xóa thất bại ! Sản phẩm trong đơn nhập hàng đã được bán";
                        return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
                    }
                }

                if (inventory_Order.state == Constants.DEBT_ORDER)
                {
                    debt debt = db.debts.Where(x => x.inventory_id == inventory_Order.id).FirstOrDefault();
                    customer_debt customer_Debt = db.customer_debt.Where(x => x.inventory_id == inventory_Order.id).FirstOrDefault();

                    bool checkAfterDebt = db.debts.Where(x => x.inventory_id == inventory_Order.id && x.created_at > debt.created_at && x.id != debt.id).Any();
                    bool checkAftercustomer_Debt = db.customer_debt.Where(x => x.customer_id == inventory_Order.supplier_id && x.created_at > customer_Debt.created_at && x.id != customer_Debt.id).Any();
                    if (checkAfterDebt || checkAftercustomer_Debt)
                    {
                        status = false;
                        mess = "Xóa thất bại ! Đơn nhập hàng đã được ghi nhận và đang trong quá trình trả nợ.";
                        return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.debts.Remove(debt);
                        db.customer_debt.Remove(customer_Debt);
                        foreach (var item in inventory_Order.import_inventory.ToList())
                        {
                            product product = db.products.Find(item.product_id);
                            product.quantity -= item.quantity;
                            db.Entry(product).State = EntityState.Modified;
                            db.import_inventory.Remove(item);
                        }
                        db.inventory_order.Remove(inventory_Order);
                    }
                }
                else
                {
                    foreach (var item in inventory_Order.import_inventory.ToList())
                    {
                        product product = db.products.Find(item.product_id);
                        product.quantity -= item.quantity;
                        db.Entry(product).State = EntityState.Modified;
                        db.import_inventory.Remove(item);
                    }
                    db.inventory_order.Remove(inventory_Order);
                }

                db.SaveChanges();
                mess = "Xóa đơn nhập hàng thành công";
            }
            catch (Exception e)
            {
                status = false;
                mess = e.Message;
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
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
