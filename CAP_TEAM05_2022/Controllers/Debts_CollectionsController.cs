using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    public class Debts_CollectionsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: Debts_Collections
        public ActionResult Index()
        {
            return View();
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
            if (method == Constants.PAYED_ORDER)
            {
                emp.total = customer.sales.Where(s => s.method == Constants.DEBT_ORDER).Sum(s => s.total);
                emp.prepayment = customer.sales.Where(s => s.method == Constants.DEBT_ORDER).Sum(s => s.prepayment);
            }
            else if (method == Constants.DEBT_ORDER)
            {
                emp.total = (decimal)customer.inventory_order.Where(s => s.state == Constants.DEBT_ORDER).Sum(s => s.Total);
                emp.prepayment = customer.inventory_order.Where(s => s.state == Constants.DEBT_ORDER).Sum(s => s.payment);
            }
            emp.code = customer.code;
            emp.note = customer.name;
            emp.created_by = customer.phone;

            return Json(emp);
        }
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
                    //Lấy danh sách đơn nợ của khách hàng
                    var sale = db.sales.Where(s => s.customer_id == customer_id && s.total != s.prepayment && s.method == Constants.DEBT_ORDER).ToList();
                    while (paid_temp > 0)
                    {
                        foreach (var item in sale)
                        {
                            if (paid_temp <= (item.total - item.prepayment))
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

                                item.prepayment += paid_temp;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                                paid_temp = 0;
                            }
                            else
                            {
                                var last_debt = db.debts.Where(d => d.sale.customer_id == item.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                                decimal remaining = (decimal)(item.total - item.prepayment);
                                debt debt = new debt();
                                debt.sale_id = item.id;
                                debt.created_by = User.Identity.GetUserId();
                                debt.created_at = create_at;
                                debt.paid = remaining;
                                debt.total = (decimal)(last_debt.total + remaining);
                                debt.note = note;
                                debt.remaining = last_debt.remaining - remaining;
                                db.debts.Add(debt);

                                item.prepayment += remaining;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                                paid_temp -= remaining;
                            }
                        }

                    }
                }
                // Đối với trả nợ cho nhà cung cấp
                else if (method == Constants.PAYING_SUPPLIER)
                {
                    // lấy danh sách đơn nợ của nhà cung cấp
                    var inventory = db.inventory_order.Where(s => s.supplier_id == customer_id && s.Total != s.payment && s.state == Constants.DEBT_ORDER).ToList();
                    while (paid_temp > 0)
                    {
                        foreach (var item in inventory)
                        {
                            if (paid_temp <= (item.Total - item.payment))
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

                                item.payment += paid_temp;
                                db.Entry(item).State = EntityState.Modified;
                                db.SaveChanges();
                                paid_temp = 0;
                            }
                            else
                            {
                                var last_debt = db.debts.Where(d => d.inventory_order.supplier_id == item.supplier_id).OrderByDescending(o => o.id).FirstOrDefault();
                                decimal remaining = (decimal)(item.Total - item.payment);
                                debt debt = new debt();
                                debt.inventory_id = item.id;
                                debt.created_by = User.Identity.GetUserId();
                                debt.created_at = create_at;
                                debt.paid = remaining;
                                debt.total = (decimal)(last_debt.total + remaining);
                                debt.note = note;
                                debt.remaining = last_debt.remaining - remaining;
                                db.debts.Add(debt);
                                item.payment += remaining;
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
    }
}