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
    
    public partial class import_inventory
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public int sold { get; set; }
        public int discount { get; set; }
        public int price { get; set; }
        public int created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
    
        public virtual user user { get; set; }
        public virtual product product { get; set; }
    }
}
