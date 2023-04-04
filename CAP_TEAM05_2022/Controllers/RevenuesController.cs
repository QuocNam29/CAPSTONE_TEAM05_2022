using CAP_TEAM05_2022.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace CAP_TEAM05_2022.Controllers
{
    public class RevenuesController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: Revenues
        public ActionResult Index()
        {
            return View(db.revenues.OrderByDescending(c => c.id).ToList());
        }

        public ActionResult _TemplatePrintRevenues(DateTime? date_Start, DateTime? date_End, string method )
        {
            ViewBag.DateStart = date_Start;
            ViewBag.DateEnd = date_End;
            ViewBag.Method = method == "Month" ? "Month" : "Day";

            return PartialView();
        }

        public ActionResult _PrintRevenueList_Date(DateTime? date_Start, DateTime? date_End)
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

        public ActionResult _PrintRevenueList_Month(DateTime? date_Start, DateTime? date_End)
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
        public ActionResult _OrderDetailsList(int order_id)
        {
            var sale = db.sales.Find(order_id);
            ViewBag.Order = sale;
            var returnSale = db.return_sale.Where(x => x.sale_details.sale_id == order_id).ToList();
            ViewBag.ReturnSale = returnSale;
            var OrderDetailsList = db.sale_details.Where(o => o.sale_id == order_id);
            return PartialView(OrderDetailsList.ToList());
        }

    }
}