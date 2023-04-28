using CAP_TEAM05_2022.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    public class LookUpOrderController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        // GET: LookUpOrder
        public ActionResult Index()
        {
            ViewBag.CodeCustomer = false;
            return View();
        }

        public ActionResult LookUpOrder(string code_customer)
        {
            if (!String.IsNullOrEmpty(code_customer))
            {
                var customer = db.customers.FirstOrDefault(c => c.code == code_customer);
                if (customer != null)
                {
                    TempData["customerCode"] = customer.code;
                    TempData["customerName"] = customer.name;
                    var sales = db.sales.Where(s => s.customer.code == code_customer);
                    return View(sales.OrderByDescending(c => c.created_at).ToList());
                }
            }
            ViewBag.CodeCustomer = true;
            return View("Index");
        }
    }
}