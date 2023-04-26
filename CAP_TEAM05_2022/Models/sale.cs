﻿//------------------------------------------------------------------------------
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

    public partial class sale
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sale()
        {
            this.customer_debt = new HashSet<customer_debt>();
            this.debts = new HashSet<debt>();
        }
    
        public int id { get; set; }
        public string code { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn khách hàng !")]
        public int customer_id { get; set; }
        public int method { get; set; }
        public decimal total { get; set; }
        public string note { get; set; }
        public int status { get; set; }
        public string created_by { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
        public Nullable<decimal> prepayment { get; set; }
        public Nullable<decimal> pay_debt { get; set; }
        public bool is_debt_price { get; set; }
        public bool is_old_debt { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<customer_debt> customer_debt { get; set; }
        public virtual customer customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<debt> debts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<sale_details> sale_details { get; set; }
        public virtual user user { get; set; }
    }
}
