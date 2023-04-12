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
    using System.ComponentModel.DataAnnotations;

    public partial class product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public product()
        {
            this.carts = new HashSet<cart>();
            this.import_inventory = new HashSet<import_inventory>();
            this.price_product = new HashSet<price_product>();
            this.return_details = new HashSet<return_details>();
            this.return_details1 = new HashSet<return_details>();
            this.sale_details = new HashSet<sale_details>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        [Required]
        public string name { get; set; }
        public Nullable<int> group_id { get; set; }
        [Required]
        public int category_id { get; set; }
        [Required]
        public string unit { get; set; }
        [Required]
        public decimal purchase_price { get; set; }
        [Required]

        public decimal sell_price { get; set; }
        [Required]
        public decimal sell_price_debt { get; set; }
        public int quantity { get; set; }
        public int status { get; set; }
        public string note { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public string name_group { get; set; }
        public string name_category { get; set; }
        public string unit_swap { get; set; }
        [Required]
        public int quantity_swap { get; set; }
        public int quantity_remaning { get; set; }
        public decimal sell_price_swap { get; set; }
        [Required]
        public decimal sell_price_debt_swap { get; set; }
        public Nullable<int> supplier_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cart> carts { get; set; }
        public virtual category category { get; set; }
        public virtual group group { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<import_inventory> import_inventory { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<price_product> price_product { get; set; }
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<return_details> return_details { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<return_details> return_details1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sale_details> sale_details { get; set; }
        public virtual customer customer { get; set; }
    }
}
