using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MagicInventoryWebsite.Models
{
    public class StockRequest
    {
        public int StockRequestID { get; set; }

        [Required]
        public int StoreID { get; set; }
        public Store Store { get; set; }

        [Required]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Required]
        [Range(0, Int32.MaxValue, ErrorMessage = "Number must be 0 or greater")]
        public int Quantity { get; set; }
    }
}
