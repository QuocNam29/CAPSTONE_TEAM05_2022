using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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
            ViewBag.isCreate = false;
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

        [HttpGet]
        public PartialViewResult _Form(string id)
        {
            if (id != null)
            {
                ViewBag.isCreate = false;
                ViewBag.aspNetUser = db.AspNetUsers.Find(id);
                return PartialView("_Form");
            }
            ViewBag.isCreate = true;
            return PartialView("_Form");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create_User([Bind(Include = "Email, RoleID, PhoneNumber, Address, Name, Password, ConfirmPassword")] RegisterViewModel aspNetUser)
        {
            string message = "";
            bool status = true;
            try
            {  
                if (ModelState.IsValid)
                {
                    var query_email = UserManager.FindByEmail(aspNetUser.Email);
                    var role = db.AspNetRoles.Find(aspNetUser.RoleID.ToString());

                    if (query_email == null)
                    {
                        var user = new ApplicationUser
                        {
                            Email = aspNetUser.Email,
                            UserName = aspNetUser.Email,
                            PhoneNumber = aspNetUser.PhoneNumber,
                            PhoneNumberConfirmed = true,
                        };
                        var result = await UserManager.CreateAsync(user, aspNetUser.Password);
                        if (result.Succeeded)
                        {
                            UserManager.AddToRole(user.Id, role.Name);
                            user users = new user
                            {
                                id = user.Id,
                                email = aspNetUser.Email,
                                remember_token = user.SecurityStamp,
                                created_at = DateTime.Now,
                                phone = aspNetUser.PhoneNumber,
                                address = aspNetUser.Address,
                                name = aspNetUser.Name,
                                asp_id = user.Id
                            };
                            db.users.Add(users);
                            db.SaveChanges();

                            // Send confirmation email
                            string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
                            await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                            message = "Thêm nhân viên thành công";
                        }
                        else
                        {
                            status = false;
                            foreach (var item in result.Errors)
                            {
                                message += item.ToString();
                            }
                            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(new { status = false, message = "Email đã tồn tại!" }, JsonRequestBehavior.AllowGet);
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Edit_User([Bind(Include = "Email, RoleID, PhoneNumber, Address, Name")] RegisterViewModel EditUser)
        {
            string message = "";
            bool status = true;
            try
            {
                user user = db.users.FirstOrDefault(x=> x.email == EditUser.Email);
                var aspNetUser = UserManager.FindById(user.id);
                string oldRole = UserManager.GetRoles(user.id).FirstOrDefault();
                AspNetRole role = db.AspNetRoles.Find(EditUser.RoleID.ToString());
                if (user.phone == EditUser.PhoneNumber)
                {
                    user.name = EditUser.Name;
                    user.phone = EditUser.PhoneNumber;
                    user.address = EditUser.Address;
                    user.updated_at = DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    aspNetUser.PhoneNumber = EditUser.PhoneNumber;
                    aspNetUser.AccessFailedCount = EditUser.RoleID;
                    db.SaveChanges();
                    message = "Cập nhật thông tin nhân viên thành công !";
                }
                else
                {
                    int check = db.users.Where(c => c.phone == EditUser.PhoneNumber).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Số điện thoại đã được sử dụng !";
                    }
                    else
                    {

                        user.name = EditUser.Name;
                        user.phone = EditUser.PhoneNumber;
                        user.address = EditUser.Address;
                        user.updated_at = DateTime.Now;
                        db.Entry(user).State = EntityState.Modified;
                        aspNetUser.PhoneNumber = EditUser.PhoneNumber;
                        aspNetUser.AccessFailedCount = EditUser.RoleID;
                        db.SaveChanges();
                        message = "Cập nhật thông tin nhân viên thành công !";
                    }

                }
                if (oldRole != null)
                {
                    // Update user role
                    UserManager.RemoveFromRole(user.id, oldRole);
                    UserManager.AddToRole(user.id, role.Name);
                }
                else
                {
                    // Add user to role
                    UserManager.AddToRole(user.id, role.Name);
                }

            }
            catch (Exception e)
            {

                status = false;
                message = e.Message;
            }

            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
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
/*        public JsonResult UpdateUser(string user_id, string role_id, string fullName, string phone, string address)
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
*/        // GET: AspNetUsers/Edit/5
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

        public ActionResult Delete_User(AspNetUser aspNetUser)
        {
            bool status = true;
            string mess = "";
            try
            {
                user user = db.users.Find(aspNetUser.Id);
                AspNetUser aspNet = db.AspNetUsers.Find(aspNetUser.Id);
                db.users.Remove(user);
                db.AspNetUsers.Remove(aspNet);
                db.SaveChanges();
                mess = "Xóa nhân viên thành công";
            }
            catch
            {
                status = false;
                mess = "Tài khoản đã được sử dụng !";
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
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
