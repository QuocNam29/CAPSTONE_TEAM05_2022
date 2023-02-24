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
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class cartsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: carts
        public ActionResult Index(int customer_id)
        {
            var carts = db.carts.Include(c => c.product).Include(c => c.customer).Where(c => c.customer_id == customer_id);
         
            return PartialView(carts.ToList().OrderByDescending(c => c.id));
        }
        public ActionResult getCartProduct(int id)
        {
            return Json(db.carts.Include(c => c.product).Include(c => c.customer).Where(c => c.customer_id == id).OrderByDescending(c => c.id).Select(x => new
            {
                cartCode = x.product.code,
                cartName = x.product.name,
                cartUnit = x.unit,
                cartQuantity = x.quantity,
                cartPrice = x.product.sell_price,
                cartTotal = x.price,             
                cartNote = x.note,
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateCart(cart cart_create)
        {
            product product = db.products.Find(cart_create.product_id);
            if (cart_create.unit == product.unit)
            {
                if (cart_create.quantity > product.quantity)
                {
                    string message1 = product.quantity.ToString() + " " + product.unit;
                    if (product.unit_swap != null)
                    {
                        message1 += " và " + (product.quantity * product.quantity_swap + product.quantity_remaning) + " " + product.unit_swap;
                    }
                    bool status1 = true;
                    return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (cart_create.quantity > (product.quantity * product.quantity_swap + product.quantity_remaning))
                {
                    string message1 = product.quantity.ToString() + " " + product.unit;
                    if (product.unit_swap != null)
                    {
                        message1 += " và " + (product.quantity * product.quantity_swap + product.quantity_remaning) + " " + product.unit_swap;
                    }
                    bool status1 = true;
                    return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                }
            }
           
            cart cart_check = db.carts.Where(c => c.customer_id == cart_create.customer_id && c.product_id == cart_create.product_id && c.unit == cart_create.unit).FirstOrDefault();
            if (cart_check == null)
            {
                cart cart = new cart();
                cart.product_id = cart_create.product_id;
                cart.customer_id = cart_create.customer_id;
                cart.quantity = cart_create.quantity;
                cart.price = cart_create.price;
                cart.note = cart_create.note;
                cart.unit = cart_create.unit;
                db.carts.Add(cart);
                db.SaveChanges();
            }
            else
            {
                cart_check.quantity += cart_create.quantity;
                cart_check.price += cart_create.price;
                db.Entry(cart_check).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (cart_create.unit == product.unit)
            {
                product.quantity -= cart_create.quantity;
            }
            else
            {
                int check_quantity = cart_create.quantity;
                while (check_quantity > 0)
                {
                    if (check_quantity <= product.quantity_remaning)
                    {
                        product.quantity_remaning -= check_quantity;
                        check_quantity = 0;
                    }
                    else
                    {
                        if (product.quantity_remaning > 0)
                        {
                            check_quantity -= product.quantity_remaning;
                            product.quantity_remaning = 0;
                        }
                        else
                        {
                            int temp_1 = (int)(check_quantity / product.quantity_swap);
                            double temp_2 = (double)((check_quantity* 1.0000000) / product.quantity_swap) - temp_1;
                            if (temp_2 > 0) 
                            {
                                product.quantity -= (temp_1 + 1);
                                int temp = (int)(product.quantity_swap *(1 - temp_2));
                                product.quantity_remaning +=  temp;
                            }
                            else
                            {
                                product.quantity -= temp_1;
                            }
                            check_quantity = 0;
                        }
                    }
                }
              
            }
            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();
            string message = "Record Saved Successfully";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCart(cart cart_create)
        {
            string message = "";
            bool status = true;
            try
            {
                product product = db.products.Find(cart_create.product_id);
                cart cart = db.carts.Find(cart_create.id);
                if (cart.unit == cart_create.unit && cart_create.unit == product.unit)
                {
                    int quantity = cart.quantity + product.quantity;
                    if (cart_create.quantity > quantity)
                    {
                        string message1 = quantity.ToString() + " " + cart_create.unit;
                        bool status1 = true;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }
                    cart.quantity = cart_create.quantity;
                    cart.unit = cart_create.unit;
                    cart.price = cart_create.price;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();

                    product.quantity = quantity - cart_create.quantity;
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (cart.unit == cart_create.unit && cart_create.unit != product.unit)
                {
                    int quantity = (int)(cart.quantity + product.quantity_remaning + product.quantity*product.quantity_swap);
                    if (cart_create.quantity > quantity)
                    {
                        string message1 = quantity.ToString() + " " + cart_create.unit;
                        bool status1 = true;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }
                    cart.quantity = cart_create.quantity;
                    cart.unit = cart_create.unit;
                    cart.price = cart_create.price;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();

                    int temp_quantity = quantity - cart_create.quantity;
                    int temp_1 = (int)(temp_quantity / product.quantity_swap);
                    int temp_check = (int)(temp_quantity % product.quantity_swap);
                    product.quantity = temp_1;
                    
                    if (temp_check > 0)
                    {
                        product.quantity_remaning = (int)(temp_quantity - (temp_1 * product.quantity_swap));
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (cart.unit != cart_create.unit && cart_create.unit != product.unit)
                {
                    int quantity = (int)(cart.quantity * product.quantity_swap + product.quantity_remaning + product.quantity * product.quantity_swap);
                    if (cart_create.quantity > quantity)
                    {
                        string message1 = quantity.ToString() + " " + cart_create.unit;
                        bool status1 = true;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }
                    cart.quantity = cart_create.quantity;
                    cart.unit = cart_create.unit;
                    cart.price = cart_create.price;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();

                    int temp_quantity = quantity - cart_create.quantity;
                    int temp_1 = (int)(temp_quantity / product.quantity_swap);
                    int temp_check = (int)(temp_quantity % product.quantity_swap);
                    product.quantity = temp_1;

                    if (temp_check > 0)
                    {
                        product.quantity_remaning = (int)(temp_quantity - (temp_1 * product.quantity_swap));
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (cart.unit != cart_create.unit && cart_create.unit == product.unit)
                {
                    int quantity = (int)(cart.quantity  + product.quantity_remaning + product.quantity * product.quantity_swap);
                    int temp_quantity_check  = (int)(quantity / product.quantity_swap);

                    if (cart_create.quantity > temp_quantity_check)
                    {
                        string message1 = temp_quantity_check.ToString() + " " + cart_create.unit;
                        bool status1 = true;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }
                    cart.quantity = cart_create.quantity;
                    cart.unit = cart_create.unit;
                    cart.price = cart_create.price;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();

                    int temp_quantity = (int)(quantity - cart_create.quantity * product.quantity_swap);
                    int temp_1 = (int)(temp_quantity / product.quantity_swap);
                    int temp_check = (int)(temp_quantity % product.quantity_swap);
                    product.quantity = temp_1;

                    if (temp_check > 0)
                    {
                        product.quantity_remaning = (int)(temp_quantity - (temp_1 * product.quantity_swap));
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }

                message = "Record Saved Successfully";
                 status = true;
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_CartProduct(cart cart)
        {
            cart cart1 = db.carts.Find(cart.id);
            product product = db.products.Find(cart1.product_id);
            if (cart1.unit == product.unit)
            {
                product.quantity += cart1.quantity;
            }
            else if (cart1.unit == product.unit_swap)
            {
                int temp_quantity = (int)(cart1.quantity + product.quantity_remaning + product.quantity * product.quantity_swap);
                int temp_1 = (int)(temp_quantity / product.quantity_swap);
                int temp_check = (int)(temp_quantity % product.quantity_swap);
                product.quantity = temp_1;

                if (temp_check > 0)
                {
                    product.quantity_remaning = (int)(temp_quantity - (temp_1 * product.quantity_swap));
                }
            }
            db.Entry(product).State = EntityState.Modified;
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
