using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    public class Debts_CollectionsController : Controller
    {
        public Debts_CollectionsController()
        {
            ViewBag.isCreate = false;
            ViewBag.isDebtsCustomer = false;
        }
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: Debts_Collections
        public ActionResult Index()
        {
            ViewBag.isDebtsCustomer = true;
            return View();
        }
        public ActionResult DebtsSupplier()
        {
            return View("Index");
        }

        public ActionResult OldDebtsSupplier()
        {
            return View();
        }

        public ActionResult _OldDebtsSupplierList()
        {
            var sales = db.inventory_order.Include(i => i.customer).Include(i => i.user).Where(s => s.is_old_debt);
            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult OldDebtsCustomer()
        {
            return View();
        }
        public ActionResult _OldDebtsCustomerList()
        {
            var sales = db.sales.Include(i => i.customer).Include(i => i.user).Where(s => s.is_old_debt);
            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }
        [HttpGet]
        public PartialViewResult _FormOldDebt()
        {
            ViewBag.isCreate = false;
            ViewBag.SupplierId = new SelectList(db.customers.Where(x => x.type == Constants.SUPPLIER).ToList(), "id", "name");

            return PartialView("_FormOldDebt", new inventory_order());
        }

        [HttpGet]
        public PartialViewResult _FormOldDebtCustomer()
        {
            ViewBag.isCreate = false;
            ViewBag.CustomerId = new SelectList(db.customers.Where(x => x.type == Constants.CUSTOMER && x.id > 0).ToList(), "id", "name");

            return PartialView("_FormOldDebtCustomer", new sale());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OldDebt([Bind(Include = "id,code,supplier_id,create_at")] inventory_order inventory_order,
                                    string debtPrice, string paymentPrice)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {

                    DateTime currentDate = DateTime.Now;
                    decimal payment = decimal.Parse(paymentPrice.Replace(",", "").Replace(".", ""));
                    decimal total = decimal.Parse(debtPrice.Replace(",", "").Replace(".", ""));

                    var latestInventoryOrder = db.inventory_order.OrderByDescending(x => x.id).FirstOrDefault(s => s.create_at.Day == currentDate.Day
                        && s.create_at.Month == currentDate.Month
                        && s.create_at.Year == currentDate.Year
                        && s.is_old_debt);
                    int InventoryOrderId = latestInventoryOrder != null ? int.Parse(latestInventoryOrder.code.Split('-').Last()) + 1 : 1;

                    inventory_order.code = $"MNC-{DateTime.Now:ddMMyy}-{InventoryOrderId:D3}";
                    inventory_order.update_at = currentDate;
                    inventory_order.create_by = User.Identity.GetUserId();
                    inventory_order.Total = total;
                    inventory_order.state = Constants.DEBT_ORDER;
                    inventory_order.payment = payment;
                    inventory_order.pay_debt = 0;
                    inventory_order.debt = inventory_order.Total - inventory_order.payment;
                    inventory_order.is_old_debt = true;
                    if (inventory_order.payment > inventory_order.Total)
                    {
                        message = "Số tiền trả trước đang lớn hơn tổng giá trị đơn nợ cũ !";
                        status = false;
                        return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                    }
                    db.inventory_order.Add(inventory_order);
                    var check_debt = db.debts.Where(d => d.inventory_order.supplier_id == inventory_order.supplier_id && d.inventory_id != null).Any();
                    if (check_debt)
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

                        db.SaveChanges();
                    }
                    db.SaveChanges();

                    message = "Tạo thành công đơn nợ cũ với công ty cung cấp";
                    return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            ViewBag.isCreate = true;
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OldDebtCustomer([Bind(Include = "id,customer_id,created_at")] sale sale,
                                   string debtPrice, string paymentPrice)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {
                    DateTime currentDate = DateTime.Now;
                    decimal payment = decimal.Parse(paymentPrice.Replace(",", "").Replace(".", ""));
                    decimal total = decimal.Parse(debtPrice.Replace(",", "").Replace(".", ""));
                    var latestOrder = db.sales.OrderByDescending(x => x.id).FirstOrDefault(s => s.created_at.Value.Day == currentDate.Day
                                                    && s.created_at.Value.Month == currentDate.Month
                                                    && s.created_at.Value.Year == currentDate.Year
                                                    && s.is_old_debt);
                    int OrderId = latestOrder != null ? int.Parse(latestOrder.code.Split('-').Last()) + 1 : 1;
                    sale.code = $"DNC-{DateTime.Now:ddMMyy}-{OrderId:D3}";
                    sale.updated_at = currentDate;
                    sale.created_by = User.Identity.GetUserId();
                    sale.total = total;
                    sale.method = Constants.DEBT_ORDER;
                    sale.prepayment = payment;
                    sale.pay_debt = 0;
                    sale.status = Constants.SHOW_STATUS;
                    sale.is_old_debt = true;
                    if (sale.prepayment > sale.total)
                    {
                        message = "Số tiền trả trước đang lớn hơn tổng giá trị đơn nợ cũ !";
                        status = false;
                        return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                    }
                    db.sales.Add(sale);
                    var check_debt = db.debts.Where(d => d.sale.customer_id == sale.customer_id && d.sale_id != null).Count();
                    if (check_debt > 0)
                    {
                        var last_debt = db.debts.Where(d => d.sale.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.paid = sale.prepayment;
                        debt.created_at = currentDate;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = last_debt.total + sale.prepayment;
                        debt.debt1 = sale.total;
                        debt.remaining = last_debt.remaining + (sale.total - debt.paid);
                        db.debts.Add(debt);

                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.sale_id = sale.id;
                        customer_Debt.created_at = currentDate;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = sale.customer_id;
                        customer_Debt.debt = (sale.total - debt.paid);
                        customer_Debt.remaining = last_debt.remaining + (sale.total - debt.paid);
                        db.customer_debt.Add(customer_Debt);
                        db.SaveChanges();
                    }
                    else
                    {
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.paid = sale.prepayment;
                        debt.created_at = currentDate;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = sale.prepayment;
                        debt.debt1 = sale.total;
                        debt.remaining = sale.total - debt.paid;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.sale_id = sale.id;
                        customer_Debt.created_at = currentDate;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = sale.customer_id;
                        customer_Debt.debt = sale.total - debt.paid;
                        customer_Debt.remaining = sale.total - debt.paid;
                        db.customer_debt.Add(customer_Debt);

                        db.SaveChanges();
                    }
                    db.SaveChanges();

                    message = "Tạo thành công đơn nợ cũ với khách hàng (không có chi tiết đơn hàng).";
                    return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            ViewBag.isCreate = true;
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _CustomerDebtsList(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Where(d => d.method == Constants.DEBT_ORDER).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                     || s.created_at.Value.Day == date_Start.Value.Day
                                                     && s.created_at.Value.Month == date_Start.Value.Month
                                                     && s.created_at.Value.Year == date_Start.Value.Year
                                                     || s.created_at.Value.Day == date_End.Value.Day
                                                     && s.created_at.Value.Month == date_End.Value.Month
                                                     && s.created_at.Value.Year == date_End.Value.Year);
            var customer = db.customers.Where(c => c.sales.Where(s => s.method == Constants.DEBT_ORDER).Count() > 0).ToList();
            return PartialView(customer.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _SupplierDebtsList(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Where(d => d.method == Constants.DEBT_ORDER).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                     || s.created_at.Value.Day == date_Start.Value.Day
                                                     && s.created_at.Value.Month == date_Start.Value.Month
                                                     && s.created_at.Value.Year == date_Start.Value.Year
                                                     || s.created_at.Value.Day == date_End.Value.Day
                                                     && s.created_at.Value.Month == date_End.Value.Month
                                                     && s.created_at.Value.Year == date_End.Value.Year);
            var customer = db.customers.Where(c => c.inventory_order.Where(s => s.state == Constants.DEBT_ORDER).Count() > 0).ToList();
            return PartialView(customer.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _DebtsDetailsList(int customer_id)
        {
            var debtDetailsList = db.customer_debt.Where(o => o.customer_id == customer_id);
            return PartialView(debtDetailsList.ToList());
        }
        [HttpPost]
        public JsonResult FindDebt(int id, int method)
        {
            customer customer = db.customers.Find(id);
            var emp = new sale();
            emp.id = id;
            if (method == Constants.COLLECTION_OF_CUSTOMERS)
            {
                emp.total = customer.sales.Where(s => s.method == Constants.DEBT_ORDER).Sum(s => s.total);
                emp.prepayment = customer.sales.Where(s => s.method == Constants.DEBT_ORDER).Sum(s => s.prepayment + s.pay_debt);
            }
            else if (method == Constants.PAYING_SUPPLIER)
            {
                emp.total = (decimal)customer.inventory_order.Where(s => s.state == Constants.DEBT_ORDER).Sum(s => s.Total);
                emp.prepayment = customer.inventory_order.Where(s => s.state == Constants.DEBT_ORDER).Sum(s => s.payment + s.pay_debt);
            }
            emp.code = customer.code;
            emp.note = customer.name;
            emp.created_by = customer.phone;

            return Json(emp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create_Debts(int customer_id, string paid, string note, int method)
        {
            string message = "";
            bool status = true;
            try
            {
                decimal paid_temp = decimal.Parse(paid.Replace(",", "").Replace(".", ""));
                DateTime create_at = DateTime.Now;
                var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                if (paid_temp > last_customer_Debt.remaining)
                {
                    message = "Số tiền thu nợ lớn hơn tồn nợ ( tồn nợ: " + String.Format("{0:0,00}", last_customer_Debt.remaining) + "đ )";
                    status = false;
                    return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                }
                // Đối với thu nợ từ khách hàng
                if (method == Constants.COLLECTION_OF_CUSTOMERS)
                {
                    if (paid_temp > 0)
                    {
                        //Lấy danh sách đơn nợ của khách hàng
                        var sale = db.sales.Where(s => s.customer_id == customer_id && s.total != (s.prepayment + s.pay_debt) && s.method == Constants.DEBT_ORDER).ToList();                  
                        while (paid_temp > 0)
                        {
                            foreach (var item in sale)
                            {
                                if (paid_temp <= (item.total - item.prepayment - item.pay_debt))
                                {
                                    var last_debt = db.debts.Where(d => d.sale.customer_id == item.customer_id).OrderByDescending(o => o.id).FirstOrDefault();

                                    debt debt = new debt();
                                    debt.sale_id = item.id;
                                    debt.created_by = User.Identity.GetUserId();
                                    debt.created_at = create_at;
                                    debt.paid = paid_temp;
                                    debt.total = (decimal)(last_debt.total + paid_temp);
                                    debt.note = note;
                                    debt.remaining = last_debt.remaining - paid_temp;
                                    db.debts.Add(debt);

                                    item.pay_debt += paid_temp;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                    paid_temp = 0;
                                }
                                else
                                {
                                    var last_debt = db.debts.Where(d => d.sale.customer_id == item.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                                    decimal remaining = (decimal)(item.total - item.prepayment - item.pay_debt);
                                    debt debt = new debt();
                                    debt.sale_id = item.id;
                                    debt.created_by = User.Identity.GetUserId();
                                    debt.created_at = create_at;
                                    debt.paid = remaining;
                                    debt.total = (decimal)(last_debt.total + remaining);
                                    debt.note = note;
                                    debt.remaining = last_debt.remaining - remaining;
                                    db.debts.Add(debt);

                                    item.pay_debt += remaining;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                    paid_temp -= remaining;
                                }
                            }

                        }
                    }
                    else
                    {
                        //Lấy danh sách đơn nợ của khách hàng
                        var sale = db.sales.Where(s => s.customer_id == customer_id && s.total != (s.prepayment + s.pay_debt) && s.method == Constants.DEBT_ORDER).ToList();
                        while (paid_temp < 0)
                        {
                            foreach (var item in sale)
                            {
                                if (paid_temp >= (item.total - item.prepayment - item.pay_debt))
                                {
                                    var last_debt = db.debts.Where(d => d.sale.customer_id == item.customer_id).OrderByDescending(o => o.id).FirstOrDefault();

                                    debt debt = new debt();
                                    debt.sale_id = item.id;
                                    debt.created_by = User.Identity.GetUserId();
                                    debt.created_at = create_at;
                                    debt.paid = paid_temp;
                                    debt.total = (decimal)(last_debt.total + paid_temp);
                                    debt.note = note;
                                    debt.remaining = last_debt.remaining - paid_temp;
                                    db.debts.Add(debt);

                                    item.pay_debt += paid_temp;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                    paid_temp = 0;
                                }
                                else
                                {
                                    var last_debt = db.debts.Where(d => d.sale.customer_id == item.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                                    decimal remaining = (decimal)(item.total - item.prepayment - item.pay_debt);
                                    debt debt = new debt();
                                    debt.sale_id = item.id;
                                    debt.created_by = User.Identity.GetUserId();
                                    debt.created_at = create_at;
                                    debt.paid = remaining;
                                    debt.total = (decimal)(last_debt.total + remaining);
                                    debt.note = note;
                                    debt.remaining = last_debt.remaining - remaining;
                                    db.debts.Add(debt);

                                    item.pay_debt += remaining;
                                    db.Entry(item).State = EntityState.Modified;
                                    db.SaveChanges();
                                    paid_temp -= remaining;
                                }
                            }

                        }
                    }
                    
                }
                // Đối với trả nợ cho công ty cung cấp
                else if (method == Constants.PAYING_SUPPLIER)
                {
                    // lấy danh sách đơn nợ của công ty cung cấp
                    var inventory = db.inventory_order.Where(s => s.supplier_id == customer_id && s.Total != (s.payment - s.pay_debt) && s.state == Constants.DEBT_ORDER).ToList();
                    while (paid_temp > 0)
                    {
                        foreach (var item in inventory)
                        {
                            if (paid_temp <= (item.Total - item.payment - item.pay_debt))
                            {
                                var last_debt = db.debts.Where(d => d.inventory_order.supplier_id == item.supplier_id).OrderByDescending(o => o.id).FirstOrDefault();

                                debt debt = new debt();
                                debt.inventory_id = item.id;
                                debt.created_by = User.Identity.GetUserId();
                                debt.created_at = create_at;
                                debt.paid = paid_temp;
                                debt.total = (decimal)(last_debt.total + paid_temp);
                                debt.note = note;
                                debt.remaining = last_debt.remaining - paid_temp;
                                db.debts.Add(debt);

                                item.pay_debt += paid_temp;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                                paid_temp = 0;
                            }
                            else
                            {
                                var last_debt = db.debts.Where(d => d.inventory_order.supplier_id == item.supplier_id).OrderByDescending(o => o.id).FirstOrDefault();
                                decimal remaining = (decimal)(item.Total - item.payment - item.pay_debt);
                                debt debt = new debt();
                                debt.inventory_id = item.id;
                                debt.created_by = User.Identity.GetUserId();
                                debt.created_at = create_at;
                                debt.paid = remaining;
                                debt.total = (decimal)(last_debt.total + remaining);
                                debt.note = note;
                                debt.remaining = last_debt.remaining - remaining;
                                db.debts.Add(debt);
                                item.pay_debt += remaining;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                                paid_temp -= remaining;
                            }
                        }

                    }
                }

                decimal paid_debt = decimal.Parse(paid.Replace(",", "").Replace(".", ""));
                customer_debt customer_Debt = new customer_debt();
                customer_Debt.customer_id = customer_id;
                customer_Debt.created_by = User.Identity.GetUserId();
                customer_Debt.created_at = create_at;
                customer_Debt.paid = paid_debt;
                customer_Debt.remaining = last_customer_Debt.remaining - paid_debt;
                customer_Debt.note = note;
                db.customer_debt.Add(customer_Debt);
                db.SaveChanges();
                message = "Thu nợ thành công";

            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete_OldDebtSupplier(inventory_order order)
        {
            bool status = true;
            string mess = "";
            try
            {
                inventory_order inventory_Order = db.inventory_order.Find(order.id);

                debt debt = db.debts.Where(x => x.inventory_id == inventory_Order.id).FirstOrDefault();
                customer_debt customer_Debt = db.customer_debt.Where(x => x.inventory_id == inventory_Order.id).FirstOrDefault();

                bool checkAfterDebt = db.debts.Where(x => x.inventory_id == inventory_Order.id && x.created_at > debt.created_at && x.id != debt.id).Any();
                bool checkAftercustomer_Debt = db.customer_debt.Where(x => x.customer_id == inventory_Order.supplier_id && x.created_at > customer_Debt.created_at && x.id != customer_Debt.id).Any();
                if (checkAfterDebt || checkAftercustomer_Debt)
                {
                    status = false;
                    mess = "Xóa thất bại ! Nợ đã được ghi nhận và đang trong quá trình trả nợ.";
                    return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.debts.Remove(debt);
                    db.customer_debt.Remove(customer_Debt);
                    db.inventory_order.Remove(inventory_Order);
                }
                db.SaveChanges();
                mess = "Xóa đơn nợ cũ thành công";
            }
            catch(Exception e)
            {
                status = false;
                mess = e.Message;
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete_OldDebtCustomer(sale order)
        {
            bool status = true;
            string mess = "";
            try
            {
                sale sale = db.sales.Find(order.id);
                var sale_Details = db.sale_details.Where(x => x.sale_id == sale.id).ToList();
                debt debt = db.debts.Where(x => x.sale_id == sale.id).FirstOrDefault();
                customer_debt customer_Debt = db.customer_debt.Where(x => x.sale_id == sale.id).FirstOrDefault();

                bool checkAfterDebt = db.debts.Where(x => x.sale_id == sale.id && x.created_at > debt.created_at && x.id != debt.id).Any();
                bool checkAftercustomer_Debt = db.customer_debt.Where(x => x.customer_id == sale.customer_id && x.created_at > customer_Debt.created_at && x.id != customer_Debt.id).Any();
                if (checkAfterDebt || checkAftercustomer_Debt)
                {
                    status = false;
                    mess = "Xóa thất bại ! Nợ đã được ghi nhận và đang trong quá trình trả nợ.";
                    return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db.debts.Remove(debt);
                    db.customer_debt.Remove(customer_Debt);
                    foreach (var item in sale_Details)
                    {
                        db.sale_details.Remove(item);
                    }
                    db.sales.Remove(sale);
                }
                db.SaveChanges();
                mess = "Xóa đơn nợ cũ thành công thành công";
            }
            catch (Exception e)
            {
                status = false;
                mess = e.Message;
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ScriptContainer()
        {
            return PartialView();
        }
        // GET: inventory_order/Create
        public ActionResult CreateOldDebtCustomer()
        {
            ViewBag.Product = db.products.Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == Constants.CUSTOMER), "id", "name");
            ViewBag.isCreate = true;
            return View();
        }
        public ActionResult CreateOldDebtCustomerDetails()
        {
            ViewBag.Product = db.products.Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == Constants.CUSTOMER), "id", "name");
            ViewBag.isCreate = true;
            return PartialView("CreateOldDebtCustomerDetails");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOldDebtCustomer([Bind(Include = "customer_id, created_at, note")] sale createSale,
            List<sale_details> saleDetails,  string Repayment, string[] priceSale)
        {
            string message = "";
            bool status = true;
            try
            {
                if (saleDetails == null || saleDetails.Count == 0)
                {
                    ModelState.AddModelError("saleDetailsError", "Vui lòng nhập chi tiết các sản phẩm của đơn nợ cũ !");
                    message = "Vui lòng nhập chi tiết các sản phẩm của đơn nợ cũ !";
                    status = false;
                }
                if (ModelState.IsValid)
                {
                    decimal prepayment = 0;
                    DateTime currentDate = DateTime.Now;
                    if (!String.IsNullOrEmpty(Repayment))
                    {
                        prepayment = decimal.Parse(Repayment.Replace(",", "").Replace(".", ""));
                    }
                    var latestOrder = db.sales.OrderByDescending(x => x.id).FirstOrDefault(s => s.created_at.Value.Day == currentDate.Day
                                                     && s.created_at.Value.Month == currentDate.Month
                                                     && s.created_at.Value.Year == currentDate.Year
                                                     && s.is_old_debt);
                    int OrderId = latestOrder != null ? int.Parse(latestOrder.code.Split('-').Last()) + 1 : 1;
                    sale sale = new sale();
                    sale.code = $"DNC-{DateTime.Now:ddMMyy}-{OrderId:D3}";
                    sale.customer_id = createSale.customer_id;
                    sale.method = Constants.DEBT_ORDER;
                    sale.prepayment = prepayment;
                    sale.pay_debt = 0;
                    sale.is_debt_price = true;
                    for (int j = 0; j < priceSale.Length; j++)
                    {
                        decimal price = decimal.Parse(priceSale[j].Replace(",", "").Replace(".", ""));
                        sale.total += price * saleDetails[j].sold;
                    }
                    sale.note = createSale.note;
                    sale.status = Constants.SHOW_STATUS;
                    sale.created_by = User.Identity.GetUserId();
                    sale.created_at = createSale.created_at;
                    sale.updated_at = currentDate;
                    sale.is_old_debt = true;
                    if (sale.prepayment > sale.total)
                    {
                        ModelState.AddModelError("saleDetailsError", "Số tiền trả trước đang lớn hơn tổng giá trị đơn nợ cũ !");
                        message = "Số tiền trả trước đang lớn hơn tổng giá trị đơn nợ cũ !";
                        status = false;
                        return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                    }
                    db.sales.Add(sale);
                    int i = 0;
                    foreach (var item in saleDetails)
                    {
                        product product = db.products.Find(item.product_id);
                        item.sale_id = sale.id;
                        decimal price = decimal.Parse(priceSale[i].Replace(",", "").Replace(".", ""));
                        item.price = price;
                        item.unit = product.unit;
                        item.created_at = currentDate;
                        item.updated_at = currentDate;
                        item.return_quantity = 0;
                        db.sale_details.Add(item);
                        i++;
                    }
                    db.SaveChanges();
                    var check_debt = db.debts.Where(d => d.sale.customer_id == createSale.customer_id && d.sale_id != null).Count();
                    if (check_debt > 0)
                    {
                        var last_debt = db.debts.Where(d => d.sale.customer_id == createSale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.paid = sale.prepayment;
                        debt.created_at = currentDate;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = last_debt.total + sale.prepayment;
                        debt.debt1 = sale.total;
                        debt.remaining = last_debt.remaining + (sale.total - debt.paid);
                        db.debts.Add(debt);

                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == createSale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.sale_id = sale.id;
                        customer_Debt.created_at = currentDate;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = createSale.customer_id;
                        customer_Debt.debt = (sale.total - debt.paid);
                        customer_Debt.remaining = last_debt.remaining + (sale.total - debt.paid);
                        db.customer_debt.Add(customer_Debt);
                        db.SaveChanges();
                    }
                    else
                    {
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.paid = sale.prepayment;
                        debt.created_at = currentDate;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = sale.prepayment;
                        debt.debt1 = sale.total;
                        debt.remaining = sale.total - debt.paid;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.sale_id = sale.id;
                        customer_Debt.created_at = currentDate;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = createSale.customer_id;
                        customer_Debt.debt = sale.total - debt.paid;
                        customer_Debt.remaining = sale.total - debt.paid;
                        db.customer_debt.Add(customer_Debt);

                        db.SaveChanges();
                    }
                    message = "Tạo đơn nợ cũ thành công.";
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
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == Constants.SUPPLIER), "id", "name");
            ViewBag.isCreate = true;
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

    }
}