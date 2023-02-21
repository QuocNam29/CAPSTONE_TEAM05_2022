using CAP_TEAM05_2022.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    public class return_saleController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: return_sale
        public ActionResult Index()
        {
            var return_sale = db.return_sale.Include(r => r.sale);
            return View(return_sale.ToList());
        }
        public ActionResult Create_Return(int sale_details_id, int product_Current_id, int quality_OD,
            string price_PCurrent, int return_option, int? product_id, int? input_qualityProduct, string choose_unit)
        {
            string message = "";
            bool status = true;
            try
            {
                string unit = "";
                int quality_OD_revenue = quality_OD;
                sale_details sale_Details = db.sale_details.Find(sale_details_id);
                sale sale = db.sales.Find(sale_Details.sale_id);
                product product = db.products.Find(sale_Details.product_id);
                var revenue = db.revenues.Where(r => r.sale_details_id == sale_details_id).OrderByDescending(o => o.id).ToList();
                unit = sale_Details.unit;
                decimal price_PCurrent1 = decimal.Parse(price_PCurrent.Replace(",", "").Replace(".", ""));
                if (return_option == 2)
                {
                    return_sale return_Sale = new return_sale();
                    return_Sale.sale_id = sale_Details.sale_id;
                    return_Sale.method = return_option;
                    return_Sale.create_at = DateTime.Now;
                    return_Sale.difference = price_PCurrent1 * quality_OD;
                    db.return_sale.Add(return_Sale);
                    return_details return_Details = new return_details();
                    return_Details.return_id = return_Sale.id;
                    return_Details.product_current_id = product_Current_id;
                    return_Details.quantity_current = quality_OD;
                    return_Details.total_current = price_PCurrent1 * quality_OD;
                    return_Details.difference = (decimal)return_Details.total_current;
                    return_Details.unit_current = sale_Details.unit;
                    db.return_details.Add(return_Details);
                    db.SaveChanges();
                    foreach (var item in revenue)
                    {
                        while (quality_OD_revenue > 0)
                        {

                            if (item.quantity > quality_OD_revenue)
                            {
                                item.quantity -= quality_OD_revenue;
                                import_inventory import_Inventory = db.import_inventory.Find(item.inventory_id);

                                if (item.unit == import_Inventory.product.unit)
                                {
                                    import_Inventory.sold -= quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                else
                                {
                                    import_Inventory.quantity_remaining += quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                db.Entry(item).State = EntityState.Modified;

                                quality_OD_revenue = 0;
                            }
                            else
                            {
                                quality_OD_revenue -= item.quantity;
                                import_inventory import_Inventory = db.import_inventory.Find(item.inventory_id);
                                if (item.unit == import_Inventory.product.unit)
                                {
                                    import_Inventory.sold -= quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                else
                                {
                                    import_Inventory.quantity_remaining += quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                db.revenues.Remove(item);
                            }
                            db.SaveChanges();
                        }
                    }
                    if (quality_OD < sale_Details.sold)
                    {
                        decimal price = (sale_Details.price / sale_Details.sold) * quality_OD;
                        sale_Details.price -= price;
                        sale_Details.sold -= quality_OD;
                        db.Entry(sale_Details).State = EntityState.Modified;
                        sale.total -= price;
                        db.Entry(sale).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (sale.sale_details.Count() == 1)
                        {
                            decimal price = (sale_Details.price / sale_Details.sold) * quality_OD;
                            sale.total -= price;
                            db.Entry(sale).State = EntityState.Modified;
                            db.sale_details.Remove(sale_Details);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.sale_details.Remove(sale_Details);
                            db.sales.Remove(sale);
                            db.SaveChanges();
                        }
                    }
                    if (unit == product.unit)
                    {
                        product.quantity += quality_OD;
                        db.Entry(product).State = EntityState.Modified;
                    }
                    else if (unit == product.unit_swap)
                    {
                        product.quantity_remaning += quality_OD;
                        db.Entry(product).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    message = "Trả sản phẩm thành công";
                }
                else if (return_option == 1)
                {
                    product product_check = db.products.Find(product_id);
                    if (choose_unit == product_check.unit)
                    {
                        if (input_qualityProduct > product_check.quantity)
                        {
                            string message1 = "Sản phẩm còn lại " + product_check.quantity.ToString() + " " + product_check.unit;
                            if (product_check.unit_swap != null)
                            {
                                message1 += " và " + (product_check.quantity * product_check.quantity_swap + product_check.quantity_remaning) + " " + product_check.unit_swap;
                            }
                            bool status1 = false;
                            return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        if (input_qualityProduct > (product_check.quantity * product_check.quantity_swap))
                        {
                            string message1 = product_check.quantity.ToString() + " " + product_check.unit;
                            if (product_check.unit_swap != null)
                            {
                                message1 += " hoặc " + (product_check.quantity * product_check.quantity_swap + product_check.quantity_remaning) + " " + product_check.unit_swap;
                            }
                            bool status1 = false;
                            return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                        }
                    }

                    sale_details sale_Details_return = new sale_details();
                    sale_Details_return.sale_id = sale.id;
                    sale_Details_return.product_id = (int)product_id;
                    sale_Details_return.sold = (int)input_qualityProduct;
                    if (product_check.unit == choose_unit)
                    {
                        sale_Details_return.price = (decimal)(product_check.sell_price * input_qualityProduct);
                        sale.total += (decimal)(product_check.sell_price * input_qualityProduct);
                    }
                    else
                    {
                        sale_Details_return.price = (decimal)(product_check.sell_price_swap * input_qualityProduct);
                        sale.total += (decimal)(product_check.sell_price_swap * input_qualityProduct);
                    }
                    sale_Details_return.unit = choose_unit;
                    sale_Details_return.created_at = DateTime.Now;
                    db.sale_details.Add(sale_Details_return);
                    int temp_quatity = (int)input_qualityProduct;
                    while (temp_quatity > 0)
                    {
                        import_inventory inventory = db.import_inventory.Where(i => (i.product_id == product_id && i.quantity != i.sold)
                        || (i.product_id == product_id && i.product.unit_swap == choose_unit && i.quantity_remaining > 0)).FirstOrDefault();
                        if (choose_unit == inventory.product.unit)
                        {
                            if (temp_quatity <= (inventory.quantity - inventory.sold))
                            {
                                revenue revenue_return = new revenue();
                                revenue_return.sale_details_id = sale_Details_return.id;
                                revenue_return.inventory_id = inventory.id;
                                if (product_check.unit == choose_unit)
                                {
                                    revenue_return.Price = product_check.sell_price;
                                }
                                else
                                {
                                    revenue_return.Price = (decimal)product_check.sell_price_swap;
                                }
                                revenue_return.quantity = temp_quatity;
                                revenue_return.unit = choose_unit;
                                db.revenues.Add(revenue_return);
                                inventory.sold += temp_quatity;
                                db.Entry(inventory).State = EntityState.Modified;
                                temp_quatity = 0;
                                db.SaveChanges();
                            }
                            else
                            {
                                int temp_inventory = (inventory.quantity - inventory.sold);
                                if (temp_quatity <= temp_inventory)
                                {
                                    inventory.sold += temp_quatity;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue_return = new revenue();
                                    revenue_return.sale_details_id = sale_Details_return.id;
                                    revenue_return.inventory_id = inventory.id;
                                    revenue_return.inventory_id = inventory.id;
                                    if (product_check.unit == choose_unit)
                                    {
                                        revenue_return.Price = product_check.sell_price;
                                    }
                                    else
                                    {
                                        revenue_return.Price = (decimal)product_check.sell_price_swap;
                                    }
                                    revenue_return.quantity = temp_inventory;
                                    revenue_return.unit = choose_unit;
                                    db.revenues.Add(revenue_return);
                                    temp_quatity = 0;
                                }
                                else
                                {
                                    inventory.sold += temp_inventory;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue_return = new revenue();
                                    revenue_return.sale_details_id = sale_Details_return.id;
                                    revenue_return.inventory_id = inventory.id;
                                    revenue_return.inventory_id = inventory.id;
                                    if (product_check.unit == choose_unit)
                                    {
                                        revenue_return.Price = product_check.sell_price;
                                    }
                                    else
                                    {
                                        revenue_return.Price = (decimal)product_check.sell_price_swap;
                                    }
                                    revenue_return.quantity = temp_inventory;
                                    revenue_return.unit = choose_unit;
                                    db.revenues.Add(revenue_return);
                                    temp_quatity -= temp_inventory;
                                }
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            if (temp_quatity <= inventory.quantity_remaining)
                            {
                                revenue revenue_return = new revenue();
                                revenue_return.sale_details_id = sale_Details_return.id;
                                revenue_return.inventory_id = inventory.id;
                                revenue_return.inventory_id = inventory.id;
                                if (product_check.unit == choose_unit)
                                {
                                    revenue_return.Price = product_check.sell_price;
                                }
                                else
                                {
                                    revenue_return.Price = (decimal)product_check.sell_price_swap;
                                }
                                revenue_return.quantity = temp_quatity;
                                revenue_return.unit = choose_unit;
                                db.revenues.Add(revenue_return);
                                inventory.sold_swap += temp_quatity;
                                inventory.quantity_remaining -= temp_quatity;
                                db.Entry(inventory).State = EntityState.Modified;
                                temp_quatity = 0;
                                db.SaveChanges();
                            }
                            else
                            {
                                int temp_inventory = (inventory.quantity - inventory.sold);

                                int quantity_remaining = (int)inventory.quantity_remaining;
                                if (temp_quatity <= quantity_remaining)
                                {
                                    inventory.sold_swap += temp_quatity;
                                    inventory.quantity_remaining -= temp_quatity;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue_return = new revenue();
                                    revenue_return.sale_details_id = sale_Details_return.id;
                                    revenue_return.inventory_id = inventory.id;
                                    revenue_return.inventory_id = inventory.id;
                                    if (product_check.unit == choose_unit)
                                    {
                                        revenue_return.Price = product_check.sell_price;
                                    }
                                    else
                                    {
                                        revenue_return.Price = (decimal)product_check.sell_price_swap;
                                    }
                                    revenue_return.quantity = quantity_remaining;
                                    revenue_return.unit = choose_unit;
                                    db.revenues.Add(revenue_return);
                                    temp_quatity = 0;
                                }
                                else if (temp_quatity > quantity_remaining && quantity_remaining > 0)
                                {
                                    inventory.sold_swap += temp_quatity;
                                    inventory.quantity_remaining = 0;
                                    db.Entry(inventory).State = EntityState.Modified;
                                    revenue revenue_return = new revenue();
                                    revenue_return.sale_details_id = sale_Details_return.id;
                                    revenue_return.inventory_id = inventory.id;
                                    revenue_return.inventory_id = inventory.id;
                                    if (product_check.unit == choose_unit)
                                    {
                                        revenue_return.Price = product_check.sell_price;
                                    }
                                    else
                                    {
                                        revenue_return.Price = (decimal)product_check.sell_price_swap;
                                    }
                                    revenue_return.quantity = quantity_remaining;
                                    revenue_return.unit = choose_unit;
                                    db.revenues.Add(revenue_return);
                                    temp_quatity -= quantity_remaining;
                                }
                                else
                                {
                                    int temp_1 = (int)(temp_quatity / inventory.product.quantity_swap);
                                    int temp_check = (int)(temp_quatity % inventory.product.quantity_swap);

                                    if (temp_check > 0)
                                    {
                                        double temp_2 = (double)((temp_quatity * 1.0000000) / inventory.product.quantity_swap) - temp_1;
                                        if (temp_inventory >= (temp_1 + 1))
                                        {
                                            inventory.sold += (temp_1 + 1);
                                            int temp = (int)(inventory.product.quantity_swap * (1 - temp_2));
                                            inventory.quantity_remaining += temp;
                                            inventory.sold_swap += temp_quatity;
                                            revenue revenue_return = new revenue();
                                            revenue_return.sale_details_id = sale_Details_return.id;
                                            revenue_return.inventory_id = inventory.id;
                                            revenue_return.inventory_id = inventory.id;
                                            if (product_check.unit == choose_unit)
                                            {
                                                revenue_return.Price = product_check.sell_price;
                                            }
                                            else
                                            {
                                                revenue_return.Price = (decimal)product_check.sell_price_swap;
                                            }
                                            revenue_return.quantity = temp_quatity;
                                            revenue_return.unit = choose_unit;
                                            db.revenues.Add(revenue_return);
                                            temp_quatity = 0;
                                        }
                                        else
                                        {
                                            inventory.sold += temp_inventory;
                                            int temp = (int)(inventory.product.quantity_swap * temp_inventory);
                                            inventory.sold_swap += temp;
                                            revenue revenue_return = new revenue();
                                            revenue_return.sale_details_id = sale_Details_return.id;
                                            revenue_return.inventory_id = inventory.id;
                                            revenue_return.inventory_id = inventory.id;
                                            if (product_check.unit == choose_unit)
                                            {
                                                revenue_return.Price = product_check.sell_price;
                                            }
                                            else
                                            {
                                                revenue_return.Price = (decimal)product_check.sell_price_swap;
                                            }
                                            revenue_return.quantity = temp;
                                            revenue_return.unit = choose_unit;
                                            db.revenues.Add(revenue_return);
                                            temp_quatity -= temp;
                                        }
                                    }
                                    else
                                    {
                                        if (temp_inventory >= temp_1)
                                        {
                                            inventory.sold += temp_1;
                                            int temp_remaining = (int)(inventory.product.quantity_swap * temp_1);
                                            inventory.sold_swap += temp_remaining;
                                            revenue revenue_return = new revenue();
                                            revenue_return.sale_details_id = sale_Details_return.id;
                                            revenue_return.inventory_id = inventory.id;
                                            revenue_return.inventory_id = inventory.id;
                                            if (product_check.unit == choose_unit)
                                            {
                                                revenue_return.Price = product_check.sell_price;
                                            }
                                            else
                                            {
                                                revenue_return.Price = (decimal)product_check.sell_price_swap;
                                            }
                                            revenue_return.quantity = temp_remaining;
                                            revenue_return.unit = choose_unit;
                                            db.revenues.Add(revenue_return);
                                            temp_quatity = 0;
                                        }
                                        else
                                        {
                                            inventory.sold += temp_inventory;
                                            int temp = (int)(inventory.product.quantity_swap * temp_inventory);
                                            inventory.sold_swap += temp;
                                            revenue revenue_return = new revenue();
                                            revenue_return.sale_details_id = sale_Details_return.id;
                                            revenue_return.inventory_id = inventory.id;
                                            revenue_return.inventory_id = inventory.id;
                                            if (product_check.unit == choose_unit)
                                            {
                                                revenue_return.Price = product_check.sell_price;
                                            }
                                            else
                                            {
                                                revenue_return.Price = (decimal)product_check.sell_price_swap;
                                            }
                                            revenue_return.quantity = temp;
                                            revenue_return.unit = choose_unit;
                                            db.revenues.Add(revenue_return);
                                            temp_quatity -= temp;
                                        }
                                    }
                                    db.Entry(inventory).State = EntityState.Modified;
                                }
                                db.SaveChanges();
                            }
                        }
                    }

                    db.SaveChanges();


                    return_sale return_Sale = new return_sale();
                    return_Sale.sale_id = sale_Details.sale_id;
                    return_Sale.method = return_option;
                    return_Sale.create_at = DateTime.Now;
                    return_Sale.difference = price_PCurrent1 * quality_OD;
                    db.return_sale.Add(return_Sale);
                    return_details return_Details = new return_details();
                    return_Details.return_id = return_Sale.id;
                    return_Details.product_current_id = product_Current_id;
                    return_Details.quantity_current = quality_OD;
                    return_Details.total_current = price_PCurrent1 * quality_OD;
                    return_Details.product_return_id = product_id;
                    return_Details.quantity_return = input_qualityProduct;
                    return_Details.unit_current = sale_Details.unit;
                    return_Details.unit_return = choose_unit;
                    if (product_check.unit == choose_unit)
                    {
                        return_Details.total_return = product_check.sell_price * input_qualityProduct;
                    }
                    else
                    {
                        return_Details.total_return = (decimal)(product_check.sell_price_swap * input_qualityProduct);
                    }
                    return_Details.difference = (decimal)(return_Details.total_current - return_Details.total_return);
                    db.return_details.Add(return_Details);
                    db.SaveChanges();
                    foreach (var item in revenue)
                    {
                        while (quality_OD_revenue > 0)
                        {

                            if (item.quantity > quality_OD_revenue)
                            {
                                item.quantity -= quality_OD_revenue;
                                import_inventory import_Inventory = db.import_inventory.Find(item.inventory_id);

                                if (item.unit == import_Inventory.product.unit)
                                {
                                    import_Inventory.sold -= quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                else
                                {
                                    import_Inventory.quantity_remaining += quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                db.Entry(item).State = EntityState.Modified;

                                quality_OD_revenue = 0;
                            }
                            else
                            {
                                quality_OD_revenue -= item.quantity;
                                import_inventory import_Inventory = db.import_inventory.Find(item.inventory_id);
                                if (item.unit == import_Inventory.product.unit)
                                {
                                    import_Inventory.sold -= quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                else
                                {
                                    import_Inventory.quantity_remaining += quality_OD_revenue;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                db.revenues.Remove(item);
                            }
                            db.SaveChanges();
                        }
                    }
                    if (quality_OD < sale_Details.sold)
                    {
                        decimal price = (sale_Details.price / sale_Details.sold) * quality_OD;
                        sale_Details.price -= price;
                        sale_Details.sold -= quality_OD;
                        db.Entry(sale_Details).State = EntityState.Modified;
                        sale.total -= price;
                        db.Entry(sale).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        if (sale.sale_details.Count() == 1)
                        {
                            decimal price = (sale_Details.price / sale_Details.sold) * quality_OD;
                            sale.total -= price;
                            db.Entry(sale).State = EntityState.Modified;
                            db.sale_details.Remove(sale_Details);
                            db.SaveChanges();
                        }
                        else
                        {
                            db.sale_details.Remove(sale_Details);
                            db.sales.Remove(sale);
                            db.SaveChanges();
                        }
                    }
                    if (unit == product.unit)
                    {
                        product.quantity += quality_OD;
                        db.Entry(product).State = EntityState.Modified;
                    }
                    else if (unit == product.unit_swap)
                    {
                        product.quantity_remaning += quality_OD;
                        db.Entry(product).State = EntityState.Modified;
                    }

                    db.SaveChanges();

                    message = "Đổi sản phẩm thành công";
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        // GET: return_sale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return_sale return_sale = db.return_sale.Find(id);
            if (return_sale == null)
            {
                return HttpNotFound();
            }
            return View(return_sale);
        }

        // GET: return_sale/Create
        public ActionResult Create()
        {
            ViewBag.sale_id = new SelectList(db.sales, "id", "code");
            return View();
        }

        // POST: return_sale/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,sale_id,method,create_at,difference")] return_sale return_sale)
        {
            if (ModelState.IsValid)
            {
                db.return_sale.Add(return_sale);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.sale_id = new SelectList(db.sales, "id", "code", return_sale.sale_id);
            return View(return_sale);
        }

        // GET: return_sale/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return_sale return_sale = db.return_sale.Find(id);
            if (return_sale == null)
            {
                return HttpNotFound();
            }
            ViewBag.sale_id = new SelectList(db.sales, "id", "code", return_sale.sale_id);
            return View(return_sale);
        }

        // POST: return_sale/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,sale_id,method,create_at,difference")] return_sale return_sale)
        {
            if (ModelState.IsValid)
            {
                db.Entry(return_sale).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.sale_id = new SelectList(db.sales, "id", "code", return_sale.sale_id);
            return View(return_sale);
        }

        // GET: return_sale/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return_sale return_sale = db.return_sale.Find(id);
            if (return_sale == null)
            {
                return HttpNotFound();
            }
            return View(return_sale);
        }

        // POST: return_sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            return_sale return_sale = db.return_sale.Find(id);
            db.return_sale.Remove(return_sale);
            db.SaveChanges();
            return RedirectToAction("Index");
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
