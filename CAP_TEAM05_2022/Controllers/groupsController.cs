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
    public class groupsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        public groupsController()
        {
            ViewBag.isCreate = false;
        }
        // GET: groups
        public ActionResult Index()
        {

            return View();
        }
        public PartialViewResult _Form(int? id)
        {
            if (id != null)
            {
                ViewBag.isCreate = false;
                var group = db.groups.Find(id);
                return PartialView("_Form", group);
            }
            ViewBag.isCreate = true;
            return PartialView("_Form", new group());
        }
        [ValidateAntiForgeryToken]
        public ActionResult Create(group group)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {
                    int check = db.groups.Where(c => c.name == group.name).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Danh mục đã tồn tại !";
                    }
                    else
                    {
                        group.status = 1;
                        group.created_by = User.Identity.GetUserId();
                        group.created_at = DateTime.Now;
                        group.slug = group.name;
                        group.code = "DM" + CodeRandom.RandomCode();
                        db.groups.Add(group);
                        db.SaveChanges();
                        message = "Tạo danh mục thành công";
                        return Json(new { status, message }, JsonRequestBehavior.AllowGet);
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
        [ValidateAntiForgeryToken]
        public ActionResult Edit(group group)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {
                    int check = db.groups.Where(c => c.name == group.name).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Tên danh mục đã tồn tại !";
                    }
                    else
                    {
                        group.updated_at = DateTime.Now;
                        db.Entry(group).State = EntityState.Modified;
                        db.SaveChanges();
                        message = "Cập nhật danh mục thành công";
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
        public ActionResult _GroupList()
        {
            var groups = db.groups.Include(g => g.user).Where(c => c.status != 3).OrderByDescending(c => c.id);
            return PartialView(groups.ToList());
        }
        public ActionResult Create_GroupProduct(string Add_name)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.groups.Where(c => c.name == Add_name).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Nhóm hàng đã tồn tại !";
                }
                else
                {

                    group GroupProduct = new group();
                    GroupProduct.name = Add_name;
                    GroupProduct.created_by = User.Identity.GetUserId();
                    GroupProduct.status = 1;
                    GroupProduct.created_at = DateTime.Now;
                    GroupProduct.slug = Add_name;
                    GroupProduct.code = "NH" + CodeRandom.RandomCode();
                    db.groups.Add(GroupProduct);
                    db.SaveChanges();
                    message = "Tạo nhóm hàng thành công";
                }
            }
            catch (Exception e)
            {

                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Delete_GroupProduct(group GroupProducts)
        {
            bool status = true;
            string mess = "";
            try
            {
                group group = db.groups.Find(GroupProducts.id);
                db.groups.Remove(group);
                db.SaveChanges();
                mess = "Xóa nhóm hàng thành công";
            }
            catch (Exception)
            {

                status = false;
                mess = "Xóa thất bại ! (còn sản phẩm thuộc nhóm hàng).";
            }

            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStatus_GroupProduct(group GroupProducts)
        {
            group group = db.groups.Find(GroupProducts.id);
            if (group.status == Constants.SHOW_STATUS)
            {
                group.status = 2;
            }
            else
            {
                group.status = 1;
            }
            group.updated_at = DateTime.Now;
            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_GroupProduct", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FindGroupProduct(int GroupProduct_id)
        {
            group group = db.groups.Find(GroupProduct_id);
            var emp = new group();
            emp.id = GroupProduct_id;
            emp.name = group.name;
            return Json(emp);
        }

        public ActionResult getGroupProduct()
        {

            return Json(db.groups.Where(c => c.status == Constants.SHOW_STATUS).OrderByDescending(c => c.id).Select(x => new
            {
                groupID = x.id,
                groupName = x.name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateGroupProduct(int id, string Edit_name)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.groups.Where(c => c.name == Edit_name).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Tên nhóm hàng đã tồn tại !";
                }
                else
                {
                    group group = db.groups.Find(id);
                    group.name = Edit_name;
                    group.updated_at = DateTime.Now;
                    db.Entry(group).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Cập nhật nhóm hàng thành công";
                }
            }
            catch (Exception e)
            {

                message = e.Message;
                status = false;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckGroupnameAvailability(string categorydata)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.groups.Where(x => x.name == categorydata && x.status != 3).FirstOrDefault();
            if (SeachData != null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }

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
