//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Inventory.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item
    {
        public int ItemID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<bool> IsPerishable { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual PerishableItem PerishableItem { get; set; }
    }
}