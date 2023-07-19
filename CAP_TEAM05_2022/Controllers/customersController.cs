using CAP_TEAM05_2022.Helper;
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
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class customersController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public customersController()
        {
            ViewBag.isCreate = false;
            ViewBag.isCustomer = false;

        }
        // GET: customers
        public ActionResult Customers()
        {
            ViewBag.isCustomer = true;
            return View("Index");
        }
        public ActionResult Suppliers()
        {
            return View("Index");
        }

        public PartialViewResult _Form(int? id, int role)
        {
            if (role == Constants.CUSTOMER)
            {
                ViewBag.isCustomer = true;

            }
            if (id != null)
            {
                ViewBag.isCreate = false;
                var customer = db.customers.Find(id);
                return PartialView("_Form", customer);
            }
            ViewBag.isCreate = true;
            return PartialView("_Form", new customer());
        }

        [ValidateAntiForgeryToken]
        public ActionResult Create(customer customer)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {
                    bool check = db.customers.Where(c => c.phone == customer.phone || c.name == customer.name).Any();
                    if (check)
                    {
                        status = false;
                        message = "Tên hoặc số điện thoại đã tồn tại";
                    }
                    else
                    {
                        if (customer.type == Constants.CUSTOMER)
                        {
                            customer.code = $"MKH-{DateTime.Now:ddMMyyHHmmss}";

                        }
                        else
                        {
                            customer.code = $"MCT-{DateTime.Now:ddMMyyHHmmssfff}";

                        }
                        customer.created_by = User.Identity.GetUserId();
                        customer.created_at = DateTime.Now;
                        customer.status = Constants.SHOW_STATUS;
                        db.customers.Add(customer);
                        db.SaveChanges();
                        message = "Thêm thành công !";
                        return Json(new { status, message, customer.id }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            ViewBag.isCreate = true;
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit(customer customer)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {

                    int check = db.customers.Where(c =>( c.phone == customer.phone || c.name == customer.name) && c.id != customer.id).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Số điện thoại đã tồn tại !";
                    }
                    else
                    {
                        customer.updated_at = DateTime.Now;
                        db.Entry(customer).State = EntityState.Modified;
                        db.SaveChanges();
                        message = "Cập nhật thông tin thành công !";
                    }
                }

            }
            catch (Exception e)
            {

                message = e.Message;
                status = false;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult _CustomerList()
        {
            var links = db.customers.Where(x => x.type == Constants.CUSTOMER && x.id > 0);
            return PartialView(links.OrderByDescending(c => c.id));
        }

        public ActionResult _SupplierList()
        {
            var links = db.customers.Where(x => x.type == Constants.SUPPLIER && x.id > 0);
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
                    customer.code = $"MKH-{DateTime.Now:ddMMyyHHmmss}";
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
                    customer.status = Constants.SHOW_STATUS;
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
            if (customer.status == Constants.SHOW_STATUS)
            {
                customer.status = Constants.HIDDEN_STATUS;
            }
            else
            {
                customer.status = Constants.SHOW_STATUS;
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
            emp.note = Constants.TypeCustomer.Single(x => x.Key == customer.type).Value.Item1;
            emp.code = customer.code;
            emp.status = customer.sales.Count();
            emp.type = (int)customer.sales.Where(s => s.method == 2).Sum(s => s.total - s.prepayment - s.pay_debt);
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
                if (customer.phone == customer_phone)
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
                             where customer.name.Contains(search) && customer.status == Constants.SHOW_STATUS && customer.type != Constants.SUPPLIER

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
                             where customer.phone.Contains(search) && customer.status == Constants.SHOW_STATUS
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
                    customer.code = $"MKH-{DateTime.Now:ddMMyyHHmmss}";
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
                    customer.status = Constants.SHOW_STATUS;
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
        public ActionResult getSupplier()
        {

            return Json(db.customers.Where(c => c.status == Constants.SHOW_STATUS && c.type == Constants.SUPPLIER).OrderByDescending(c => c.id).Select(x => new
            {
                supplierID = x.id,
                supplierName = x.name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete_Customer(customer customer)
        {
            bool status = true;
            string mess = "";
            try
            {
                customer customers = db.customers.Find(customer.id);
                int type = customers.type;
                db.customers.Remove(customers);
                db.SaveChanges();
                mess = "Xóa" + (type == Constants.CUSTOMER ? " khách hàng " : " công ty cung cấp ") + "thành công.";
            }
            catch
            {
                status = false;
                mess = "Dữ liệu đã được sử dụng cho chức năng khác (bán hàng, nhập kho...).";
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierList(string searchTerm)
        {
            var supplierList = db.customers.Where(x=> x.type == Constants.SUPPLIER).ToList();

            if (searchTerm != null)
            {
                supplierList = db.customers.Where(x => x.name.Contains(searchTerm) && x.type == Constants.SUPPLIER).ToList();
            }

            var modifiedData = supplierList.Select(x => new
            {
                id = x.id,
                text = x.name
            });
            return Json(modifiedData, JsonRequestBehavior.AllowGet);
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
