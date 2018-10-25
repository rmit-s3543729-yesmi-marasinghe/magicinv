/*This code uses sections of https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-8 */

using System.Collections.Generic;


namespace MagicInventoryWebsite.Models
{
    public partial class Order
    {
        public int OrderID { get; set; }
        public string Username { get; set; }
        public decimal Total { get; set; }
        public System.DateTime OrderDate { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
