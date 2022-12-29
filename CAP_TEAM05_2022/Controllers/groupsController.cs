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
    public class groupsController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();

        // GET: groups
        public ActionResult Index()
        {
            var groups = db.groups.Include(g => g.user).Where(c => c.status != 3).OrderByDescending(c => c.id);
            return View(groups.ToList());
        }
        public ActionResult Create_GroupProduct(string name_GroupProduct)
        {
            string email = Session["user_email"].ToString();
            user user = db.users.Where(u => u.email == email).FirstOrDefault();
            group GroupProduct = new group();
            GroupProduct.name = name_GroupProduct;
            GroupProduct.created_by = user.id;
            GroupProduct.status = 1;
            GroupProduct.created_at = DateTime.Now;
            GroupProduct.slug = name_GroupProduct;
            GroupProduct.code = "NH" + CodeRandom.RandomCode();
            db.groups.Add(GroupProduct);
            db.SaveChanges();
            Session["notification"] = "Thêm mới thành công!";
            return RedirectToAction("Index");
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
            if (group.status == 1)
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
       /* public JsonResult GetGroupProduct(int Id)
        {
            group group = db.groups.Find(Id);
            return Json(new { success = true, data = group }, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult getGroupProduct()
        {
           
            return Json(db.groups.Where(c => c.status == 1).OrderByDescending(c => c.id).Select(x => new
            {
                groupID = x.id,
                groupName = x.name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        /*public ActionResult Edit_GroupProduct(int GroupProduct_id, string name_GroupProduct)
        {
            group group = db.groups.Find(GroupProduct_id);
            group.name = name_GroupProduct;
            group.updated_at = DateTime.Now;
            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }*/
        public JsonResult UpdateGroupProduct(group GroupProducts)
        {
            group group = db.groups.Find(GroupProducts.id);
            group.name = GroupProducts.name;
            group.updated_at = DateTime.Now;
            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();
            string message = "Record Saved Successfully ";
            bool status = true;
            return Json(new { status = status, message = message }, JsonRequestBehavior.AllowGet);
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
