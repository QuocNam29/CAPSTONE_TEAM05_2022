using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;

namespace CAP_TEAM05_2022.Controllers
{
    public class salesController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: sales
        public ActionResult Index()
        {
            var sales = db.sales.Include(s => s.customer).Include(s => s.user);
            return View(sales.ToList());
        }

        public JsonResult CreateSale(sale createSale)
        {
            string email = Session["user_email"].ToString();
            user user = db.users.Where(u => u.email == email).FirstOrDefault();
            sale sale = new sale();
            sale.code = "MDH"+ CodeRandom.RandomCode();
            sale.customer_id = createSale.customer_id;
            sale.method = 1;
            sale.total = createSale.total;
            sale.discount = createSale.discount;
            sale.vat = createSale.vat;
            sale.note = createSale.note;
            sale.status = 1;
            sale.created_by = user.id;
            sale.created_at = DateTime.Now;
            db.sales.Add(sale);
            var cart = db.carts.Where(c => c.customer_id == createSale.customer_id);
            foreach (var item in cart)
            {
                sale_details sale_Details = new sale_details();
                sale_Details.sale_id = sale.id;
                sale_Details.product_id = item.product_id;
                sale_Details.sold = item.quantity;
                sale_Details.price = item.price * 1000;
                sale_Details.discount = item.discount;
                sale_Details.created_at = DateTime.Now;
                db.sale_details.Add(sale_Details);
                cart cart1 = db.carts.Find(item.id);
                db.carts.Remove(cart1);
            }
            db.SaveChanges();
            string message = "Record Saved Successfully ";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
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
