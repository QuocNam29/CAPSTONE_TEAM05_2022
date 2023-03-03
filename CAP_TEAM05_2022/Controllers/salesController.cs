using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]

    public class salesController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: sales
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult _OrderDetailsList(int order_id)
        {
            var sale = db.sales.Find(order_id);
            if (sale != null)
            {
                TempData["order_code"] = sale.code;
                TempData["order_total"] = sale.total;
                if (sale.prepayment != null)
                {
                    TempData["order_prepayment"] = sale.prepayment;
                }
            }

            var OrderDetailsList = db.sale_details.Where(o => o.sale_id == order_id);
            return PartialView(OrderDetailsList.ToList());
        }
        public ActionResult _HistoryOrder(int order_customer, int? method)
        {
            var HistoryOrder = db.sales.Where(o => o.customer_id == order_customer);
            if (method == 2)
            {
                HistoryOrder = HistoryOrder.Where(s => s.method == 2);
            }
            return PartialView(HistoryOrder.OrderByDescending(o => o.id).ToList());
        }
        public ActionResult _OrderList(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Day == date_Start.Value.Day
                                                    && s.created_at.Value.Month == date_Start.Value.Month
                                                    && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Day == date_End.Value.Day
                                                    && s.created_at.Value.Month == date_End.Value.Month
                                                    && s.created_at.Value.Year == date_End.Value.Year);

            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }

        public ActionResult Revenue()
        {
            return View(db.revenues.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _RevenueList_Date(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = (DateTime.Now);
            }
            if (date_End == null)
            {
                date_End = (DateTime.Now);
            }
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Day == date_Start.Value.Day
                                                    && s.created_at.Value.Month == date_Start.Value.Month
                                                    && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Day == date_End.Value.Day
                                                    && s.created_at.Value.Month == date_End.Value.Month
                                                    && s.created_at.Value.Year == date_End.Value.Year);

            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _RevenueList_Month(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Include(s => s.customer).Include(s => s.user);

            sales = sales.Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                || s.created_at.Value.Month == date_Start.Value.Month && s.created_at.Value.Year == date_Start.Value.Year
                                                || s.created_at.Value.Month == date_End.Value.Month && s.created_at.Value.Year == date_End.Value.Year);

            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }

        public JsonResult CreateSale(sale createSale)
        {
            string message = "";
            bool status = true;
            try
            {
                sale sale = new sale();
                sale.code = "MDH" + CodeRandom.RandomCode();
                sale.customer_id = createSale.customer_id;
                if (createSale.method > 0)
                {
                    sale.method = 2;
                    sale.prepayment = createSale.method;
                }
                else
                {
                    sale.method = 1;
                }
                sale.total = createSale.total;
                sale.note = createSale.note;
                sale.status = 1;
                sale.created_by = User.Identity.GetUserId();
                sale.created_at = DateTime.Now;
                db.sales.Add(sale);
                db.SaveChanges();
                var cart = db.carts.Where(c => c.customer_id == createSale.customer_id).ToList();
                foreach (var item in cart)
                {
                    sale_details sale_Details = new sale_details();
                    sale_Details.sale_id = sale.id;
                    sale_Details.product_id = item.product_id;
                    sale_Details.sold = item.quantity;
                    sale_Details.price = item.price;
                    sale_Details.unit = item.unit;
                    sale_Details.created_at = DateTime.Now;
                    db.sale_details.Add(sale_Details);
                    int temp_quatity = item.quantity;
                    while (temp_quatity > 0)
                    {
                        import_inventory inventory = db.import_inventory.Where(i => (i.product_id == item.product_id && i.quantity != i.sold)
                        || (i.product_id == item.product_id && i.product.unit_swap == item.unit && i.quantity_remaining > 0)).FirstOrDefault();
                        if (item.unit == inventory.product.unit)
                        {
                            if (temp_quatity <= (inventory.quantity - inventory.sold))
                            {
                                revenue revenue = new revenue();
                                revenue.sale_details_id = sale_Details.id;
                                revenue.inventory_id = inventory.id;
                                revenue.Price = item.price / item.quantity;
                                revenue.quantity = temp_quatity;
                                revenue.unit = item.unit;
                                db.revenues.Add(revenue);
                                inventory.sold += temp_quatity;
                                db.Entry(inventory).State = EntityState.Modified;
                                temp_quatity = 0;
                                db.SaveChanges();
                            }
                            else
                            {
                                int temp_inventory = (inventory.quantity - inventory.sold);
                                if (temp_quatity <= temp_inventory)
                                {
                                    inventory.sold += temp_quatity;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue = new revenue();
                                    revenue.sale_details_id = sale_Details.id;
                                    revenue.inventory_id = inventory.id;
                                    revenue.Price = item.price / item.quantity;
                                    revenue.quantity = temp_inventory;
                                    revenue.unit = item.unit;
                                    db.revenues.Add(revenue);
                                    temp_quatity = 0;
                                }
                                else
                                {
                                    inventory.sold += temp_inventory;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue = new revenue();
                                    revenue.sale_details_id = sale_Details.id;
                                    revenue.inventory_id = inventory.id;
                                    revenue.Price = item.price / item.quantity;
                                    revenue.quantity = temp_inventory;
                                    revenue.unit = item.unit;
                                    db.revenues.Add(revenue);
                                    temp_quatity -= temp_inventory;
                                }
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (temp_quatity <= inventory.quantity_remaining)
                            {
                                revenue revenue = new revenue();
                                revenue.sale_details_id = sale_Details.id;
                                revenue.inventory_id = inventory.id;
                                revenue.Price = item.price / item.quantity;
                                revenue.quantity = temp_quatity;
                                revenue.unit = item.unit;
                                db.revenues.Add(revenue);
                                inventory.sold_swap += temp_quatity;
                                inventory.quantity_remaining -= temp_quatity;
                                db.Entry(inventory).State = EntityState.Modified;
                                temp_quatity = 0;
                                db.SaveChanges();
                            }
                            else
                            {
                                int temp_inventory = (inventory.quantity - inventory.sold);

                                int quantity_remaining = (int)inventory.quantity_remaining;
                                if (temp_quatity <= quantity_remaining)
                                {
                                    inventory.sold_swap += temp_quatity;
                                    inventory.quantity_remaining -= temp_quatity;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue = new revenue();
                                    revenue.sale_details_id = sale_Details.id;
                                    revenue.inventory_id = inventory.id;
                                    revenue.Price = item.price / item.quantity;
                                    revenue.quantity = quantity_remaining;
                                    revenue.unit = item.unit;
                                    db.revenues.Add(revenue);
                                    temp_quatity = 0;
                                }
                                else if (temp_quatity > quantity_remaining && quantity_remaining > 0)
                                {
                                    inventory.sold_swap += temp_quatity;
                                    inventory.quantity_remaining = 0;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue = new revenue();
                                    revenue.sale_details_id = sale_Details.id;
                                    revenue.inventory_id = inventory.id;
                                    revenue.Price = item.price / item.quantity;
                                    revenue.quantity = quantity_remaining;
                                    revenue.unit = item.unit;
                                    db.revenues.Add(revenue);
                                    temp_quatity -= quantity_remaining;
                                }
                                else
                                {
                                    int temp_1 = (int)(temp_quatity / inventory.product.quantity_swap);
                                    int temp_check = (int)(temp_quatity % inventory.product.quantity_swap);

                                    if (temp_check > 0)
                                    {
                                        double temp_2 = (double)((temp_quatity * 1.0000000) / inventory.product.quantity_swap) - temp_1;
                                        if (temp_inventory >= (temp_1 + 1))
                                        {
                                            inventory.sold += (temp_1 + 1);
                                            int temp = (int)(inventory.product.quantity_swap * (1 - temp_2));
                                            inventory.quantity_remaining += temp;
                                            inventory.sold_swap += temp_quatity;
                                            revenue revenue = new revenue();
                                            revenue.sale_details_id = sale_Details.id;
                                            revenue.inventory_id = inventory.id;
                                            revenue.Price = item.price / item.quantity;
                                            revenue.quantity = temp_quatity;
                                            revenue.unit = item.unit;
                                            db.revenues.Add(revenue);
                                            temp_quatity = 0;
                                        }
                                        else
                                        {
                                            inventory.sold += temp_inventory;
                                            int temp = (int)(inventory.product.quantity_swap * temp_inventory);
                                            inventory.sold_swap += temp;
                                            revenue revenue = new revenue();
                                            revenue.sale_details_id = sale_Details.id;
                                            revenue.inventory_id = inventory.id;
                                            revenue.Price = item.price / item.quantity;
                                            revenue.quantity = temp;
                                            revenue.unit = item.unit;
                                            db.revenues.Add(revenue);
                                            temp_quatity -= temp;
                                        }
                                    }
                                    else
                                    {
                                        if (temp_inventory >= temp_1)
                                        {
                                            inventory.sold += temp_1;
                                            int temp_remaining = (int)(inventory.product.quantity_swap * temp_1);
                                            inventory.sold_swap += temp_remaining;
                                            revenue revenue = new revenue();
                                            revenue.sale_details_id = sale_Details.id;
                                            revenue.inventory_id = inventory.id;
                                            revenue.Price = item.price / item.quantity;
                                            revenue.quantity = temp_remaining;
                                            revenue.unit = item.unit;
                                            db.revenues.Add(revenue);
                                            temp_quatity = 0;
                                        }
                                        else
                                        {
                                            inventory.sold += temp_inventory;
                                            int temp = (int)(inventory.product.quantity_swap * temp_inventory);
                                            inventory.sold_swap += temp;
                                            revenue revenue = new revenue();
                                            revenue.sale_details_id = sale_Details.id;
                                            revenue.inventory_id = inventory.id;
                                            revenue.Price = item.price / item.quantity;
                                            revenue.quantity = temp;
                                            revenue.unit = item.unit;
                                            db.revenues.Add(revenue);
                                            temp_quatity -= temp;
                                        }
                                    }
                                    db.Entry(inventory).State = EntityState.Modified;
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                    cart cart1 = db.carts.Find(item.id);
                    db.carts.Remove(cart1);
                    db.SaveChanges();
                }
                if (createSale.method > 0)
                {

                    var check_debt = db.debts.Where(d => d.sale.customer_id == createSale.customer_id).Count();
                    var check_customer_debt = db.customer_debt.Where(d => d.customer_id == createSale.customer_id).Count();
                    if (check_debt > 0)
                    {
                        var last_debt = db.debts.Where(d => d.sale.customer_id == createSale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.paid = createSale.method;
                        debt.created_at = DateTime.Now;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = last_debt.total + createSale.method;
                        debt.debt1 = sale.total;
                        debt.remaining = last_debt.remaining + (sale.total - debt.paid);
                        db.debts.Add(debt);

                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == createSale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.created_at = DateTime.Now;
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
                        debt.paid = createSale.method;
                        debt.created_at = DateTime.Now;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = createSale.method;
                        debt.debt1 = sale.total;
                        debt.remaining = sale.total - debt.paid;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.created_at = DateTime.Now;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = createSale.customer_id;
                        customer_Debt.debt = sale.total - debt.paid;
                        customer_Debt.remaining = sale.total - debt.paid;
                        db.customer_debt.Add(customer_Debt);

                        db.SaveChanges();
                    }
                }
                message = "Bạn có muốn in hóa đơn ?";
                return Json(new
                {
                    status,
                    message,
                    sale_id = sale.id,
                    sale_code = sale.code,
                    sale_method = sale.method,
                    sale_total = String.Format("{0:0,00}", sale.total),
                    sale_prepayment = String.Format("{0:0,00}", sale.prepayment),
                    sale_create = String.Format("{0:HH:mm - dd/MM/yyy}", sale.created_at)
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                message = e.Message;
                status = false;
            }


            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
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
