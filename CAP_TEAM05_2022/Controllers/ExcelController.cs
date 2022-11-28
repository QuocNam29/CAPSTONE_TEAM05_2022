﻿using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CAP_TEAM05_2022.Controllers
{
    public class ExcelController : Controller
    {
        private CP25Team05Entities db = new CP25Team05Entities();
        // GET: Excel
        public ActionResult Index()
        {
            return View();
        }

        public void ExportExcel(int group_id, int category_id)
        {
            var list = from l in db.products
                       where l.@group.status == 1 && l.category.status == 1
                       select l;
            if (group_id != -1)
            {
                list = list.Where(p => p.group_id == group_id);
            }
            if (category_id != -1)
            {
                list = list.Where(p => p.category_id == category_id);
            }
            list = list.Where(c => c.status != 3).OrderByDescending(c => c.id);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("SanPham");
            FormatExcel(Sheet, 1);
            Sheet.Cells["A1"].Value = "Mã sản phẩm";
            Sheet.Cells["B1"].Value = "Tên sản phẩm";
            Sheet.Cells["C1"].Value = "Nhóm hàng";
            Sheet.Cells["D1"].Value = "Danh mục";
            Sheet.Cells["E1"].Value = "Đơn vị";
            Sheet.Cells["F1"].Value = "Giá bán";
            Sheet.Cells["G1"].Value = "Giá nhập";
            Sheet.Cells["H1"].Value = "Số lượng nhập";
            Sheet.Cells["I1"].Value = "Đã bán";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.group.name;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.category.name;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.unit;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.sell_price;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.purchase_price;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.quantity;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.import_inventory.Where(i => i.product_id == item.id).Sum(s => s.sold);
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "San_Pham.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        public void ExportTemplateExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            var list_group = db.groups.Where(g => g.status != 3).ToList();
            var list_category = db.categories.Where(g => g.status != 3).ToList();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("NhapSanPham");
            FormatExcel(Sheet, 3);
            Sheet.Cells["A1"].Value = "Tên sản phẩm";
            Sheet.Cells["B1"].Value = "Nhóm hàng";
            Sheet.Cells["C1"].Value = "Danh mục";
            Sheet.Cells["D1"].Value = "Đơn vị";
            Sheet.Cells["E1"].Value = "Giá bán";
            Sheet.Cells["F1"].Value = "Giá nhập";
            Sheet.Cells["G1"].Value = "Số lượng nhập";
            var validation_group = Sheet.Cells["B2:B999999"].DataValidation.AddListDataValidation();
            validation_group.ShowErrorMessage = true;
            validation_group.ErrorStyle = ExcelDataValidationWarningStyle.information;
            validation_group.ErrorTitle = "Lỗi nhập nhóm hàng";
            validation_group.Error = "Nhóm hàng này không có trong hệ thống, vui lòng chọn lại !";
            foreach (var item in list_group)
            {
                validation_group.Formula.Values.Add(item.name);
            }
            var validation_category = Sheet.Cells["C2:C999999"].DataValidation.AddListDataValidation();
            validation_category.ShowErrorMessage = true;
            validation_category.ErrorStyle = ExcelDataValidationWarningStyle.information;
            validation_category.ErrorTitle = "Lỗi nhập danh mục";
            validation_category.Error = "Danh mục này không có trong hệ thống, vui lòng chọn lại !";
            foreach (var item in list_category)
            {
                validation_category.Formula.Values.Add(item.name);
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            ExcelWorksheet Sheet_group = ep.Workbook.Worksheets.Add("NhomHang");
            FormatExcel(Sheet_group, 2);
            Sheet_group.DefaultColWidth = 20;
            Sheet_group.Cells.Style.WrapText = true;

            Sheet_group.Cells["A1"].Value = "Mã nhóm hàng";
            Sheet_group.Cells["B1"].Value = "Tên nhóm hàng";


            int row_group = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list_group)
            {
                Sheet_group.Cells[string.Format("A{0}", row_group)].Value = item.code;
                Sheet_group.Cells[string.Format("B{0}", row_group)].Value = item.name;
                row_group++;
            }
            Sheet_group.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();

            ExcelWorksheet Sheet_category = ep.Workbook.Worksheets.Add("DanhMuc");
            FormatExcel(Sheet_category, 2);
            Sheet_category.Cells["A1"].Value = "Mã danh mục";
            Sheet_category.Cells["B1"].Value = "Tên danh mục";
            int row_category = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list_category)
            {
                Sheet_category.Cells[string.Format("A{0}", row_category)].Value = item.code;
                Sheet_category.Cells[string.Format("B{0}", row_category)].Value = item.name;
                row_category++;
            }
            Sheet_category.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "Mau_Nhap_Lieu.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
        public void FormatExcel(ExcelWorksheet Sheet, int i)
        {
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            if (i == 1)
            {
                // Lấy range vào tạo format cho range đó ở đây là từ A1 tới I1
                using (var range = Sheet.Cells["A1:I1"])
                {

                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Set Border
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    // Set màu ch Border
                    range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    range.Style.Font.Bold = true;
                }
                using (var range = Sheet.Cells["A:C"])
                {
                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
            else if (i == 2)
            {
                // Lấy range vào tạo format cho range đó ở đây là từ A1 tới I1
                using (var range = Sheet.Cells["A1:B1"])
                {

                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Set Border
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    // Set màu ch Border
                    range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    range.Style.Font.Bold = true;
                }
                using (var range = Sheet.Cells["A:C"])
                {
                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }
            else if (i == 3)
            {
                // Lấy range vào tạo format cho range đó ở đây là từ A1 tới I1
                using (var range = Sheet.Cells["A1:G1"])
                {

                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Set Border
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    // Set màu ch Border
                    range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    range.Style.Font.Bold = true;
                }
                using (var range = Sheet.Cells["A:C"])
                {
                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
            }

        }

        [HttpPost]
        public ActionResult ImportExcel(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (extension.ToLower() == ".xls" || extension.ToLower() == ".xlsx")
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    postedFile.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }

                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                                //Get the name of First Sheet.
                                string sheetName = dtExcelSchema.Rows[1]["TABLE_NAME"].ToString();

                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();

                            }
                        }
                    }

                    int addRow = 0;
                    int rowFailFormat = 0;
                    int rowExist = 0;
                    int missData = 0;
                    try
                    {
                        //Insert records to database table.
                        foreach (DataRow row in dt.Rows)
                        {
                            if (!String.IsNullOrEmpty(row["Tên sản phẩm"].ToString().Trim())
                               && !String.IsNullOrEmpty(row["Nhóm hàng"].ToString().Trim())
                           && !String.IsNullOrEmpty(row["Danh mục"].ToString().Trim())
                            && !String.IsNullOrEmpty(row["Đơn vị"].ToString().Trim())
                             && !String.IsNullOrEmpty(row["Giá bán"].ToString().Trim())
                              && !String.IsNullOrEmpty(row["Giá nhập"].ToString().Trim())
                              && !String.IsNullOrEmpty(row["Số lượng nhập"].ToString().Trim()))
                            {
                                Session["name_product"] = row["Tên sản phẩm"].ToString().Trim();
                                Session["group_product"] = row["Nhóm hàng"].ToString().Trim();
                                Session["category_product"] = row["Danh mục"].ToString().Trim();
                                Session["unit_product"] = row["Đơn vị"].ToString().Trim();
                                Session["sell_price_product"] = row["Giá bán"].ToString().Trim();
                                Session["purchase_price_product"] = row["Giá nhập"].ToString().Trim();
                                Session["quantity_product"] = row["Số lượng nhập"].ToString().Trim();

                            }
                            else if (String.IsNullOrEmpty(row["Nhóm hàng"].ToString())
                               || String.IsNullOrEmpty(row["Danh mục"].ToString())
                               || String.IsNullOrEmpty(row["Đơn vị"].ToString())
                               || String.IsNullOrEmpty(row["Giá bán"].ToString())
                               || String.IsNullOrEmpty(row["Giá nhập"].ToString())
                               || String.IsNullOrEmpty(row["Số lượng nhập"].ToString()))

                            {
                                Session["name_product"] = row["Tên sản phẩm"].ToString().Trim();
                                Session["group_product"] = row["Nhóm hàng"].ToString().Trim();
                                Session["category_product"] = row["Danh mục"].ToString().Trim();
                                Session["unit_product"] = null;
                            }
                            else
                            {
                                Session["name_product"] = null;
                            }


                            if (Session["name_product"] != null)
                            {
                                string name_product = Session["name_product"].ToString();
                                if (Session["unit_product"] != null)
                                {
                                    string name_group = Session["group_product"].ToString();
                                string name_category = Session["category_product"].ToString();
                                var check_group = db.groups.Where(g => g.name == name_group && g.status != 3).FirstOrDefault();
                                var check_category = db.categories.Where(g => g.name == name_category && g.status != 3).FirstOrDefault();
                                
                                    if (check_group != null && check_category != null)
                                    {
                                        int group_id = check_group.id;
                                        int category_id = check_category.id;
                                        string unit_product = Session["unit_product"].ToString().Trim();
                                        int sell_price_product = int.Parse(Session["sell_price_product"].ToString().Trim());
                                        int purchase_price_product = int.Parse(Session["purchase_price_product"].ToString().Trim());
                                        int quantity_product = int.Parse(Session["quantity_product"].ToString().Trim());
                                        string email = Session["user_email"].ToString();
                                        user user = db.users.Where(u => u.email == email).FirstOrDefault();

                                        var check_product = db.products.Where(c => c.name == name_product && c.group_id == group_id
                                        && c.category_id == category_id && c.status != 3).FirstOrDefault();
                                        if (check_product == null)
                                        {
                                            
                                            product product = new product();
                                            product.name = name_product;
                                            product.status = 1;
                                            product.unit = unit_product;
                                            product.category_id = category_id;
                                            product.group_id = group_id;
                                            product.created_by = user.id;
                                            product.sell_price = sell_price_product;
                                            product.purchase_price = purchase_price_product;
                                            product.quantity = quantity_product;
                                            product.created_at = DateTime.Now;
                                            product.code = "SP" + CodeRandom.RandomCode();
                                            db.products.Add(product);
                                            import_inventory inventory = new import_inventory();
                                            inventory.product_id = product.id;
                                            inventory.quantity = quantity_product;
                                            inventory.price_import = purchase_price_product;
                                            inventory.sold = 0;
                                            inventory.created_by = user.id;
                                            inventory.created_at = DateTime.Now;
                                            db.import_inventory.Add(inventory);
                                            db.SaveChanges();

                                            addRow++;
                                        }
                                        else
                                        {
                                            check_product.sell_price = sell_price_product;
                                            check_product.purchase_price = purchase_price_product;
                                            check_product.quantity += quantity_product;
                                            check_product.updated_at = DateTime.Now;
                                            db.Entry(check_product).State = EntityState.Modified;
                                            import_inventory inventory = new import_inventory();
                                            inventory.product_id = check_product.id;
                                            inventory.quantity = quantity_product;
                                            inventory.price_import = purchase_price_product;
                                            inventory.sold = 0;
                                            inventory.created_by = user.id;
                                            inventory.created_at = DateTime.Now;
                                            db.import_inventory.Add(inventory);
                                            db.SaveChanges();
                                            rowExist++;
                                        }
                                    }
                                    else
                                    {
                                        rowFailFormat++;
                                    }
                                }
                                else
                                {
                                    missData++;
                                }


                            }

                        }
                        Session["ViewBag.FileStatus"] = null;
                        Session["ViewBag.Success"] = addRow;
                        Session["ViewBag.FailFormat"] = rowFailFormat;
                        Session["ViewBag.Exist"] = rowExist;
                        Session["ViewBag.MissData"] = missData;
                    }
                    catch (Exception e)
                    {
                        string bug = e.ToString();
                        bool check_bug = bug.Contains("does not belong to table");
                        bool check_bug1 = bug.Contains("Object reference not set to an instance of an object");
                        if (check_bug == true)
                        {
                            Session["ViewBag.Success"] = null;
                            Session["ViewBag.FileStatus"] = "Cấu trúc bảng hoặc tên bảng trong tệp excel không đúng định dạng!";
                        }
                        else if (check_bug1 == true)
                        {
                            Session["ViewBag.Success"] = null;
                            Session["ViewBag.FileStatus"] = "Bạn đã bỏ sót dữ liệu ở dòng đầu tiên trong tệp excel!";
                        }
                        else
                        {
                            Session["ViewBag.Success"] = null;
                            Session["ViewBag.FileStatus"] = e.ToString();
                        }
                    }
                    Session["name_product"] = null;
                    Session["group_product"] = null;
                    Session["category_product"] = null;
                    Session["unit_product"] = null;
                    Session["sell_price_product"] = null;
                    Session["purchase_price_product"] = null;
                    Session["quantity_product"] = null;

                }
                else
                {
                    Session["ViewBag.Success"] = null;
                    Session["ViewBag.FileStatus"] = "Định dạng tập tin không hợp lệ, vui lòng nhập file Excel !";
                }

            }
            else
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = "Bạn chưa chọn tệp để nhập!";
            }

            return RedirectToAction("Index", "products");
        }

    }
}