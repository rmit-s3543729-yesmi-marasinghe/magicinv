/*This code uses sections of https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-8 */

using System.ComponentModel.DataAnnotations;

namespace MagicInventoryWebsite.Models
{
    public class Cart
    {
        [Key]
        public int RecordID { get; set; }// PK
        public string CartID { get; set; }

        public int ProductID { get; set; }
        //FK relationship for one to many relationship
        public Product Product { get; set; }

        public int StoreID { get; set; }
        //FK relationship for one to many relationship
        public Store Store { get; set; }

        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }
        
    }
}
