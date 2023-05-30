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

    public class categoriesController : Controller
    {
        public categoriesController()
        {
            ViewBag.isCreate = false;
        }
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: categories
        public ActionResult Index()
        {
            var categories = db.categories.OrderByDescending(c => c.id).ToList();
            return View(categories);
        }

        [HttpGet]
        public PartialViewResult _Form(int? id)
        {
            if (id != null)
            {
                ViewBag.isCreate = false;
                var category = db.categories.Find(id);
                return PartialView("_Form", category);
            }
            ViewBag.isCreate = true;
            return PartialView("_Form", new category());
        }

        [ValidateAntiForgeryToken]
        public ActionResult Create(category category)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {
                    int check = db.categories.Where(c => c.name == category.name).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Danh mục đã tồn tại !";
                    }
                    else
                    {
                        category.status = Constants.SHOW_STATUS;
                        category.created_by = User.Identity.GetUserId();
                        category.created_at = DateTime.Now;
                        category.code =  $"MDM-{DateTime.Now:ddMMyyHHmmssfff}";
                        db.categories.Add(category);
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
        public ActionResult Edit(category category)
        {
            string message = "";
            bool status = true;
            try
            {
                if (ModelState.IsValid)
                {
                    int check = db.categories.Where(c => c.name == category.name).Count();
                    if (check > 0)
                    {
                        status = false;
                        message = "Tên danh mục đã tồn tại !";
                    }
                    else
                    {
                        category.updated_at = DateTime.Now;
                        db.Entry(category).State = EntityState.Modified;
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
        public ActionResult _CategoryList()
        {
            var categories = db.categories.Include(c => c.user).OrderByDescending(c => c.id);
            return PartialView(categories.ToList());
        }
        [HttpPost]
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
            if (categories.status == Constants.SHOW_STATUS)
            {
                categories.status = Constants.HIDDEN_STATUS;
            }
            else
            {
                categories.status = Constants.SHOW_STATUS;
            }
            categories.updated_at = DateTime.Now;
            db.Entry(categories).State = EntityState.Modified;
            db.SaveChanges();
            return Json("EditStatus_Category", JsonRequestBehavior.AllowGet);
        }
        public ActionResult getCategory()
        {

            return Json(db.categories.Where(c => c.status == Constants.SHOW_STATUS).OrderByDescending(c => c.id).Select(x => new
            {
                categoryID = x.id,
                categoryName = x.name
            }).ToList(), JsonRequestBehavior.AllowGet);
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
