using CAP_TEAM05_2022.Models;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateCart([Bind(Include = "product_id, unit, customer_id, note ")] cart cart_create, string quantity)
        {
            //Server
            double quantity_test = double.Parse(quantity.Replace(",", "."));
            //local
            //double quantity_test = double.Parse(quantity.Replace(".", ","));
            int temp_quantity = (int)(quantity_test);
            double temp_check = (quantity_test - temp_quantity);

            cart_create.quantity = temp_quantity;
            product product = db.products.Find(cart_create.product_id);
            if (cart_create.unit == product.unit)
            {
                if (cart_create.quantity > product.quantity)
                {
                    string message1 = product.quantity.ToString() + " " + product.unit;
                    if (product.unit_swap != null)
                    {
                        message1 += " và " + (product.quantity * product.quantity_swap + product.quantity_remaning) + " " + product.unit_swap + temp_check;
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
                        message1 += " và " + (product.quantity * product.quantity_swap + product.quantity_remaning) + " " + product.unit_swap + temp_check;
                    }
                    bool status1 = false;
                    return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                }
            }
            if (temp_quantity > 0)
            {
                var price = db.price_product.Where(x => x.product_id == cart_create.product_id && x.unit == cart_create.unit);
                cart cart_check = db.carts.Where(c => c.customer_id == cart_create.customer_id && c.product_id == cart_create.product_id && c.unit == cart_create.unit).FirstOrDefault();
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
                            double temp_2 = (double)((check_quantity * 1.0000000) / product.quantity_swap) - temp_1;
                            if (temp_2 > 0)
                            {
                                product.quantity -= (temp_1 + 1);
                                int temp = (int)(product.quantity_swap * (1 - temp_2));
                                product.quantity_remaning += temp;
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

            if (temp_check > 0)
            {
                int temp_finish = (int)(product.quantity_swap * (temp_check));
                var price_swap = db.price_product.Where(x => x.product_id == cart_create.product_id && x.unit != cart_create.unit);
                cart cart_check_swap = db.carts.Where(c => c.customer_id == cart_create.customer_id && c.product_id == cart_create.product_id && c.unit != cart_create.unit).FirstOrDefault();
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

                int check_quantity = temp_finish;
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
                            double temp_2 = (double)((check_quantity * 1.0000000) / product.quantity_swap) - temp_1;
                            if (temp_2 > 0)
                            {
                                product.quantity -= (temp_1 + 1);
                                int temp = (int)(product.quantity_swap * (1 - temp_2));
                                product.quantity_remaning += temp;
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
        public JsonResult UpdateCart([Bind(Include = "id, product_id, unit, quantity, customer_id, note ")] cart cart_create)
        {
            string message = "";
            bool status = true;
            try
            {
                product product = db.products.Find(cart_create.product_id);
                var price = db.price_product.Where(x => x.product_id == cart_create.product_id && x.unit == cart_create.unit);

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

                    product.quantity = quantity - cart_create.quantity;
                    db.Entry(product).State = EntityState.Modified;
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
                    cart.price = price.Any() ? (int)price.OrderByDescending(x => x.id).FirstOrDefault().price : 0;
                    cart.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;
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
