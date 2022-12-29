﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;

namespace CAP_TEAM05_2022.Controllers
{
    [LoginVerification]

    public class salesController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: sales
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult _OrderList(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start = DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End = DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at.Value.Day >= date_Start.Value.Day && s.created_at.Value.Day <= date_End.Value.Day
            && s.created_at.Value.Month >= date_Start.Value.Month && s.created_at.Value.Month <= date_End.Value.Month
            && s.created_at.Value.Year >= date_Start.Value.Year && s.created_at.Value.Day <= date_End.Value.Year);

            return PartialView(sales.ToList());
        }


        public ActionResult Revenue()
        {
            return View(db.revenues.ToList());
        }
        public ActionResult _RevenueList_Date(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start =(DateTime.Now);
            }
            if (date_End == null)
            {
                date_End = (DateTime.Now);
            }
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at.Value.Day >= date_Start.Value.Day && s.created_at.Value.Day <= date_End.Value.Day
            && s.created_at.Value.Month >= date_Start.Value.Month && s.created_at.Value.Month <= date_End.Value.Month
            && s.created_at.Value.Year >= date_Start.Value.Year && s.created_at.Value.Day <= date_End.Value.Year);
          
            return PartialView(sales.ToList());
        }
        public ActionResult _RevenueList_Month(DateTime? date_Start, DateTime? date_End)
        {
            if (date_Start == null)
            {
                date_Start =DateTime.Now.AddDays((-DateTime.Now.Day) + 1);
            }
            if (date_End == null)
            {
                date_End =DateTime.Now.AddMonths(1).AddDays(-(DateTime.Now.Day));
            }
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at.Value.Month >= date_Start.Value.Month && s.created_at.Value.Month <= date_End.Value.Month
                                                         && s.created_at.Value.Year >= date_Start.Value.Year && s.created_at.Value.Day <= date_End.Value.Year);

            return PartialView(sales.ToList());
        }

        public JsonResult CreateSale(sale createSale)
        {
            string email = Session["user_email"].ToString();
            user user = db.users.Where(u => u.email == email).FirstOrDefault();
            sale sale = new sale();
            sale.code = "MDH"+ CodeRandom.RandomCode();
            sale.customer_id = createSale.customer_id;
            if (createSale .method > 0)
            {
                sale.method = 2;

            }
            else
            {
                sale.method = 1;
            }
            sale.total = createSale.total;
            sale.discount = createSale.discount;
            sale.vat = createSale.vat;
            sale.note = createSale.note;
            sale.status = 1;
            sale.created_by = user.id;
            sale.created_at = DateTime.Now;
            db.sales.Add(sale);
            var cart = db.carts.Where(c => c.customer_id == createSale.customer_id);
            foreach (var item in cart)
            {
  
                sale_details sale_Details = new sale_details();
                sale_Details.sale_id = sale.id;
                sale_Details.product_id = item.product_id;
                sale_Details.sold = item.quantity;
                sale_Details.price = item.price;
                sale_Details.discount = item.discount;
                sale_Details.created_at = DateTime.Now;
                db.sale_details.Add(sale_Details);
                
                import_inventory inventory = db.import_inventory.Where(i => i.product_id == item.product_id && i.sold <= i.quantity 
                                                                    && (i.quantity - i.sold) >= item.quantity).FirstOrDefault();
                inventory.sold += item.quantity;
                db.Entry(inventory).State = EntityState.Modified;
              
                revenue revenue = new revenue();
                revenue.sale_details_id = sale_Details.id;
                revenue.inventory_id = inventory.id;
                revenue.Price = item.price/item.quantity;
                revenue.quantity = item.quantity;
                db.revenues.Add(revenue);
                    cart cart1 = db.carts.Find(item.id);
                db.carts.Remove(cart1);
            }
            db.SaveChanges();
            if (createSale.method > 0)
            {
                debt debt = new debt();
                debt.sale_id = sale.id;
                debt.paid = createSale.method;
                debt.created_at = DateTime.Now;
                debt.created_by = user.id;
                debt.total = createSale.method;
                db.debts.Add(debt);
                db.SaveChanges();
            }
            string message = "Record Saved Successfully ";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
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
