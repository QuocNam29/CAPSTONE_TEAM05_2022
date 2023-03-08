using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;

namespace CAP_TEAM05_2022.Controllers
{
    public class inventory_orderController : Controller
    {
        public inventory_orderController()
        {
            ViewBag.isCreate = false;
        }
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: inventory_order
        public ActionResult Index()
        {
            var inventory_order = db.inventory_order.Include(i => i.customer).Include(i => i.user);
            return View(inventory_order.ToList());
        }
        public ActionResult _HistoryInventory(int order_customer, int? method)
        {
            var HistoryOrder = db.inventory_order.Where(o => o.supplier_id == order_customer);
            if (method == 2)
            {
                HistoryOrder = HistoryOrder.Where(s => s.state == 2);
            }
            return PartialView(HistoryOrder.OrderByDescending(o => o.id).ToList());
        }

        // GET: inventory_order/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            inventory_order inventory_order = db.inventory_order.Find(id);
            if (inventory_order == null)
            {
                return HttpNotFound();
            }
            return View(inventory_order);
        }

        // GET: inventory_order/Create
        public ActionResult Create()
        {
            ViewBag.Uniform = db.products.Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == 2), "id", "name");
            ViewBag.isCreate = true;
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,code,supplier_id,create_at,update_at,create_by,state,Total,payment,debt")] inventory_order inventory_order, List<import_inventory> importInventory, int method)
        {
            if (ModelState.IsValid)
            {
                DateTime currentDate = DateTime.Now;
                decimal total = 0;
                foreach (var inventory in importInventory)
                {
                    inventory.inventory_id = inventory_order.id;
                    inventory.sold = 0;
                    inventory.sold_swap = 0;
                    inventory.quantity_remaining = 0;
                    inventory.created_by = User.Identity.GetUserId();
                    inventory.created_at = currentDate;
                    inventory.supplier_id = inventory_order.supplier_id;
                    db.import_inventory.Add(inventory);
                    total += inventory.price_import * inventory.quantity;
                    product product = db.products.Find(inventory.product_id);
                    product.quantity += inventory.quantity;
                    db.Entry(product).State = EntityState.Modified;
                }
                inventory_order.code = "MPN" + CodeRandom.RandomCode();
                inventory_order.create_at = currentDate;
                inventory_order.update_at = currentDate;
                inventory_order.create_by = User.Identity.GetUserId();
                inventory_order.Total = total;
                inventory_order.state = method;
                if (method == 2)
                {
                    inventory_order.debt = inventory_order.Total - inventory_order.payment;
                }             
                db.inventory_order.Add(inventory_order);
                if (method == 2)
                {
                   
                    var check_debt = db.debts.Where(d => d.sale.customer_id == inventory_order.supplier_id && d.inventory_id != null).Count();
                    if (check_debt > 0)
                    {
                        var last_debt = db.debts.Where(d => d.sale.customer_id == inventory_order.supplier_id && d.inventory_id != null).OrderByDescending(o => o.id).FirstOrDefault();
                        debt debt = new debt();
                        debt.inventory_id = inventory_order.id;
                        debt.paid = inventory_order.payment;
                        debt.created_at = currentDate;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = last_debt.total + inventory_order.payment;
                        debt.debt1 = total;
                        debt.remaining = last_debt.remaining + (total - debt.paid);
                        db.debts.Add(debt);

                        var last_customer_Debt = db.customer_debt.Where(d => d.customer_id == inventory_order.supplier_id && d.inventory_id != null).OrderByDescending(o => o.id).FirstOrDefault();
                        customer_debt customer_Debt = new customer_debt(); 
                        customer_Debt.inventory_id = inventory_order.id;
                        customer_Debt.created_at = currentDate;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = inventory_order.supplier_id;
                        customer_Debt.debt = (total - debt.paid);
                        customer_Debt.remaining = last_debt.remaining + (total - debt.paid);
                        db.customer_debt.Add(customer_Debt);
                    }
                    else
                    {
                        debt debt = new debt();
                        debt.inventory_id = inventory_order.id;
                        debt.paid = inventory_order.payment;
                        debt.created_at = currentDate;
                        debt.created_by = User.Identity.GetUserId();
                        debt.total = inventory_order.payment;
                        debt.debt1 = total;
                        debt.remaining = total - debt.paid;
                        db.debts.Add(debt);

                        customer_debt customer_Debt = new customer_debt();
                        customer_Debt.inventory_id = inventory_order.id;
                        customer_Debt.created_at = currentDate;
                        customer_Debt.created_by = User.Identity.GetUserId();
                        customer_Debt.customer_id = inventory_order.supplier_id;
                        customer_Debt.debt = total - debt.paid;
                        customer_Debt.remaining = total - debt.paid;
                        db.customer_debt.Add(customer_Debt);

                        db.SaveChanges();
                    }
                    db.SaveChanges();                  
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Uniform = db.products.Select(x => new
            {
                Id = x.id,
                Name = (x.category.name + " - " + x.name + " (" + x.unit + (x.unit_swap != null ? "/" + x.quantity_swap + x.unit_swap : "") + ")").ToString()
            });
            ViewBag.Customer = new SelectList(db.customers.Where(c => c.type == 2), "id", "name");
            ViewBag.isCreate = true;
            return View(inventory_order);
        }

        // GET: inventory_order/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            inventory_order inventory_order = db.inventory_order.Find(id);
            if (inventory_order == null)
            {
                return HttpNotFound();
            }
            ViewBag.supplier_id = new SelectList(db.customers, "id", "created_by", inventory_order.supplier_id);
            ViewBag.create_by = new SelectList(db.users, "id", "name", inventory_order.create_by);
            return View(inventory_order);
        }

        // POST: inventory_order/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,code,supplier_id,create_at,update_at,create_by,state,Total,payment,debt")] inventory_order inventory_order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventory_order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.supplier_id = new SelectList(db.customers, "id", "created_by", inventory_order.supplier_id);
            ViewBag.create_by = new SelectList(db.users, "id", "name", inventory_order.create_by);
            return View(inventory_order);
        }

        // GET: inventory_order/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            inventory_order inventory_order = db.inventory_order.Find(id);
            if (inventory_order == null)
            {
                return HttpNotFound();
            }
            return View(inventory_order);
        }

        // POST: inventory_order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            inventory_order inventory_order = db.inventory_order.Find(id);
            db.inventory_order.Remove(inventory_order);
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
