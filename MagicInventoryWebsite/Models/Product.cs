using System.ComponentModel.DataAnnotations;

namespace MagicInventoryWebsite.Models
{
    public class Product
    {
        public int ProductID { get; set; }//automatic PK

        [StringLength(60, MinimumLength = 2)]
        public string Name { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}
