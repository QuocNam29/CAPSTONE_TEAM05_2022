﻿using CAP_TEAM05_2022.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class import_inventoryController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public import_inventoryController()
        {
            ViewBag.isCreate = false;
        }
        // GET: import_inventory
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InventoryList(DateTime? date_start, DateTime? date_end)
        {
            if (date_start == null)
            {
                date_start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_end == null)
            {
                date_end = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var import_inventory = db.import_inventory.Include(i => i.user).Include(i => i.product).Where(s => s.created_at >= date_start && s.created_at <= date_end
                                                    || s.created_at.Value.Day == date_start.Value.Day
                                                    && s.created_at.Value.Month == date_start.Value.Month
                                                    && s.created_at.Value.Year == date_start.Value.Year
                                                    || s.created_at.Value.Day == date_end.Value.Day
                                                    && s.created_at.Value.Month == date_end.Value.Month
                                                    && s.created_at.Value.Year == date_end.Value.Year);
            return PartialView(import_inventory.OrderByDescending(i => i.id).ToList());
        }

        public ActionResult EditImportProduct(int id_importProduct, int quantity_import, string priceImport)
        {
            string message = "";
            bool status = true;
            DateTime create_at = DateTime.Now;
            if (quantity_import <= 0)
            {
                status = false;
                message = "Vui lòng kiểm tra lại số lượng muốn đổi trả !";
                return Json(new { status, message }, JsonRequestBehavior.AllowGet);
            }
            try
            {

                import_inventory inventory = db.import_inventory.Find(id_importProduct);
                if (quantity_import < (inventory.sold + inventory.return_quantity))
                {
                    status = false;
                    message = "Số lượng sản phẩm " + inventory.product.name + " do công ty cung cấp  " + '"'
                        + inventory.customer.name + '"' + " cung cấp đã sử dụng "
                        + (inventory.sold + inventory.return_quantity) + " " + inventory.product.unit;
                }
                else
                {
                    decimal price = decimal.Parse(priceImport.Replace(",", "")); //.Replace(".", ","));
                    int quantitychange = inventory.quantity - (int)quantity_import;
                    inventory.quantity = (int)quantity_import;
                    inventory.price_import = price;
                    inventory.updated_at = create_at;
                    db.Entry(inventory).State = EntityState.Modified;

                    inventory_order inventory_Order = db.inventory_order.Find(inventory.inventory_id);
                    decimal totalOld = inventory_Order.Total; // tổng đơn hàng cũ
                    inventory_Order.Total = inventory_Order.import_inventory.Sum(x => (x.quantity - x.return_quantity) * x.price_import); // tổng đơn hàng mới
                    inventory_Order.update_at = create_at;
                    decimal difference = totalOld - inventory_Order.Total ; // Chênh lệch giá tiền trước và sau

                    if (inventory_Order.state == Constants.DEBT_ORDER)
                    {
                        if (inventory_Order.Total < inventory_Order.payment + inventory_Order.pay_debt)
                        {
                            status = false;
                            message = "Giá trị của đơn nhập hàng này đã được thanh toán vượt quá tổng giá trị đơn hàng, vui lòng kiểm tra lại !";
                            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            inventory_Order.debt -= difference;
                            var DebtList = db.debts.Where(x => x.inventory_order.supplier_id == inventory_Order.supplier_id && x.inventory_id >= inventory_Order.id).ToList();
                            foreach (var debtItem in DebtList)
                            {
                                if (debtItem.inventory_id == inventory_Order.id && debtItem.debt1 != null)
                                {
                                    debtItem.debt1 -= difference;
                                }
                                debtItem.remaining -= difference;
                            }
                            var CustomerDebtList = db.customer_debt.Where(x => x.customer_id == inventory_Order.supplier_id).ToList();
                            foreach (var debtItem in CustomerDebtList)
                            {
                                if (debtItem.inventory_id == inventory_Order.id && debtItem.debt != null)
                                {
                                    debtItem.debt -= difference;
                                }
                                debtItem.remaining -= difference;
                            }
                        }
                    }
                    product product = db.products.Find(inventory.product_id);
                    product.quantity -= quantitychange;
                    db.Entry(product).State = EntityState.Modified;
                                    
                    db.Entry(inventory_Order).State = EntityState.Modified;



                    db.SaveChanges();
                    message = "Cập nhật thông tin thành công";
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
