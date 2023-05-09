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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public import_inventory()
        {
            this.return_supplier = new HashSet<return_supplier>();
            this.revenues = new HashSet<revenue>();
        }
    
        public int id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal price_import { get; set; }
        public int sold { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public int sold_swap { get; set; }
        public int quantity_remaining { get; set; }
        public Nullable<int> supplier_id { get; set; }
        public Nullable<int> inventory_id { get; set; }
        public int return_quantity { get; set; }
    
        public virtual customer customer { get; set; }
        public virtual inventory_order inventory_order { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<return_supplier> return_supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<revenue> revenues { get; set; }
        public virtual product product { get; set; }
    }
}
