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
        public ActionResult getCartProduct(int customer_id)
        {
            return Json(db.carts.Include(c => c.product).Include(c => c.customer).Where(c => c.customer_id == customer_id).OrderByDescending(c => c.id).Select(x => new
            {
                cartCode = x.product.code,
                cartName = x.product.name,
                cartUnit = x.product.unit,
                cartQuantity = x.quantity,
                cartPrice = x.product.sell_price,
                cartDiscount = x.discount,
                cartTotal = x.price,             
                cartNote = x.note,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateCart(cart cart_create)
        {
            product product = db.products.Find(cart_create.product_id);
            if (cart_create.quantity > product.quantity)
            {
                string message1 = product.quantity.ToString();
                bool status1 = true;
                return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
            }
            cart cart_check = db.carts.Where(c => c.customer_id == cart_create.customer_id && c.product_id == cart_create.product_id
            && c.discount == cart_create.discount).FirstOrDefault();
            if (cart_check == null)
            {
                cart cart = new cart();
                cart.product_id = cart_create.product_id;
                cart.customer_id = cart_create.customer_id;
                cart.quantity = cart_create.quantity;
                cart.price = cart_create.price;
                cart.discount = cart_create.discount;
                cart.note = cart_create.note;
                db.carts.Add(cart);
                db.SaveChanges();
            }
            else
            {
                cart_check.quantity += cart_create.quantity;
                cart_check.price = cart_create.price;
                db.Entry(cart_check).State = EntityState.Modified;
                db.SaveChanges();
            }
            
            
            product.quantity -= cart_create.quantity;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            string message = "Record Saved Successfully";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCart(cart cart_create)
        {
            product product = db.products.Find(cart_create.product_id);
            cart cart = db.carts.Find(cart_create.id);
            int quantity = cart.quantity + product.quantity;
            if (cart_create.quantity > quantity)
            {
                string message1 = quantity.ToString();
                bool status1 = true;
                return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
            }
           
            
            cart.quantity = cart_create.quantity;
            cart.price = cart_create.price;
            cart.discount = cart_create.discount;
            cart.note = cart_create.note;
            db.Entry(cart).State = EntityState.Modified;
            db.SaveChanges();

            product.quantity = quantity - cart_create.quantity;
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            string message = "Record Saved Successfully";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_CartProduct(cart cart)
        {
            cart cart1 = db.carts.Find(cart.id);
            db.carts.Remove(cart1);
            db.SaveChanges();
            return Json("Delete_CartProduct", JsonRequestBehavior.AllowGet);
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
