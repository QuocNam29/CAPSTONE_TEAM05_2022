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
    
    public partial class debt
    {
        public int id { get; set; }
        public Nullable<int> sale_id { get; set; }
        public Nullable<int> inventory_id { get; set; }
        public Nullable<decimal> paid { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> debt1 { get; set; }
        public Nullable<decimal> remaining { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public string note { get; set; }
    
        public virtual sale sale { get; set; }
        public virtual user user { get; set; }
        public virtual import_inventory import_inventory { get; set; }
    }
}
