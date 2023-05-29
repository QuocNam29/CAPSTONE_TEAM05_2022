using System;
using System.Collections.Generic;

namespace CAP_TEAM05_2022.Helper
{
    public class Constants
    {
        public static readonly int KHACH_VL = 0;


        public static readonly string ADMIN_ROLE = "Quản trị viên";
        public static readonly string STAFF_ROLE = "Nhân viên";


        public static readonly int ADMIN_ACCOUNT = 1;
        public static readonly int STAFF_ACCOUNT = 2;
        public static readonly Dictionary<int, string> RoleUser = new Dictionary<int, string> {
            {ADMIN_ACCOUNT, "Quản trị viên"},
            {STAFF_ACCOUNT, "Nhân viên" }
        };

        /// <summary>
        /// Type Customer
        ///  </summary>
        public static readonly int CUSTOMER = 1;
        public static readonly int SUPPLIER = 2;
        /// </summary>
        public static readonly Dictionary<int, Tuple<string, string>> TypeCustomer = new Dictionary<int, Tuple<string, string>> {
            {CUSTOMER, new Tuple<string, string>("Khách hàng", "cus-simple") },
            {SUPPLIER, new Tuple<string, string>("Nhà cung cấp", "cus-supplier") }
        };

        ///Trạng thái ẩn/hiện
        ///
        public static int SHOW_STATUS = 1;
        public static int HIDDEN_STATUS = 2;
        public static readonly Dictionary<int, string> Status = new Dictionary<int, string> {
            {SHOW_STATUS, "Trạng thái hiện"},
            {HIDDEN_STATUS, "Trạng thái ẩn" }
        };

        ///Phương thức thanh toán đơn hàng, đơn nhập hàng
        ///
        public static int PAYED_ORDER = 1;
        public static int DEBT_ORDER = 2;

        public static readonly Dictionary<int, string> MethodOrder = new Dictionary<int, string> {
            {PAYED_ORDER, "Đã thanh toán"},
            {DEBT_ORDER, "Ghi nợ" }
        };

        ///Đối tượng thu chi
        ///
        public static int COLLECTION_OF_CUSTOMERS = 1;
        public static int PAYING_SUPPLIER = 2;
        public static readonly Dictionary<int, string> RevenueExpenditureObject = new Dictionary<int, string> {
            {COLLECTION_OF_CUSTOMERS, "Thu nợ từ khách hàng"},
            {PAYING_SUPPLIER, "Chi trả cho nhầ cug cấp" }
        };

        ///Check quy đổi đơn vị sản phẩm
        ///
        public static int UNIT_CONVERT = 1;
        public static int NO_UNIT_CONVERT = 0;

        ///Lựa chọn đỏin trả sản phẩm đổi hàng / trả hàng
        ///
        public static int CHANGE_OPTION = 1;
        public static int RETURN_OPTION = 2;

        ///Lựa chọn xem chi tiết đơn hàng hoặc đổi trả (javascript)
        ///
        public static int VIEW_DETAILS = 1;
        public static int MODAL_RETURN = 2;

        ///Lọc báo cáo thu chi theo ngày và tháng
        ///
        public static int REVENUE_DAY = 1;
        public static int REVENUE_MONTH = 2;

        ///Kiểm tra pahir forr=m create không
        ///
        public static int CREATE_FORM = 1;
        public static int NOT_CREATE_FORM = 0;

        ///Kiểm tra tạo mới hay copy sản phẩm
        ///
        public static int CREATE_PRODUCT = 1;
        public static int COPY_PRODUCT = 2;

        ///Kiểm tra ghi nợ bằng giá bán hay tiền mặt
        ///
        public static int DEBT_METHOD_PRICE = 1;
        public static int CASH_METHOD_PRICE = 2;

        ///Action của bán hàng
        ///
        public static int ADD_PRODUCT_TO_CART = 1;
        public static int UPDATE_PRODUCT_TO_CART = 2;

        /// <summary>
        /// connectionString for Import Excel Data
        /// </summary>
        public const string MS_EXCEL = "application/vnd.ms-excel";
        public const string OPENXMLFORMATS = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Microsoft_Jet_OLEDB_4 = "Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;";
        public const string Microsoft_ACE_OLEDB_12 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
    }
}

