using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CAP_TEAM05_2022.Controllers
{
    public class AspNetUsersController : Controller
    {
        private ApplicationUserManager _userManager;
        private CP25Team05Entities db = new CP25Team05Entities();
        public AspNetUsersController()
        {
        }

        public AspNetUsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: AspNetUsers
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _UserList()
        {
            return PartialView(db.AspNetUsers.ToList());
        }

        // GET: AspNetUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

       
        public ActionResult Create_User(string email, string role_id, string fullName, string password, string phone, string address)
        {
            try
            {
                var query_email = UserManager.FindByEmail(email);
                var role = db.AspNetRoles.Find(role_id);
                if (query_email == null)
                {

                    ApplicationUser user = new ApplicationUser()
                    {
                        Email = email,
                        UserName = fullName,
                        PhoneNumber = phone,
                        AccessFailedCount = 1,
                    };
                    
                    IdentityResult result = UserManager.Create(user, password);
                    
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user.Id, role.Name);

                        user users = new user();
                        users.id = user.Id;
                        users.email = email;
                        users.remember_token = user.SecurityStamp;
                        users.created_at = DateTime.Now;
                        users.phone = phone;
                        users.address = address;
                        users.name = fullName;
                        db.users.Add(users);
                        db.SaveChanges();
                        return Json(new { status = true, message = "Thêm thành công!" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = false, message = result.Errors }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = false, message = "Email đã tồn tại!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { status = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
            


        }
        public ActionResult EditStatus_User(AspNetUser user)
        {
            AspNetUser asp_user = db.AspNetUsers.Find(user.Id);
            if (asp_user.AccessFailedCount == 1)
            {
                asp_user.AccessFailedCount = 2;
            }
            else
            {
                asp_user.AccessFailedCount = 1;
            }
            db.Entry(asp_user).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_User", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FindUser(string user_id)
        {
            user user = db.users.Find(user_id);
            AspNetUser user1 = db.AspNetUsers.Find(user_id);

            var emp = new user();
            emp.id = user_id;
            emp.name = user.name;
            emp.phone = user.phone;
            emp.email = user.email;
            emp.address = user.address;
            emp.remember_token = user1.AspNetRoles.FirstOrDefault().Id;
            return Json(emp);
        }
        public JsonResult UpdateUser(string user_id, string role_id, string fullName, string phone, string address)
        {
            string message = "";
            bool status = true;
            try
            {
                user user = db.users.Find(user_id);
                if (user.phone == phone)
                {
                    user.name = fullName;
                    user.phone = phone;
                    user.address = address;
                    user.updated_at = DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    AspNetUser aspNetUser = db.AspNetUsers.Find(user_id);
                    aspNetUser.PhoneNumber = phone;
                    aspNetUser.UserName = fullName;
                    aspNetUser.AccessFailedCount = int.Parse(role_id);
                    db.SaveChanges();
                    message = "Cập nhật thông tin nhân viên thành công !";
                }
                else
                {
                    int check = db.users.Where(c => c.phone == phone).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Số điện thoại đã được sử dụng !";
                    }
                    else
                    {

                        user.name = fullName;
                        user.phone = phone;
                        user.address = address;
                        user.updated_at = DateTime.Now;
                        db.Entry(user).State = EntityState.Modified;
                        AspNetUser aspNetUser = db.AspNetUsers.Find(user_id);
                        aspNetUser.PhoneNumber = phone;
                        aspNetUser.UserName = fullName;
                        aspNetUser.AccessFailedCount = int.Parse(role_id);
                        db.SaveChanges();
                        message = "Cập nhật thông tin nhân viên thành công !";
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
        // GET: AspNetUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,avatar,phone,address,status,created_at,updated_at,deleted_at")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
