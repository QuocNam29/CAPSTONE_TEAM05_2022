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
        public double? Total { get; set; }
        public int? State { get; set; }
        public int? CountSupplier { get; set; }
        public int? CountUser { get; set; }
        public int? CountAdmin { get; set; }
        public int? CountStaff { get; set; }
        public int? CountOrder { get; set; }
        public double? SumCon1 { get; set; }

        public string[] ItemArr1 { get; set; }
        public string[] ItemArr2 { get; set; }
        public string[] ItemArr3 { get; set; }
        public string[] ItemArr4 { get; set; }
        public string[] ItemArr5 { get; set; }


        public int[] CountArr1 { get; set; }
        public int[] CountArr2 { get; set; }
        public int[] CountArr3 { get; set; }
        public int[] CountArr4 { get; set; }
        public int[] CountArrLoan1 { get; set; }
        public int[] CountArrLoan2 { get; set; }


        public DateTime? Date { get; set; }
        public DateTime? Date2 { get; set; }
        public DateTime[] DateArr1 { get; set; }


        public dynamic DataPool1 { get; set; }
        public dynamic DataPool2 { get; set; }
        public dynamic DataPoo3 { get; set; }
    }
}