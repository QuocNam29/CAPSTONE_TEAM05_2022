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
    
    public partial class revenue
    {
        public int id { get; set; }
        public int inventory_id { get; set; }
        public int sale_details_id { get; set; }
        public decimal Price { get; set; }
        public int quantity { get; set; }
        public string unit { get; set; }
    
        public virtual import_inventory import_inventory { get; set; }
        public virtual sale_details sale_details { get; set; }
    }
}
