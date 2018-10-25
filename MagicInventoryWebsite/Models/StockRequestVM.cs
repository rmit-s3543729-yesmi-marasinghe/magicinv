using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MagicInventoryWebsite.Models
{
    public class StockRequestVM
    {
        public int StockRequestVMID { get; set; }

        public string StoreName { get; set; }
        public int StoreID { get; set; }
        //public Store Store { get; set; }

        public string ProductName { get; set; }
      //  public Product Product { get; set; }

        public int Quantity { get; set; }

        public int OwnerStockLevel { get; set; }

        public int ProductID { get; set; }

      //  public int StoreStockLevel { get; set; }

        public bool StockAvailability { get; set; }

    }
}
