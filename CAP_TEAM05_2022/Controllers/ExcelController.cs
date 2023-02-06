using CAP_TEAM05_2022.Helper;
using CAP_TEAM05_2022.Models;
using Microsoft.AspNet.Identity;
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
        private List<product> Product_list = null;
        public ExcelController()
        {
            var session = System.Web.HttpContext.Current.Session;
            if (session["Product_list"] != null)
            {
                Product_list = session["Product_list"] as List<product>;
            }
            else
            {
                Product_list = new List<product>();
                session["Product_list"] = Product_list;
            }
        }
        private CP25Team05Entities db = new CP25Team05Entities();
        // GET: Excel
        public ActionResult Index()
        {
            return View();
        }

        public void ExportExcel(int group_id, int category_id)
        {
            string nameFile = "SP_"+DateTime.Now+ ".xlsx";
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
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        public void ExportTemplateExcel()
        {
            string nameFile = "Mau_Nhap_" + DateTime.Now + ".xlsx";

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
          /*  ExcelWorksheet Sheet_group = ep.Workbook.Worksheets.Add("NhomHang");
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
            Response.Clear();*/
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
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
            else if (i == 4)
            {
                // Lấy range vào tạo format cho range đó ở đây là từ A1 tới I1
                using (var range = Sheet.Cells["A1:F1"])
                {

                    // Canh giữa cho các text
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    // Set Border
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thick;
                    // Set màu ch Border
                    range.Style.Border.Bottom.Color.SetColor(Color.Black);
                    range.Style.Font.Bold = true;
                }
                using (var range = Sheet.Cells["A:F"])
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
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();

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
                    int row_excel = 1;
                    Product_list.Clear();
                    try
                    {
                        //Insert records to database table.
                        foreach (DataRow row in dt.Rows)
                        {
                            row_excel++;
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
                                            product.created_by = User.Identity.GetUserId();
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
                                            inventory.created_by = User.Identity.GetUserId();
                                            inventory.created_at = DateTime.Now;
                                            db.import_inventory.Add(inventory);
                                            db.SaveChanges();

                                            addRow++;
                                            Product_list.Add(new product
                                            {
                                                id = row_excel,
                                                name = name_product,
                                                status = 4,
                                                name_group = Session["group_product"].ToString(),
                                                name_category = Session["category_product"].ToString(),
                                                unit = unit_product,
                                                purchase_price = purchase_price_product,
                                                sell_price = sell_price_product,
                                                quantity = quantity_product,

                                            }) ;
                                        }
                                        else
                                        {
                                            int quantity_current = check_product.quantity;
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
                                            inventory.created_by = User.Identity.GetUserId();
                                            inventory.created_at = DateTime.Now;
                                            db.import_inventory.Add(inventory);
                                            db.SaveChanges();
                                            rowExist++;
                                            Product_list.Add(new product
                                            {
                                                id = row_excel,
                                                name = name_product,
                                                status = 7,
                                                name_group = Session["group_product"].ToString(),
                                                name_category = Session["category_product"].ToString(),
                                                unit = unit_product,
                                                purchase_price = purchase_price_product,
                                                sell_price = sell_price_product,
                                                quantity = check_product.quantity,
                                                note = quantity_current.ToString()
                                            }) ;
                                        }
                                    }
                                    else
                                    {
                                        rowFailFormat++;
                                        string unit_product = Session["unit_product"].ToString().Trim();
                                        int sell_price_product = int.Parse(Session["sell_price_product"].ToString().Trim());
                                        int purchase_price_product = int.Parse(Session["purchase_price_product"].ToString().Trim());
                                        int quantity_product = int.Parse(Session["quantity_product"].ToString().Trim());
                                        Product_list.Add(new product
                                        {
                                            id = row_excel,
                                            name = name_product,
                                            status = 5,
                                            name_group = Session["group_product"].ToString(),
                                            name_category = Session["category_product"].ToString(),
                                            unit = unit_product,
                                            purchase_price = purchase_price_product,
                                            sell_price = sell_price_product,
                                            quantity = quantity_product,
                                        });
                                    }
                                }
                                else
                                {
                                    missData++;
                                    Product_list.Add(new product
                                    {
                                        id = row_excel,
                                        name = name_product,
                                        status = 6
                                    }) ;
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
                    Session["Import_exist"] = "true";

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
        public ActionResult ProductList_import()
        {
            return PartialView(Product_list);

        }
        public void ExportExcel_Inventory(DateTime? date_start, DateTime? date_end)
        {
            string nameFile = "Kho_Hang_từ_" + String.Format("{0:yyyy-MM-dd}", date_start) + "_đến_" + String.Format("{0:yyyy-MM-dd}", date_end) + ".xlsx";
            var import_inventory = db.import_inventory.Include(i => i.user).Include(i => i.product).Where(s => s.created_at >= date_start && s.created_at <= date_end
                                                    || s.created_at.Value.Day == date_start.Value.Day
                                                    && s.created_at.Value.Month == date_start.Value.Month
                                                    && s.created_at.Value.Year == date_start.Value.Year
                                                    || s.created_at.Value.Day == date_end.Value.Day
                                                    && s.created_at.Value.Month == date_end.Value.Month
                                                    && s.created_at.Value.Year == date_end.Value.Year).ToList();
            var product = db.products.Where(c => c.status != 3).OrderByDescending(c => c.id).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("TonKho");
            FormatExcel(Sheet, 1);
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Cells["A1"].Value = "Mã sản phẩm";
            Sheet.Cells["B1"].Value = "Tên sản phẩm";
            Sheet.Cells["C1"].Value = "Đơn vị";
            Sheet.Cells["D1"].Value = "Đơn giá nhập";
            Sheet.Cells["E1"].Value = "Số lượng nhập";
            Sheet.Cells["F1"].Value = "Đơn giá bán";
            Sheet.Cells["G1"].Value = "Số lượng bán";
            Sheet.Cells["H1"].Value = "Tồn tổng";
            Sheet.Cells["I1"].Value = "Thành tiền";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in product)
            {
                var inventory = item.import_inventory.ToList();
                var invento_groupby = inventory.GroupBy(s => s.price_import).ToList();

                foreach (var item1 in invento_groupby)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.name;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.unit;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item1.Key;
                    Sheet.Cells[string.Format("E{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity);
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.sell_price;
                    Sheet.Cells[string.Format("G{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.sold);
                    Sheet.Cells[string.Format("H{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity - i.sold);
                    Sheet.Cells[string.Format("I{0}", row)].Value = item1.Key * (inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity - i.sold));
                    row++;
                }
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            ExcelWorksheet Sheet_sell = ep.Workbook.Worksheets.Add("ThongKeBan");
            FormatExcel(Sheet_sell, 4);
            Sheet_sell.DefaultColWidth = 20;
            Sheet_sell.Cells.Style.WrapText = true;
            Sheet_sell.Cells["A1"].Value = "Mã sản phẩm";
            Sheet_sell.Cells["B1"].Value = "Tên sản phẩm";
            Sheet_sell.Cells["C1"].Value = "Đơn vị";
            Sheet_sell.Cells["D1"].Value = "Đơn giá bán";
            Sheet_sell.Cells["E1"].Value = "Số lượng bán";
            Sheet_sell.Cells["F1"].Value = "Thành tiền";
          

            int row_sell = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in import_inventory.Where(s => s.sold > 0))
            {
                Sheet_sell.Cells[string.Format("A{0}", row_sell)].Value = item.product.code;
                Sheet_sell.Cells[string.Format("B{0}", row_sell)].Value = item.product.name;
                Sheet_sell.Cells[string.Format("C{0}", row_sell)].Value = item.product.unit;
                Sheet_sell.Cells[string.Format("D{0}", row_sell)].Value = item.product.sell_price;
                Sheet_sell.Cells[string.Format("E{0}", row_sell)].Value = item.sold;
                Sheet_sell.Cells[string.Format("F{0}", row_sell)].Value = item.product.sell_price * (item.sold);

                row_sell++;
            }
            Sheet_sell.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();

            ExcelWorksheet Sheet_import = ep.Workbook.Worksheets.Add("ThongKeNhap");
            FormatExcel(Sheet_import, 4);
            Sheet_import.DefaultColWidth = 20;
            Sheet_import.Cells.Style.WrapText = true;
            Sheet_import.Cells["A1"].Value = "Mã sản phẩm";
            Sheet_import.Cells["B1"].Value = "Tên sản phẩm";
            Sheet_import.Cells["C1"].Value = "Đơn vị";
            Sheet_import.Cells["D1"].Value = "Đơn giá nhập";
            Sheet_import.Cells["E1"].Value = "Số lượng nhập";
            Sheet_import.Cells["F1"].Value = "Thành tiền";


            int row_import = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in import_inventory)
            {
                Sheet_import.Cells[string.Format("A{0}", row_import)].Value = item.product.code;
                Sheet_import.Cells[string.Format("B{0}", row_import)].Value = item.product.name;
                Sheet_import.Cells[string.Format("C{0}", row_import)].Value = item.product.unit;
                Sheet_import.Cells[string.Format("D{0}", row_import)].Value = item.price_import;
                Sheet_import.Cells[string.Format("E{0}", row_import)].Value = item.quantity;
                Sheet_import.Cells[string.Format("F{0}", row_import)].Value = item.price_import * (item.quantity);

                row_import++;
            }
            Sheet_import.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        public ActionResult ImportFail_continues(int id)
        {
            var import = Product_list.Where(x => x.id == id).FirstOrDefault();
           
            var check_group = db.groups.Where(g => g.name == import.name_group && g.status != 3).FirstOrDefault();
            group GroupProduct = new group();
            if (check_group == null)
            {
                
                GroupProduct.name = import.name_group;
                GroupProduct.created_by = User.Identity.GetUserId();
                GroupProduct.status = 1;
                GroupProduct.created_at = DateTime.Now;
                GroupProduct.slug = import.name_group;
                GroupProduct.code = "NH" + CodeRandom.RandomCode();
                db.groups.Add(GroupProduct);
                db.SaveChanges();
            }
            var check_category = db.categories.Where(g => g.name == import.name_category && g.status != 3).FirstOrDefault();
            category category = new category();
            if (check_category == null)
            {
                
                category.name = import.name_category;
                category.status = 1;
                category.created_by = User.Identity.GetUserId();
                category.created_at = DateTime.Now;
                category.slug = import.name_category;
                category.code = "DM" + CodeRandom.RandomCode();
                db.categories.Add(category);
                db.SaveChanges();
            }

            product product = new product();
            product.name = import.name;
            product.status = 1;
            product.unit = import.unit;
            if (check_group == null)
            {
                product.group_id = GroupProduct.id;
            }
            else
            {
                product.group_id = check_group.id;
            }
            if (check_category == null)
            {
                product.category_id = category.id;
            }
            else
            {
                product.category_id = check_category.id;
            }
           
            product.created_by = User.Identity.GetUserId();
            product.sell_price = import.sell_price;
            product.purchase_price = import.purchase_price;
            product.quantity = import.quantity;
            product.created_at = DateTime.Now;
            product.code = "SP" + CodeRandom.RandomCode();
            db.products.Add(product);
            import_inventory inventory = new import_inventory();
            inventory.product_id = product.id;
            inventory.quantity = import.quantity;
            inventory.price_import = import.purchase_price;
            inventory.sold = 0;
            inventory.created_by = User.Identity.GetUserId();
            inventory.created_at = DateTime.Now;
            db.import_inventory.Add(inventory);
            db.SaveChanges();
            Product_list.RemoveAll(p => p.id == id);
            return Json("ImportFail_continues", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImportFail_continues_ALL()
        {
            var product_list = Product_list.Where(p => p.status == 5).ToList();
            foreach (var item in product_list)
            {
                var import = Product_list.Where(x => x.id == item.id).FirstOrDefault();
               
                var check_group = db.groups.Where(g => g.name == import.name_group && g.status != 3).FirstOrDefault();
                group GroupProduct = new group();
                if (check_group == null)
                {

                    GroupProduct.name = import.name_group;
                    GroupProduct.created_by = User.Identity.GetUserId();
                    GroupProduct.status = 1;
                    GroupProduct.created_at = DateTime.Now;
                    GroupProduct.slug = import.name_group;
                    GroupProduct.code = "NH" + CodeRandom.RandomCode();
                    db.groups.Add(GroupProduct);
                    db.SaveChanges();
                }
                var check_category = db.categories.Where(g => g.name == import.name_category && g.status != 3).FirstOrDefault();
                category category = new category();
                if (check_category == null)
                {

                    category.name = import.name_category;
                    category.status = 1;
                    category.created_by = User.Identity.GetUserId();
                    category.created_at = DateTime.Now;
                    category.slug = import.name_category;
                    category.code = "DM" + CodeRandom.RandomCode();
                    db.categories.Add(category);
                    db.SaveChanges();
                }

                product product = new product();
                product.name = import.name;
                product.status = 1;
                product.unit = import.unit;
                if (check_group == null)
                {
                    product.group_id = GroupProduct.id;
                }
                else
                {
                    product.group_id = check_group.id;
                }
                if (check_category == null)
                {
                    product.category_id = category.id;
                }
                else
                {
                    product.category_id = check_category.id;
                }

                product.created_by = User.Identity.GetUserId();
                product.sell_price = import.sell_price;
                product.purchase_price = import.purchase_price;
                product.quantity = import.quantity;
                product.created_at = DateTime.Now;
                product.code = "SP" + CodeRandom.RandomCode();
                db.products.Add(product);
                import_inventory inventory = new import_inventory();
                inventory.product_id = product.id;
                inventory.quantity = import.quantity;
                inventory.price_import = import.purchase_price;
                inventory.sold = 0;
                inventory.created_by = User.Identity.GetUserId();
                inventory.created_at = DateTime.Now;
                db.import_inventory.Add(inventory);
                db.SaveChanges();
               
            }
            foreach (var item in product_list)
            {
                Product_list.RemoveAll(p => p.id == item.id);
            }

            return Json("ImportFail_continues_ALL", JsonRequestBehavior.AllowGet);
        }

        public void ExportExcel_RevenueDate(DateTime? date_Start, DateTime? date_End)
        {

            string nameFile = "Doanh thu" + String.Format("{0:dd-MM-yyyy}", date_Start) + " đến " + String.Format("{0:dd-MM-yyyy}", date_End) + ".xlsx";
           
            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Day == date_Start.Value.Day
                                                    && s.created_at.Value.Month == date_Start.Value.Month
                                                    && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Day == date_End.Value.Day
                                                    && s.created_at.Value.Month == date_End.Value.Month
                                                    && s.created_at.Value.Year == date_End.Value.Year)
                .OrderByDescending(o => o.id);
          
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("TonKho");
            FormatExcel(Sheet, 1);
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Cells["A1"].Value = "Mã hóa đơn";
            Sheet.Cells["B1"].Value = "Tên Tổng hóa đơn";
            Sheet.Cells["C1"].Value = "VAT(%)";
            Sheet.Cells["D1"].Value = "Chiết khấu(%)";
            Sheet.Cells["E1"].Value = "Thành tiền";
            Sheet.Cells["F1"].Value = "Trả trước(nếu có)";
            Sheet.Cells["G1"].Value = "Còn nợ";
            Sheet.Cells["H1"].Value = "Trạng thái";
            Sheet.Cells["I1"].Value = "Ngày giao dịch";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in sales)
            {
               
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.total / (1 + double.Parse(item.vat.ToString()) / 100 - double.Parse(item.discount.ToString()) / 100);
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.vat;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.discount;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.total;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.prepayment;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.total - item.prepayment;
                if (item.method == 1)
                {
                    Sheet.Cells[string.Format("H{0}", row)].Value = "Đã thanh toán";
                }
                else if (item.method == 2)
                {
                    Sheet.Cells[string.Format("H{0}", row)].Value = "Ghi nợ";
                }
                   
                    Sheet.Cells[string.Format("I{0}", row)].Value = String.Format("{0:dd-MM-yyyy HH:mm:yy}", item.created_at); ;
                    row++;
                
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();          
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
        public void ExportExcel_RevenueMonth(DateTime? date_Start, DateTime? date_End)
        {
            string nameFile = "";
            if (date_Start == date_End)
            {
                nameFile = "Doanh thu tháng " + String.Format("{0:MM-yyyy}", date_End) + ".xlsx";
            }
            else
            {
                nameFile = "Doanh thu từ tháng" + String.Format("{0:MM-yyyy}", date_Start) + " đến tháng " + String.Format("{0:MM-yyyy}", date_End) + ".xlsx";
            }

            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Month == date_Start.Value.Month && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Month == date_End.Value.Month && s.created_at.Value.Year == date_End.Value.Year)
                .OrderByDescending(o => o.id);    
          
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("TonKho");
            FormatExcel(Sheet, 1);
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Cells["A1"].Value = "Mã hóa đơn";
            Sheet.Cells["B1"].Value = "Tên Tổng hóa đơn";
            Sheet.Cells["C1"].Value = "VAT(%)";
            Sheet.Cells["D1"].Value = "Chiết khấu(%)";
            Sheet.Cells["E1"].Value = "Thành tiền";
            Sheet.Cells["F1"].Value = "Trả trước(nếu có)";
            Sheet.Cells["G1"].Value = "Còn nợ";
            Sheet.Cells["H1"].Value = "Trạng thái";
            Sheet.Cells["I1"].Value = "Ngày giao dịch";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in sales)
            {
               
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.total / (1 + double.Parse(item.vat.ToString()) / 100 - double.Parse(item.discount.ToString()) / 100);
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.vat;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.discount;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.total;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.prepayment;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.total - item.prepayment;
                if (item.method == 1)
                {
                    Sheet.Cells[string.Format("H{0}", row)].Value = "Đã thanh toán";
                }
                else if (item.method == 2)
                {
                    Sheet.Cells[string.Format("H{0}", row)].Value = "Ghi nợ";
                }
                   
                    Sheet.Cells[string.Format("I{0}", row)].Value = String.Format("{0:dd-MM-yyyy HH:mm:yy}",item.created_at);
                    row++;
                
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();          
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
        public void ExportExcel_profit()
        {
            string nameFile = "Lợi nhuận xuất  " + String.Format("{0:dd-MM-yyyy}", DateTime.Now) + ".xlsx";
           

            var revenues = db.revenues.OrderByDescending(c => c.id).ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("TonKho");
            FormatExcel(Sheet, 1);
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Cells["A1"].Value = "Mã sản phẩm";
            Sheet.Cells["B1"].Value = "Tên sản phẩm";
            Sheet.Cells["C1"].Value = "Đơn vị";
            Sheet.Cells["D1"].Value = "Số lượng gốc";
            Sheet.Cells["E1"].Value = "Đơn giá gốc";
            Sheet.Cells["F1"].Value = "Số lượng bán";
            Sheet.Cells["G1"].Value = "Đơn giá bán";
            Sheet.Cells["H1"].Value = "Thành tiền";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in revenues)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.sale_details.product.code;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.sale_details.product.name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.sale_details.product.unit;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.sale_details.product.quantity;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.sale_details.product.purchase_price;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.quantity;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Price;
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Price - item.sale_details.product.purchase_price) *item.quantity;

                row++;

            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
    }
}

