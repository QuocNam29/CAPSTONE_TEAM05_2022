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
    public class return_supplierController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: return_supplier
        public ActionResult _ReturnSupplier()
        {
            var return_supplier = db.return_supplier.Include(r => r.import_inventory);
            return View(return_supplier.OrderByDescending(o => o.id).ToList());
        }

        public ActionResult Create_returnSupplier(int id_inventory, int quantity, string note)
        {
            string message = "";
            bool status = true;
            DateTime create_at = DateTime.Now;
            if (quantity <= 0)
            {
                status = false;
                message = "Vui lòng kiểm tra lại số lượng muốn đổi trả !";
                return Json(new { status, message }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                import_inventory inventory = db.import_inventory.Find(id_inventory);
                if (quantity > (inventory.quantity - inventory.sold - inventory.return_quantity))
                {
                    status = false;
                    message = "Số lượng sản phẩm " + inventory.product.name + " do nhà cung cấp  " + '"'
                        + inventory.customer.name + '"' + " cung cấp chỉ còn lại "
                        + (inventory.quantity - inventory.sold - inventory.return_quantity) + " " + inventory.product.unit;
                }
                else
                {

                    return_supplier return_Supplier = new return_supplier();
                    return_Supplier.inventory_id = id_inventory;
                    return_Supplier.code = $"MTH-{DateTime.Now:ddMMyyHHss}";
                    return_Supplier.quantity = quantity;
                    return_Supplier.note = note;
                    return_Supplier.cost_difference = inventory.price_import * quantity;
                    return_Supplier.created_at = create_at;
                    db.return_supplier.Add(return_Supplier);

                    inventory.return_quantity += quantity;
                    db.Entry(inventory).State = EntityState.Modified;

                    inventory_order inventory_Order = db.inventory_order.Find(inventory.inventory_id);
                    inventory_Order.Total = inventory_Order.import_inventory.Sum(x => (x.quantity - x.return_quantity) * x.price_import);
                    inventory_Order.update_at = create_at;

                    product product = db.products.Find(inventory.product_id);
                    product.quantity -= quantity;
                    db.Entry(product).State = EntityState.Modified;

                    if (inventory_Order.state == Constants.DEBT_ORDER)
                    {
                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == inventory_Order.supplier_id).OrderByDescending(o => o.id).FirstOrDefault();
                        var last_debt = db.debts.Where(d => d.inventory_order.supplier_id == inventory_Order.supplier_id).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.inventory_id = inventory_Order.id;
                        debt.created_by = User.Identity.GetUserId();
                        debt.created_at = create_at;
                        debt.paid = (inventory.price_import * quantity);
                        debt.total = (decimal)(last_debt.total + (inventory.price_import * quantity));
                        debt.note = note;
                        debt.remaining = last_debt.remaining - (inventory.price_import * quantity);
                        debt.return_supplier_id = return_Supplier.id;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.customer_id = inventory_Order.supplier_id;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.created_at = create_at;
                        customer_Debt.paid = (inventory.price_import * quantity);
                        customer_Debt.remaining = last_customer_Debt.remaining - (inventory.price_import * quantity);
                        customer_Debt.note = note;
                        customer_Debt.return_supplier_id = return_Supplier.id;
                        db.customer_debt.Add(customer_Debt);

                    }
                    db.Entry(inventory_Order).State = EntityState.Modified;


                    db.SaveChanges();
                    message = "Tổng tiền thu lại từ nhà cung cấp là: " + (inventory.price_import * quantity).ToString("N0") + "VNĐ";
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
