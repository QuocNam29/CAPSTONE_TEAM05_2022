//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CAP_TEAM05_2022.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class return_details
    {
        public int id { get; set; }
        public int return_id { get; set; }
        public int product_current_id { get; set; }
        public int quantity_current { get; set; }
        public decimal total_current { get; set; }
        public Nullable<int> product_return_id { get; set; }
        public Nullable<int> quantity_return { get; set; }
        public Nullable<decimal> total_return { get; set; }
        public decimal difference { get; set; }
    
        public virtual product product { get; set; }
        public virtual product product1 { get; set; }
        public virtual return_sale return_sale { get; set; }
    }
}