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
            try
            {
                import_inventory inventory = db.import_inventory.Find(id_inventory);
                if (quantity > (inventory.quantity - inventory.sold))
                {
                    status = false;
                    message = "Số lượng sản phẩm " + inventory.product.name + " do nhà cung cấp  " +'"'
                        + inventory.customer.name + '"'+" cung cấp chỉ còn lại " 
                        + (inventory.quantity - inventory.sold) + " " + inventory.product.unit;
                }
                else
                {

                    return_supplier return_Supplier = new return_supplier();
                    return_Supplier.inventory_id = id_inventory;
                    return_Supplier.quantity = quantity;
                    return_Supplier.note = note;
                    return_Supplier.cost_difference = inventory.price_import * quantity;
                    return_Supplier.created_at = DateTime.Now;
                    db.return_supplier.Add(return_Supplier);

                    inventory.quantity -= quantity;                   
                    db.Entry(inventory).State = EntityState.Modified;

                    inventory_order inventory_Order = db.inventory_order.Find(inventory.inventory_id);
                    inventory_Order.Total = inventory_Order.import_inventory.Sum(x => x.quantity * x.price_import);
                    inventory_Order.update_at = DateTime.Now;
                    db.Entry(inventory_Order).State = EntityState.Modified;

                    product product = db.products.Find(inventory.product_id);
                    product.quantity -= quantity;
                    db.Entry(product).State = EntityState.Modified;
                    
                    db.SaveChanges();
                    message = "Tổng tiền hoàn lại cho nhà cung cấp là: "+ (inventory.price_import * quantity).ToString("N0") + "VNĐ";
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
