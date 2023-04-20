using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class cartsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public cartsController()
        {
            ViewBag.isNewCreate = false;
        }
        // GET: carts
        public ActionResult Index(int customer_id)
        {
            string userID = User.Identity.GetUserId();
            var carts = db.carts.Include(c => c.product).Include(c => c.customer).Where(c => c.customer_id == customer_id &&  c.user_id == userID);
            return PartialView(carts.ToList().OrderByDescending(c => c.id));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateCart([Bind(Include = "product_id, unit, customer_id, note ")] cart cart_create, string quantity)
        {
            string userID = User.Identity.GetUserId();

            //Server
            double quantity_test = double.Parse(quantity.Replace(",", "."));
            //local
            //double quantity_test = double.Parse(quantity.Replace(".", ","));
            int temp_quantity = (int)(quantity_test);
            double temp_check = (quantity_test - temp_quantity);

            cart_create.quantity = temp_quantity;
            product product = db.products.Find(cart_create.product_id);
            // kiếm tra đủ số lượng tỏng kho hay không
            if (cart_create.unit == product.unit)
            {
                if (cart_create.quantity > product.quantity)
                {
                    string message1 = product.quantity.ToString() + " " + product.unit;
                    if (product.unit_swap != null)
                    {
                        message1 += " hoặc " + (product.quantity * product.quantity_swap + product.quantity_remaning) + " " + product.unit_swap;
                    }
                    bool status1 = false;
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
                        message1 += " hoặc " + (product.quantity * product.quantity_swap + product.quantity_remaning) + " " + product.unit_swap;
                    }
                    bool status1 = false;
                    return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                }
            }
            // nếu là đơn vị ban đầu mà số lượng lớn hơn 0  thì thêm vào giỏ hàng
            if (temp_quantity > 0)
            {
                var price = db.price_product.Where(x => x.product_id == cart_create.product_id && x.unit == cart_create.unit);
                cart cart_check = db.carts.Where(c => c.customer_id == cart_create.customer_id && c.user_id == userID && c.product_id == cart_create.product_id && c.unit == cart_create.unit).FirstOrDefault();
                if (cart_check == null)
                {
                    cart cart = new cart();
                    cart.product_id = cart_create.product_id;
                    cart.customer_id = cart_create.customer_id;
                    cart.quantity = cart_create.quantity;
                    cart.price = price.Any() ? (int)price.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
                    cart.note = cart_create.note;
                    cart.unit = cart_create.unit;
                    cart.user_id = userID;
                    db.carts.Add(cart);
                    db.SaveChanges();
                }
                else
                {
                    cart_check.quantity += cart_create.quantity;
                    db.Entry(cart_check).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            // nếu là đơn vị quy đổi mà số lượng lớn hơn 0 thì thêm vào giỏ hàng (bán số lẻ (2.5 bao))
            if (temp_check > 0)
            {
                int temp_finish = (int)(product.quantity_swap * (temp_check));
                var price_swap = db.price_product.Where(x => x.product_id == cart_create.product_id && x.unit != cart_create.unit);
                cart cart_check_swap = db.carts.Where(c => c.customer_id == cart_create.customer_id && c.user_id == userID &&  c.product_id == cart_create.product_id && c.unit != cart_create.unit).FirstOrDefault();
                if (cart_check_swap == null)
                {
                    cart cart = new cart();
                    cart.product_id = cart_create.product_id;
                    cart.customer_id = cart_create.customer_id;
                    cart.quantity = temp_finish;
                    cart.price = price_swap.Any() ? (int)price_swap.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price_swap.Any() ? price_swap.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
                    cart.note = cart_create.note;
                    cart.unit = product.unit_swap;
                    db.carts.Add(cart);
                    db.SaveChanges();
                }
                else
                {
                    cart_check_swap.quantity += temp_finish;
                    db.Entry(cart_check_swap).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            db.SaveChanges();
            string message = "Record Saved Successfully";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCart([Bind(Include = "id, product_id, unit, quantity, customer_id, note ")] cart cart_create)
        {
            string message = "";
            bool status = true;
            try
            {
                // tìm sản phẩm
                product product = db.products.Find(cart_create.product_id);
                var price = db.price_product.Where(x => x.product_id == cart_create.product_id && x.unit == cart_create.unit);
                // tìm cart
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
                    cart.price = price.Any() ? (int)price.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();

                    db.SaveChanges();
                }
                else if (cart.unit == cart_create.unit && cart_create.unit != product.unit)
                {
                    int quantity = (int)(cart.quantity + product.quantity_remaning + product.quantity * product.quantity_swap);
                    if (cart_create.quantity > quantity)
                    {
                        string message1 = quantity.ToString() + " " + cart_create.unit;
                        bool status1 = true;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }
                    cart.quantity = cart_create.quantity;
                    cart.unit = cart_create.unit;
                    cart.price = price.Any() ? (int)price.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
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
                    cart.price = price.Any() ? (int)price.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (cart.unit != cart_create.unit && cart_create.unit == product.unit)
                {
                    int quantity = (int)(cart.quantity + product.quantity_remaning + product.quantity * product.quantity_swap);
                    int temp_quantity_check = (int)(quantity / product.quantity_swap);

                    if (cart_create.quantity > temp_quantity_check)
                    {
                        string message1 = temp_quantity_check.ToString() + " " + cart_create.unit;
                        bool status1 = true;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }
                    cart.quantity = cart_create.quantity;
                    cart.unit = cart_create.unit;
                    cart.price = price.Any() ? (int)price.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
                    cart.note = cart_create.note;
                    db.Entry(cart).State = EntityState.Modified;
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
