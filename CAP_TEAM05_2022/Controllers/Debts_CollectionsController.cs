using CAP_TEAM05_2022.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            var sales = db.sales.Where(d=> d.method == 2).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Day == date_Start.Value.Day
                                                    && s.created_at.Value.Month == date_Start.Value.Month
                                                    && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Day == date_End.Value.Day
                                                    && s.created_at.Value.Month == date_End.Value.Month
                                                    && s.created_at.Value.Year == date_End.Value.Year);

            return PartialView(sales.OrderByDescending(c => c.id).ToList());
        }
        public ActionResult _DebtsDetailsList(int order_id)
        {
            var sale = db.sales.Find(order_id);
            if (sale != null)
            {
                TempData["order_code"] = sale.code;
                TempData["order_vat"] = sale.vat;
                TempData["order_discount"] = sale.discount;
                TempData["order_total"] = sale.total;
            }

            var OrderDetailsList = db.sale_details.Where(o => o.sale_id == order_id);
            return PartialView(OrderDetailsList.ToList());
        }
    }
}