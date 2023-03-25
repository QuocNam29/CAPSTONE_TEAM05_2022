using System;
using System.Collections.Generic;

namespace CAP_TEAM05_2022.Helper
{
    public class Constants
    {
        /// <summary>
        /// subdomain
        /// </summary>
        public static readonly string subdomain = "";
        public static readonly string webUrl = "";





        /// To use switch case User roles
        /// </summary>
        public const string ACCOUNT_ADMIN = "1";
        public const string ACCOUNT_STUDENT = "2";
        public const string ACCOUNT_STAFF = "3";
        public const string ACCOUNT_TEACHER = "4";
        public const string ACCOUNT_LEADER = "5";
        public const string TEMP_ACCOUNT = "temp";

        /// <summary>
        /// Alert Type For Message
        /// </summary>
        public const string ALERT_TYPE_SUCCESS = "success"; // THÀNH CÔNG
        public const string ALERT_TYPE_ERROR = "error"; // THẤT BẠI
        public const string ALERT_TYPE_WARNING = "warning"; // CẢNH BÁO

        /// </summary>
        /// To use in MailMessages
        /// </summary>
        public static readonly string adminSuffix = "";
        public static readonly string EMAIL_FROM = "";
        public static readonly string EMAIL_SERVER = "";
        public static readonly string EMAIL_CREDENTAIL = "";
        public static readonly int EMAIL_PORT = 25;



        /// <summary>
        /// Type Customer
        ///  </summary>
        public static readonly int CUSTOMER = 1;
        public static readonly int WHOLESALE_CUSTOMER = 2;
        public static readonly int SUPPLIER = 3;
        /// </summary>
        public static readonly Dictionary<int, Tuple<string, string>> TypeCustomer = new Dictionary<int, Tuple<string, string>> {
            {CUSTOMER, new Tuple<string, string>("Khách mua lẻ", "cus-simple") },
            {WHOLESALE_CUSTOMER,new Tuple<string, string>("Khách mua lẻ", "cus-multi") },
            {SUPPLIER, new Tuple<string, string>("Nhà cung cấp", "cus-supplier") }
        };

        ///Loan Form registration state
        ///
        public static int SHOW_STATUS = 1;
        public static int HIDDEN_STATUS = 2;
        public static readonly Dictionary<int, string> Status = new Dictionary<int, string> {
            {SHOW_STATUS, "Trạng thái hiện"},
            {HIDDEN_STATUS, "Trạng thái ẩn" }
        };


        /// <summary>
        /// connectionString for Import Excel Data
        /// </summary>
        public const string MS_EXCEL = "application/vnd.ms-excel";
        public const string OPENXMLFORMATS = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Microsoft_Jet_OLEDB_4 = "Provider=Microsoft.Jet.OLEDB.4.0; data source={0}; Extended Properties=Excel 8.0;";
        public const string Microsoft_ACE_OLEDB_12 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
    }
}

