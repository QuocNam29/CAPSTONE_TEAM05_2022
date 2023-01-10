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
    public class categoriesController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: categories
        public ActionResult Index()
        {
            var categories = db.categories.Where(c => c.status != 3).OrderByDescending(c => c.id).ToList();
            return View(categories);
        }
        public ActionResult _CategoryList()
        {
            var categories = db.categories.Include(c => c.user).Where(c => c.status != 3).OrderByDescending(c => c.id);
            return PartialView(categories.ToList());
        }

        public ActionResult Create_Category(string name_category)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.categories.Where(c => c.name == name_category).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Danh mục đã tồn tại !";
                }
                else
                {
                    string email = Session["user_email"].ToString();
                    user user = db.users.Where(u => u.email == email).FirstOrDefault();
                    category category = new category();
                    category.name = name_category;
                    category.status = 1;
                    category.created_by = user.id;
                    category.created_at = DateTime.Now;
                    category.slug = name_category;
                    category.code = "DM" + CodeRandom.RandomCode();
                    db.categories.Add(category);
                    db.SaveChanges();
                    message = "Tạo danh mục thành công";
                }

            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Delete_Category(category categorys)
        {
            bool status = true;
            string mess = "";
            try
            {
                category categories = db.categories.Find(categorys.id);
                db.categories.Remove(categories);
                db.SaveChanges();
                mess = "Xóa danh mục thành công";
            }
            catch
            {
                status = false;
                mess = "Xóa thất bại ! (còn sản phẩm thuộc danh mục).";
            }
            return Json(new { status = status, message = mess }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditStatus_Category(category categorys)
        {
            category categories = db.categories.Find(categorys.id);
            if (categories.status == 1)
            {
                categories.status = 2;
            }
            else
            {
                categories.status = 1;
            }
            categories.updated_at = DateTime.Now;
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Category", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCategory(int id, string name_category)
        {
            string message = "";
            bool status = true;
            try
            {
                int check = db.categories.Where(c => c.name == name_category).Count();
                if (check > 0)
                {
                    status = false;
                    message = "Tên danh mục đã tồn tại !";
                }
                else
                {
                    category categories = db.categories.Find(id);
                    categories.name = name_category;
                    categories.updated_at = DateTime.Now;
                    db.Entry(categories).State = EntityState.Modified;
                    db.SaveChanges();
                    message = "Cập nhật danh mục thành công";
                }
            }
            catch (Exception e)
            {

                message = e.Message;
                status = false;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult FindCategory(int GroupProduct_id)
        {
            category categories = db.categories.Find(GroupProduct_id);
            var emp = new category();
            emp.id = GroupProduct_id;
            emp.name = categories.name;
            return Json(emp);
        }
        public ActionResult getCategory()
        {

            return Json(db.categories.Where(c => c.status == 1).OrderByDescending(c => c.id).Select(x => new
            {
                categoryID = x.id,
                categoryName = x.name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Edit_Category(int category_id, string name_category)
        {
            category categories = db.categories.Find(category_id);
            categories.name = name_category;
            categories.updated_at = DateTime.Now;
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult CheckCategorynameAvailability(string categorydata)
        {
            System.Threading.Thread.Sleep(200);
            var SeachData = db.categories.Where(x => x.name == categorydata && x.status != 3).FirstOrDefault();
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
