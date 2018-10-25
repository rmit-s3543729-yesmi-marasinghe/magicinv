using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicInventoryWebsite.Models
{
    public class OwnerInventory
    {
        //PK  and FK                 for the controller
        [Key, ForeignKey("Product"), Display(Name = "Product ID")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        //The StockLevel cannot be negative and is required
        [Required]

        [Range(0, Int32.MaxValue, ErrorMessage = "Number must be 0 or greater")]
        [Display(Name = "Stock Level")]
        public int StockLevel { get; set; }
    }
}
