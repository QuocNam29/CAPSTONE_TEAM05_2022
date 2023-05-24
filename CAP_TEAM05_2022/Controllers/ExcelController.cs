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
using System.Data.Entity.Core.Objects;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Constants = CAP_TEAM05_2022.Helper.Constants;


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

        public void HeaderExcel(ExcelWorksheet Sheet)
        {
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Cells["A1:H1"].Merge = true;
            Sheet.Cells["A1:H1"].Style.Font.Bold = true;
            Sheet.Cells["A1:H1"].Style.Font.Size = 16;
            Sheet.Cells["A1"].Value = "DOANH NGHIỆP BUÔN BÁN PHÂN BÓN THUỐC TRỪ SÂU TẤN THÀNH";

            Sheet.Cells["A2:H2"].Merge = true;
            Sheet.Cells["A2"].Value = "Địa chỉ: Tổ 20, ấp Tân Trung B, xã Tân Hưng, huyện Tân Châu, tỉnh Tây Ninh";

            Sheet.Cells["A3:H3"].Merge = true;
            Sheet.Cells["A3"].Value = "Ngân Hàng: BIDV - 75210000089921 - TRAN QUOC NAM";

            Sheet.Cells["A4:H4"].Merge = true;
            Sheet.Cells["A4"].Value = "Điện thoại: 0854858818";
            Sheet.Cells["A1:H4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
        public void ExportExcel(int? supplier_id, int? category_id)
        {
            string nameFile = "SP_" + DateTime.Now + ".xlsx";
            var list = from l in db.products
                       where /*l.@group.status == Constants.SHOW_STATUS &&*/ l.category.status == Constants.SHOW_STATUS
                       select l;
            if (category_id != null)
            {
                list = list.Where(p => p.category_id == category_id);
            }
            if (supplier_id != null)
            {
                list = list.Where(p => p.supplier_id == supplier_id);
            }
            list = list.OrderByDescending(c => c.id);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("SanPham");
            FormatExcel(Sheet, 1);
            Sheet.Cells["A1"].Value = "Mã sản phẩm";
            Sheet.Cells["B1"].Value = "Tên sản phẩm";
            Sheet.Cells["C1"].Value = "Công ty";
            Sheet.Cells["D1"].Value = "Danh mục";
            Sheet.Cells["E1"].Value = "Số lượng tồn";
            Sheet.Cells["F1"].Value = "Số lượng tồn bán lẻ";
            Sheet.Cells["G1"].Value = "Đơn vị quy đổi (nếu có)";
            Sheet.Cells["H1"].Value = "Giá bán";
            Sheet.Cells["I1"].Value = "Giá bán ghi nợ";
            Sheet.Cells["J1"].Value = "Đã bán";
            Sheet.Cells["K1"].Value = "Đã bán lẻ";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list)
            {
                string sold = "";
                string sold_swap = "";
                string quantity = item.quantity + " " + item.unit;
                string quantity_swap = "";
                if (item.unit_swap != null)
                {
                    quantity_swap = item.quantity_remaning + " " + item.unit_swap;
                    int temp1 = (item.import_inventory.Sum(i => i.sold_swap) / item.quantity_swap);
                    int temp2 = (item.import_inventory.Sum(i => i.sold_swap) % item.quantity_swap);
                    if (temp2 > 0)
                    {
                        double temp3 = (double)(item.import_inventory.Sum(i => i.sold_swap) * 1.000000001 / item.quantity_swap) - temp1;
                        sold += (item.import_inventory.Sum(i => i.sold) - 1) + " " + item.unit;
                        sold_swap += (int)(temp3 * item.quantity_swap) + " " + item.unit_swap;

                    }
                    else
                    {
                        sold += item.import_inventory.Sum(i => i.sold) + " " + item.unit;
                    }
                }
                Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.name;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.customer.name;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.category.name;
                Sheet.Cells[string.Format("E{0}", row)].Value = quantity;
                Sheet.Cells[string.Format("F{0}", row)].Value = quantity_swap;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.unit_swap != null ? "1 " + item.unit + " = " + item.quantity_swap + " " + item.unit_swap : "";
                Sheet.Cells[string.Format("H{0}", row)].Value = item.sell_price;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.sell_price_debt;
                Sheet.Cells[string.Format("J{0}", row)].Value = sold;
                Sheet.Cells[string.Format("K{0}", row)].Value = sold_swap;
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
            var list_group = db.groups.ToList();
            var list_category = db.categories.Where(x=> x.status == Constants.SHOW_STATUS).ToList();
            var list_supplier = db.customers.Where(g => g.type == Constants.SUPPLIER && g.status == Constants.SHOW_STATUS).ToList();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("NhapSanPham");
            FormatExcel(Sheet, 3);
            Sheet.Cells["A1"].Value = "Tên sản phẩm";
            Sheet.Cells["B1"].Value = "Nhà cung cấp";
            Sheet.Cells["C1"].Value = "Danh mục";
            Sheet.Cells["D1"].Value = "Số lượng tồn";
            Sheet.Cells["E1"].Value = "Đơn vị";
            Sheet.Cells["F1"].Value = "Số lượng quy đổi";
            Sheet.Cells["G1"].Value = "Đơn vị quy đổi";
            Sheet.Cells["H1"].Value = "Đơn giá nhập";
            Sheet.Cells["I1"].Value = "Đơn giá bán";
            Sheet.Cells["J1"].Value = "Đơn giá bán nợ";

            ExcelWorksheet SheetCongTy = ep.Workbook.Worksheets.Add("CongTyCungCap");
            int rowCongTy = 2;
            SheetCongTy.Cells["A1"].Value = "Tên công ty cung cấp";
            foreach (var item in list_supplier)
            {
                SheetCongTy.Cells[string.Format("A{0}", rowCongTy)].Value = item.name;
                rowCongTy++;
            }
            for (int i = 2; i < 500; i++)
            {
                var validation_supplier = Sheet.Cells[string.Format("B{0}", i)].DataValidation.AddListDataValidation();
                validation_supplier.ShowErrorMessage = true;
                validation_supplier.ErrorStyle = ExcelDataValidationWarningStyle.information;
                validation_supplier.ErrorTitle = "Lỗi nhập nhà cung cấp";
                validation_supplier.Error = "Nhà cung cấp này không có trong hệ thống, vui lòng chọn lại !";
                validation_supplier.Formula.ExcelFormula = string.Format("CongTyCungCap!A2:A{0}", rowCongTy - 1);
            }
            ExcelWorksheet SheetDanhMuc = ep.Workbook.Worksheets.Add("DanhMuc");
            int rowDanhmuc = 2;
            SheetDanhMuc.Cells["A1"].Value = "Tên danh mục";
            foreach (var item in list_category)
            {
                SheetDanhMuc.Cells[string.Format("A{0}", rowDanhmuc)].Value = item.name;
                rowDanhmuc++;
            }
            for (int i = 2; i < 500; i++)
            {
                var validation_category = Sheet.Cells[string.Format("C{0}", i)].DataValidation.AddListDataValidation();
                validation_category.ShowErrorMessage = true;
                validation_category.ErrorStyle = ExcelDataValidationWarningStyle.information;
                validation_category.ErrorTitle = "Lỗi nhập danh mục";
                validation_category.Error = "Danh mục này không có trong hệ thống, vui lòng chọn lại !";
                validation_category.Formula.ExcelFormula = string.Format("DanhMuc!A2:A{0}", rowDanhmuc - 1);
            }
            
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
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
                using (var range = Sheet.Cells["A1:K1"])
                {
                    Sheet.Cells["A1:K1"].Style.Font.Size = 13;

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
                    Sheet.Cells["A1:B1"].Style.Font.Size = 13;

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
                // Lấy range vào tạo format cho range đó ở đây là từ A1 tới K1
                using (var range = Sheet.Cells["A1:K1"])
                {
                    Sheet.Cells["A1:K1"].Style.Font.Size = 13;

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
                    Sheet.Cells["A1:F1"].Style.Font.Size = 13;

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
            else if (i == 5)
            {
                // Lấy range vào tạo format cho range đó ở đây là từ A1 tới I1
                using (var range = Sheet.Cells["A1:H1"])
                {
                    Sheet.Cells["A1:H1"].Style.Font.Size = 13;

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
                    DateTime curentDate = DateTime.Now;
                    Product_list.Clear();
                    try
                    {
                        //Insert records to database table.
                        foreach (DataRow row in dt.Rows)
                        {
                            row_excel++;
                            if (!String.IsNullOrEmpty(row["Tên sản phẩm"].ToString().Trim())
                               && !String.IsNullOrEmpty(row["Nhà cung cấp"].ToString().Trim())
                           && !String.IsNullOrEmpty(row["Danh mục"].ToString().Trim())
                           && !String.IsNullOrEmpty(row["Số lượng tồn"].ToString().Trim())
                            && !String.IsNullOrEmpty(row["Đơn vị"].ToString().Trim())
                            && !String.IsNullOrEmpty(row["Số lượng quy đổi"].ToString().Trim())
                            && !String.IsNullOrEmpty(row["Đơn vị quy đổi"].ToString().Trim())
                             && !String.IsNullOrEmpty(row["Đơn giá nhập"].ToString().Trim())
                              && !String.IsNullOrEmpty(row["Đơn giá bán"].ToString().Trim())
                              && !String.IsNullOrEmpty(row["Đơn giá bán nợ"].ToString().Trim()))
                            {
                                Session["name_product"] = row["Tên sản phẩm"].ToString().Trim();
                                Session["supplier_product"] = row["Nhà cung cấp"].ToString().Trim();
                                Session["category_product"] = row["Danh mục"].ToString().Trim();
                                Session["quantity_product"] = row["Số lượng tồn"].ToString().Trim();
                                Session["unit_product"] = row["Đơn vị"].ToString().Trim();
                                Session["quantity_swap_product"] = row["Số lượng quy đổi"].ToString().Trim();
                                Session["unit_swap_product"] = row["Đơn vị quy đổi"].ToString().Trim();
                                Session["purchase_price_product"] = row["Đơn giá nhập"].ToString().Trim();
                                Session["sell_price_product"] = row["Đơn giá bán"].ToString().Trim();
                                Session["sell_debt_product"] = row["Đơn giá bán nợ"].ToString().Trim();

                            }
                            else if (
                                  String.IsNullOrEmpty(row["Số lượng tồn"].ToString())
                               || String.IsNullOrEmpty(row["Đơn vị"].ToString())
                               || String.IsNullOrEmpty(row["Số lượng quy đổi"].ToString())
                               || String.IsNullOrEmpty(row["Đơn vị quy đổi"].ToString())
                               || String.IsNullOrEmpty(row["Đơn giá nhập"].ToString())
                               || String.IsNullOrEmpty(row["Đơn giá bán"].ToString())
                               || String.IsNullOrEmpty(row["Đơn giá bán nợ"].ToString()))

                            {
                                Session["name_product"] = row["Tên sản phẩm"].ToString().Trim();
                                Session["unit_product"] = null;
                            }
                            else
                            {
                                Session["name_product"] = null;
                            }


                            if (Session["name_product"] != null)
                            {
                                string name_product = Session["name_product"].ToString();
                                // thực hiện nhập dữ liệu nếu không có trường dữ liệu nào trống
                                if (Session["unit_product"] != null)
                                {
                                    string name_supplier = Session["supplier_product"].ToString();
                                    string name_category = Session["category_product"].ToString();
                                    var check_supplier = db.customers.Where(g => g.name == name_supplier && g.type == Constants.SUPPLIER).FirstOrDefault();
                                    var check_category = db.categories.Where(g => g.name == name_category).FirstOrDefault();

                                    string unit_product = Session["unit_product"].ToString().Trim().ToLower();
                                    decimal sell_price_product = decimal.Parse(Session["sell_price_product"].ToString().Trim());
                                    decimal purchase_price_product = decimal.Parse(Session["purchase_price_product"].ToString().Trim());
                                    int quantity_product = int.Parse(Session["quantity_product"].ToString().Trim());
                                    int quantity_swap_product = int.Parse(Session["quantity_swap_product"].ToString().Trim());
                                    string unit_swap_product = Session["unit_swap_product"].ToString().Trim().ToLower();
                                    decimal sell_debt_product = decimal.Parse(Session["sell_debt_product"].ToString().Trim());

                                    // kiểm tra nhóm hàng, danh mục, nhà cung cấp đã tồn tại chưa
                                    if (check_category != null && check_supplier != null)
                                    {
                                        //thực hiện nhập sản phẩm
                                        int category_id = check_category.id;
                                        int supplier_id = check_supplier.id;

                                        var check_product = db.products.FirstOrDefault(c => c.name == name_product && c.category_id == category_id 
                                                                                && c.supplier_id == supplier_id && c.unit == unit_product
                                                                                && c.unit_swap == unit_swap_product && c.quantity_swap == quantity_swap_product);
                                        //Nếu không có sản phẩm nào tồn tại thì nhập sản phẩm
                                        if (check_product == null)
                                        {
                                            product product = new product();
                                            product.name = name_product;
                                            product.status = Constants.SHOW_STATUS;
                                            product.unit = unit_product;
                                            product.category_id = category_id;
                                            product.supplier_id = supplier_id;
                                            product.created_by = User.Identity.GetUserId();
                                            product.sell_price = sell_price_product;
                                            product.sell_price_debt = sell_debt_product;
                                            product.purchase_price = purchase_price_product;
                                            product.quantity = quantity_product;
                                            product.quantity_swap = quantity_swap_product;
                                            product.unit_swap = unit_swap_product;
                                            product.sell_price_swap = (decimal)(sell_price_product / quantity_product);
                                            product.sell_price_debt_swap = (decimal)(sell_debt_product / quantity_product);
                                            product.created_at = curentDate;
                                            product.code = "SP" + CodeRandom.RandomCode();
                                            product.name_category = name_category;
                                            product.name_group = name_supplier;
                                            db.products.Add(product);

                                            price_product price_Product = new price_product();
                                            price_Product.product_id = product.id;
                                            price_Product.price = product.sell_price;
                                            price_Product.price_debt = product.sell_price_debt;
                                            price_Product.updated_at = curentDate;
                                            price_Product.unit = product.unit;
                                            db.price_product.Add(price_Product);

                                            price_product price_Product_swap = new price_product();
                                            price_Product_swap.product_id = product.id;
                                            price_Product_swap.price = product.sell_price_swap;
                                            price_Product_swap.price_debt = product.sell_price_debt_swap;
                                            price_Product_swap.updated_at = curentDate;
                                            price_Product_swap.unit = product.unit_swap;
                                            db.price_product.Add(price_Product_swap);

                                            inventory_order inventory_Order = new inventory_order();
                                            inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                                            inventory_Order.create_at = curentDate;
                                            inventory_Order.update_at = curentDate;
                                            inventory_Order.create_by = User.Identity.GetUserId();
                                            inventory_Order.Total = product.purchase_price * product.quantity;
                                            inventory_Order.state = Constants.PAYED_ORDER;
                                            inventory_Order.supplier_id = supplier_id;
                                            db.inventory_order.Add(inventory_Order);

                                            import_inventory inventory = new import_inventory();
                                            inventory.inventory_id = inventory_Order.id;
                                            inventory.product_id = product.id;
                                            inventory.quantity = product.quantity;
                                            inventory.price_import = product.purchase_price;
                                            inventory.sold = 0;
                                            inventory.sold_swap = 0;
                                            inventory.return_quantity = 0;
                                            inventory.quantity_remaining = 0;
                                            inventory.created_by = User.Identity.GetUserId();
                                            inventory.created_at = curentDate;
                                            inventory.supplier_id = supplier_id;
                                            db.import_inventory.Add(inventory);
                                            db.SaveChanges();

                                            addRow++;
                                            product.status = 4;
                                            Product_list.Add(product);
                                        }
                                        //nếu sản phẩm đã tồn tại rồi thì lưu vào session
                                        else
                                        {
                                            int quantity_current = check_product.quantity;
                                            rowExist++;
                                            Product_list.Add(new product
                                            {
                                                id = row_excel,
                                                name = name_product,
                                                status = 7, // đã tồn tại
                                                name_category = name_category,
                                                name_group = name_supplier,
                                                unit = unit_product,
                                                unit_swap = unit_swap_product,
                                                purchase_price = purchase_price_product,
                                                sell_price = sell_price_product,
                                                sell_price_debt = sell_debt_product,
                                                quantity = quantity_product,
                                                quantity_swap = quantity_swap_product
                                            });
                                        }
                                    }
                                    else
                                    {
                                        rowFailFormat++;
                                        Product_list.Add(new product
                                        {
                                            id = row_excel,
                                            name = name_product,
                                            status = 5, // sai định dạng
                                            name_category = name_category,
                                            name_group = name_supplier,
                                            unit = unit_product,
                                            unit_swap = unit_swap_product,
                                            purchase_price = purchase_price_product,
                                            sell_price = sell_price_product,
                                            sell_price_debt = sell_debt_product,
                                            quantity = quantity_product,
                                            quantity_swap = quantity_swap_product

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
                                        status = 6, // miss dữ liệu
                                    });
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
            string nameFile = "Kho_Hàng_từ_" + String.Format("{0:yyyy-MM-dd}", date_start) + "_đến_" + String.Format("{0:yyyy-MM-dd}", date_end) + ".xlsx";
            var import_inventory = db.import_inventory.Include(i => i.user).Include(i => i.product).Where(s => s.created_at >= date_start && s.created_at <= date_end
                                                    || s.created_at.Value.Day == date_start.Value.Day
                                                    && s.created_at.Value.Month == date_start.Value.Month
                                                    && s.created_at.Value.Year == date_start.Value.Year
                                                    || s.created_at.Value.Day == date_end.Value.Day
                                                    && s.created_at.Value.Month == date_end.Value.Month
                                                    && s.created_at.Value.Year == date_end.Value.Year).OrderByDescending(i => i.id).ToList();
            var product = db.products.Where(c => c.status != 3).OrderByDescending(c => c.id).ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("TonKho");
            FormatExcel(Sheet, 1);
            Sheet.DefaultColWidth = 20;
            Sheet.Cells.Style.WrapText = true;
            Sheet.Cells["A1"].Value = "Mã sản phẩm";
            Sheet.Cells["B1"].Value = "Tên sản phẩm";
            Sheet.Cells["C1"].Value = "Số lượng nhập";
            Sheet.Cells["D1"].Value = "Đơn giá nhập";
            Sheet.Cells["E1"].Value = "Đơn giá bán";
            Sheet.Cells["F1"].Value = "Đơn giá bán lẻ";
            Sheet.Cells["G1"].Value = "Số lượng bán";
            Sheet.Cells["G1"].Value = "Số lượng đổi trả";
            Sheet.Cells["H1"].Value = "Tồn tổng";
            Sheet.Cells["I1"].Value = "Tồn lẻ (nếu có)";
            Sheet.Cells["J1"].Value = "Thành tiền";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in product)
            {
                var inventory = item.import_inventory.ToList();
                var invento_groupby = inventory.GroupBy(s => s.price_import).ToList();
                decimal total_temp1 = 0;
                decimal total_temp2 = 0;

                foreach (var item1 in invento_groupby)
                {
                    string quantityReturn = "";
                    if (inventory.Where(i => i.price_import == item1.Key).Sum(i => i.return_quantity) > 0)
                    {
                        quantityReturn += inventory.Where(i => i.price_import == item1.Key).Sum(i => i.return_quantity) + " " + item.unit;
                    }
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.code;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.name;
                    Sheet.Cells[string.Format("C{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity) + " " + item.unit + "(" + item.quantity_swap + item.unit_swap + ")";
                    Sheet.Cells[string.Format("D{0}", row)].Value = item1.Key.ToString("N0") + "/" + item.unit;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.sell_price.ToString("N0") + "/" + item.unit;
                    if (item.unit_swap == null)
                    {
                        Sheet.Cells[string.Format("F{0}", row)].Value = 0;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("F{0}", row)].Value = ((decimal)item.sell_price_swap).ToString("N0") + "/" + item.unit_swap;
                    }
                    Sheet.Cells[string.Format("G{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.sold) + " " + item.unit;
                    Sheet.Cells[string.Format("G{0}", row)].Value = quantityReturn;
                    Sheet.Cells[string.Format("H{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity - i.sold - i.return_quantity) + " " + item.unit;
                    if (item.unit_swap == null)
                    {
                        Sheet.Cells[string.Format("I{0}", row)].Value = 0;
                    }
                    else
                    {
                        Sheet.Cells[string.Format("I{0}", row)].Value = inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity_remaining) + " " + item.unit_swap;
                    }
                    total_temp1 = (item1.Key * (inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity - i.sold - i.return_quantity)));
                    if (item.unit_swap != null)
                    {
                        total_temp2 = (decimal)((item1.Key / (item.quantity_swap)) * (inventory.Where(i => i.price_import == item1.Key).Sum(i => i.quantity_remaining)));
                    }
                    Sheet.Cells[string.Format("J{0}", row)].Value = total_temp1 + total_temp2;
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
            Sheet_sell.Cells["C1"].Value = "Số lượng bán";
            Sheet_sell.Cells["D1"].Value = "Đơn giá bán";
            Sheet_sell.Cells["E1"].Value = "Thành tiền";
            Sheet_sell.Cells["F1"].Value = "Ngày giao dịch";


            int row_sell = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in import_inventory.Where(s => s.sold > 0))
            {
                var revenues = item.revenues.OrderByDescending(x => x.sale_details.created_at).ToList();
                foreach (var item1 in revenues)
                {
                    Sheet_sell.Cells[string.Format("A{0}", row_sell)].Value = item1.sale_details.product.code;
                    Sheet_sell.Cells[string.Format("B{0}", row_sell)].Value = item1.sale_details.product.name;
                    Sheet_sell.Cells[string.Format("C{0}", row_sell)].Value = item1.quantity + " " + item1.unit;
                    Sheet_sell.Cells[string.Format("D{0}", row_sell)].Value = item1.Price.ToString("N0") + "/" + item1.unit;
                    Sheet_sell.Cells[string.Format("E{0}", row_sell)].Value = (item1.Price * item1.quantity);
                    Sheet_sell.Cells[string.Format("F{0}", row_sell)].Value = String.Format("{0:HH:mm - dd/MM/yyy}", item1.sale_details.created_at);

                    row_sell++;
                }
            }
            Sheet_sell.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();

            ExcelWorksheet Sheet_import = ep.Workbook.Worksheets.Add("ThongKeNhap");
            FormatExcel(Sheet_import, 5);
            Sheet_import.DefaultColWidth = 20;
            Sheet_import.Cells.Style.WrapText = true;
            Sheet_import.Cells["A1"].Value = "Mã sản phẩm";
            Sheet_import.Cells["B1"].Value = "Tên sản phẩm";
            Sheet_import.Cells["C1"].Value = "Số lượng nhập";
            Sheet_import.Cells["D1"].Value = "Số lương trả";
            Sheet_import.Cells["E1"].Value = "Đơn giá nhập";
            Sheet_import.Cells["F1"].Value = "Thành tiền";
            Sheet_import.Cells["G1"].Value = "Nhà cung cấp";
            Sheet_import.Cells["H1"].Value = "Ngày nhập";


            int row_import = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in import_inventory)
            {
                Sheet_import.Cells[string.Format("A{0}", row_import)].Value = item.product.code;
                Sheet_import.Cells[string.Format("B{0}", row_import)].Value = item.product.name;
                Sheet_import.Cells[string.Format("C{0}", row_import)].Value = item.quantity + " " + item.product.unit + "(" + item.product.quantity_swap + item.product.unit_swap + ")";
                Sheet_import.Cells[string.Format("D{0}", row_import)].Value = item.return_quantity + " " + item.product.unit;
                Sheet_import.Cells[string.Format("E{0}", row_import)].Value = item.price_import.ToString("N0") + "/" + item.product.unit;
                Sheet_import.Cells[string.Format("F{0}", row_import)].Value = item.price_import * (item.quantity - item.return_quantity);
                Sheet_import.Cells[string.Format("G{0}", row_import)].Value = item.customer?.name;
                Sheet_import.Cells[string.Format("H{0}", row_import)].Value = String.Format("{0:HH:mm - dd/MM/yyy}", item.created_at);

                row_import++;
            }
            Sheet_import.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        [HttpPost]
        public ActionResult ImportFail_continues(int id)
        {
            string message = "";
            bool status = true;
            DateTime currentDate = DateTime.Now;
            try
            {
                var import = Product_list.Where(x => x.id == id).FirstOrDefault();
                var check_supplier = db.customers.Where(s => s.name == import.name_group && s.type == Constants.SUPPLIER).FirstOrDefault();
                var check_category = db.categories.Where(g => g.name == import.name_category && g.status != 3).FirstOrDefault();
                category category = new category();
                if (check_category == null)
                {
                    category.name = import.name_category;
                    category.status = 1;
                    category.created_by = User.Identity.GetUserId();
                    category.created_at = currentDate;
                    category.code = "DM" + CodeRandom.RandomCode();
                    db.categories.Add(category);
                }

                customer supplier = new customer();
                if (check_supplier == null)
                {
                    supplier.type = Constants.SUPPLIER;
                    supplier.name = import.name_group;
                    supplier.phone = "0000000000";
                    supplier.address = "-";
                    supplier.status = Constants.SHOW_STATUS;
                    supplier.created_by = User.Identity.GetUserId();
                    supplier.created_at = currentDate;
                    supplier.code = "MKH" + CodeRandom.RandomCode();
                    supplier.note = "Tạo do nhập file excel";
                    db.customers.Add(supplier);
                }

                product product = new product();
                product.name = import.name;
                product.status = Constants.SHOW_STATUS;
                product.unit = import.unit;
                product.unit_swap = import.unit_swap;
                product.category_id = check_category == null ? category.id : check_category.id;
                product.supplier_id = check_supplier == null ? supplier.id : check_supplier.id;
                product.created_by = User.Identity.GetUserId();
                product.sell_price = import.sell_price;
                product.sell_price_debt = import.sell_price_debt;
                product.purchase_price = import.purchase_price;
                product.quantity = import.quantity;
                product.quantity_swap = import.quantity_swap;
                product.created_at = currentDate;
                product.code = "SP" + CodeRandom.RandomCode();
                product.sell_price_swap = product.sell_price / product.quantity_swap;
                product.sell_price_debt_swap = product.sell_price_debt / product.quantity_swap;
                db.products.Add(product);

                price_product price_Product = new price_product();
                price_Product.product_id = product.id;
                price_Product.price = product.sell_price;
                price_Product.price_debt = product.sell_price_debt;
                price_Product.updated_at = currentDate;
                price_Product.unit = product.unit;
                db.price_product.Add(price_Product);

                price_product price_Product_swap = new price_product();
                price_Product_swap.product_id = product.id;
                price_Product_swap.price = product.sell_price_swap;
                price_Product_swap.price_debt = product.sell_price_debt_swap;
                price_Product_swap.updated_at = currentDate;
                price_Product_swap.unit = product.unit_swap;
                db.price_product.Add(price_Product_swap);

                inventory_order inventory_Order = new inventory_order();
                inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                inventory_Order.create_at = currentDate;
                inventory_Order.update_at = currentDate;
                inventory_Order.create_by = User.Identity.GetUserId();
                inventory_Order.Total = product.purchase_price * product.quantity;
                inventory_Order.state = Constants.PAYED_ORDER;
                inventory_Order.supplier_id = check_supplier == null ? supplier.id : check_supplier.id;
                db.inventory_order.Add(inventory_Order);

                import_inventory inventory = new import_inventory();
                inventory.inventory_id = inventory_Order.id;
                inventory.product_id = product.id;
                inventory.quantity = product.quantity;
                inventory.price_import = product.purchase_price;
                inventory.sold = 0;
                inventory.sold_swap = 0;
                inventory.return_quantity = 0;
                inventory.quantity_remaining = 0;
                inventory.created_by = User.Identity.GetUserId();
                inventory.created_at = currentDate;
                inventory.supplier_id = check_supplier == null ? supplier.id : check_supplier.id;
                db.import_inventory.Add(inventory);
                db.SaveChanges();
                Product_list.RemoveAll(p => p.id == id);
                message = "Đã nhập thành công sản phẩm.";
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExitData_continues(int id)
        {
            string message = "";
            bool status = true;
            DateTime currentDate = DateTime.Now;
            try
            {
                var import = Product_list.Find(x => x.id == id);
                var check_supplier = db.customers.FirstOrDefault(s => s.name == import.name_group && s.type == Constants.SUPPLIER);
                var check_category = db.categories.FirstOrDefault(g => g.name == import.name_category);

                var product = db.products.FirstOrDefault(c => c.name == import.name && c.category_id == check_category.id
                                                              && c.supplier_id == check_supplier.id && c.unit == import.unit
                                                              && c.unit_swap == import.unit_swap && c.quantity_swap == import.quantity_swap);

                if (import.sell_price != product.sell_price || import.sell_price_debt != product.sell_price_debt)
                {
                    price_product price_Product = new price_product();
                    price_Product.product_id = product.id;
                    price_Product.price = import.sell_price;
                    price_Product.price_debt = import.sell_price_debt;
                    price_Product.updated_at = currentDate;
                    price_Product.unit = product.unit;
                    db.price_product.Add(price_Product);
                }
                import.sell_price_swap = import.sell_price / import.quantity_swap;
                import.sell_price_debt_swap = import.sell_price_debt / import.quantity_swap;
                if (import.sell_price_swap != product.sell_price_swap || import.sell_price_debt_swap != product.sell_price_debt_swap)
                {
                    price_product price_Product_swap = new price_product();
                    price_Product_swap.product_id = product.id;
                    price_Product_swap.price = import.sell_price_swap;
                    price_Product_swap.price_debt = import.sell_price_debt_swap;
                    price_Product_swap.updated_at = currentDate;
                    price_Product_swap.unit = product.unit_swap;
                    db.price_product.Add(price_Product_swap);
                }
                product.sell_price = import.sell_price;
                product.sell_price_debt = import.sell_price_debt;
                product.purchase_price = import.purchase_price;
                product.quantity += import.quantity;
                product.updated_at = currentDate;
                product.sell_price_swap = import.sell_price_swap;
                product.sell_price_debt_swap = import.sell_price_debt_swap;
                db.Entry(product).State = EntityState.Modified;

                inventory_order inventory_Order = new inventory_order();
                inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                inventory_Order.create_at = currentDate;
                inventory_Order.update_at = currentDate;
                inventory_Order.create_by = User.Identity.GetUserId();
                inventory_Order.Total = import.purchase_price * import.quantity;
                inventory_Order.state = Constants.PAYED_ORDER;
                inventory_Order.supplier_id = check_supplier.id;
                db.inventory_order.Add(inventory_Order);

                import_inventory inventory = new import_inventory();
                inventory.inventory_id = inventory_Order.id;
                inventory.product_id = product.id;
                inventory.quantity =import.quantity;
                inventory.price_import = import.purchase_price;
                inventory.sold = 0;
                inventory.sold_swap = 0;
                inventory.return_quantity = 0;
                inventory.quantity_remaining = 0;
                inventory.created_by = User.Identity.GetUserId();
                inventory.created_at = currentDate;
                inventory.supplier_id = check_supplier.id;
                db.import_inventory.Add(inventory);
                db.SaveChanges();
                Product_list.RemoveAll(p => p.id == id);
                message = "Đã nhập thành công sản phẩm.";
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ImportFail_continues_ALL()
        {
            string message = "";
            bool status = true;
            DateTime currentDate = DateTime.Now;
            try
            {
                var product_list = Product_list.Where(p => p.status == 5).ToList();
                foreach (var item in product_list)
                {
                    var check_supplier = db.customers.Where(s => s.name == item.name_group && s.type == Constants.SUPPLIER && s.status == Constants.SHOW_STATUS).FirstOrDefault();
                    var check_category = db.categories.Where(g => g.name == item.name_category && g.status != 3).FirstOrDefault();
                    category category = new category();
                    if (check_category == null)
                    {

                        category.name = item.name_category;
                        category.status = 1;
                        category.created_by = User.Identity.GetUserId();
                        category.created_at = currentDate;
                        category.code = "DM" + CodeRandom.RandomCode();
                        db.categories.Add(category);
                    }
                    customer supplier = new customer();
                    if (check_supplier == null)
                    {
                        supplier.type = Constants.SUPPLIER;
                        supplier.name = item.name_group;
                        supplier.phone = "0000000000";
                        supplier.address = "-";
                        supplier.status = Constants.SHOW_STATUS;
                        supplier.created_by = User.Identity.GetUserId();
                        supplier.created_at = currentDate;
                        supplier.code = "MKH" + CodeRandom.RandomCode();
                        supplier.note = "Tạo do nhập file excel";
                        db.customers.Add(supplier);
                    }
                    product product = new product();
                    product.name = item.name;
                    product.status = Constants.SHOW_STATUS;
                    product.unit = item.unit;
                    product.unit_swap = item.unit_swap;
                    product.category_id = check_category == null ? category.id : check_category.id;
                    product.supplier_id = check_supplier == null ? supplier.id : check_supplier.id;
                    product.created_by = User.Identity.GetUserId();
                    product.sell_price = item.sell_price;
                    product.sell_price_debt = item.sell_price_debt;
                    product.purchase_price = item.purchase_price;
                    product.quantity = item.quantity;
                    product.quantity_swap = item.quantity_swap;
                    product.created_at = currentDate;
                    product.code = "SP" + CodeRandom.RandomCode();
                    product.sell_price_swap = product.sell_price / product.quantity_swap;
                    product.sell_price_debt_swap = product.sell_price_debt / product.quantity_swap;
                    db.products.Add(product);

                    price_product price_Product = new price_product();
                    price_Product.product_id = product.id;
                    price_Product.price = product.sell_price;
                    price_Product.price_debt = product.sell_price_debt;
                    price_Product.updated_at = currentDate;
                    price_Product.unit = product.unit;
                    db.price_product.Add(price_Product);

                    price_product price_Product_swap = new price_product();
                    price_Product_swap.product_id = product.id;
                    price_Product_swap.price = product.sell_price_swap;
                    price_Product_swap.price_debt = product.sell_price_debt_swap;
                    price_Product_swap.updated_at = currentDate;
                    price_Product_swap.unit = product.unit_swap;
                    db.price_product.Add(price_Product_swap);

                    inventory_order inventory_Order = new inventory_order();
                    inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                    inventory_Order.create_at = currentDate;
                    inventory_Order.update_at = currentDate;
                    inventory_Order.create_by = User.Identity.GetUserId();
                    inventory_Order.Total = product.purchase_price * product.quantity;
                    inventory_Order.state = Constants.PAYED_ORDER;
                    inventory_Order.supplier_id = check_supplier == null ? supplier.id : check_supplier.id;
                    db.inventory_order.Add(inventory_Order);

                    import_inventory inventory = new import_inventory();
                    inventory.inventory_id = inventory_Order.id;
                    inventory.product_id = product.id;
                    inventory.quantity = product.quantity;
                    inventory.price_import = product.purchase_price;
                    inventory.sold = 0;
                    inventory.sold_swap = 0;
                    inventory.return_quantity = 0;
                    inventory.quantity_remaining = 0;
                    inventory.created_by = User.Identity.GetUserId();
                    inventory.created_at = currentDate;
                    inventory.supplier_id = check_supplier == null ? supplier.id : check_supplier.id;
                    db.import_inventory.Add(inventory);
                    db.SaveChanges();
                    Product_list.RemoveAll(p => p.id == item.id);
                    message = "Đã nhập thành công tất cả sản phẩm.";

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
        public ActionResult ExitData_continues_ALL()
        {
            string message = "";
            bool status = true;
            DateTime currentDate = DateTime.Now;
            try
            {
                var product_list = Product_list.Where(p => p.status == 7).ToList();
                foreach (var item in product_list)
                {
                    var check_supplier = db.customers.FirstOrDefault(s => s.name == item.name_group && s.type == Constants.SUPPLIER);
                    var check_category = db.categories.FirstOrDefault(g => g.name == item.name_category);

                    var product = db.products.FirstOrDefault(c => c.name == item.name && c.category_id == check_category.id
                                                                  && c.supplier_id == check_supplier.id && c.unit == item.unit
                                                                  && c.unit_swap == item.unit_swap && c.quantity_swap == item.quantity_swap);

                    if (item.sell_price != product.sell_price || item.sell_price_debt != product.sell_price_debt)
                    {
                        price_product price_Product = new price_product();
                        price_Product.product_id = product.id;
                        price_Product.price = item.sell_price;
                        price_Product.price_debt = item.sell_price_debt;
                        price_Product.updated_at = currentDate;
                        price_Product.unit = product.unit;
                        db.price_product.Add(price_Product);
                    }
                    item.sell_price_swap = item.sell_price / item.quantity_swap;
                    item.sell_price_debt_swap = item.sell_price_debt / item.quantity_swap;
                    if (item.sell_price_swap != product.sell_price_swap || item.sell_price_debt_swap != product.sell_price_debt_swap)
                    {
                        price_product price_Product_swap = new price_product();
                        price_Product_swap.product_id = product.id;
                        price_Product_swap.price = item.sell_price_swap;
                        price_Product_swap.price_debt = item.sell_price_debt_swap;
                        price_Product_swap.updated_at = currentDate;
                        price_Product_swap.unit = product.unit_swap;
                        db.price_product.Add(price_Product_swap);
                    }
                    product.sell_price = item.sell_price;
                    product.sell_price_debt = item.sell_price_debt;
                    product.purchase_price = item.purchase_price;
                    product.quantity += item.quantity;
                    product.updated_at = currentDate;
                    product.sell_price_swap = item.sell_price_swap;
                    product.sell_price_debt_swap = item.sell_price_debt_swap;
                    db.Entry(product).State = EntityState.Modified;

                    inventory_order inventory_Order = new inventory_order();
                    inventory_Order.code = "MPN" + CodeRandom.RandomCode();
                    inventory_Order.create_at = currentDate;
                    inventory_Order.update_at = currentDate;
                    inventory_Order.create_by = User.Identity.GetUserId();
                    inventory_Order.Total = item.purchase_price * item.quantity;
                    inventory_Order.state = Constants.PAYED_ORDER;
                    inventory_Order.supplier_id = check_supplier.id;
                    db.inventory_order.Add(inventory_Order);

                    import_inventory inventory = new import_inventory();
                    inventory.inventory_id = inventory_Order.id;
                    inventory.product_id = product.id;
                    inventory.quantity = item.quantity;
                    inventory.price_import = item.purchase_price;
                    inventory.sold = 0;
                    inventory.sold_swap = 0;
                    inventory.return_quantity = 0;
                    inventory.quantity_remaining = 0;
                    inventory.created_by = User.Identity.GetUserId();
                    inventory.created_at = currentDate;
                    inventory.supplier_id = check_supplier.id;
                    db.import_inventory.Add(inventory);
                    db.SaveChanges();
                    Product_list.RemoveAll(p => p.id == item.id);
                    message = "Đã nhập thành công tất cả sản phẩm.";
                }
            }
            catch (Exception e)
            {
                status = false;
                message = e.Message;
            }
            return Json(new { status, message }, JsonRequestBehavior.AllowGet);
        }

        public void ExportExcel_RevenueDate(DateTime? date_Start, DateTime? date_End)
        {

            string nameFile = "Doanh thu_" + String.Format("{0:dd-MM-yyyy}", date_Start) + " đến " + String.Format("{0:dd-MM-yyyy}", date_End) + ".xlsx";

            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Day == date_Start.Value.Day
                                                    && s.created_at.Value.Month == date_Start.Value.Month
                                                    && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Day == date_End.Value.Day
                                                    && s.created_at.Value.Month == date_End.Value.Month
                                                    && s.created_at.Value.Year == date_End.Value.Year)
                .OrderByDescending(o => o.id);
            decimal tongNhap = 0;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Doanh Thu");
            HeaderExcel(Sheet);
            Sheet.Cells["A6:A6"].Style.Font.Bold = true;
            Sheet.Cells["A6:A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["A6"].Value = "Từ ngày: ";
            Sheet.Cells["B6:B6"].Merge = true;
            Sheet.Cells["B6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["B6"].Value = String.Format("{0:dd-MM-yyyy}", date_Start);

            Sheet.Cells["C6:C6"].Merge = true;
            Sheet.Cells["C6:C6"].Style.Font.Bold = true;
            Sheet.Cells["C6:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C6"].Value = "Đến hết ngày: ";
            Sheet.Cells["D6:D6"].Merge = true;
            Sheet.Cells["D6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["D6"].Value = String.Format("{0:dd-MM-yyyy}", date_End);

            Sheet.Cells["E7:F7"].Merge = true;
            Sheet.Cells["E7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E7"].Value = "Tổng tiền bán được: ";
            Sheet.Cells["G7:H7"].Merge = true;
            Sheet.Cells["G7:H7"].Style.Font.Bold = true;
            Sheet.Cells["G7"].Value = sales.Sum(x => x.total).ToString("N0") + " VNĐ";

            Sheet.Cells["E8:F8"].Merge = true;
            Sheet.Cells["E8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E8"].Value = "Tổng tiền nhập hàng: ";
            Sheet.Cells["G8:H8"].Merge = true;
            Sheet.Cells["G8:H8"].Style.Font.Bold = true;
            Sheet.Cells["G8"].Value = tongNhap.ToString("N0") + " VNĐ";

            Sheet.Cells["E9:F9"].Merge = true;
            Sheet.Cells["E9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E9"].Value = "Lợi nhuận: ";
            Sheet.Cells["G9:H9"].Merge = true;
            Sheet.Cells["G9:H9"].Style.Font.Bold = true;
            Sheet.Cells["G9"].Value = (sales.Sum(x => x.total) - tongNhap).ToString("N0") + " VNĐ";

            Sheet.Cells["A11:H11"].Style.Font.Bold = true;
            Sheet.Cells["A11:H11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["A11:H11"].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            //Sheet.Cells["A11:H11"].Style.Font.Color.SetColor(Color.White);


            Sheet.Cells["A11"].Value = "Khách hàng";
            Sheet.Cells["B11"].Value = "Mã hóa đơn";
            Sheet.Cells["C11"].Value = "Ngày giao dịch";
            Sheet.Cells["D11"].Value = "Tổng hóa đơn";
            Sheet.Cells["E11"].Value = "Thanh toán";
            Sheet.Cells["F11"].Value = "Tồn nợ";
            Sheet.Cells["G11"].Value = "Trạng Thái";
            Sheet.Cells["H11"].Value = "Nhân viên bán";

            int row = 12;// dòng bắt đầu ghi dữ liệu
            foreach (var item in sales)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.customer.name;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.code;
                Sheet.Cells[string.Format("C{0}", row)].Value = String.Format("{0:HH:mm - dd/MM/yyy}", item.created_at);
                Sheet.Cells[string.Format("D{0}", row)].Value = item.total;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.prepayment != null && item.method == Constants.DEBT_ORDER ? (int)(item.prepayment + item.pay_debt) : 0;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.prepayment != null && item.method == Constants.DEBT_ORDER ? (int)(item.total - (item.prepayment + item.pay_debt)) : 0;
                Sheet.Cells[string.Format("G{0}", row)].Value = (item.method == 1 ? "Đã thanh toán" : "Ghi nợ") + (item.total == item.prepayment ? "(Đã thanh toán xong)" : "");
                Sheet.Cells[string.Format("H{0}", row)].Value = item.user?.name;
                row++;
                foreach (var item1 in item.sale_details)
                {
                    foreach (var item2 in item1.revenues)
                    {
                        if (item2.unit == item2.import_inventory.product.unit)
                        {
                            tongNhap += item2.quantity * item2.import_inventory.price_import;
                        }
                        else
                        {
                            tongNhap += item2.quantity * (decimal)(item2.import_inventory.price_import / item2.import_inventory.product.quantity_swap);

                        }
                    }
                }
            }
            Sheet.Cells["G8"].Value = tongNhap.ToString("N0") + " VNĐ";
            Sheet.Cells["G9"].Value = (sales.Sum(x => x.total) - tongNhap).ToString("N0") + " VNĐ";

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
                nameFile = "Doanh_Thu_Thang_" + String.Format("{0:MM-yyyy}", date_End) + ".xlsx";
            }
            else
            {
                nameFile = "Doanh_Thu_Thang_" + String.Format("{0:MM-yyyy}", date_Start) + "_Den_Thang_ " + String.Format("{0:MM-yyyy}", date_End) + ".xlsx";
            }

            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Month == date_Start.Value.Month && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Month == date_End.Value.Month && s.created_at.Value.Year == date_End.Value.Year)
                .OrderByDescending(o => o.id);
            decimal tongNhap = 0;


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Doanh Thu");
            HeaderExcel(Sheet);
            Sheet.Cells["A6:A6"].Style.Font.Bold = true;
            Sheet.Cells["A6:A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["A6"].Value = "Từ tháng: ";
            Sheet.Cells["B6:B6"].Merge = true;
            Sheet.Cells["B6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["B6"].Value = String.Format("{0:MM-yyyy}", date_Start);

            Sheet.Cells["C6:C6"].Merge = true;
            Sheet.Cells["C6:C6"].Style.Font.Bold = true;
            Sheet.Cells["C6:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C6"].Value = "Đến cuối tháng: ";
            Sheet.Cells["D6:D6"].Merge = true;
            Sheet.Cells["D6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["D6"].Value = String.Format("{0:MM-yyyy}", date_End);

            Sheet.Cells["E7:F7"].Merge = true;
            Sheet.Cells["E7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E7"].Value = "Tổng tiền bán được: ";
            Sheet.Cells["G7:H7"].Merge = true;
            Sheet.Cells["G7:H7"].Style.Font.Bold = true;
            Sheet.Cells["G7"].Value = sales.Sum(x => x.total).ToString("N0") + " VNĐ";

            Sheet.Cells["E8:F8"].Merge = true;
            Sheet.Cells["E8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E8"].Value = "Tổng tiền nhập hàng: ";
            Sheet.Cells["G8:H8"].Merge = true;
            Sheet.Cells["G8:H8"].Style.Font.Bold = true;
            Sheet.Cells["G8"].Value = tongNhap.ToString("N0") + " VNĐ";

            Sheet.Cells["E9:F9"].Merge = true;
            Sheet.Cells["E9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E9"].Value = "Lợi nhuận: ";
            Sheet.Cells["G9:H9"].Merge = true;
            Sheet.Cells["G9:H9"].Style.Font.Bold = true;
            Sheet.Cells["G9"].Value = (sales.Sum(x => x.total) - tongNhap).ToString("N0") + " VNĐ";

            Sheet.Cells["A11:H11"].Style.Font.Bold = true;
            Sheet.Cells["A11:H11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["A11:H11"].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            //Sheet.Cells["A11:H11"].Style.Font.Color.SetColor(Color.White);


            Sheet.Cells["A11"].Value = "Khách hàng";
            Sheet.Cells["B11"].Value = "Mã hóa đơn";
            Sheet.Cells["C11"].Value = "Ngày giao dịch";
            Sheet.Cells["D11"].Value = "Tổng hóa đơn";
            Sheet.Cells["E11"].Value = "Thanh toán";
            Sheet.Cells["F11"].Value = "Tồn nợ";
            Sheet.Cells["G11"].Value = "Trạng Thái";
            Sheet.Cells["H11"].Value = "Nhân viên bán";

            int row = 12;// dòng bắt đầu ghi dữ liệu
            foreach (var item in sales)
            {

                Sheet.Cells[string.Format("A{0}", row)].Value = item.customer.name;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.code;
                Sheet.Cells[string.Format("C{0}", row)].Value = String.Format("{0:HH:mm - dd/MM/yyy}", item.created_at);
                Sheet.Cells[string.Format("D{0}", row)].Value = item.total;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.prepayment != null && item.method == Constants.DEBT_ORDER ? (int)(item.prepayment + item.pay_debt) : 0;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.prepayment != null && item.method == Constants.DEBT_ORDER ? (int)(item.total - (item.prepayment + item.pay_debt)) : 0;
                Sheet.Cells[string.Format("G{0}", row)].Value = (item.method == 1 ? "Đã thanh toán" : "Ghi nợ") + (item.total == item.prepayment ? "(Đã thanh toán xong)" : "");
                Sheet.Cells[string.Format("H{0}", row)].Value = item.user?.name;
                row++;
                foreach (var item1 in item.sale_details)
                {
                    foreach (var item2 in item1.revenues)
                    {
                        if (item2.unit == item2.import_inventory.product.unit)
                        {
                            tongNhap += item2.quantity * item2.import_inventory.price_import;
                        }
                        else
                        {
                            tongNhap += item2.quantity * (decimal)(item2.import_inventory.price_import / item2.import_inventory.product.quantity_swap);

                        }
                    }
                }
            }
            Sheet.Cells["G8"].Value = tongNhap.ToString("N0") + " VNĐ";
            Sheet.Cells["G9"].Value = (sales.Sum(x => x.total) - tongNhap).ToString("N0") + " VNĐ";

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
                Sheet.Cells[string.Format("H{0}", row)].Value = (item.Price - item.sale_details.product.purchase_price) * item.quantity;

                row++;

            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        [Obsolete]
        public void ExportExcel_SumRevenue(DateTime? date_Start, DateTime? date_End)
        {

            string nameFile = "Doanh_thu_tong_hop_" + String.Format("{0:dd-MM-yyyy}", date_Start) + " đến " + String.Format("{0:dd-MM-yyyy}", date_End) + ".xlsx";

            var sales = db.sales.Include(s => s.customer).Include(s => s.user).Where(s => s.created_at >= date_Start && s.created_at <= date_End
                                                    || s.created_at.Value.Day == date_Start.Value.Day
                                                    && s.created_at.Value.Month == date_Start.Value.Month
                                                    && s.created_at.Value.Year == date_Start.Value.Year
                                                    || s.created_at.Value.Day == date_End.Value.Day
                                                    && s.created_at.Value.Month == date_End.Value.Month
                                                    && s.created_at.Value.Year == date_End.Value.Year)
                .OrderByDescending(o => o.id);

            decimal tongNhap = 0;
            foreach (var item in sales)
            {
                foreach (var item1 in item.sale_details)
                {
                    foreach (var item2 in item1.revenues)
                    {
                        if (item2.unit == item2.import_inventory.product.unit)
                        {
                            tongNhap += item2.quantity * item2.import_inventory.price_import;
                        }
                        else
                        {
                            tongNhap += item2.quantity * (decimal)(item2.import_inventory.price_import / item2.import_inventory.product.quantity_swap);

                        }
                    }
                }
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Doanh Thu");
            HeaderExcel(Sheet);
            Sheet.Cells["A6:A6"].Style.Font.Bold = true;
            Sheet.Cells["A6:A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["A6"].Value = "Từ ngày: ";
            Sheet.Cells["B6:B6"].Merge = true;
            Sheet.Cells["B6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["B6"].Value = String.Format("{0:dd-MM-yyyy}", date_Start);

            Sheet.Cells["C6:C6"].Merge = true;
            Sheet.Cells["C6:C6"].Style.Font.Bold = true;
            Sheet.Cells["C6:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C6"].Value = "Đến hết ngày: ";
            Sheet.Cells["D6:D6"].Merge = true;
            Sheet.Cells["D6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["D6"].Value = String.Format("{0:dd-MM-yyyy}", date_End);

            Sheet.Cells["C7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C7"].Value = "Tổng tiền bán được: ";
            Sheet.Cells["D7"].Style.Font.Bold = true;
            Sheet.Cells["D7"].Value = sales.Sum(x => x.total).ToString("N0") + " VNĐ";

            Sheet.Cells["C8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C8"].Value = "Tổng tiền nhập hàng: ";
            Sheet.Cells["D8"].Style.Font.Bold = true;
            Sheet.Cells["D8"].Value = tongNhap.ToString("N0") + " VNĐ";

            Sheet.Cells["C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C9"].Value = "Lợi nhuận: ";
            Sheet.Cells["D9"].Style.Font.Bold = true;
            Sheet.Cells["D9"].Value = (sales.Sum(x => x.total) - tongNhap).ToString("N0") + " VNĐ";

            Sheet.Cells["A11:D11"].Style.Font.Bold = true;
            Sheet.Cells["A11:D11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["A11:D11"].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            //Sheet.Cells["A11:H11"].Style.Font.Color.SetColor(Color.White);


            Sheet.Cells["A11"].Value = "STT";
            Sheet.Cells["B11"].Value = "Ngày giao dịch";
            Sheet.Cells["C11"].Value = "Số đơn hàng";
            Sheet.Cells["D11"].Value = "Tổng tiền";
            int no = 1;
            int row = 12;// dòng bắt đầu ghi dữ liệu
            foreach (var item in sales.GroupBy(x => EntityFunctions.TruncateTime(x.created_at)))
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = no;
                Sheet.Cells[string.Format("B{0}", row)].Value = String.Format("{0:dd-MM-yyyy}", item.Key);
                Sheet.Cells[string.Format("C{0}", row)].Value = sales.Where(x => EntityFunctions.TruncateTime(x.created_at) == item.Key).Count();
                Sheet.Cells[string.Format("D{0}", row)].Value = sales.Where(x => EntityFunctions.TruncateTime(x.created_at) == item.Key).Sum(x => x.total).ToString("N0") + " VNĐ";
                row++;
                no++;
            }

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

        [Obsolete]
        public void ExportExcel_DetailsRevenue(DateTime? date_Start, DateTime? date_End)
        {

            string nameFile = "Doanh_thu_chi_tiet_" + String.Format("{0:dd-MM-yyyy}", date_Start) + " đến " + String.Format("{0:dd-MM-yyyy}", date_End) + ".xlsx";

            if (date_Start == null)
            {
                date_Start = (DateTime.Now);
            }
            if (date_End == null)
            {
                date_End = (DateTime.Now);
            }
            var revenues = db.revenues.Where(s => s.sale_details.sale.created_at >= date_Start && s.sale_details.sale.created_at <= date_End
                                                    || s.sale_details.sale.created_at.Value.Day == date_Start.Value.Day
                                                    && s.sale_details.sale.created_at.Value.Month == date_Start.Value.Month
                                                    && s.sale_details.sale.created_at.Value.Year == date_Start.Value.Year
                                                    || s.sale_details.sale.created_at.Value.Day == date_End.Value.Day
                                                    && s.sale_details.sale.created_at.Value.Month == date_End.Value.Month
                                                    && s.sale_details.sale.created_at.Value.Year == date_End.Value.Year)
                .OrderByDescending(o => o.id);


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Báo cáo chi tiết");
            HeaderExcel(Sheet);
            Sheet.Cells["A6:A6"].Style.Font.Bold = true;
            Sheet.Cells["A6:A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["A6"].Value = "Từ ngày: ";
            Sheet.Cells["B6:B6"].Merge = true;
            Sheet.Cells["B6:B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["B6"].Value = String.Format("{0:dd-MM-yyyy}", date_Start);

            Sheet.Cells["C6:C6"].Merge = true;
            Sheet.Cells["C6:C6"].Style.Font.Bold = true;
            Sheet.Cells["C6:C6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["C6"].Value = "Đến hết ngày: ";
            Sheet.Cells["D6:D6"].Merge = true;
            Sheet.Cells["D6:D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            Sheet.Cells["D6"].Value = String.Format("{0:dd-MM-yyyy}", date_End);

            Sheet.Cells["E7:F7"].Merge = true;
            Sheet.Cells["E7:F7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E7"].Value = "Tổng tiền bán được: ";
            Sheet.Cells["G7:H7"].Merge = true;
            Sheet.Cells["G7:H7"].Style.Font.Bold = true;

            Sheet.Cells["E8:F8"].Merge = true;
            Sheet.Cells["E8:F8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E8"].Value = "Tổng tiền nhập hàng: ";
            Sheet.Cells["G8:H8"].Merge = true;
            Sheet.Cells["G8:H8"].Style.Font.Bold = true;

            Sheet.Cells["E9:F9"].Merge = true;
            Sheet.Cells["E9:F9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            Sheet.Cells["E9"].Value = "Lợi nhuận: ";
            Sheet.Cells["G9:H9"].Merge = true;
            Sheet.Cells["G9:H9"].Style.Font.Bold = true;

            Sheet.Cells["A11:I11"].Style.Font.Bold = true;
            Sheet.Cells["A11:I11"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            Sheet.Cells["A11:I11"].Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);
            //Sheet.Cells["A11:H11"].Style.Font.Color.SetColor(Color.White);


            Sheet.Cells["A11"].Value = "STT";
            Sheet.Cells["B11"].Value = "Tên sản phẩm";
            Sheet.Cells["C11"].Value = "Đơn vị";
            Sheet.Cells["D11"].Value = "Đơn giá nhập";
            Sheet.Cells["E11"].Value = "Số lượng bán";
            Sheet.Cells["F11"].Value = "Đơn giá bán";
            Sheet.Cells["G11"].Value = "Tổng giá bán";
            Sheet.Cells["H11"].Value = "Lợi nhuận";
            Sheet.Cells["I11"].Value = "Ngày giao dịch";
            int no = 1;
            int row = 12;// dòng bắt đầu ghi dữ liệu
            decimal sumRevenue = 0;
            decimal sumSell = 0;
            foreach (var item in revenues)
            {
                sumSell += item.Price * item.quantity;
                Sheet.Cells[string.Format("A{0}", row)].Value = no;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.sale_details.product.name;
                Sheet.Cells[string.Format("C{0}", row)].Value = "1" + item.sale_details.product.unit + " = " + item.sale_details.product.quantity_swap + " " + item.sale_details.product.unit_swap;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.import_inventory.price_import.ToString("N0") + " VNĐ / " + item.sale_details.product.unit;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.quantity + " " + item.unit;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Price.ToString("N0") + " VNĐ";
                Sheet.Cells[string.Format("G{0}", row)].Value = (item.Price * item.quantity).ToString("N0");
                if (item.unit == item.sale_details.product.unit)
                {
                    sumRevenue += (item.Price - item.import_inventory.price_import) * item.quantity;
                    Sheet.Cells[string.Format("H{0}", row)].Value = ((item.Price - item.import_inventory.price_import) * item.quantity).ToString("N0");

                }
                else if (item.unit == item.sale_details.product.unit_swap)
                {
                    sumRevenue += (item.Price - (item.import_inventory.price_import / item.sale_details.product.quantity_swap)) * item.quantity;
                    Sheet.Cells[string.Format("H{0}", row)].Value = ((item.Price - (item.import_inventory.price_import / item.sale_details.product.quantity_swap)) * item.quantity).ToString("N0");

                }
                Sheet.Cells[string.Format("I{0}", row)].Value = String.Format("{0:HH:mm - dd/MM/yyy}", item.sale_details.sale.created_at);
                row++;
                no++;
            }
            Sheet.Cells["G7"].Value = sumSell.ToString("N0") + " VNĐ";
            Sheet.Cells["G8"].Value = (sumSell - sumRevenue).ToString("N0") + " VNĐ";
            Sheet.Cells["G9"].Value = sumRevenue.ToString("N0") + " VNĐ";

            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + nameFile);
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }

    }
}

