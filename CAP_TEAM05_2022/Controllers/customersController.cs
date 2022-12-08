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

namespace CAP_TEAM05_2022.Controllers
{
    [LoginVerification]
    public class customersController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: customers
        public ActionResult Index()
        {
            return View(db.customers.ToList());
        }
        public ActionResult Create_Customer(string customer_name, string customer_phone,
           string customer_email, DateTime? customers_birth, string customer_account,
           string customer_bank, int customer_type,  string customer_address, string customer_note)
        {
            string email = Session["user_email"].ToString();
            user user = db.users.Where(u => u.email == email).FirstOrDefault();
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
            customer.created_by = user.id;
            customer.created_at = DateTime.Now;
            customer.status = 1;
            db.customers.Add(customer);
            db.SaveChanges();
            Session["notification"] = "Thêm mới thành công!";
            return RedirectToAction("Index");
        }

        // GET: customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,created_by,name,email,phone,address,type,status,account_number,bank,note,birthday,price_level,created_at,updated_at,deleted_at")] customer customer)
        {
            if (ModelState.IsValid)
            {
                db.customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,created_by,name,email,phone,address,type,status,account_number,bank,note,birthday,price_level,created_at,updated_at,deleted_at")] customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customer customer = db.customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            customer customer = db.customers.Find(id);
            db.customers.Remove(customer);
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
