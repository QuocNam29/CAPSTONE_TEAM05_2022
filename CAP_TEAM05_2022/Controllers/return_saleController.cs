using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    public class return_saleController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: return_sale
        public ActionResult Index()
        {
            var return_sale = db.return_sale.Include(r => r.sale_details.sale).OrderByDescending(x=> x.id);
            return View(return_sale.ToList());
        }
        public ActionResult Create_Return(int sale_details_id, int product_Current_id, int quality_OD,
            string price_PCurrent, int return_option, int? product_id, int? input_qualityProduct, string choose_unit)
        {
            string message = "";
            bool status = true;
            DateTime created_at = DateTime.Now;
            try
            {
                if (input_qualityProduct != null && input_qualityProduct <= 0)
                {
                    string message1 = "Vui lòng kiểm tra lại số lượng sản phẩm đổi mới !";
                    bool status1 = false;
                    return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                }
                string unit = "";
                int quality_OD_revenue = quality_OD; // sản phẩm muốn trả
                sale_details sale_Details = db.sale_details.Find(sale_details_id);
                sale sale = db.sales.Find(sale_Details.sale_id);
                product product = db.products.Find(sale_Details.product_id);

                // tìm thu chi của sản phẩm đó trong chi tiết đơn hàng
                var revenue = db.revenues.Where(r => r.sale_details_id == sale_details_id).OrderByDescending(o => o.id).ToList();
                unit = sale_Details.unit;
                decimal price_PCurrent1 = decimal.Parse(price_PCurrent.Replace(",", "").Replace(".", ""));
                if (return_option == Constants.RETURN_OPTION)
                {
                    //Tạo đơn đổi trả sản phẩm
                    return_sale return_Sale = new return_sale();
                    return_Sale.saleDetails_id = sale_Details.id;
                    return_Sale.method = return_option;
                    return_Sale.code = $"MTH-{DateTime.Now:ddMMyyHHss}";
                    return_Sale.create_at = created_at;
                    return_Sale.difference = price_PCurrent1 * quality_OD;
                    db.return_sale.Add(return_Sale);

                    // tạo chi tiết đổi tra
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
                        // kiểm tra sản phẩm muốn trả đã hoàn trả hết chưa 
                        while (quality_OD_revenue > 0)
                        {
                            //Kiểm trta số lượng sản phẩm bán theo từng phần thu chi theo lần nhập kho
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
                                    int temp_1 = (int)(quality_OD_revenue / import_Inventory.product.quantity_swap);
                                    int temp_check = (int)(quality_OD_revenue % import_Inventory.product.quantity_swap);

                                    if (temp_check > 0)
                                    {
                                        double temp_2 = (double)((quality_OD_revenue * 1.0000000) / import_Inventory.product.quantity_swap) - temp_1;

                                        import_Inventory.sold -= temp_1;

                                        import_Inventory.quantity_remaining += (int)(import_Inventory.product.quantity_swap * temp_2);
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        import_Inventory.sold -= temp_1;
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                }
                                db.Entry(item).State = EntityState.Modified;

                                quality_OD_revenue = 0;
                                db.SaveChanges();
                            }
                            else
                            {
                                // lấy dữ liệu lần import đó
                                import_inventory import_Inventory = db.import_inventory.Find(item.inventory_id);

                                if (item.unit == import_Inventory.product.unit)
                                {
                                    // nếu đã bán trong kho nhỏ hơn số hoàn trả thì đã bán trả về số lần trong kho
                                    import_Inventory.sold -= item.quantity;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                else
                                {
                                    int temp_1 = (int)(quality_OD_revenue / import_Inventory.product.quantity_swap);
                                    int temp_check = (int)(quality_OD_revenue % import_Inventory.product.quantity_swap);

                                    if (temp_check > 0)
                                    {
                                        double temp_2 = (double)((quality_OD_revenue * 1.0000000) / import_Inventory.product.quantity_swap) - temp_1;

                                        import_Inventory.sold -= temp_1;

                                        import_Inventory.quantity_remaining += (int)(import_Inventory.product.quantity_swap * temp_2);
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        import_Inventory.sold -= temp_1;
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                }
                                db.revenues.Remove(item);
                                quality_OD_revenue -= item.quantity;
                                db.SaveChanges();
                                break;
                            }

                        }
                    }
                    db.SaveChanges();
                    if (sale.method == Constants.DEBT_ORDER)
                    {
                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        var last_debt = db.debts.Where(d => d.sale.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.created_by = User.Identity.GetUserId();
                        debt.created_at = created_at;
                        debt.paid = (price_PCurrent1 * quality_OD);
                        debt.total = (decimal)(last_debt.total + (price_PCurrent1 * quality_OD));
                        debt.remaining = last_debt.remaining - (price_PCurrent1 * quality_OD);
                        debt.return_sale_id = return_Sale.id;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.customer_id = sale.customer_id;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.created_at = created_at;
                        customer_Debt.paid = (price_PCurrent1 * quality_OD);
                        customer_Debt.remaining = last_customer_Debt.remaining - (price_PCurrent1 * quality_OD);
                        customer_Debt.return_sale_id = return_Sale.id;
                        db.customer_debt.Add(customer_Debt);
                        db.SaveChanges();
                    }
                    decimal price = sale_Details.price * quality_OD;
                    sale_Details.return_quantity += quality_OD;
                    db.Entry(sale_Details).State = EntityState.Modified;
                    sale.total -= price;
                    if (sale.total < (sale.pay_debt + sale.prepayment))
                    {
                        decimal priceReturn = price;
                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        var last_debt = db.debts.Where(d => d.sale.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.created_by = User.Identity.GetUserId();
                        debt.created_at = created_at;
                        debt.paid = sale.total - sale.pay_debt - sale.prepayment;
                        debt.total = (decimal)(last_debt.total + debt.paid);
                        debt.remaining = last_debt.remaining - debt.paid;
                        debt.return_sale_id = return_Sale.id;
                        debt.note = "Doanh nghiệp hoàn tiền cho khách hàng do đổi trả hàng cho doanh nghiệp";
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.customer_id = sale.customer_id;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.created_at = created_at;
                        customer_Debt.paid = sale.total - sale.pay_debt - sale.prepayment;
                        customer_Debt.remaining = last_customer_Debt.remaining - customer_Debt.paid;
                        customer_Debt.return_sale_id = return_Sale.id;
                        customer_Debt.note = "Doanh nghiệp hoàn tiền cho khách hàng do đổi trả hàng cho doanh nghiệp";
                        db.customer_debt.Add(customer_Debt);
                        db.SaveChanges();
                        while (priceReturn > 0)
                        {
                            if (sale.pay_debt > 0)
                            {
                                if (sale.pay_debt >= priceReturn)
                                {
                                    sale.pay_debt = (sale.total - sale.prepayment);
                                    priceReturn = 0;
                                }
                                else
                                {
                                    priceReturn -= (decimal)sale.pay_debt;
                                    sale.pay_debt = 0;
                                }
                            }
                            else
                            {
                                sale.prepayment -= priceReturn;
                                if (sale.prepayment < 0)
                                {
                                    sale.prepayment = 0;
                                }
                                priceReturn = 0;
                            }
                        }
                    }
                    db.Entry(sale).State = EntityState.Modified;
                    db.SaveChanges();

                    //công thêm sản phẩm nếu trả hàng
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
                else if (return_option == Constants.CHANGE_OPTION)
                {
                    if (product_id == null)
                    {
                        string message1 = "Bạn chưa chọn sản phẩm đổi mới !";

                        bool status1 = false;
                        return Json(new { status = status1, message = message1 }, JsonRequestBehavior.AllowGet);
                    }

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
                    sale_Details_return.unit = choose_unit;
                    var price = db.price_product.Where(x => x.product_id == sale_Details_return.product_id && x.unit == sale_Details_return.unit);
                    sale_Details_return.price_id = price.Any() ? price.OrderByDescending(x => x.id).FirstOrDefault().id : 0;

                    if (product_check.unit == choose_unit)
                    {
                        sale_Details_return.price = sale.is_debt_price ? product_check.sell_price_debt : product_check.sell_price;
                    }
                    else
                    {
                        sale_Details_return.price = sale.is_debt_price ? product_check.sell_price_debt_swap : product_check.sell_price_swap;
                    }
                    sale.total += (decimal)(sale_Details_return.price * input_qualityProduct);
                    sale_Details_return.created_at = created_at;
                    sale_Details_return.return_quantity = 0;
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
                                revenue_return.Price = sale_Details_return.price;
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
                                    revenue_return.Price = sale_Details_return.price;
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
                                    revenue_return.Price = sale_Details_return.price;
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
                                revenue_return.Price = sale_Details_return.price;
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
                                    revenue_return.Price = sale_Details_return.price;
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
                                    revenue_return.Price = sale_Details_return.price;
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
                                            revenue_return.Price = sale_Details_return.price;
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
                                            revenue_return.Price = sale_Details_return.price;
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
                                            revenue_return.Price = sale_Details_return.price;
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
                                            revenue_return.Price = sale_Details_return.price;
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
                    decimal total_return = 0;
                    total_return = (decimal)(sale_Details_return.price * input_qualityProduct);
                    return_sale return_Sale = new return_sale();
                    return_Sale.saleDetails_id = sale_Details.id;
                    return_Sale.method = return_option;
                    return_Sale.code = $"MDH-{DateTime.Now:ddMMyyHHss}"; ;
                    return_Sale.create_at = created_at;
                    return_Sale.difference = (price_PCurrent1 * quality_OD) - total_return;
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
                    return_Details.total_return = total_return;
                    return_Details.difference = (decimal)(return_Details.total_current - return_Details.total_return);
                    db.return_details.Add(return_Details);
                    db.SaveChanges();
                    // xử lý đổi trả trong kho
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
                                    int temp_1 = (int)(quality_OD_revenue / import_Inventory.product.quantity_swap);
                                    int temp_check = (int)(quality_OD_revenue % import_Inventory.product.quantity_swap);

                                    if (temp_check > 0)
                                    {
                                        double temp_2 = (double)((quality_OD_revenue * 1.0000000) / import_Inventory.product.quantity_swap) - temp_1;

                                        import_Inventory.sold -= temp_1;

                                        import_Inventory.quantity_remaining += (int)(import_Inventory.product.quantity_swap * temp_2);
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        import_Inventory.sold -= temp_1;
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                }
                                db.Entry(item).State = EntityState.Modified;
                                quality_OD_revenue = 0;
                                db.SaveChanges();

                            }
                            else
                            {
                                import_inventory import_Inventory = db.import_inventory.Find(item.inventory_id);
                                if (item.unit == import_Inventory.product.unit)
                                {
                                    import_Inventory.sold -= item.quantity;
                                    db.Entry(import_Inventory).State = EntityState.Modified;
                                }
                                else
                                {
                                    int temp_1 = (int)(quality_OD_revenue / import_Inventory.product.quantity_swap);
                                    int temp_check = (int)(quality_OD_revenue % import_Inventory.product.quantity_swap);

                                    if (temp_check > 0)
                                    {
                                        double temp_2 = (double)((quality_OD_revenue * 1.0000000) / import_Inventory.product.quantity_swap) - temp_1;

                                        import_Inventory.sold -= temp_1;

                                        import_Inventory.quantity_remaining += (int)(import_Inventory.product.quantity_swap * temp_2);
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        import_Inventory.sold -= temp_1;
                                        db.Entry(import_Inventory).State = EntityState.Modified;
                                    }
                                }
                                db.revenues.Remove(item);
                                quality_OD_revenue -= item.quantity;
                                db.SaveChanges();
                                break;
                            }
                        }
                    }
                    // tạp nợ cho lần đổi sản phẩm
                    if (sale.method == Constants.DEBT_ORDER)
                    {
                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        var last_debt = db.debts.Where(d => d.sale.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.created_by = User.Identity.GetUserId();
                        debt.created_at = created_at;
                        if (return_Sale.difference >= 0)
                        {
                            debt.paid = return_Sale.difference;
                            debt.total = (decimal)(last_debt.total + debt.paid);
                            debt.remaining = last_debt.remaining - debt.paid;
                        }
                        else
                        {
                            debt.debt1 = -return_Sale.difference;
                            debt.total = (decimal)(last_debt.total);
                            debt.remaining = last_debt.remaining + debt.debt1;
                        }


                        debt.return_sale_id = return_Sale.id;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.customer_id = sale.customer_id;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.created_at = created_at;
                        if (return_Sale.difference >= 0)
                        {
                            customer_Debt.paid = return_Sale.difference;
                            customer_Debt.remaining = last_customer_Debt.remaining - customer_Debt.paid;
                        }
                        else
                        {
                            customer_Debt.debt = -return_Sale.difference;
                            customer_Debt.remaining = last_customer_Debt.remaining + customer_Debt.debt;
                        }

                        customer_Debt.return_sale_id = return_Sale.id;
                        db.customer_debt.Add(customer_Debt);
                        db.SaveChanges();

                    }
                    decimal price_Total = sale_Details.price * quality_OD;
                    sale_Details.return_quantity += quality_OD;
                    db.Entry(sale_Details).State = EntityState.Modified;
                    sale.total -= price_Total;
                    if (sale.total < (sale.pay_debt + sale.prepayment))
                    {
                        decimal priceReturn = price_Total;
                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        var last_debt = db.debts.Where(d => d.sale.customer_id == sale.customer_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.sale_id = sale.id;
                        debt.created_by = User.Identity.GetUserId();
                        debt.created_at = created_at;
                        debt.paid = sale.total - sale.pay_debt - sale.prepayment;
                        debt.total = (decimal)(last_debt.total + debt.paid);
                        debt.remaining = last_debt.remaining - debt.paid;
                        debt.return_sale_id = return_Sale.id;
                        debt.note = "Doanh nghiệp hoàn tiền cho khách hàng do đổi trả hàng cho doanh nghiệp";
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.customer_id = sale.customer_id;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.created_at = created_at;
                        customer_Debt.paid = sale.total - sale.pay_debt - sale.prepayment;
                        customer_Debt.remaining = last_customer_Debt.remaining - customer_Debt.paid;
                        customer_Debt.return_sale_id = return_Sale.id;
                        customer_Debt.note = "Doanh nghiệp hoàn tiền cho khách hàng do đổi trả hàng cho doanh nghiệp";
                        db.customer_debt.Add(customer_Debt);
                        db.SaveChanges();
                        while (priceReturn > 0)
                        {
                            if (sale.pay_debt > 0)
                            {
                                if (sale.pay_debt >= priceReturn)
                                {
                                    sale.pay_debt = (sale.total - sale.prepayment);
                                    priceReturn = 0;
                                }
                                else
                                {
                                    priceReturn -= (decimal)sale.pay_debt;
                                    sale.pay_debt = 0;
                                }
                            }
                            else
                            {
                                sale.prepayment -= priceReturn;
                                if (sale.prepayment < 0)
                                {
                                    sale.prepayment = 0;
                                }
                                priceReturn = 0;
                            }
                        }
                        db.Entry(sale).State = EntityState.Modified;

                    }
                    db.Entry(sale).State = EntityState.Modified;
                    db.SaveChanges();

                    //hoàn trả số lượng sản pẩm
                    //cộng số lượng sp đã trả lại
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
                    // Trừ số lượng sp đã đổi mới

                    if (choose_unit == product_check.unit)
                    {
                        product_check.quantity -= (int)input_qualityProduct;
                    }
                    else if (choose_unit == product_check.unit_swap)
                    {
                        int check_quantity = (int)input_qualityProduct;
                        while (check_quantity > 0)
                        {
                            if (check_quantity <= product_check.quantity_remaning)
                            {
                                product_check.quantity_remaning -= check_quantity;
                                check_quantity = 0;
                            }
                            else
                            {
                                if (product_check.quantity_remaning > 0)
                                {
                                    check_quantity -= product_check.quantity_remaning;
                                    product_check.quantity_remaning = 0;
                                }
                                else
                                {
                                    int temp_1 = (int)(check_quantity / product_check.quantity_swap);
                                    double temp_2 = (double)((check_quantity * 1.0000000) / product_check.quantity_swap) - temp_1;
                                    if (temp_2 > 0)
                                    {
                                        product_check.quantity -= (temp_1 + 1);
                                        int temp = (int)(product_check.quantity_swap * (1 - temp_2));
                                        product_check.quantity_remaning += temp;
                                    }
                                    else
                                    {
                                        product_check.quantity -= temp_1;
                                    }
                                    check_quantity = 0;
                                }
                            }
                        }                       
                    }
                    db.Entry(product_check).State = EntityState.Modified;

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
