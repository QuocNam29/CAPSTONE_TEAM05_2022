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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CP25Team05Entities : DbContext
    {
        public CP25Team05Entities()
            : base("name=CP25Team05Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<cart> carts { get; set; }
        public virtual DbSet<category> categories { get; set; }
        public virtual DbSet<customer> customers { get; set; }
        public virtual DbSet<debt> debts { get; set; }
        public virtual DbSet<group> groups { get; set; }
        public virtual DbSet<import_inventory> import_inventory { get; set; }
        public virtual DbSet<personal_access_tokens> personal_access_tokens { get; set; }
        public virtual DbSet<product> products { get; set; }
        public virtual DbSet<revenue> revenues { get; set; }
        public virtual DbSet<sale_details> sale_details { get; set; }
        public virtual DbSet<sale> sales { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<password_resets> password_resets { get; set; }
    }
}
