using CAP_TEAM05_2022.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    public class DashboardController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        // GET: Dashboard
        public ActionResult Index(DateTime? from, DateTime? to)
        {
            var data = new StatisticsViewModel();
            var user = db.AspNetUsers.ToList();
            var order = db.sales.Where(x => (!from.HasValue || DbFunctions.TruncateTime(x.created_at) >= DbFunctions.TruncateTime(from.Value))
                && (!to.HasValue || DbFunctions.TruncateTime(x.created_at) <= DbFunctions.TruncateTime(to.Value))).ToList();
            var product = db.products.Where(x => x.status == Constants.SHOW_STATUS).ToList();
            var supplier = db.customers.Where(x => x.type == Constants.SUPPLIER).ToList();

            data.CountUser = user.Count;
            data.CountAdmin = user.Where(x => x.AspNetRoles.FirstOrDefault().Name == Constants.ADMIN_ROLE).ToList().Count;
            data.CountStaff = user.Where(x => x.AspNetRoles.FirstOrDefault().Name == Constants.STAFF_ROLE).ToList().Count;

            data.CountOrder = order.Count;
            data.CountArr1 = new int[3];
            data.CountArr1[0] = order.Where(x => x.method == Constants.PAYED_ORDER).Count();
            data.CountArr1[1] = order.Where(x => x.method == Constants.DEBT_ORDER && x.is_debt_price == false).Count();
            data.CountArr1[2] = order.Where(x => x.method == Constants.DEBT_ORDER && x.is_debt_price == true).Count();

            data.CountSupplier = supplier.Count;


            return View(data);
        }
    }
}