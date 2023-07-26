using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CAP_TEAM05_2022.Models
{
    public class StatisticsViewModel
    {
        public string ItemName { get; set; }
        public string ItemName2 { get; set; }
        public string ItemName3 { get; set; }
        public string ItemName4 { get; set; }


        public int? Quantity { get; set; }
        public decimal? TotalRevenue { get; set; }
        public int? State { get; set; }
        public int? CountSupplier { get; set; }
        public int? CountUser { get; set; }
        public int? CountAdmin { get; set; }
        public int? CountStaff { get; set; }
        public int? CountOrder { get; set; }
        public int? CountCustomer { get; set; }

        public string[] ItemArr1 { get; set; }
        public string[] ItemArr2 { get; set; }
        public string[] ItemArr3 { get; set; }
        // tên khách hàng mua nhiều nhất
        public string[] ItemArr4 { get; set; }
        public string[] ItemArr5 { get; set; }


        public int[] CountArr1 { get; set; }
        public decimal[] CountArr2 { get; set; }
        public int[] CountArr3 { get; set; }
        // lấy tiền những khách hàng mua nhiều nhất
        public decimal[] CountArr4 { get; set; }



    }
}