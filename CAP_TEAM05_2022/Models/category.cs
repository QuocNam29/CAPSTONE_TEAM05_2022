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

    public partial class category
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public category()
        {
            this.products = new HashSet<product>();
        }

        public int id { get; set; }
        [Required(ErrorMessage = "Category code cannot be empty !")]
        [StringLength(100, ErrorMessage = "Code length must be between 1 and 50.", MinimumLength = 1)]
        public string code { get; set; }
        [Required(ErrorMessage = "Created_by cannot be empty !")]
        public int created_by { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên danh mục !")]
        [StringLength(100, ErrorMessage = "Name length must be between 1 and 255.", MinimumLength = 1)]
        public string name { get; set; }
        public string slug { get; set; }
        [Required(ErrorMessage = "Status code cannot be empty !")]
        public int status { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public Nullable<System.DateTime> updated_at { get; set; }
        public Nullable<System.DateTime> deleted_at { get; set; }
    
        public virtual user user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<product> products { get; set; }
    }
}
