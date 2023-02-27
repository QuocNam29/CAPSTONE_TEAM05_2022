using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CAP_TEAM05_2022.Controllers
{
    [CustomAuthorize(Roles = "Quản trị viên, Nhân viên")]
    public class AspNetUsersController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;
        private CP25Team05Entities db = new CP25Team05Entities();
        public AspNetUsersController()
        {
        }
        public AspNetUsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

       
        public async Task<ActionResult> Create_User(string email, string role_id, string fullName, string password, string phone, string address)
        {
            try
            {
                var query_email = UserManager.FindByEmail(email);
                var role = db.AspNetRoles.Find(role_id);
                if (query_email == null)
                {
                    var user = new ApplicationUser
                    {
                        Email = email,
                        UserName = email,
                        PhoneNumber = phone,
                        PhoneNumberConfirmed = true,
                    };
                    var result = await UserManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        UserManager.AddToRole(user.Id, role.Name);
                        user users = new user();
                        users.id = user.Id;
                        users.email = email;
                        users.remember_token = user.SecurityStamp;
                        users.created_at = DateTime.Now;
                        users.phone = phone;
                        users.address = address;
                        users.name = fullName;
                        users.asp_id = user.Id;
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
            if (asp_user.LockoutEnabled == true)
            {
                asp_user.LockoutEnabled = false;
            }
            else
            {
                asp_user.LockoutEnabled = true;
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
                var aspNetUser = UserManager.FindById(user_id);
                string oldRole = UserManager.GetRoles(user_id).FirstOrDefault();
                AspNetRole role = db.AspNetRoles.Find(role_id);
                if (user.phone == phone)
                {
                    user.name = fullName;
                    user.phone = phone;
                    user.address = address;
                    user.updated_at = DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    aspNetUser.PhoneNumber = phone;
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
                        aspNetUser.PhoneNumber = phone;
                        aspNetUser.AccessFailedCount = int.Parse(role_id);
                        db.SaveChanges();
                        message = "Cập nhật thông tin nhân viên thành công !";
                    }

                }
                if (oldRole != null)
                {
                    // Update user role
                    UserManager.RemoveFromRole(user_id, oldRole);
                    UserManager.AddToRole(user_id, role.Name);
                }
                else
                {
                    // Add user to role
                    UserManager.AddToRole(user_id, role.Name);
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
        public ActionResult Edit()
        {
            string id = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(user user, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                user user1 = db.users.Find(user.id);
                if (user1.avatar != null)
                {
                    Session["imgPath"] = user1.avatar;

                }
                else
                {
                    Session["imgPath"] = "default-avatar.png";
                }
                user1.name = user.name; 
                user1.address = user.address;
                user1.phone = user.phone;
                user1.updated_at = DateTime.Now;
                if (file != null)
                {
                    string filename = Path.GetFileName(file.FileName);
                    string _filename = DateTime.Now.ToString("yymmssfff") + filename;

                    string extension = Path.GetExtension(file.FileName);

                    string path = Path.Combine(Server.MapPath("~/Template/assets/images/"), _filename);

                    user1.avatar = _filename;

                    if (extension.ToLower() == ".jpg" || extension.ToLower() == ".jpeg" || extension.ToLower() == ".png")
                    {
                        if (file.ContentLength <= 4000000)
                        {
                            db.Entry(user1).State = EntityState.Modified;
                            string oldImgPath = Request.MapPath(Session["imgPath"].ToString());

                            if (db.SaveChanges() > 0)
                            {
                                file.SaveAs(path);
                                if (System.IO.File.Exists(oldImgPath))
                                {
                                    System.IO.File.Delete(oldImgPath);
                                }
                                return RedirectToAction("Edit");
                            }
                        }
                        else
                        {
                            ViewBag.msg = "Hình ảnh phải lớn hơn hoặc bằng 4MB!";
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Định dạng file không hợp lệ!";
                    }
                }
                else
                {
                    user1.avatar = Session["imgPath"].ToString();
                }
                db.Entry(user1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Edit");
            }
            return View(user);
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
