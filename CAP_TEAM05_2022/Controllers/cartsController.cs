using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Models;

namespace CAP_TEAM05_2022.Controllers
{
    public class cartsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: carts
        public ActionResult Index(int customer_id)
        {
            var carts = db.carts.Include(c => c.product).Include(c => c.customer).Where(c => c.customer_id == customer_id);
         
            return PartialView(carts.ToList().OrderByDescending(c => c.id));
        }

        public JsonResult CreateCart(cart cart_create)
        {
            cart cart  = new cart();
            cart.product_id = cart_create.product_id;
            cart.customer_id = cart_create.customer_id;
            cart.quantity = cart_create.quantity;
            cart.price = cart_create.price*1000;
            cart.discount = cart_create.discount;
            cart.note = cart_create.note;
            db.carts.Add(cart);
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
