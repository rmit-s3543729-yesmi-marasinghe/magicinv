using System;
using System.ComponentModel.DataAnnotations;

namespace MagicInventoryWebsite.Models
{
    public class StoreInventory
    {
        public int StoreID { get; set; }
        //this achieves a FK relationship 
        public Store Store { get; set; }

        public int ProductID { get; set; }
        //FK relationship for one to many relationship
        public Product Product { get; set; }

        [Required]
        [Range(0, Int32.MaxValue, ErrorMessage = "Number must be 0 or greater")]
        public int StockLevel { get; set; }
    }
}
