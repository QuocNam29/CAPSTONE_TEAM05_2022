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
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class customersController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: customers
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _CustomerList(int? type)
        {
            var links = from l in db.customers
                        select l;
            if (type != null)
            {
                links = links.Where(p => p.type == type);
            }

            return PartialView(links.OrderByDescending(c => c.id));
        }
        public ActionResult Create_Customer(string customer_name, string customer_phone,
           string customer_email, DateTime? customers_birth, string customer_account,
           string customer_bank, int customer_type, string customer_address, string customer_note)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.customers.Where(c => c.phone == customer_phone ).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Khách hàng đã tồn tại ! (Vui lòng kiểm tra lại số điện thoại)";
                }
                else
                {
                   
                    customer customer = new customer();
                    customer.name = customer_name;
                    customer.code = "MKH" + CodeRandom.RandomCode();
                    customer.phone = customer_phone;
                    if (!String.IsNullOrWhiteSpace(customer_email))
                    {
                        customer.email = customer_email;
                    }
                    if (customers_birth != null)
                    {
                        customer.birthday = customers_birth;
                    }
                    if (!String.IsNullOrWhiteSpace(customer_account))
                    {
                        customer.account_number = customer_account;
                    }
                    if (!String.IsNullOrWhiteSpace(customer_bank))
                    {
                        customer.bank = customer_bank;
                    }
                    customer.type = customer_type;
                    customer.address = customer_address;
                    if (!String.IsNullOrWhiteSpace(customer_note))
                    {
                        customer.note = customer_note;
                    }
                    customer.created_by = User.Identity.GetUserId();
                    customer.created_at = DateTime.Now;
                    customer.status = 1;
                    db.customers.Add(customer);
                    db.SaveChanges();
                    message = "Thêm khách hàng thành công !";
                }
            }
            catch (Exception e)
            {

                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStatus_Customer(customer customer_current)
        {
            customer customer = db.customers.Find(customer_current.id);
            if (customer.status == 1)
            {
                customer.status = 2;
            }
            else
            {
                customer.status = 1;
            }
            customer.updated_at = DateTime.Now;
            db.Entry(customer).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_GroupProduct", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult FindCustomer(int customer_id)
        {
            customer customer = db.customers.Find(customer_id);
            var emp = new customer();
            emp.id = customer_id;
            emp.name = customer.name;
            emp.phone = customer.phone;
            emp.email = customer.email;
            emp.birthday = customer.birthday;
            emp.account_number = customer.account_number;
            emp.bank = customer.bank;
            emp.type = customer.type;
            emp.address = customer.address;
            emp.note = customer.note;
            return Json(emp);
        }
        [HttpPost]
        public JsonResult FindCustomer_name(int customer_id)
        {
            customer customer = db.customers.Find(customer_id);
            var emp = new customer();
            emp.id = customer.id;
            emp.name = customer.name;
            emp.phone = customer.phone;
            if (customer.type == 0)
            {
                emp.note = "Khách mua lẻ";
            }
            else if (customer.type == 1)
            {
                emp.note = "Khách mua sĩ";
            }
            else if (customer.type == 2)
            {
                emp.note = "Nhà cung cấp";
            }
            emp.code = customer.code;
            emp.status = customer.sales.Count();
            emp.type = (int)customer.sales.Where(s => s.method == 2).Sum(s => s.total - s.prepayment);
            return Json(emp);
        }
        public JsonResult UpdateCustomer(int Customer_id, string customer_name, string customer_phone,
           string customer_email, DateTime? customers_birth, string customer_account,
           string customer_bank, int customer_type, string customer_address, string customer_note)
        {
            string message = "";
            bool status = true;
            try
            {
                customer customer = db.customers.Find(Customer_id);
                if (customer.phone == customer_phone )
                {
                    customer.name = customer_name;
                    customer.phone = customer_phone;
                    if (!String.IsNullOrWhiteSpace(customer_email))
                    {
                        customer.email = customer_email;
                    }
                    if (customers_birth != null)
                    {
                        customer.birthday = customers_birth;
                    }
                    if (!String.IsNullOrWhiteSpace(customer_account))
                    {
                        customer.account_number = customer_account;
                    }
                    if (!String.IsNullOrWhiteSpace(customer_bank))
                    {
                        customer.bank = customer_bank;
                    }
                    customer.type = customer_type;
                    customer.address = customer_address;
                    if (!String.IsNullOrWhiteSpace(customer_note))
                    {
                        customer.note = customer_note;
                    }
                    customer.updated_at = DateTime.Now;
                    db.Entry(customer).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Cập nhật thông tin khách hàng thành công !";
                }
                else
                {
                    int check = db.customers.Where(c => c.phone == customer_phone).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Khách hàng đã tồn tại !";
                    }
                    else
                    {
                        customer.name = customer_name;
                        customer.phone = customer_phone;
                        if (!String.IsNullOrWhiteSpace(customer_email))
                        {
                            customer.email = customer_email;
                        }
                        if (customers_birth != null)
                        {
                            customer.birthday = customers_birth;
                        }
                        if (!String.IsNullOrWhiteSpace(customer_account))
                        {
                            customer.account_number = customer_account;
                        }
                        if (!String.IsNullOrWhiteSpace(customer_bank))
                        {
                            customer.bank = customer_bank;
                        }
                        customer.type = customer_type;
                        customer.address = customer_address;
                        if (!String.IsNullOrWhiteSpace(customer_note))
                        {
                            customer.note = customer_note;
                        }
                        customer.updated_at = DateTime.Now;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                        message = "Cập nhật thông tin khách hàng thành công !";
                    }
                }
             
            }
            catch (Exception e)
            {

                status = false;
                message = e.Message;
            }
            
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSearchValue(string search)
        {
            var customers = (from customer in db.customers
                             where customer.name.Contains(search) && customer.status != 3
                             select new
                             {
                                 label = customer.name,
                                 val = customer.id
                             }).ToList();

            return Json(customers);
        }
        [HttpPost]
        public JsonResult GetSearch_phoneValue(string search)
        {
            var customers = (from customer in db.customers
                             where customer.phone.Contains(search) && customer.status != 3
                             select new
                             {
                                 label = customer.phone,
                                 val = customer.id
                             }).ToList();

            return Json(customers);
        }
        public JsonResult CheckCustomernameAvailability(string customer_name)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.customers.Where(x => x.name == customer_name).FirstOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        public JsonResult CheckCustomerPhoneAvailability(string phone)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.customers.Where(x => x.phone == phone).FirstOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

        }
        public JsonResult AddCustomerSale(string customer_name, string customer_phone,
           string customer_email, DateTime? customers_birth, string customer_account,
           string customer_bank, int customer_type, string customer_address, string customer_note)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.customers.Where(c => c.phone == customer_phone).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Khách hàng đã tồn tại ! (Vui lòng kiểm tra lại số điện thoại)";
                }
                else
                {
                    
                    customer customer = new customer();
                    customer.name = customer_name;
                    customer.code = "MKH" + CodeRandom.RandomCode();
                    customer.phone = customer_phone;
                    if (!String.IsNullOrWhiteSpace(customer_email))
                    {
                        customer.email = customer_email;
                    }
                    if (customers_birth != null)
                    {
                        customer.birthday = customers_birth;
                    }
                    if (!String.IsNullOrWhiteSpace(customer_account))
                    {
                        customer.account_number = customer_account;
                    }
                    if (!String.IsNullOrWhiteSpace(customer_bank))
                    {
                        customer.bank = customer_bank;
                    }
                    customer.type = customer_type;
                    customer.address = customer_address;
                    if (!String.IsNullOrWhiteSpace(customer_note))
                    {
                        customer.note = customer_note;
                    }
                    customer.created_by = User.Identity.GetUserId();
                    customer.created_at = DateTime.Now;
                    customer.status = 1;
                    db.customers.Add(customer);
                    db.SaveChanges();
                    message = "Thêm khách hàng thành công !";
                    return Json(new { status, message, customer }, JsonRequestBehavior.AllowGet);

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
