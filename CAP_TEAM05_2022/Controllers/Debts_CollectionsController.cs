using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

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
        public ActionResult _DebtsList(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Where(d => d.method == 2).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                     || s.created_at.Value.Day == date_Start.Value.Day
                                                     && s.created_at.Value.Month == date_Start.Value.Month
                                                     && s.created_at.Value.Year == date_Start.Value.Year
                                                     || s.created_at.Value.Day == date_End.Value.Day
                                                     && s.created_at.Value.Month == date_End.Value.Month
                                                     && s.created_at.Value.Year == date_End.Value.Year);
            var customer = db.customers.Where(c => c.sales.Where(s => s.method == 2).Count() > 0).ToList();
            return PartialView(customer.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _DebtsDetailsList(int customer_id)
        {
            var debtDetailsList = db.customer_debt.Where(o => o.customer_id == customer_id);
            return PartialView(debtDetailsList.ToList());
        }
        [HttpPost]
        public JsonResult FindDebt(int id)
        {
            customer customer = db.customers.Find(id);
            var emp = new sale();
            emp.id = id;
            emp.total = customer.sales.Where(s => s.method == 2).Sum(s => s.total);
            emp.prepayment = customer.sales.Where(s => s.method == 2).Sum(s => s.prepayment);
            emp.code = customer.code;
            emp.note = customer.name;
            emp.created_by = customer.phone;

            return Json(emp);
        }
        public ActionResult Create_Debts(int customer_id, string paid, string note)
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
                     message = "Số tiền thu nợ lớn hơn tồn nợ ( tồn nợ: " + String.Format("{0:0,00}", last_customer_Debt.remaining)+"đ )";                 
                     status = false;
                    return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                }
                var sale = db.sales.Where(s => s.customer_id == customer_id && s.total != s.prepayment).ToList();
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
                decimal paid_debt =  decimal.Parse(paid.Replace(",", "").Replace(".", ""));
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