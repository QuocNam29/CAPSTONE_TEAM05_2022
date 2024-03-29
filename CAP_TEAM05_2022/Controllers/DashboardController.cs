﻿using CAP_TEAM05_2022.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


namespace CAP_TEAM05_2022.Controllers
{
    public class DashboardController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        // GET: Dashboard
        public ActionResult Index(DateTime? from, DateTime? to)
        {
            var data = new StatisticsViewModel();
            var user = db.AspNetUsers.ToList();
            var order = db.sales.Where(x => (!from.HasValue || DbFunctions.TruncateTime(x.created_at) >= DbFunctions.TruncateTime(from.Value))
                && (!to.HasValue || DbFunctions.TruncateTime(x.created_at) <= DbFunctions.TruncateTime(to.Value))).ToList();
            var product = db.products.Where(x => x.status == Constants.SHOW_STATUS).ToList();
            var supplier = db.customers.Where(x => x.type == Constants.SUPPLIER).ToList();
            var customer = db.customers.Where(x => x.type == Constants.CUSTOMER && x.id > 0).ToList();
            if (from == null)
            {
                from = (DateTime.Now.AddMonths(-1));
            }
            if (to == null)
            {
                to = (DateTime.Now);
            }
            ViewBag.From = from;
            ViewBag.To = to;
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => (s.created_at >= from && s.created_at <= to
                                                    || s.created_at.Value.Day == from.Value.Day
                                                    && s.created_at.Value.Month == from.Value.Month
                                                    && s.created_at.Value.Year == from.Value.Year
                                                    || s.created_at.Value.Day == to.Value.Day
                                                    && s.created_at.Value.Month == to.Value.Month
                                                    && s.created_at.Value.Year == to.Value.Year)
                                                    && s.is_old_debt != true);

            data.TotalRevenue = 0;
            data.CountUser = user.Count;
            data.CountAdmin = user.Where(x => x.AspNetRoles.FirstOrDefault().Name == Constants.ADMIN_ROLE).ToList().Count;
            data.CountStaff = user.Where(x => x.AspNetRoles.FirstOrDefault().Name == Constants.STAFF_ROLE).ToList().Count;

            data.CountOrder = order.Count;
            data.CountArr1 = new int[3];
            data.CountArr1[0] = order.Where(x => x.method == Constants.PAYED_ORDER).Count();
            data.CountArr1[1] = order.Where(x => x.method == Constants.DEBT_ORDER && x.is_debt_price == false).Count();
            data.CountArr1[2] = order.Where(x => x.method == Constants.DEBT_ORDER && x.is_debt_price == true).Count();

            data.CountCustomer = customer.Count;
            data.CountSupplier = supplier.Count;
            int i = 0;
            data.ItemArr2 = new string[sales.GroupBy(x => DbFunctions.TruncateTime(x.created_at)).Count()];
            data.CountArr2 = new decimal[sales.GroupBy(x => DbFunctions.TruncateTime(x.created_at)).Count()];
            data.CountArr3 = new int[sales.GroupBy(x => DbFunctions.TruncateTime(x.created_at)).Count()];
            foreach (var item in sales.GroupBy(x => DbFunctions.TruncateTime(x.created_at)))               
             {
                data.ItemArr2[i] = ((DateTime)(item.Key)).ToString("dd/MM/yyyy");
                data.CountArr3[i] = sales.Where(x => DbFunctions.TruncateTime(x.created_at) == item.Key).Count();
                decimal nhapHang = 0;
                var saleGroupBy = sales.Where(x => DbFunctions.TruncateTime(x.created_at) == item.Key).ToList();
                    foreach (var item1 in saleGroupBy)
                    {
                        foreach (var item2 in item1.sale_details)
                        {
                            foreach (var item3 in item2.revenues)
                            {
                                if (item3.unit == item3.import_inventory.product.unit)
                                {
                                    nhapHang += item3.quantity * item3.import_inventory.price_import;

                                }
                                else
                                {
                                    nhapHang += item3.quantity * (decimal)(item3.import_inventory.price_import / item3.import_inventory.product.quantity_swap);

                                }
                            }
                        }
                    }
                    data.CountArr2[i] = sales.Where(x => DbFunctions.TruncateTime(x.created_at) == item.Key).Sum(x => x.total) - nhapHang;
                data.TotalRevenue += data.CountArr2[i];
               i++;                
            }
            var TopCustomer = customer.OrderByDescending(c => c.sales.Where(s => (s.created_at >= from && s.created_at <= to
                                                               || s.created_at.Value.Day == from.Value.Day
                                                               && s.created_at.Value.Month == from.Value.Month
                                                               && s.created_at.Value.Year == from.Value.Year
                                                               || s.created_at.Value.Day == to.Value.Day
                                                               && s.created_at.Value.Month == to.Value.Month
                                                               && s.created_at.Value.Year == to.Value.Year)
                                                               && s.is_old_debt != true).Sum(y => y.total)).Take((10));
            data.ItemArr4 = new string[10];
            data.CountArr4 = new decimal[10];
            int j = 0;
            foreach (var item in TopCustomer)
            {
                data.ItemArr4[j] = item.name;
                data.CountArr4[j] = item.sales.Where(s => (s.created_at >= from && s.created_at <= to
                                                    || s.created_at.Value.Day == from.Value.Day
                                                    && s.created_at.Value.Month == from.Value.Month
                                                    && s.created_at.Value.Year == from.Value.Year
                                                    || s.created_at.Value.Day == to.Value.Day
                                                    && s.created_at.Value.Month == to.Value.Month
                                                    && s.created_at.Value.Year == to.Value.Year)
                                                    && s.is_old_debt != true).Sum(x => x.total);
                j++;
            }
            return View(data);
        }

        public ActionResult _TopCustomer(DateTime? from, DateTime? to, int? TopOption)
        {
            var data = new StatisticsViewModel();

            if (from == null)
            {
                from = (DateTime.Now.AddMonths(-1));
            }
            if (to == null)
            {
                to = (DateTime.Now);
            }
            if (TopOption == null)
            {
                TopOption = 10;
                ViewBag.Flat = false;
            }
            var customer = db.customers.Where(x => x.type == Constants.CUSTOMER && x.id > 0).ToList();
            var TopCustomer = customer.OrderByDescending(c => c.sales.Where(s => (s.created_at >= from && s.created_at <= to
                                                    || s.created_at.Value.Day == from.Value.Day
                                                    && s.created_at.Value.Month == from.Value.Month
                                                    && s.created_at.Value.Year == from.Value.Year
                                                    || s.created_at.Value.Day == to.Value.Day
                                                    && s.created_at.Value.Month == to.Value.Month
                                                    && s.created_at.Value.Year == to.Value.Year)
                                                    && s.is_old_debt != true).Sum(y => y.total)).Take((int)TopOption);

            data.ItemArr4 = new string[(int)TopOption];
            data.CountArr4 = new decimal[(int)TopOption];
            int j = 0;
            foreach (var item in TopCustomer)
            {
                data.ItemArr4[j] = item.name;
                data.CountArr4[j] = item.sales.Where(s => (s.created_at >= from && s.created_at <= to
                                                    || s.created_at.Value.Day == from.Value.Day
                                                    && s.created_at.Value.Month == from.Value.Month
                                                    && s.created_at.Value.Year == from.Value.Year
                                                    || s.created_at.Value.Day == to.Value.Day
                                                    && s.created_at.Value.Month == to.Value.Month
                                                    && s.created_at.Value.Year == to.Value.Year)
                                                    && s.is_old_debt != true).Sum(x => x.total);
                j++;
            }

            return View(data);
        }
    }
}